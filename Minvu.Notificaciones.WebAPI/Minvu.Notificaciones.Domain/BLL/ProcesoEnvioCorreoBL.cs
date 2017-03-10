using Excel;
using FluentScheduler;
//using Microsoft.Exchange.WebServices.Data;
using Minvu.Notificaciones.Domain.Util;
using Minvu.Notificaciones.DTO;
using Minvu.Notificaciones.IData.DAO;
using Minvu.Notificaciones.IData.Log;
using Minvu.Notificaciones.IData.ORM;
using Minvu.Notificaciones.Personalizadas.Entidades;
using OpenPop.Mime;
using OpenPop.Pop3;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace Minvu.Notificaciones.Domain.BLL
{
	public class ProcesoEnvioCorreoBL : Registry
	{
		public ProcesoEnvioCorreoBL()
		{
			Log.RegistrarInfo("Iniciada la aplicacion web");
			Schedule(() => ProcesarEnvios()).ToRunNow().AndEvery(Convert.ToInt32(ConfigurationManager.AppSettings["IntervaloJobProcesoCorreo"])).Seconds();
			Schedule(() => ProcesarEstadoRecepcionEnvios()).ToRunNow().AndEvery(Convert.ToInt32(ConfigurationManager.AppSettings["IntervaloJobProcesoCorreo"])).Seconds();
		}

		/// <summary>
		/// Método que se ejecuta periódicamente (cada cantidad de segundos indicada por la constante "IntervaloJobProcesoCorreo") que procesa los envíos que estén listos para ser enviados
		/// y valida la fuente de datos, reemplaza y valida descriptores, convierte imagenes embedidas a cids, valida palabras prohibidas y envía los correos haciendo pausa cada
		/// cantidad de segundos configurada en la constante "PausaCadaVezEnvioBatchCorreos"
		/// </summary>
		public void ProcesarEnvios()
		{
			string rutaArchivoControlProceso = string.Empty;
			try
			{
				string rutaFuenteDatos = string.Empty;
				string rutaAdjuntos = string.Empty;
				rutaArchivoControlProceso = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "corriendoProcesoEnvios.txt";
				List<ENVIO> enviosPorEnviar = ManejoCorreoDAO.ObtenerEnviosPorEnviar();
				//Log.RegistrarInfo("enviosPorEnviar.Count:" + (enviosPorEnviar == null ? "null" : enviosPorEnviar.Count.ToString()));
				if (enviosPorEnviar != null && enviosPorEnviar.Count > 0)
				{
					if (File.Exists(rutaArchivoControlProceso))
					{
						//Log.RegistrarInfo("Ya existe archivo de control de envio de correos");
						return;
					}
					else
					{
						Log.RegistrarInfo("Iniciando el proceso de envío de correos. Envios por enviar:" + enviosPorEnviar.Count);
						FileStream fsControl = File.Create(rutaArchivoControlProceso);
						fsControl.Close();
						Log.RegistrarInfo("Archivo de control creado en " + rutaArchivoControlProceso+", archivo existe? "+File.Exists(rutaArchivoControlProceso));
					}
				}
				else
				{
					return;
				}
				foreach (ENVIO envio in enviosPorEnviar)
				{
					PLANTILLA_CORREO plantilla = ManejoCorreoDAO.ObtenerPlantillaCorreo((int)envio.IDPLANTILLACORREO);
					try
					{
						string cc = string.Empty, cco = string.Empty, descripcion;
						MatchCollection matches = null; //calces descriptores

						rutaFuenteDatos = string.Empty;
						if (bool.Parse(ConfigurationManager.AppSettings["RutaAdjuntosEsWeb"]))
						{
							rutaFuenteDatos = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + ConfigurationManager.AppSettings["RutaAdjuntos"].Substring(2);
						}
						else
						{
							rutaFuenteDatos = ConfigurationManager.AppSettings["RutaAdjuntos"];
						}
						rutaFuenteDatos += Convert.ToString(Path.DirectorySeparatorChar) + envio.IDENVIO;
						rutaAdjuntos = rutaFuenteDatos;
						rutaFuenteDatos += Convert.ToString(Path.DirectorySeparatorChar) + "fuenteDatos";
						rutaFuenteDatos += Convert.ToString(Path.DirectorySeparatorChar);
						DirectoryInfo dirInfo = new DirectoryInfo(rutaFuenteDatos);
						DataTable dtDescriptoresConDirecciones = new DataTable();
						DataTable dtCCCCO = new DataTable();
						//if (envioCorreoDTO.UsarDireccionesFuenteDatos)
						//{
						//	if (envioCorreoDTO.FuenteDatos != null && envioCorreoDTO.FuenteDatos.Length > 0 && dirInfo.Exists)
						//	{
						bool usarDireccionesFuenteDatos = false;
						if (dirInfo.Exists)
						{ 
							FileInfo[] files = dirInfo.GetFiles("*.xls?");
							if (files != null && files.Length >0 )
							{ 
								rutaFuenteDatos = files[0].FullName;
								FileStream fs = new FileStream(rutaFuenteDatos, FileMode.Open);
								IExcelDataReader excelReader = null;
								if (files[0].Extension.ToLower() == ".xlsx")
								{
									excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
								}
								else
								{
									excelReader = ExcelReaderFactory.CreateBinaryReader(fs);
								}
								excelReader.IsFirstRowAsColumnNames = true;
								DataSet result = excelReader.AsDataSet();
								dtDescriptoresConDirecciones = result.Tables["descriptores"];
								dtCCCCO = result.Tables["CC-CCO"];

								if (dtDescriptoresConDirecciones != null && dtCCCCO != null) usarDireccionesFuenteDatos = true;
								else
								{
									throw new InvalidDataException(MensajeBLL.ObtenerMensaje("MensajeErrorFaltanHojasEnExcel","MENSAJE_ERROR"));
								}

								var resultCC = dtCCCCO.AsEnumerable()
											.Where(row => row[0] != null && Convert.ToString(row[0]) != string.Empty).ToList();
								if (resultCC.Count > 0)
								{
									cc = dtCCCCO.AsEnumerable()
												.Where(row => row[0] != null && Convert.ToString(row[0]) != string.Empty)
												.Select(row => Convert.ToString(row[0]))
												.Aggregate((s1, s2) => String.Concat(s1, ",", s2));
								}
								var resultCCCCO = dtCCCCO.AsEnumerable()
												.Where(row => row[1] != null && Convert.ToString(row[1]) != string.Empty).ToList();
								if (resultCCCCO.Count > 0)
								{
									cco = dtCCCCO.AsEnumerable()
													.Where(row => row[1] != null && Convert.ToString(row[1]) != string.Empty)
													.Select(row => Convert.ToString(row[1]))
													.Aggregate((s1, s2) => String.Concat(s1, ",", s2));
								}
								plantilla.CUERPO = plantilla.CUERPO.Replace("&lt;", "<");
								plantilla.CUERPO = plantilla.CUERPO.Replace("&gt;", ">");
								var pm = new PreMailer.Net.PreMailer(plantilla.CUERPO);
								matches = Regex.Matches(plantilla.CUERPO, ConfigurationManager.AppSettings["ExprRegularDescriptores"]);
								List<string> descriptoresFaltantes = new List<string>();
								foreach (Match match in matches)
								{
									if (!dtDescriptoresConDirecciones.Columns.Contains(match.Groups[1].Value))
									{
										descriptoresFaltantes.Add(match.Groups[1].Value);
										break;
									}
								}
								if (descriptoresFaltantes.Count > 0)
								{
									descripcion = string.Format(MensajeBLL.ObtenerMensaje("MensajeErrorFaltanDescriptoresEnExcel", "MENSAJE_ERROR"),
																										envio.IDENVIO,
																										string.Join(",", descriptoresFaltantes),
																										rutaFuenteDatos,
																										bool.Parse(ConfigurationManager.AppSettings["RutaAdjuntosEsWeb"]) ? MensajeBLL.ObtenerMensaje("MensajeRutaFuenteDatosEnServWeb", "MENSAJE_ERROR") : string.Empty);
									Log.RegistrarWarning(descripcion, envio.IDUSUARIO);
								}
							}

							//StringBuilder para = new StringBuilder();
							//int cantFilas = 0;
							//string para = dtDescriptoresConDirecciones.AsEnumerable().Select(row => Convert.ToString(row[0]))
							//				.Aggregate((s1, s2) => String.Concat(s1, ",", s2));

							////NO SE DEBE HACER ASI PUES DE LO CONTRARIO CADA DESTINATARIO VERIA LA LISTA ENTERA DE DESTINATARIOS
							////se hizo con stringbuilder pues pueden ser cientos o miles de destinatarios y concatenando strings
							////directamente puede tener problemas de rendimiento
							//foreach (DataRow dr in dtDescriptoresConDirecciones.Rows)
							//{
							//	para.Append(Convert.ToString(dr[0]));
							//	cantFilas++;
							//	if (cantFilas< dtDescriptoresConDirecciones.Rows.Count) para.Append(',');
							//}

						}
						if (!usarDireccionesFuenteDatos)
						{
							cc = envio.CONCOPIA;
							cco = envio.CONCOPIAOCULTA;
						}

						//ENVIO envioActualiza = ManejoCorreoDAO.ObtenerEnvio(envio.IDENVIO);
						envio.FECHAHORA = DateTime.Now;
						envio.CONCOPIA = cc;
						envio.CONCOPIAOCULTA = cco;
						ManejoCorreoDAO.GuardarEnvio(envio);

						Dictionary<string, byte[]> adjuntos = new Dictionary<string, byte[]>();
						DirectoryInfo dirAdjuntos = new DirectoryInfo(rutaAdjuntos);
						if (dirAdjuntos.Exists)
						{
							FileInfo[] fileAdjuntos = dirAdjuntos.EnumerateFiles().ToArray();
							if (fileAdjuntos != null && fileAdjuntos.Length > 0)
							{
								foreach (FileInfo fileAdjunto in fileAdjuntos)
								{
									//byte[] contenidoAdjunto = new byte[fsAdjunto.]
									byte[] contenidoAdjunto = File.ReadAllBytes(fileAdjunto.FullName);
									adjuntos.Add(fileAdjunto.Name, contenidoAdjunto);
								}
							}
						}

						CORREO correo = new CORREO();
						DESTINATARIO destinatario = new DESTINATARIO();
						int IDESTADO_CORREO_CREADO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoCreado"]);
						int IDESTADO_CORREO_ERROR_REINTENTAR = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoErrorReintentar"]); 

						//List<PalabraProhibidaDTO> palabrasProhibidas = PalabraProhibidaDAO.ObtenerListaPalabras();

						DataTable dtDescriptores = dtDescriptoresConDirecciones;
						if (!usarDireccionesFuenteDatos)
						{
							
							dtDescriptores = new DataTable("descriptores");
							//dtDescriptores.Columns.Add("Destinatarios", typeof(string));
							dtDescriptores.Columns.Add("Para", typeof(string));

							List<CORREO> correosSinFteDatos = ManejoCorreoDAO.ObtenerCorreos(envio.IDENVIO);
							/*
							foreach (DataColumn dc in dtDescriptoresConDirecciones.Columns)
							{
								dtDescriptores.Columns.Add(dc.ColumnName, dc.DataType);
							}
							*/
							foreach (CORREO correoSinFte in correosSinFteDatos)
							{
								//DataRow drPrimeraFila = dtDescriptoresConDirecciones.Rows[0];
								DataRow drNuevaFila = dtDescriptores.NewRow();
								destinatario = ManejoCorreoDAO.ObtenerDestinatario((int)correoSinFte.IDDESTINATARIO);
								/*
								foreach (DataColumn dc in dtDescriptoresConDirecciones.Columns)
								{
									drNuevaFila[dc.ColumnName] = drPrimeraFila[dc.ColumnName];
								}
								*/
								drNuevaFila[0] = destinatario.CASILLACORREO;
								dtDescriptores.Rows.Add(drNuevaFila);
							}
						}
						List<string> correosConError = new List<string>();
						int cantEnviadosDeCorrido = 0, cantIntentosEnvio = 0;
						SISTEMA_EMISOR sistEmisor = SistemaEmisorDAO.ObtenerSistemaEmisor((int)plantilla.IDSISTEMAEMISOR);

						DataRow[] drs = dtDescriptores.Select(dtDescriptores.Columns[0].ColumnName + " <> ''");
						StringBuilder direccionesPara = new StringBuilder();
						foreach (DataRow dr in drs)
						{
							direccionesPara.Append(dr[0]);
							direccionesPara.Append("\r\n");
						}

						string cuerpoGral = plantilla.CUERPO;
						string cuerpoGralConImgBase64 = plantilla.CUERPO;
						MD5 md5 = MD5.Create();
						Dictionary<string, byte[]> cids = new Dictionary<string, byte[]>();
						MatchCollection matchesImgs = null;
						List<string> tagsImagenes = new List<string>();
						while ((matchesImgs = Regex.Matches(cuerpoGral, ConfigurationManager.AppSettings["ExprRegularImagenes"])).Count > 0)
						//foreach (Match match in matchesImgs)
						{
							Match match = matchesImgs[0];
							string base64Img = match.Groups[1].Value;
							string someAttributes = match.Groups[2].Value;
							string imgName = Utils.ByteArrayToString(md5.ComputeHash(Encoding.UTF8.GetBytes(base64Img)));
							string imgHtml = string.Format(ConfigurationManager.AppSettings["PlantillaImagen"], imgName, someAttributes);
							byte[] byteImg = Convert.FromBase64String(base64Img);
							cids.Add(imgName, byteImg);
							cuerpoGral = cuerpoGral.Substring(0, match.Index) + imgHtml + cuerpoGral.Substring(match.Index + match.Length);
							tagsImagenes.Add(imgHtml);
						}

						//List<string> palabrasProhibidasEncontradas = new List<string>();
						//string cuerpoUpperCase = cuerpoGral;
						//foreach (string imgHtml in tagsImagenes)
						//{
						//	cuerpoUpperCase = cuerpoUpperCase.Replace(imgHtml, string.Empty);
						//}
						//cuerpoUpperCase = cuerpoUpperCase.ToUpper();
						//string asuntoUpperCase = plantilla.ASUNTO.ToUpper();
						//foreach (PalabraProhibidaDTO palabraProhibida in palabrasProhibidas)
						//{
						//	if (cuerpoUpperCase.Contains(palabraProhibida.Palabra.ToUpper()) ||
						//			asuntoUpperCase.Contains(palabraProhibida.Palabra.ToUpper()))
						//	{
						//		palabrasProhibidasEncontradas.Add(palabraProhibida.Palabra);
						//	}
						//}

						//if (palabrasProhibidasEncontradas.Count > 0)
						//{
						//	EVENTO evento = new EVENTO();
						//	evento.IDUSUARIO = envio.IDUSUARIO;
						//	evento.FECHA = DateTime.Now;
						//	evento.DESCRIPCION = string.Format(MensajeBLL.ObtenerMensaje("MensajeErrorHayPalabrasProhibidas", "MENSAJE_ERROR"),
						//																		envio.IDENVIO,
						//																		string.Join(",", palabrasProhibidasEncontradas));
						//}
						descripcion = string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoEnvioGlobal", "MENSAJE_INFORMACION"),
																							envio.IDENVIO,
																							direccionesPara,
																							cc,
																							cco,
																							plantilla.ASUNTO,
																							cuerpoGral,
																							adjuntos.Count,
																							Utils.BytesToString(adjuntos.Sum(i => i.Value.Length)),
																							cids.Count,
																							Utils.BytesToString(cids.Sum(c => c.Value.Length)));
						direccionesPara.Clear();
						direccionesPara.Append("Listado de destinatarios:<br><ul>");
						foreach (DataRow dr in drs)
						{
							direccionesPara.Append("<li>");
							direccionesPara.Append(dr[0]);
							direccionesPara.Append("</li>");
						}
						string cuerpoCCCCO = cuerpoGral.Replace("<<", "&lt;&lt;").Replace(">>", "&gt;&gt;");
						direccionesPara.Append("</ul><br><hr><br>");
						if (cc != null && cc.Length>0)
						{
							foreach (string dircc in cc.Split(','))
							{
								try {
									destinatario = ManejoCorreoDAO.ObtenerDestinatario(dircc);
									if (destinatario == null)
									{
										destinatario = new DESTINATARIO();
										destinatario.CASILLACORREO = dircc;
										ManejoCorreoDAO.GuardarDestinatario(destinatario);
									}

									correo = ManejoCorreoDAO.ObtenerCorreo(envio.IDENVIO, destinatario.IDDESTINATARIO);
									//PreMailer.Net.PreMailer preMailer = new PreMailer.Net.PreMailer(cuerpo);
									//cuerpo = preMailer.MoveCssInline(true, null, null, true).Html;
									if (correo == null)
									{
										correo = new CORREO();
										correo.ASUNTO = "[(CC) RESUMEN CORREOS ENVIADOS desde \"" + sistEmisor.NOMBRESISTEMA + "\"]: " + plantilla.ASUNTO;
										correo.CUERPO = direccionesPara.ToString() + cuerpoCCCCO;
										correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoCreado"]);
										correo.FECHAHORA = DateTime.Now;
										correo.IDCORREO = 0;
										correo.IDDESTINATARIO = destinatario.IDDESTINATARIO;
										correo.IDENVIO = envio.IDENVIO;
										correo.IDSISTEMAEMISOR = plantilla.IDSISTEMAEMISOR;
									}
									correo.CUERPO = direccionesPara.ToString() + cuerpoCCCCO;
									correo.CUERPOVISUALIZACION = correo.CUERPO;
									if (correo.IDESTADOCORREO != IDESTADO_CORREO_CREADO && correo.IDESTADOCORREO != IDESTADO_CORREO_ERROR_REINTENTAR) break;

									ManejoCorreoDAO.GuardarCorreo(correo);
									Email.EnviarCorreo(sistEmisor.CASILLACORREO, sistEmisor.NOMBRESISTEMA, dircc, "", "", adjuntos, correo.ASUNTO, correo.CUERPO, cids, correo.IDCORREO);
									correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoEnviadoOk"]);
									ManejoCorreoDAO.GuardarCorreo(correo);
								}
								catch (IOException iex)
								{
									descripcion = string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoCorreoEnviadoError", "MENSAJE_INFORMACION"),
																		destinatario.CASILLACORREO, cc, cco, correo.ASUNTO, correo.CUERPO,
																		adjuntos.Count, Utils.BytesToString(adjuntos.Sum(a => a.Value.Length)),
																		cids.Count, Utils.BytesToString(cids.Sum(c => c.Value.Length)), iex.StackTrace);
									Log.RegistrarError(iex, descripcion, envio.IDUSUARIO);
									correosConError.Add(dircc);
									correo = ManejoCorreoDAO.ObtenerCorreo(envio.IDENVIO, destinatario.IDDESTINATARIO);
									correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoErrorReintentar"]);
									ManejoCorreoDAO.GuardarCorreo(correo);
								}
								catch (SmtpFailedRecipientsException ex)
								{
									correo = ManejoCorreoDAO.ObtenerCorreo(envio.IDENVIO, destinatario.IDDESTINATARIO);
									for (int i = 0; i < ex.InnerExceptions.Length; i++)
									{
										SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
										if (status == SmtpStatusCode.MailboxBusy ||
											status == SmtpStatusCode.MailboxUnavailable)
										{
											//Console.WriteLine("Delivery failed - retrying in 5 seconds.");
											correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoErrorReintentar"]);
											break;
										}
										else
										{
											//Console.WriteLine("Failed to deliver message to {0}", ex.InnerExceptions[i].FailedRecipient);
											correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoEnviadoConObs"]);
											break;
										}
									}
									ManejoCorreoDAO.GuardarCorreo(correo);
									correosConError.Add(dircc);
									descripcion = string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoCorreoEnviadoError", "MENSAJE_INFORMACION"),
																		destinatario.CASILLACORREO, cc, cco, correo.ASUNTO, correo.CUERPO,
																		adjuntos.Count, Utils.BytesToString(adjuntos.Sum(a => a.Value.Length)),
																		cids.Count, Utils.BytesToString(cids.Sum(c => c.Value.Length)), ex.StackTrace);
									Log.RegistrarError(ex, descripcion, envio.IDUSUARIO);
								}
								catch (SmtpException sex)
								{
									correo = ManejoCorreoDAO.ObtenerCorreo(envio.IDENVIO, destinatario.IDDESTINATARIO);
									if (sex.Message.ToLower().Contains(MensajeBLL.ObtenerMensaje("MensajeErrorCuotaDeSalidaDeCorreoExcedida", "MENSAJE_ERROR"))
										|| sex.StatusCode == SmtpStatusCode.ServiceNotAvailable)
									{
										correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoErrorReintentar"]);
									}
									else
									{
										correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoError"]);
									}
									ManejoCorreoDAO.GuardarCorreo(correo);
									correosConError.Add(dircc);
									descripcion = string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoCorreoEnviadoError", "MENSAJE_INFORMACION"),
																		destinatario.CASILLACORREO, cc, cco, correo.ASUNTO, correo.CUERPO,
																		adjuntos.Count, Utils.BytesToString(adjuntos.Sum(a => a.Value.Length)),
																		cids.Count, Utils.BytesToString(cids.Sum(c => c.Value.Length)), sex.StackTrace);
									Log.RegistrarError(sex, descripcion, envio.IDUSUARIO);
								}
								catch (Exception ex)
								{
									correo = ManejoCorreoDAO.ObtenerCorreo(envio.IDENVIO, destinatario.IDDESTINATARIO);
									correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoError"]);
									ManejoCorreoDAO.GuardarCorreo(correo);
									correosConError.Add(dircc);


									descripcion = string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoCorreoEnviadoError", "MENSAJE_INFORMACION"),
																		destinatario.CASILLACORREO, cc, cco, correo.ASUNTO, correo.CUERPO,
																		adjuntos.Count, Utils.BytesToString(adjuntos.Sum(a => a.Value.Length)),
																		cids.Count, Utils.BytesToString(cids.Sum(c => c.Value.Length)), ex.StackTrace);
									Log.RegistrarError(ex, descripcion, envio.IDUSUARIO);
								}
								/*
								cantIntentosEnvio++;
								if ((cantIntentosEnvio % Convert.ToInt32(ConfigurationManager.AppSettings["CantCorreosPorVez"])) == 0)
								{
									Thread.Sleep(TimeSpan.FromSeconds(Convert.ToInt32(ConfigurationManager.AppSettings["PausaCadaVezEnvioBatchCorreos"])));
								}
								*/
							}
						}

						if (cco != null && cco.Length>0)
						{
							foreach (string dircco in cco.Split(','))
							{
								try
								{
									destinatario = ManejoCorreoDAO.ObtenerDestinatario(dircco);
									if (destinatario == null)
									{
										destinatario = new DESTINATARIO();
										destinatario.CASILLACORREO = dircco;
										ManejoCorreoDAO.GuardarDestinatario(destinatario);
									}

									correo = ManejoCorreoDAO.ObtenerCorreo(envio.IDENVIO, destinatario.IDDESTINATARIO);
									//PreMailer.Net.PreMailer preMailer = new PreMailer.Net.PreMailer(cuerpo);
									//cuerpo = preMailer.MoveCssInline(true, null, null, true).Html;
									if (correo == null)
									{
										correo = new CORREO();
										correo.ASUNTO = "[(CCO) RESUMEN CORREOS ENVIADOS desde \"" + sistEmisor.NOMBRESISTEMA + "\"]: " + plantilla.ASUNTO;
										correo.CUERPO = direccionesPara.ToString() + cuerpoCCCCO;
										correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoCreado"]);
										correo.FECHAHORA = DateTime.Now;
										correo.IDCORREO = 0;
										correo.IDDESTINATARIO = destinatario.IDDESTINATARIO;
										correo.IDENVIO = envio.IDENVIO;
										correo.IDSISTEMAEMISOR = plantilla.IDSISTEMAEMISOR;
									}
									correo.CUERPO = direccionesPara.ToString() + cuerpoCCCCO;
									correo.CUERPOVISUALIZACION = correo.CUERPO;
									if (correo.IDESTADOCORREO != IDESTADO_CORREO_CREADO && correo.IDESTADOCORREO != IDESTADO_CORREO_ERROR_REINTENTAR) break;
									ManejoCorreoDAO.GuardarCorreo(correo);
									Email.EnviarCorreo(sistEmisor.CASILLACORREO, sistEmisor.NOMBRESISTEMA, dircco, "", "", adjuntos, correo.ASUNTO, correo.CUERPO, cids, correo.IDCORREO);
									correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoEnviadoOk"]);
									ManejoCorreoDAO.GuardarCorreo(correo);
								}
								catch (IOException iex)
								{
									descripcion = string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoCorreoEnviadoError", "MENSAJE_INFORMACION"),
																		destinatario.CASILLACORREO, cc, cco, correo.ASUNTO, correo.CUERPO,
																		adjuntos.Count, Utils.BytesToString(adjuntos.Sum(a => a.Value.Length)),
																		cids.Count, Utils.BytesToString(cids.Sum(c => c.Value.Length)), iex.StackTrace);
									Log.RegistrarError(iex, descripcion, envio.IDUSUARIO);
									correosConError.Add(dircco);
									correo = ManejoCorreoDAO.ObtenerCorreo(envio.IDENVIO, destinatario.IDDESTINATARIO);
									correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoErrorReintentar"]);
									ManejoCorreoDAO.GuardarCorreo(correo);
								}
								catch (SmtpFailedRecipientsException ex)
								{
									correo = ManejoCorreoDAO.ObtenerCorreo(envio.IDENVIO, destinatario.IDDESTINATARIO);
									for (int i = 0; i < ex.InnerExceptions.Length; i++)
									{
										SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
										if (status == SmtpStatusCode.MailboxBusy ||
											status == SmtpStatusCode.MailboxUnavailable)
										{
											//Console.WriteLine("Delivery failed - retrying in 5 seconds.");
											correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoErrorReintentar"]);
											break;
										}
										else
										{
											//Console.WriteLine("Failed to deliver message to {0}", ex.InnerExceptions[i].FailedRecipient);
											correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoEnviadoConObs"]);
											break;
										}
									}
									ManejoCorreoDAO.GuardarCorreo(correo);
									correosConError.Add(dircco);
									descripcion = string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoCorreoEnviadoError", "MENSAJE_INFORMACION"),
																		destinatario.CASILLACORREO, cc, cco, correo.ASUNTO, correo.CUERPO,
																		adjuntos.Count, Utils.BytesToString(adjuntos.Sum(a => a.Value.Length)),
																		cids.Count, Utils.BytesToString(cids.Sum(c => c.Value.Length)), ex.StackTrace);
									Log.RegistrarError(ex, descripcion, envio.IDUSUARIO);
								}
								catch (SmtpException sex)
								{
									correo = ManejoCorreoDAO.ObtenerCorreo(envio.IDENVIO, destinatario.IDDESTINATARIO);
									if (sex.Message.ToLower().Contains(MensajeBLL.ObtenerMensaje("MensajeErrorCuotaDeSalidaDeCorreoExcedida", "MENSAJE_ERROR"))
										|| sex.StatusCode == SmtpStatusCode.ServiceNotAvailable)
									{
										correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoErrorReintentar"]);
									}
									else
									{
										correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoError"]);
									}
									ManejoCorreoDAO.GuardarCorreo(correo);
									correosConError.Add(dircco);
									descripcion = string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoCorreoEnviadoError", "MENSAJE_INFORMACION"),
																		destinatario.CASILLACORREO, cc, cco, correo.ASUNTO, correo.CUERPO,
																		adjuntos.Count, Utils.BytesToString(adjuntos.Sum(a => a.Value.Length)),
																		cids.Count, Utils.BytesToString(cids.Sum(c => c.Value.Length)), sex.StackTrace);
									Log.RegistrarError(sex, descripcion, envio.IDUSUARIO);
								}
								catch (Exception ex)
								{
									correo = ManejoCorreoDAO.ObtenerCorreo(envio.IDENVIO, destinatario.IDDESTINATARIO);
									correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoError"]);
									ManejoCorreoDAO.GuardarCorreo(correo);
									correosConError.Add(dircco);


									descripcion = string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoCorreoEnviadoError", "MENSAJE_INFORMACION"),
																		destinatario.CASILLACORREO, cc, cco, correo.ASUNTO, correo.CUERPO,
																		adjuntos.Count, Utils.BytesToString(adjuntos.Sum(a => a.Value.Length)),
																		cids.Count, Utils.BytesToString(cids.Sum(c => c.Value.Length)), ex.StackTrace);
									Log.RegistrarError(ex, descripcion, envio.IDUSUARIO);
								}
								/*
								cantIntentosEnvio++;
								if ((cantIntentosEnvio % Convert.ToInt32(ConfigurationManager.AppSettings["CantCorreosPorVez"])) == 0)
								{
									Thread.Sleep(TimeSpan.FromSeconds(Convert.ToInt32(ConfigurationManager.AppSettings["PausaCadaVezEnvioBatchCorreos"])));
								}
								*/
							}
						}
					

						//envio de cada correo
						foreach (DataRow dr in drs)
						{
							string para = Convert.ToString(dr[0]).Trim();
							string cuerpo = cuerpoGral;
							string cuerpoVisibleHtml = cuerpoGralConImgBase64;
							//guarda destinatarios
							destinatario = ManejoCorreoDAO.ObtenerDestinatario(para);
							if (destinatario == null)
							{
								destinatario = new DESTINATARIO();
								destinatario.CASILLACORREO = para;
								ManejoCorreoDAO.GuardarDestinatario(destinatario);
							}

							if (usarDireccionesFuenteDatos)
							{
								foreach (Match match in matches)
								{
									cuerpo = cuerpo.Replace("<<" + match.Groups[1].Value + ">>", Convert.ToString(dr[match.Groups[1].Value]));
									cuerpoVisibleHtml = cuerpoVisibleHtml.Replace("<<" + match.Groups[1].Value + ">>", Convert.ToString(dr[match.Groups[1].Value]));
								}
							}
							try
							{
								correo = ManejoCorreoDAO.ObtenerCorreo(envio.IDENVIO, destinatario.IDDESTINATARIO);
								//PreMailer.Net.PreMailer preMailer = new PreMailer.Net.PreMailer(cuerpo);
								//cuerpo = preMailer.MoveCssInline(true, null, null, true).Html;
								if (correo == null)
								{
									correo = new CORREO();
									correo.ASUNTO = plantilla.ASUNTO;
									correo.CUERPO = cuerpo;
									correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoCreado"]);
									correo.FECHAHORA = DateTime.Now;
									correo.IDCORREO = 0;
									correo.IDDESTINATARIO = destinatario.IDDESTINATARIO;
									correo.IDENVIO = envio.IDENVIO;
									correo.IDSISTEMAEMISOR = plantilla.IDSISTEMAEMISOR;
								}
								correo.CUERPO = cuerpo;
								correo.CUERPOVISUALIZACION = cuerpoVisibleHtml;
								if (correo.IDESTADOCORREO != IDESTADO_CORREO_CREADO && correo.IDESTADOCORREO != IDESTADO_CORREO_ERROR_REINTENTAR) break;
								ManejoCorreoDAO.GuardarCorreo(correo);
								//se envia cuando no se ha enviado o cuando falló con un error recuperable
								if (correo.IDESTADOCORREO == Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoErrorReintentar"])
									|| correo.IDESTADOCORREO == Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoCreado"]))
								{
									//correo.TIPOENVIO = "";
									//Log.RegistrarInfo(string.Format("[ProcesarEnvios] Enviando correo a {0}, desde {1} <{2}> con {3} adjuntos y {4} cids. ID Correo {5}",
									//																para, sistEmisor.NOMBRESISTEMA, sistEmisor.CASILLACORREO, adjuntos == null ? "null" : adjuntos.Count.ToString(), cids == null ? "null" : cids.Count.ToString(), correo.IDCORREO));
									Email.EnviarCorreo(sistEmisor.CASILLACORREO, sistEmisor.NOMBRESISTEMA, para, "", "", adjuntos, correo.ASUNTO, cuerpo, cids, correo.IDCORREO);
									//Email.EnviarCorreo(sistEmisor.CASILLACORREO, sistEmisor.NOMBRESISTEMA, para, cc, cco, adjuntos, correo.ASUNTO, cuerpo, cids, correo.IDCORREO);
									//correo = ManejoCorreoDAO.ObtenerCorreo(correo.IDCORREO);
									correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoEnviadoOk"]);
									ManejoCorreoDAO.GuardarCorreo(correo);
									cantEnviadosDeCorrido++;
									descripcion = string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoCorreoEnviadoOK", "MENSAJE_INFORMACION"),
																		destinatario.CASILLACORREO, cc, cco, correo.ASUNTO, correo.CUERPO,
																		adjuntos.Count, Utils.BytesToString(adjuntos.Sum(a => a.Value.Length)),
																		cids.Count, Utils.BytesToString(cids.Sum(c => c.Value.Length)));
								}
							}
							catch (IOException iex)
							{
								descripcion = string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoCorreoEnviadoError", "MENSAJE_INFORMACION"),
																	destinatario.CASILLACORREO, cc, cco, correo.ASUNTO, correo.CUERPO,
																	adjuntos.Count, Utils.BytesToString(adjuntos.Sum(a => a.Value.Length)),
																	cids.Count, Utils.BytesToString(cids.Sum(c => c.Value.Length)), iex.StackTrace);
								Log.RegistrarError(iex, descripcion, envio.IDUSUARIO);
								correosConError.Add(para);
								correo = ManejoCorreoDAO.ObtenerCorreo(envio.IDENVIO, destinatario.IDDESTINATARIO);
								correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoErrorReintentar"]);
								ManejoCorreoDAO.GuardarCorreo(correo);
							}
							catch (SmtpFailedRecipientsException ex)
							{
								correo = ManejoCorreoDAO.ObtenerCorreo(envio.IDENVIO, destinatario.IDDESTINATARIO);
								for (int i = 0; i < ex.InnerExceptions.Length; i++)
								{
									SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
									if (status == SmtpStatusCode.MailboxBusy ||
										status == SmtpStatusCode.MailboxUnavailable)
									{
										//Console.WriteLine("Delivery failed - retrying in 5 seconds.");
										correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoErrorReintentar"]);
										break;
									}
									else
									{
										//Console.WriteLine("Failed to deliver message to {0}", ex.InnerExceptions[i].FailedRecipient);
										correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoEnviadoConObs"]);
										break;
									}
								}
								ManejoCorreoDAO.GuardarCorreo(correo);
								correosConError.Add(para);
								descripcion = string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoCorreoEnviadoError", "MENSAJE_INFORMACION"),
																	destinatario.CASILLACORREO, cc, cco, correo.ASUNTO, correo.CUERPO,
																	adjuntos.Count, Utils.BytesToString(adjuntos.Sum(a => a.Value.Length)),
																	cids.Count, Utils.BytesToString(cids.Sum(c => c.Value.Length)), ex.StackTrace);
								Log.RegistrarError(ex, descripcion, envio.IDUSUARIO);
							}
							catch (SmtpException sex)
							{
								correo = ManejoCorreoDAO.ObtenerCorreo(envio.IDENVIO, destinatario.IDDESTINATARIO);
								if (sex.Message.ToLower().Contains(MensajeBLL.ObtenerMensaje("MensajeErrorCuotaDeSalidaDeCorreoExcedida", "MENSAJE_ERROR"))
									|| sex.StatusCode == SmtpStatusCode.ServiceNotAvailable)
								{
									correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoErrorReintentar"]);
								}
								else
								{
									correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoError"]);
								}
								ManejoCorreoDAO.GuardarCorreo(correo);
								correosConError.Add(para);
								descripcion = string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoCorreoEnviadoError", "MENSAJE_INFORMACION"),
																	destinatario.CASILLACORREO, cc, cco, correo.ASUNTO, correo.CUERPO,
																	adjuntos.Count, Utils.BytesToString(adjuntos.Sum(a => a.Value.Length)),
																	cids.Count, Utils.BytesToString(cids.Sum(c => c.Value.Length)), sex.StackTrace);
								Log.RegistrarError(sex, descripcion, envio.IDUSUARIO);
							}
							catch (Exception ex)
							{
								correo = ManejoCorreoDAO.ObtenerCorreo(envio.IDENVIO, destinatario.IDDESTINATARIO);
								correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoError"]);
								ManejoCorreoDAO.GuardarCorreo(correo);
								correosConError.Add(para);


								descripcion = string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoCorreoEnviadoError", "MENSAJE_INFORMACION"),
																	destinatario.CASILLACORREO, cc, cco, correo.ASUNTO, correo.CUERPO,
																	adjuntos.Count, Utils.BytesToString(adjuntos.Sum(a => a.Value.Length)),
																	cids.Count, Utils.BytesToString(cids.Sum(c => c.Value.Length)), ex.StackTrace);
								Log.RegistrarError(ex, descripcion, envio.IDUSUARIO);
							}
							cantIntentosEnvio++;
							if ((cantIntentosEnvio % Convert.ToInt32(ConfigurationManager.AppSettings["CantCorreosPorVez"])) == 0)
							{
								Thread.Sleep(TimeSpan.FromSeconds(Convert.ToInt32(ConfigurationManager.AppSettings["PausaCadaVezEnvioBatchCorreos"])));
							}
						}
						if (correosConError.Count > 0)
						{
							//ENVIO envio = ManejoCorreoDAO.ObtenerEnvio(respAdjunto.IdEnvio);
							envio.IDESTADOENVIO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoEnvioError"]);
							ManejoCorreoDAO.GuardarEnvio(envio);
							descripcion = string.Format(MensajeBLL.ObtenerMensaje("MensajeErrorProblemaAlEnviarCorreos", "MENSAJE_ERROR"),
																								envio.IDENVIO,
																								string.Join(",", correosConError));
							Log.RegistrarWarning(descripcion, envio.IDUSUARIO);
						}
						else
						{
							envio.IDESTADOENVIO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoEnvioEnviado"]);
							ManejoCorreoDAO.GuardarEnvio(envio);
							descripcion = string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoEnvioGlobalOk", "MENSAJE_INFORMACION"),
																							envio.IDENVIO,
																							direccionesPara,
																							cc,
																							cco,
																							plantilla.ASUNTO,
																							cuerpoGral,
																							adjuntos.Count,
																							Utils.BytesToString(adjuntos.Sum(i => i.Value.Length)),
																							cids.Count,
																							Utils.BytesToString(cids.Sum(c => c.Value.Length)));
						}
					}
					catch (DuplicateNameException dex)
					{
						string descripcion = string.Format(MensajeBLL.ObtenerMensaje("MensajeErrorHayDescriptoresDuplicados", "MENSAJE_ERROR"),
																							envio.IDENVIO,
																							rutaFuenteDatos,
																							bool.Parse(ConfigurationManager.AppSettings["RutaAdjuntosEsWeb"]) ? MensajeBLL.ObtenerMensaje("MensajeRutaFuenteDatosEnServWeb", "MENSAJE_ERROR") : string.Empty);
						Log.RegistrarError(dex, descripcion, envio.IDUSUARIO);
					}
				}
			}
			catch (Exception ex)
			{
				Log.RegistrarError(ex, ex.Message);
			}
			if (File.Exists(rutaArchivoControlProceso))
			{
				File.Delete(rutaArchivoControlProceso);
				Log.RegistrarInfo("Finalizando el proceso");
			}
			//leer todos los registros de tabla ENVIO que esten con el estado 2 (Listo para enviar)
			//por cada uno extraer el excel de fuente de datos del disco con el codigo de envio
			//llamar a ManejoCorreoBL.EnviarCorreo
			//en ManejoCorreoBL.EnviarCorreo traspasar la logica de lectura del excel y envio de correo aca. Dejar solo la parte que
			//guarda el envio y los adjuntos. El envio debe quedar con estado creado (1) si se apreta guardar y la plantilla se debe guardar 
			//como borrador (1) al guardar. Si se aprieta enviar el envio debe quedar con estado Listo Para Enviar (2)
			//creando un nuevo registro si angular pasa IdEnvio = 0 y actualizando si Angular pasa IdEnvio != 0
			//y luego este proceso lo toma y crea los registros en tabla corrreo junto con el envio y al enviar todo correctamente
			//marca el envio como Enviado (3). Si hay error al enviar correo, el envio queda marcado como error (4)
			//preguntar si al presionar nuevamente enviar si se envia a todos de nuevo o a los que fallaron. Según yo, 
			//lo normal es que la persona si apreto enviar es porque quiere enviar todo de nuevo.
			//En todo caso se pregunta si no existe un registro de correo dado in Iddestinatario e IdEnvio que este En estado EnvioOK (2)
			//y si no existe se reenvia el correo con la info ya existente, de lo contrario se crea un nuevo registro en tabla CORREO
			//con estado creado (1) al principio y enviado (2) al tener exito (o 3=error si fracaso)
		}

		/// <summary>
		/// método que pretendía leer los correos desde la casilla de salida de éstos para ver si habían rebotes pero quedó descartado ya que
		/// TAURUS tiene eliminado el protocolo POP3 y no se ha llegado a consenso sobre de que forma notificar al sistema de notificaciones
		/// hay un rebote.
		/// </summary>
		public void ProcesarEstadoRecepcionEnvios()
		{
			/*
			try
			{
				string rutaArchivoControlRebotes = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "corriendoProcesoRebotes.txt";
				if (File.Exists(rutaArchivoControlRebotes))
				{
					return;
				}
				else
				{
					FileStream fs = File.Create(rutaArchivoControlRebotes);
					fs.Close();
				}
				
				using (Pop3Client client = new Pop3Client())
				{
					//aca debiera haber un for sobre tabla SISTEMA_EMISOR y leer los nombres de usuario y clave para la casilla
					//para poder conectarse
					int idCorreo;
					HashSet<int> enviosFallidos = new HashSet<int>();
					client.Connect(ConfigurationManager.AppSettings["ServidorCorreoPOP3"],
												Convert.ToInt32(ConfigurationManager.AppSettings["PuertoCorreoPOP3"]),
												false);
					client.Authenticate(ConfigurationManager.AppSettings["UsuarioCorreo"], ConfigurationManager.AppSettings["ClaveCorreo"]);
					int messageCount = client.GetMessageCount();
					for (int i = 1; i <= messageCount; i++)
					{
						Message message = client.GetMessage(i);
						if (message.FindAllTextVersions().Count > 1)
						{
							string bodyMensajeOriginal = Encoding.UTF8.GetString(message.FindAllTextVersions()[1].Body);
							Match matchIdCorreo = Regex.Match(bodyMensajeOriginal, ConfigurationManager.AppSettings["ExprRegularContentIdCorreoFallido"]);
							if (matchIdCorreo != null && matchIdCorreo.Success)
							{
								if (message.Headers.Subject.ToLower().Contains("failure"))
								{
									idCorreo = Convert.ToInt32(matchIdCorreo.Groups[1].Value);
									CORREO correo = ManejoCorreoDAO.ObtenerCorreo(idCorreo);
									if (correo != null && correo.IDDESTINATARIO != null)
									{
										correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoError"]);
										ManejoCorreoDAO.GuardarCorreo(correo);
										enviosFallidos.Add((int)correo.IDENVIO);
										DESTINATARIO destinatario = ManejoCorreoDAO.ObtenerDestinatario((int)correo.IDDESTINATARIO);
										Log.RegistrarWarning(string.Format("No se pudo enviar el correo para el envio {0} y al correo {1}", correo.IDENVIO, destinatario.CASILLACORREO));
									}
								}
							}
						}
					}
					foreach (int idEnvio in enviosFallidos)
					{
						ENVIO envio = ManejoCorreoDAO.ObtenerEnvio(idEnvio);
						envio.IDESTADOENVIO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoEnvioError"]);
						ManejoCorreoDAO.GuardarEnvio(envio);
					}
				}
				if (File.Exists(rutaArchivoControlRebotes))
				{
					File.Delete(rutaArchivoControlRebotes);
				}
			}
			catch (Exception ex)
			{
				Log.RegistrarError(ex, ex.Message);
			}
			*/
		}

		/*
		public static void RestaurarImagenesCorreo()
		{
			FiltroGrillaCorreos filtro = new FiltroGrillaCorreos();
			filtro.nroPagina = 1;
			filtro.tamanoPagina = 0;
			filtro.fecha = DateTime.MinValue.ToString(ConfigurationManager.AppSettings["FormatoFechaDisplay"], CultureInfo.InvariantCulture);
			filtro.nombreUsuario = string.Empty;
			filtro.asunto = string.Empty;
			int cantResults = 0;
			List<CORREO> correos = CorreoDAO.FiltrarCorreos(filtro, out cantResults);
			foreach (CORREO correo in correos)
			{
				PLANTILLA_CORREO plantilla = ManejoCorreoDAO.ObtenerPlantillaCorreo((int)correo.ENVIO.IDPLANTILLACORREO);
				string cuerpoCorreoConImagenes = plantilla.CUERPO;
				MD5 md5 = MD5.Create();
				MatchCollection matchesImgs = null;
				Dictionary<string,string> tagsImagenes = new Dictionary<string,string>();
				while ((matchesImgs = Regex.Matches(cuerpoCorreoConImagenes, ConfigurationManager.AppSettings["ExprRegularImagenes"])).Count > 0)
				//foreach (Match match in matchesImgs)
				{
					Match match = matchesImgs[0];
					string base64Img = match.Groups[1].Value;
					string someAttributes = match.Groups[2].Value;
					string imgName = Utils.ByteArrayToString(md5.ComputeHash(Encoding.UTF8.GetBytes(base64Img)));
					string imgHtml = string.Format(ConfigurationManager.AppSettings["ExprRegularImagenesCids"], imgName);
					byte[] byteImg = Convert.FromBase64String(base64Img);
					cuerpoCorreoConImagenes = cuerpoCorreoConImagenes.Substring(0, match.Index) + imgHtml + cuerpoCorreoConImagenes.Substring(match.Index + match.Length);
					tagsImagenes.Add(imgHtml, match.Value);
				}

				foreach (string imgHtml in tagsImagenes.Keys)
				{
					if (correo.IDCORREO == 1097)
					{
						;
					}
					//correo.CUERPO = Regex.Replace(correo.Cu.Replace(tagsImagenes[imgHtml], imgHtml);
					correo.CUERPO_VISUALIZACION = Regex.Replace(correo.CUERPO_VISUALIZACION, imgHtml, tagsImagenes[imgHtml]);
				}
				ManejoCorreoDAO.GuardarCorreo(correo);
			}
		}
		*/
	}
}
