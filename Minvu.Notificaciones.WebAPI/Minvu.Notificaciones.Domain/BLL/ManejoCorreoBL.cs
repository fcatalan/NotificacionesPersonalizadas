
using Excel;
using Minvu.Notificaciones.Domain.Util;
using Minvu.Notificaciones.DTO;
using Minvu.Notificaciones.IData.DAO;
using Minvu.Notificaciones.IData.Log;
using Minvu.Notificaciones.IData.ORM;
using Minvu.Notificaciones.Personalizadas.Entidades;
using Minvu.Notificaciones.Personalizadas.Entidades.RespuestasJSON;
using Minvu.Security;
using Minvu.Security.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace Minvu.Notificaciones.Domain.BLL
{
	public class ManejoCorreoBL
	{
		/// <summary>
		/// tipos de formatos de excel soportados
		/// </summary>
		private enum FormatoExcel { Excel2003, Excel2007oSuperior, Invalido };

		private static RespuestaConversionFuenteDatosDTO GuardarFuenteDatos(string fuenteDatos, int idEnvio, string nombreUsuario)
		{
			RespuestaConversionFuenteDatosDTO resp = new RespuestaConversionFuenteDatosDTO();
			resp.CodError = 0;
			resp.MsjError = null;
			FormatoExcel formatoExcel = FormatoExcel.Invalido;
			byte[] byteArrExcel = Convert.FromBase64String(fuenteDatos);
			string rutaFuenteDatos = string.Empty;
			if (bool.Parse(ConfigurationManager.AppSettings["RutaAdjuntosEsWeb"]))
			{
				rutaFuenteDatos = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["RutaAdjuntos"]);
			}
			else
			{
				rutaFuenteDatos = ConfigurationManager.AppSettings["RutaAdjuntos"];
			}
			rutaFuenteDatos += Convert.ToString(Path.DirectorySeparatorChar) + idEnvio;
			rutaFuenteDatos += Convert.ToString(Path.DirectorySeparatorChar) + "fuenteDatos";

			MemoryStream ms = new MemoryStream(byteArrExcel);
			IExcelDataReader reader = ExcelReaderFactory.CreateBinaryReader(ms);
			if (reader.IsValid) formatoExcel = FormatoExcel.Excel2003;
			reader.Close();
			ms.Close();
			ms = new MemoryStream(byteArrExcel);
			reader = ExcelReaderFactory.CreateOpenXmlReader(ms);
			if (reader.IsValid) formatoExcel = FormatoExcel.Excel2007oSuperior;
			reader.Close();
			ms.Close();
			ms = new MemoryStream(byteArrExcel);
			string extensionExcel = string.Empty;
			switch (formatoExcel)
			{
				case FormatoExcel.Invalido:
					resp.CodError = 6;
					resp.MsjError = MensajeBLL.ObtenerMensaje("MensajeRutaExcelInvalido", "MENSAJE_ERROR");
					return resp;
				case FormatoExcel.Excel2003:
					extensionExcel = ".xls";
					reader = ExcelReaderFactory.CreateBinaryReader(ms);
					break;
				case FormatoExcel.Excel2007oSuperior:
					extensionExcel = ".xlsx";
					reader = ExcelReaderFactory.CreateOpenXmlReader(ms);
					break;
			}
			reader.IsFirstRowAsColumnNames = true;
			DataSet ds = reader.AsDataSet();
			if (ds.Tables["descriptores"] == null)
			{
				resp.CodError = 7;
				resp.MsjError = MensajeBLL.ObtenerMensaje("MensajeErrorFaltaHojaDescriptores", "MENSAJE_ERROR");
				return resp;
			}
			if (ds.Tables["CC-CCO"] == null)
			{
				resp.CodError = 8;
				resp.MsjError = MensajeBLL.ObtenerMensaje("MensajeErrorFaltaHojaCCCCO", "MENSAJE_ERROR");
				return resp;
			}
			if (ds.Tables["descriptores"].Rows.Count == 0)
			{
				resp.CodError = 9;
				resp.MsjError = MensajeBLL.ObtenerMensaje("MensajeErrorNoHayDestinatarios", "MENSAJE_ERROR");
				return resp;
			}
			if (ds.Tables["descriptores"].Columns.Count == 0)
			{
				resp.CodError = 10;
				resp.MsjError = MensajeBLL.ObtenerMensaje("MensajeErrorFaltaColumnaDestinatarios", "MENSAJE_ERROR");
				return resp;
			}

			Directory.CreateDirectory(rutaFuenteDatos);
			//rutaFuenteDatos += Convert.ToString(Path.DirectorySeparatorChar) + "fuenteDatos.xls";
			rutaFuenteDatos += Path.DirectorySeparatorChar + "fuenteDatos" + extensionExcel;
			FileStream fs = new FileStream(rutaFuenteDatos, FileMode.Create);
			fs.Write(byteArrExcel, 0, byteArrExcel.Length);
			fs.Close();
			Log.RegistrarInfo(string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoFuenteDeDatosGuardada", "MENSAJE_INFORMACION"),
													rutaFuenteDatos), nombreUsuario);

			resp.dataset = ds;
			resp.byteArray = byteArrExcel;
			return resp;
		}

		/// <summary>
		/// Guarda solamente la plantilla del correo, NO GUARDA en tabla CORREO.
		/// Esto es para guardar un borrador
		/// <param name="envioCorreoDTO">Objeto que guarda el asunto, cuerpo y direcciones de correo (cc, cco, para) 
		/// ademas del codigo de plantilla. codigo de sistema emisor y si se usan fuentes de datos. 
		/// Se usa para mapear la entrada del formulario de envio de correo</param>
		/// <returns>Un objeto con el ID de plantilla guardado, y campos para controlar errores (CodError=0 : Sin error )<returns/>
		/// </summary>
		public static RespuestaPlantilla GuardarCorreo(EnvioDTO envioCorreoDTO)
		{
			RespuestaPlantilla respGenerica = new RespuestaPlantilla();
			string nombreUsuario = string.Empty;
			try
			{
				Ticket tic = SingleSignOn.Authenticate();
				nombreUsuario = tic.MinvuPrincipal.MinvuIdentity.UserName;
				//switch (envioCorreoDTO.PaginaOrigen)
				//{
				//case EnvioDTO.PaginaOrigenEnum.NUEVO_CORREO_SIN_PLANTILLA:
				PLANTILLA_CORREO plantillaCorreo = new PLANTILLA_CORREO();
				plantillaCorreo.IDPLANTILLACORREO = envioCorreoDTO.IdPlantilla; //al principio viene en 0, la segunda vez que se apreta guardar viene con valor

				if (plantillaCorreo.IDPLANTILLACORREO != 0)
				{
					PLANTILLA_CORREO plantillaExistente = ManejoCorreoDAO.ObtenerPlantillaCorreo(plantillaCorreo.IDPLANTILLACORREO);
					plantillaCorreo.FECHACREACION = plantillaExistente.FECHACREACION;
				}
				else
				{
					plantillaCorreo.FECHACREACION = DateTime.Now;
					plantillaCorreo.IDESTADOPLANTILLACORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoPlantillaBorrador"]);
				}
				int largoMaximoNombrePlantilla = ContextoBD.GetMaxLength<PLANTILLA_CORREO>(plantilla => plantilla.NOMBRE);
				int largoRestanteAsuntoNombrePlantilla = largoMaximoNombrePlantilla - ConfigurationManager.AppSettings["FormatoFechaDisplayConHora"].Length - 3;

				plantillaCorreo.ASUNTO = envioCorreoDTO.Asunto;
				plantillaCorreo.CUERPO = envioCorreoDTO.Cuerpo;
				plantillaCorreo.IDUSUARIO = nombreUsuario;
				string asuntoCortado = plantillaCorreo.ASUNTO.Length <= largoRestanteAsuntoNombrePlantilla ? plantillaCorreo.ASUNTO : plantillaCorreo.ASUNTO.Substring(0, largoRestanteAsuntoNombrePlantilla);
				plantillaCorreo.NOMBRE = asuntoCortado + " - " + ((DateTime)plantillaCorreo.FECHACREACION).ToString(ConfigurationManager.AppSettings["FormatoFechaDisplayConHora"], CultureInfo.InvariantCulture);
				plantillaCorreo.IDESTADOPLANTILLACORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoPlantillaBorrador"]); //guardar como borrador
				plantillaCorreo.IDSISTEMAEMISOR = envioCorreoDTO.IdSistemaEmisor;
				ManejoCorreoDAO.GuardarPlantilla(plantillaCorreo);
				respGenerica.CodError = 0;
				respGenerica.IdPlantilla = plantillaCorreo.IDPLANTILLACORREO;
				respGenerica.MsjError = null;
				Log.RegistrarInfo(string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoPlantillaGuardadaBorrador", "MENSAJE_INFORMACION"),
																				nombreUsuario,
																				(envioCorreoDTO.IdPlantilla == 0 ? "creado" : "guardado"),
																				plantillaCorreo.NOMBRE,
																				plantillaCorreo.IDPLANTILLACORREO),
																				nombreUsuario);
			}
			catch (Exception ex)
			{
				Log.RegistrarError(ex, ex.Message, nombreUsuario);
				respGenerica.CodError = 1;
				respGenerica.IdPlantilla = 0;
				respGenerica.MsjError = MensajeBLL.ObtenerMensaje("MensajeErrorCrearPlantilla", "MENSAJE_ERROR");
			}
			return respGenerica;
		}

		/// <summary>
		/// Guarda los adjuntos, la plantilla, envío y fuente de datos. 
		/// por como está construido el modelo. La plantilla la guarda como borrador. Le coloca
		/// un nombre recortado armado con el asunto y fecha a la plantilla. Los adjuntos los
		/// guarda en una carpeta definida en el web.config (RutaAdjuntos). Dicha ruta puede ser
		/// relativa al sitio (web.config RutaAdjuntosEsWeb="true") o absoluta (RutaAdjuntosEsWeb="false")
		/// <param name="envioCorreoDTO">Al igual que GuardarCorreo(...) es el mapeo de la entrada del formulario
		/// de la pantalla de envío de correo<</param>
		/// <returns>Objeto con los ids de los adjuntos, id de plantilla, id de envio y campos para controlar errores (CodError:0 = sin error)</returns>
		/// </summary>
		private static RespuestaDatosCorreo GuardarDatosCorreo(EnvioDTO envioCorreoDTO)
		{
			string nombreUsuario = string.Empty;
			RespuestaDatosCorreo respDatosCorreo = new RespuestaDatosCorreo();
			try
			{
				#region Guardando datos correo
				int idPlantillaOld = 0;
				Ticket tic = SingleSignOn.Authenticate();
				Log.RegistrarInfo("Obteniendo informacion Ticket");
				nombreUsuario = tic.MinvuPrincipal.MinvuIdentity.UserName;
				Log.RegistrarInfo("Nombre usuario desde Ticket:" + nombreUsuario);


				PLANTILLA_CORREO plantillaCorreo = new PLANTILLA_CORREO();
				plantillaCorreo.IDPLANTILLACORREO = envioCorreoDTO.IdPlantilla; //al principio viene en 0, la segunda vez que se apreta guardar viene con valor
				plantillaCorreo.IDSISTEMAEMISOR = envioCorreoDTO.IdSistemaEmisor;

				idPlantillaOld = plantillaCorreo.IDPLANTILLACORREO;
				if (plantillaCorreo.IDPLANTILLACORREO != 0)
				{
					PLANTILLA_CORREO plantillaExistente = ManejoCorreoDAO.ObtenerPlantillaCorreo(plantillaCorreo.IDPLANTILLACORREO);
					plantillaCorreo.FECHACREACION = plantillaExistente.FECHACREACION;
				}
				else
				{
					plantillaCorreo.FECHACREACION = DateTime.Now;
				}

				int largoMaximoNombrePlantilla = ContextoBD.GetMaxLength<PLANTILLA_CORREO>(plantilla => plantilla.NOMBRE);
				int largoRestanteAsuntoNombrePlantilla = largoMaximoNombrePlantilla - ConfigurationManager.AppSettings["FormatoFechaDisplayConHora"].Length - 3;
				plantillaCorreo.ASUNTO = envioCorreoDTO.Asunto;
				plantillaCorreo.CUERPO = envioCorreoDTO.Cuerpo;
				string asuntoCortado = plantillaCorreo.ASUNTO.Length <= largoRestanteAsuntoNombrePlantilla ? plantillaCorreo.ASUNTO : plantillaCorreo.ASUNTO.Substring(0, largoRestanteAsuntoNombrePlantilla);
				plantillaCorreo.IDUSUARIO = nombreUsuario;//tic.MinvuPrincipal.MinvuIdentity.UserName;
				plantillaCorreo.NOMBRE = asuntoCortado + " - " + ((DateTime)plantillaCorreo.FECHACREACION).ToString(ConfigurationManager.AppSettings["FormatoFechaDisplayConHora"], CultureInfo.InvariantCulture);
				plantillaCorreo.IDESTADOPLANTILLACORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoPlantillaBorrador"]); //guardar como borrador

				ManejoCorreoDAO.GuardarPlantilla(plantillaCorreo);
				Log.RegistrarInfo(string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoPlantillaGuardadaBorrador", "MENSAJE_INFORMACION"),
																				nombreUsuario,
																				(idPlantillaOld == 0 ? "creado" : "guardado"),
																				plantillaCorreo.NOMBRE, 
																				plantillaCorreo.IDPLANTILLACORREO),
																				nombreUsuario);
				respDatosCorreo.IdPlantilla = plantillaCorreo.IDPLANTILLACORREO;

				ENVIO envioCorreo = new ENVIO();
				envioCorreo.IDENVIO = envioCorreoDTO.IdEnvio;
				envioCorreo.IDPLANTILLACORREO = plantillaCorreo.IDPLANTILLACORREO;
				//envioCorreo.IDSISTEMAEMISOR = envioCorreoDTO.IdSistemaEmisor;

				envioCorreo.IDUSUARIO = nombreUsuario;//tic.MinvuPrincipal.MinvuIdentity.UserName;
				envioCorreo.FECHAHORA = null;
				envioCorreo.IDESTADOENVIO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoEnvioCreado"]); //creado
				if (!envioCorreoDTO.UsarDireccionesFuenteDatos)
				{
					envioCorreo.CONCOPIA = envioCorreoDTO.CC;
					envioCorreo.CONCOPIAOCULTA = envioCorreoDTO.CCo;
				}

				ManejoCorreoDAO.GuardarEnvio(envioCorreo);
				ESTADO_ENVIO estadoEnvio = EstadoDAO.ObtenerEstadoEnvio(Convert.ToInt32(envioCorreo.IDESTADOENVIO));
				Log.RegistrarInfo(string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoEnvioGuardadoComoCreado", "MENSAJE_INFORMACION"),
																				nombreUsuario, 
																				envioCorreo.IDENVIO,
																				estadoEnvio.DESCRIPCION), 
																				envioCorreo.IDUSUARIO);
				#endregion

				#region Guardando Fuente de datos
				respDatosCorreo.IdEnvio = envioCorreo.IDENVIO;
				respDatosCorreo.IdAdjuntos = new List<int>();

				RespuestaConversionFuenteDatosDTO respConversionFteDatos = null;
				if (envioCorreoDTO.FuenteDatos != null && envioCorreoDTO.UsarDireccionesFuenteDatos)
				{
					respConversionFteDatos = GuardarFuenteDatos(envioCorreoDTO.FuenteDatos, respDatosCorreo.IdEnvio, nombreUsuario);
					if (respConversionFteDatos.CodError != 0)
					{
						respDatosCorreo.CodError = respConversionFteDatos.CodError;
						respDatosCorreo.MsjError = respConversionFteDatos.MsjError;
						return respDatosCorreo;
					}
				}
				else if (envioCorreoDTO.UsarDireccionesFuenteDatos && envioCorreoDTO.FuenteDatos == null)
				{
					respDatosCorreo.CodError = 11;
					respDatosCorreo.MsjError = MensajeBLL.ObtenerMensaje("MensajeErrorFaltaFuenteDatos", "MENSAJE_ERROR");
					return respDatosCorreo;
				}
				#endregion

				#region validando palabas prohibidas
				List<PalabraProhibidaDTO> palabrasProhibidas = PalabraProhibidaDAO.ObtenerListaPalabras();
				HashSet<string> palabrasProhibidasEncontradasAsunto = new HashSet<string>();
				HashSet<string> palabrasProhibidasEncontradasCuerpo = new HashSet<string>();
				HashSet<string> palabrasProhibidasEncontradasFteDatos = new HashSet<string>();
				string cuerpoUpperCase = envioCorreoDTO.Cuerpo;
				List<string> tagsImagenes = new List<string>();
				MatchCollection matchesAlt = null, matchesTitle = null;
				MD5 md5 = MD5.Create();
				Dictionary<string, byte[]> cids = new Dictionary<string, byte[]>();
				string cuerpoGral = envioCorreoDTO.Cuerpo;
				/*
				while ((matchesImgs = Regex.Matches(cuerpoGral, ConfigurationManager.AppSettings["ExprRegularImagenes"])).Count > 0)
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
				foreach (string imgHtml in tagsImagenes)
				{
					cuerpoUpperCase = cuerpoUpperCase.Replace(imgHtml, string.Empty);
				}
				*/
				//cuerpoUpperCase = cuerpoUpperCase.ToUpper();
				string asuntoUpperCase = envioCorreoDTO.Asunto.ToUpper();

				//validacion de palabras prohibidas en asunto
				foreach (PalabraProhibidaDTO palabraProhibida in palabrasProhibidas)
				{
					if (asuntoUpperCase.Contains(palabraProhibida.Palabra.ToUpper()))
					{
						palabrasProhibidasEncontradasAsunto.Add(palabraProhibida.Palabra);
					}
				}

				//buscando texto en tags Alt y Title de imagenes que pueden verse si no aparece la imagen o si se posiciona el cursor sobre la imagen
				matchesAlt = Regex.Matches(cuerpoUpperCase, ConfigurationManager.AppSettings["ExprRegularTagAlt"]);
				matchesTitle = Regex.Matches(cuerpoUpperCase, ConfigurationManager.AppSettings["ExprRegularTagTitle"]);
				foreach (PalabraProhibidaDTO palabraProhibida in palabrasProhibidas)
				{
					foreach(Match match in matchesAlt)
					{
						if (match.Value.ToUpper().Contains(palabraProhibida.Palabra.ToUpper()))
						{
							palabrasProhibidasEncontradasCuerpo.Add(palabraProhibida.Palabra);
						}
					}
					foreach (Match match in matchesTitle)
					{
						if (match.Value.ToUpper().Contains(palabraProhibida.Palabra.ToUpper()))
						{
							palabrasProhibidasEncontradasCuerpo.Add(palabraProhibida.Palabra);
						}
					}
				}

				//luego que se validan los atributos visibles de imagen se valida lo demas sacando las imagenes de la validacion
				//(pues el base64 puede tener palabras prohibidas pero no es visible)
				cuerpoUpperCase = Regex.Replace(cuerpoUpperCase, ConfigurationManager.AppSettings["ExprRegularImagenes"], string.Empty);
				cuerpoUpperCase = cuerpoUpperCase.ToUpper();

				//validacion de palabras prohibidas en cuerpo
				foreach (PalabraProhibidaDTO palabraProhibida in palabrasProhibidas)
				{
					if (cuerpoUpperCase.Contains(palabraProhibida.Palabra.ToUpper()))
					{
						palabrasProhibidasEncontradasCuerpo.Add(palabraProhibida.Palabra);
					}
				}
				
				//validacion de palabras prohibidas en fuente de datos
				if (envioCorreoDTO.FuenteDatos != null && envioCorreoDTO.UsarDireccionesFuenteDatos)
				{
					DataSet ds = respConversionFteDatos.dataset;
					DataTable dtDescriptores = ds.Tables["descriptores"];
					DataRow[] drs = dtDescriptores.Select(dtDescriptores.Columns[0].ColumnName + " <> ''");
					foreach (DataRow dr in drs)
					{
						for(int i=1;i<dtDescriptores.Columns.Count;i++)
						{
							foreach (PalabraProhibidaDTO palabraProhibida in palabrasProhibidas)
							{
								if (dr[i] != null && Convert.ToString(dr[i]).ToUpper().Contains(palabraProhibida.Palabra.ToUpper()))
								{
									palabrasProhibidasEncontradasFteDatos.Add(palabraProhibida.Palabra);
								}
							}
						}
					}
				}

				string mensajeHayPalabrasProhibidasEnAsunto = MensajeBLL.ObtenerMensaje("MensajeErrorHayPalabrasProhibidasEnAsunto", "MENSAJE_ERROR");
				string mensajeHayPalabrasProhibidasEnCuerpo = MensajeBLL.ObtenerMensaje("MensajeErrorHayPalabrasProhibidasEnCuerpo", "MENSAJE_ERROR");
				string mensajeHayPalabrasProhibidasEnFteDatos = MensajeBLL.ObtenerMensaje("MensajeErrorHayPalabrasProhibidasEnFuenteDatos", "MENSAJE_ERROR");

				bool hayPalabrasProhibidas = false;

				if (palabrasProhibidasEncontradasAsunto.Count > 0)
				{
					hayPalabrasProhibidas = true;
					mensajeHayPalabrasProhibidasEnAsunto = string.Format(mensajeHayPalabrasProhibidasEnAsunto, string.Join(", ", palabrasProhibidasEncontradasAsunto));
					EVENTO evento = new EVENTO();
					evento.IDUSUARIO = nombreUsuario;
					evento.FECHA = DateTime.Now;
					evento.DESCRIPCION = mensajeHayPalabrasProhibidasEnAsunto;
				} 

				if (palabrasProhibidasEncontradasCuerpo.Count > 0)
				{
					hayPalabrasProhibidas = true;
					mensajeHayPalabrasProhibidasEnCuerpo = string.Format(mensajeHayPalabrasProhibidasEnCuerpo, string.Join(", ", palabrasProhibidasEncontradasCuerpo));
					EVENTO evento = new EVENTO();
					evento.IDUSUARIO = nombreUsuario;
					evento.FECHA = DateTime.Now;
					evento.DESCRIPCION = mensajeHayPalabrasProhibidasEnCuerpo;
				} 

				if (palabrasProhibidasEncontradasFteDatos.Count > 0)
				{
					hayPalabrasProhibidas = true;
					mensajeHayPalabrasProhibidasEnFteDatos = string.Format(mensajeHayPalabrasProhibidasEnFteDatos, string.Join(", ", palabrasProhibidasEncontradasFteDatos));
					EVENTO evento = new EVENTO();
					evento.IDUSUARIO = nombreUsuario;
					evento.FECHA = DateTime.Now;
					evento.DESCRIPCION = mensajeHayPalabrasProhibidasEnFteDatos;
				} 

				if (hayPalabrasProhibidas)
				{
					respDatosCorreo.CodError = 12;
					string plantillaMensaje = MensajeBLL.ObtenerMensaje("PlantillaMensajeHayPalabrasProhibidas", "MENSAJE_ERROR");
					string plantillaItemMensaje = MensajeBLL.ObtenerMensaje("PlantillaMensajeItemHayPalabrasProhibidas", "MENSAJE_ERROR");
					string mensajesErrorPalabras = string.Empty;
					if (palabrasProhibidasEncontradasAsunto.Count > 0)
						mensajesErrorPalabras += string.Format(plantillaItemMensaje, mensajeHayPalabrasProhibidasEnAsunto);
					if (palabrasProhibidasEncontradasCuerpo.Count > 0)
						mensajesErrorPalabras += string.Format(plantillaItemMensaje, mensajeHayPalabrasProhibidasEnCuerpo);
					if (palabrasProhibidasEncontradasFteDatos.Count > 0)
						mensajesErrorPalabras += string.Format(plantillaItemMensaje, mensajeHayPalabrasProhibidasEnFteDatos);
					plantillaMensaje = string.Format(plantillaMensaje, mensajesErrorPalabras);
					respDatosCorreo.MsjError = plantillaMensaje;
					return respDatosCorreo;
				}
				#endregion

				#region guardando adjuntos
				if (envioCorreoDTO.Adjuntos != null && envioCorreoDTO.Adjuntos.items.Count > 0)
				{
					List<string>keys = envioCorreoDTO.Adjuntos.items.Keys.ToList();
					foreach (string nombreAdjunto in keys)
					{
						ADJUNTO adjunto = new ADJUNTO();
						adjunto.IDADJUNTO = 0;
						adjunto.IDENVIO = envioCorreo.IDENVIO;
						adjunto.NOMBRE = nombreAdjunto;
						string rutaAdjuntos = string.Empty;
						string rutaFuenteDatos = string.Empty;
						if (bool.Parse(ConfigurationManager.AppSettings["RutaAdjuntosEsWeb"]))
						{
							rutaAdjuntos = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["RutaAdjuntos"]);
						}
						else
						{
							rutaAdjuntos = ConfigurationManager.AppSettings["RutaAdjuntos"];
						}
						rutaAdjuntos += Convert.ToString(Path.DirectorySeparatorChar) + envioCorreo.IDENVIO;
						Directory.CreateDirectory(rutaAdjuntos);
						FileStream fs = File.Create(rutaAdjuntos + Path.DirectorySeparatorChar + nombreAdjunto);
						byte[] byteArr = Convert.FromBase64String(envioCorreoDTO.Adjuntos.items[nombreAdjunto]);
						fs.Write(byteArr, 0, byteArr.Length);
						fs.Close();
						adjunto.RUTA = rutaAdjuntos + Path.DirectorySeparatorChar + nombreAdjunto;
						adjunto.TAMANIO = byteArr.Length;
						ADJUNTO adjuntoExistente = ManejoCorreoDAO.ObtenerAdjunto(adjunto.NOMBRE, (int)adjunto.IDENVIO);
						if (adjuntoExistente != null) adjunto.IDADJUNTO = adjuntoExistente.IDADJUNTO;
						ManejoCorreoDAO.GuardarAdjunto(adjunto);
						respDatosCorreo.IdAdjuntos.Add(adjunto.IDADJUNTO);
						Log.RegistrarInfo(string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoAdjuntoGuardado", "MENSAJE_INFORMACION"),
																adjunto.NOMBRE, adjunto.RUTA, Utils.BytesToString(byteArr.Length)), envioCorreo.IDUSUARIO);

						if (bool.Parse(ConfigurationManager.AppSettings["RutaAdjuntosEsWeb"]))
						{
							rutaAdjuntos = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["RutaAdjuntos"]);
						}
						else
						{
							rutaAdjuntos = ConfigurationManager.AppSettings["RutaAdjuntos"];
						}
						rutaAdjuntos += Convert.ToString(Path.DirectorySeparatorChar) + respDatosCorreo.IdEnvio;
						envioCorreoDTO.Adjuntos.items[nombreAdjunto] = rutaAdjuntos + Path.DirectorySeparatorChar + nombreAdjunto;
						/*
						if (envioCorreoDTO.Adjuntos != null && envioCorreoDTO.Adjuntos.items.Count > 0)
						{
							//List<string> nombresArchivos = envioCorreoDTO.Adjuntos.items.Keys.ToList();
							foreach (string nombreArchivo in nombresArchivos)
							{
								envioCorreoDTO.Adjuntos.items[nombreArchivo] = rutaAdjuntos + Path.DirectorySeparatorChar + nombreArchivo;
							}
						}
						*/
					}
				}
				#endregion
				respDatosCorreo.CodError = 0;
				respDatosCorreo.MsjError = null;
			}
			catch (Exception ex)
			{
				Log.RegistrarError(ex, ex.Message, nombreUsuario);
				if (respDatosCorreo.IdPlantilla == 0) respDatosCorreo.CodError = 1;
				else if (respDatosCorreo.IdEnvio == 0) respDatosCorreo.CodError = 2;
				else respDatosCorreo.CodError = 3;
				respDatosCorreo.MsjError = MensajeBLL.ObtenerMensaje("MensajeErrorCrearPlantilla", "MENSAJE_ERROR");
			}
			return respDatosCorreo;
		}

		/// <summary>
		/// Deja guardado el correo (en tabla CORREO) listo para ser procesado por ProcesoEnvioCorreoBL.ProcesarEnvios()
		/// Guarda la fuente de datos en disco para que ese método lo tome y haga los reemplazos de descriptores. También
		/// guarda en disco el request json para tener
		/// <param name="envioCorreoDTO">Información del formulario de la página de envío de correos de angular</param>
		/// <returns>Respuesta con un código de error y mensjae de error (CodError = 0 : sin error)</returns>
		/// </summary>
		public static RespuestaGenerica EnviarCorreo(EnvioDTO envioCorreoDTO)
		{
			string nombreUsuario = string.Empty;
			RespuestaGenerica respGenerica = new RespuestaGenerica();
			try
			{
				Ticket tic = SingleSignOn.Authenticate();
				nombreUsuario = tic.MinvuPrincipal.MinvuIdentity.UserName;
				//guarda una plantilla, envio y adjuntos si existen
				RespuestaDatosCorreo respGuardarDatos = GuardarDatosCorreo(envioCorreoDTO);
				respGenerica.CodError = respGuardarDatos.CodError;
				respGenerica.MsjError = respGuardarDatos.MsjError;

				if (respGenerica.CodError != 0)
				{
					return respGenerica;
				}
				//if (respAdjunto.CodError == 0)
				//{
					//if (envioCorreoDTO.FuenteDatos != null)
					//{
						//byte[] byteArrExcel = Convert.FromBase64String(envioCorreoDTO.FuenteDatos);
						//MemoryStream ms = new MemoryStream(byteArrExcel);
						//IExcelDataReader reader = ExcelReaderFactory.CreateBinaryReader(ms);
						//if (reader.IsValid) formatoExcel = FormatoExcel.Excel2003;
						//reader.Close();
						//ms.Close();
						//ms = new MemoryStream(byteArrExcel);
						//reader = ExcelReaderFactory.CreateOpenXmlReader(ms);
						//if (reader.IsValid) formatoExcel = FormatoExcel.Excel2007oSuperior;
						//reader.Close();
						//ms.Close();
						//ms = new MemoryStream(byteArrExcel);
						//string extensionExcel = string.Empty;
						//switch (formatoExcel)
						//{
						//	case FormatoExcel.Invalido:
						//		respGenerica.CodError = 6;
						//		respGenerica.MsjError = MensajeBLL.ObtenerMensaje("MensajeRutaExcelInvalido", "MENSAJE_ERROR");
						//		return respGenerica;
						//	case FormatoExcel.Excel2003:
						//		extensionExcel = ".xls";
						//		reader = ExcelReaderFactory.CreateBinaryReader(ms);
						//		break;
						//	case FormatoExcel.Excel2007oSuperior:
						//		extensionExcel = ".xlsx";
						//		reader = ExcelReaderFactory.CreateOpenXmlReader(ms);
						//		break;
						//}
						//reader.IsFirstRowAsColumnNames = true;
						//RespuestaConversionFuenteDatosDTO respConversionFteDatos = ConvertirFuentedDeDatosEnDataset(envioCorreoDTO.FuenteDatos); 
						//if (respConversionFteDatos.CodError != 0)
						//{
						//	respGenerica.CodError = respConversionFteDatos.CodError;
						//	respGenerica.MsjError = respConversionFteDatos.MsjError;
						//	return respGenerica;
						//}
						//DataSet ds = respConversionFteDatos.dataset;
						//if (ds.Tables["descriptores"] == null)
						//{
						//	respGenerica.CodError = 7;
						//	respGenerica.MsjError = MensajeBLL.ObtenerMensaje("MensajeErrorFaltaHojaDescriptores", "MENSAJE_ERROR");
						//	return respGenerica;
						//}
						//if (ds.Tables["CC-CCO"] == null)
						//{
						//	respGenerica.CodError = 8;
						//	respGenerica.MsjError = MensajeBLL.ObtenerMensaje("MensajeErrorFaltaHojaCCCCO", "MENSAJE_ERROR");
						//	return respGenerica;
						//}
						//if (ds.Tables["descriptores"].Rows.Count == 0)
						//{
						//	respGenerica.CodError = 9;
						//	respGenerica.MsjError = MensajeBLL.ObtenerMensaje("MensajeErrorNoHayDestinatarios", "MENSAJE_ERROR");
						//	return respGenerica;
						//}
						//if (ds.Tables["descriptores"].Columns.Count == 0)
						//{
						//	respGenerica.CodError = 10;
						//	respGenerica.MsjError = MensajeBLL.ObtenerMensaje("MensajeErrorFaltaColumnaDestinatarios", "MENSAJE_ERROR");
						//	return respGenerica;
						//}

						//if (bool.Parse(ConfigurationManager.AppSettings["RutaAdjuntosEsWeb"]))
						//{
						//	rutaFuenteDatos = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["RutaAdjuntos"]);
						//}
						//else
						//{
						//	rutaFuenteDatos = ConfigurationManager.AppSettings["RutaAdjuntos"];
						//}
						//rutaFuenteDatos += Convert.ToString(Path.DirectorySeparatorChar) + respAdjunto.IdEnvio;
						//rutaFuenteDatos += Convert.ToString(Path.DirectorySeparatorChar) + "fuenteDatos";
						//Directory.CreateDirectory(rutaFuenteDatos);
						////rutaFuenteDatos += Convert.ToString(Path.DirectorySeparatorChar) + "fuenteDatos.xls";
						//rutaFuenteDatos += Path.DirectorySeparatorChar + "fuenteDatos" + extensionExcel;
						//FileStream fs = new FileStream(rutaFuenteDatos, FileMode.Create);
						//fs.Write(byteArrExcel, 0, byteArrExcel.Length);
						//fs.Close();
						//Log.RegistrarInfo(string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoFuenteDeDatosGuardada", "MENSAJE_INFORMACION"),
						//										rutaFuenteDatos), nombreUsuario);

				//} else
				//{
				//	respGenerica.CodError = respAdjunto.CodError;
				//	respGenerica.MsjError = respAdjunto.MsjError;
				//	return respGenerica;
				//}

					//string rutaAdjuntos = string.Empty;
					//if (bool.Parse(ConfigurationManager.AppSettings["RutaAdjuntosEsWeb"]))
					//{
					//	rutaAdjuntos = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["RutaAdjuntos"]);
					//}
					//else
					//{
					//	rutaAdjuntos = ConfigurationManager.AppSettings["RutaAdjuntos"];
					//}
					//rutaAdjuntos += Convert.ToString(Path.DirectorySeparatorChar) + respAdjunto.IdEnvio;

					//if (envioCorreoDTO.Adjuntos != null && envioCorreoDTO.Adjuntos.items.Count > 0)
					//{
					//	List<string> nombresArchivos = envioCorreoDTO.Adjuntos.items.Keys.ToList();
					//	foreach (string nombreArchivo in nombresArchivos)
					//	{
					//		envioCorreoDTO.Adjuntos.items[nombreArchivo] = rutaAdjuntos + Path.DirectorySeparatorChar + nombreArchivo;
					//	}
					//}

					ENVIO envio = ManejoCorreoDAO.ObtenerEnvio(respGuardarDatos.IdEnvio);
					envio.IDESTADOENVIO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoEnvioListoParaEnviar"]);
					ManejoCorreoDAO.GuardarEnvio(envio);
					PLANTILLA_CORREO plantilla = ManejoCorreoDAO.ObtenerPlantillaCorreo(respGuardarDatos.IdPlantilla);
					plantilla.IDESTADOPLANTILLACORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoPlantillaPlantilla"]);
					ManejoCorreoDAO.GuardarPlantilla(plantilla);

					if (!envioCorreoDTO.UsarDireccionesFuenteDatos)
					{
						string[] direccionesPara = envioCorreoDTO.Para.Split(',');
						if (direccionesPara != null && direccionesPara.Length > 0)
						{
							foreach (string para in direccionesPara)
							{
								DESTINATARIO destinatario = new DESTINATARIO();
								destinatario.CASILLACORREO = para;
								ManejoCorreoDAO.GuardarDestinatario(destinatario);

								CORREO correo = new CORREO();
								correo.ASUNTO = envioCorreoDTO.Asunto;
								correo.CUERPO = envioCorreoDTO.Cuerpo;
								correo.IDDESTINATARIO = destinatario.IDDESTINATARIO;
								correo.IDENVIO = envio.IDENVIO;
								correo.IDESTADOCORREO = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoCorreoCreado"]);
								correo.IDSISTEMAEMISOR = envioCorreoDTO.IdSistemaEmisor;
								correo.CUERPOVISUALIZACION = correo.CUERPO;
								ManejoCorreoDAO.GuardarCorreo(correo);
							}
						}
					}
					Log.RegistrarInfo(string.Format(MensajeBLL.ObtenerMensaje("MensajeInfoEnvioListoParaSerProcesado", "MENSAJE_INFORMACION"),
															respGuardarDatos.IdEnvio), nombreUsuario);
					
				//} else
				//{
				//	respGenerica.CodError = 5;
				//	respGenerica.MsjError = respAdjunto.MsjError;
				//	return respGenerica;
				//}
			}
			catch (Exception ex)
			{
				respGenerica.CodError = 5;
				respGenerica.MsjError = MensajeBLL.ObtenerMensaje("MensajeErrorEnvioGlobal", "MENSAJE_ERROR");
				Log.RegistrarError(ex, ex.Message, nombreUsuario);
				return respGenerica;
			}
			return respGenerica;
		}

		/// <summary>
		/// Método para mostrar previsualización de correo antes de enviar. Dicho botón se encuentra
		/// en la página de envío de correo (nuevo, con plantilla y con borrador)
		/// <param name="prevCorreoDTO">Recibe el asunto, el destinatario, el cuerpo del correo y la fuente de datos 
		/// para procesar los descriptores y simular la vista del correo</param>
		/// <returns>retorna un objeto con el cuerpo, asunto, de, para, la cantidad de correos y control de errores (CodError=0: sin error)</returns>
		/// </summary>
		public static RespuestaPrevisualizacionCorreo PrevisualizarCorreo(PrevisualizacionCorreoDTO prevCorreoDTO)
		{
			string nombreUsuario = string.Empty;
			RespuestaPrevisualizacionCorreo respPrev = new RespuestaPrevisualizacionCorreo();
			try
			{
				Ticket tic = SingleSignOn.Authenticate();
				nombreUsuario = tic.MinvuPrincipal.MinvuIdentity.UserName;
				//guarda una plantilla, envio y adjuntos si existen
				FormatoExcel formatoExcel = FormatoExcel.Invalido;
				byte[] byteArrExcel = Convert.FromBase64String(prevCorreoDTO.FuenteDatos);
				MemoryStream ms = new MemoryStream(byteArrExcel);
				IExcelDataReader reader = ExcelReaderFactory.CreateBinaryReader(ms);
				if (reader.IsValid) formatoExcel = FormatoExcel.Excel2003;
				reader.Close();
				ms.Close();
				ms = new MemoryStream(byteArrExcel);
				reader = ExcelReaderFactory.CreateOpenXmlReader(ms);
				if (reader.IsValid) formatoExcel = FormatoExcel.Excel2007oSuperior;
				reader.Close();
				ms.Close();
				string extensionExcel = string.Empty;
				ms = new MemoryStream(byteArrExcel);
				switch (formatoExcel)
				{
					case FormatoExcel.Invalido:
					respPrev.CodError = 1;
						respPrev.MsjError = MensajeBLL.ObtenerMensaje("MensajeRutaExcelInvalido", "MENSAJE_ERROR");
						return respPrev;
					case FormatoExcel.Excel2003:
						reader = ExcelReaderFactory.CreateBinaryReader(ms);
					break;
					case FormatoExcel.Excel2007oSuperior:
						reader = ExcelReaderFactory.CreateOpenXmlReader(ms);
					break;
				}
				reader.IsFirstRowAsColumnNames = true;
				DataSet result = reader.AsDataSet();
				if (result.Tables["descriptores"] == null)
				{
					respPrev.CodError = 3;
					respPrev.MsjError = MensajeBLL.ObtenerMensaje("MensajeErrorFaltaHojaDescriptores", "MENSAJE_ERROR");
					return respPrev;
				}
				DataTable dtDescriptoresConDirecciones = result.Tables["descriptores"].AsEnumerable().Where(dRow => Convert.ToString(dRow[0]) != "").CopyToDataTable();
				if (dtDescriptoresConDirecciones.Rows.Count == 0)
				{
					respPrev.CodError = 4;
					respPrev.MsjError = MensajeBLL.ObtenerMensaje("MensajeErrorNoHayDestinatarios", "MENSAJE_ERROR");
					return respPrev;
				}
				if (dtDescriptoresConDirecciones.Columns.Count == 0)
				{
					respPrev.CodError = 10;
					respPrev.MsjError = MensajeBLL.ObtenerMensaje("MensajeErrorFaltaColumnaDestinatarios", "MENSAJE_ERROR");
					return respPrev;
				}
				prevCorreoDTO.Cuerpo = prevCorreoDTO.Cuerpo.Replace("&lt;", "<");
				prevCorreoDTO.Cuerpo = prevCorreoDTO.Cuerpo.Replace("&gt;", ">");

				MatchCollection matches = Regex.Matches(prevCorreoDTO.Cuerpo, ConfigurationManager.AppSettings["ExprRegularDescriptores"]);
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
					string descripcion = string.Format(MensajeBLL.ObtenerMensaje("MensajeErrorFaltanDescriptoresEnExcelPrevisualizar", "MENSAJE_ERROR"),
																						string.Join(",", descriptoresFaltantes));
					respPrev.CodError = 2;
					respPrev.MsjError = descripcion;
					return respPrev;
				}
				DataRow dr = dtDescriptoresConDirecciones.Rows[prevCorreoDTO.NroCorreo];
				foreach (Match match in matches)
				{
					prevCorreoDTO.Cuerpo = prevCorreoDTO.Cuerpo.Replace("<<" + match.Groups[1].Value + ">>", Convert.ToString(dr[match.Groups[1].Value]));
				}
				respPrev.Cuerpo = prevCorreoDTO.Cuerpo;
				respPrev.CodError = 0;
				respPrev.MsjError = null;
				respPrev.De = prevCorreoDTO.De;
				respPrev.CantCorreos = dtDescriptoresConDirecciones.Rows.Count;
				respPrev.Asunto = prevCorreoDTO.Asunto;
				respPrev.Para = Convert.ToString(dr[0]);
			}
			catch (Exception ex)
			{
				Log.RegistrarError(ex, ex.Message, nombreUsuario);
			}
			return respPrev;
		}
	}
}
