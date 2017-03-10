using Minvu.Notificaciones.IData.ORM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.IData.DAO
{
	public class ManejoCorreoDAO
	{
		public static void GuardarCorreo(CORREO correo)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				if (correo.IDCORREO == 0)
				{
					contexto.CORREO.Add(correo);
				} else
				{
					CORREO correoExistente = contexto.CORREO.Find(correo.IDCORREO);
					correoExistente.IDDESTINATARIO = correo.IDDESTINATARIO;
					correoExistente.IDENVIO = correo.IDENVIO;
					correoExistente.FECHAHORA = correo.FECHAHORA;
					correoExistente.IDESTADOCORREO = correo.IDESTADOCORREO;
					correoExistente.CUERPO = correo.CUERPO;
					correoExistente.CUERPOVISUALIZACION = correo.CUERPOVISUALIZACION;
					correoExistente.ASUNTO = correo.ASUNTO;
				}
				contexto.SaveChanges();
			}
		}

		public static void GuardarPlantilla(PLANTILLA_CORREO plantillaCorreo)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				if (plantillaCorreo.IDPLANTILLACORREO == 0)
				{
					contexto.PLANTILLA_CORREO.Add(plantillaCorreo);
				} else
				{
					PLANTILLA_CORREO plantillaExistente = contexto.PLANTILLA_CORREO.Find(plantillaCorreo.IDPLANTILLACORREO);
					plantillaExistente.FECHACREACION = plantillaCorreo.FECHACREACION;
					plantillaExistente.ASUNTO = plantillaCorreo.ASUNTO;
					plantillaExistente.CUERPO = plantillaCorreo.CUERPO;
					plantillaExistente.IDUSUARIO = plantillaCorreo.IDUSUARIO;
					plantillaExistente.NOMBRE = plantillaCorreo.NOMBRE;
					plantillaExistente.IDESTADOPLANTILLACORREO = plantillaCorreo.IDESTADOPLANTILLACORREO;
				}
				contexto.SaveChanges();
			}
		}

		public static void GuardarEnvio(ENVIO envio)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				if (envio.IDENVIO == 0)
				{
					contexto.ENVIO.Add(envio);
				} else
				{
					ENVIO envioExistente = contexto.ENVIO.Find(envio.IDENVIO);
					envioExistente.IDESTADOENVIO = envio.IDESTADOENVIO;
					envioExistente.FECHAHORA = envio.FECHAHORA;
					envioExistente.IDPLANTILLACORREO = envio.IDPLANTILLACORREO;
					envioExistente.IDUSUARIO = envio.IDUSUARIO;
					envioExistente.CONCOPIA = envio.CONCOPIA;
					envioExistente.CONCOPIAOCULTA = envio.CONCOPIAOCULTA;
				}
				contexto.SaveChanges();
			}
		}

		public static void GuardarDestinatario(DESTINATARIO destinatario)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				if (destinatario.IDDESTINATARIO == 0)
				{
					DESTINATARIO destExiste = contexto.DESTINATARIO.Where(d => d.CASILLACORREO == destinatario.CASILLACORREO).FirstOrDefault();
					if (destExiste != null)
					{
						destinatario.IDDESTINATARIO = destExiste.IDDESTINATARIO;
					}
					else
					{
						contexto.DESTINATARIO.Add(destinatario);
					}
				} else
				{
					DESTINATARIO destinatarioExistente = contexto.DESTINATARIO.Find(destinatario.IDDESTINATARIO);
					destinatarioExistente.CASILLACORREO = destinatario.CASILLACORREO;
				}
				contexto.SaveChanges();
			}
		}

		public static void GuardarAdjunto(ADJUNTO adjunto)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				if (adjunto.IDADJUNTO == 0)
				{
					contexto.ADJUNTO.Add(adjunto);
				} else
				{
					ADJUNTO adjuntoExiste = contexto.ADJUNTO.Find(adjunto.IDADJUNTO);
					adjuntoExiste.IDENVIO = adjunto.IDENVIO;
					adjuntoExiste.NOMBRE = adjunto.NOMBRE;
					adjuntoExiste.RUTA = adjunto.RUTA;
					adjuntoExiste.TAMANIO = adjunto.TAMANIO;
				}
				contexto.SaveChanges();
			}
		}

		public static PLANTILLA_CORREO ObtenerPlantillaCorreo(int idPlantilla)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				return contexto.PLANTILLA_CORREO.Find(idPlantilla);
			}
		}

		public static ADJUNTO ObtenerAdjunto(string nombre, int idEnvio)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				return contexto.ADJUNTO.Where(ad => ad.NOMBRE == nombre && ad.IDENVIO == idEnvio).FirstOrDefault();
			}
		}

		public static CORREO ObtenerCorreo(int idCorreo)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				return contexto.CORREO.Find(idCorreo);
			}
		}

		public static List<CORREO> ObtenerCorreos(int idEnvio)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				return contexto.CORREO.Where(c => c.IDENVIO == idEnvio).ToList();
			}
		}
		public static CORREO ObtenerCorreo(int idEnvio, int idDestinatario)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				return contexto.CORREO.Where(c => c.IDENVIO == idEnvio && c.IDDESTINATARIO == idDestinatario).FirstOrDefault();
			}
		}

		public static DESTINATARIO ObtenerDestinatario(string casillaCorreo)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				return contexto.DESTINATARIO.Where(d => d.CASILLACORREO == casillaCorreo).FirstOrDefault();
			}
		}

		public static DESTINATARIO ObtenerDestinatario(int idDestinatario)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				return contexto.DESTINATARIO.Find(idDestinatario);
			}
		}
		public static ENVIO ObtenerEnvio(int idEnvio)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				return contexto.ENVIO.Find(idEnvio);
			}
		}

		public static List<ENVIO> ObtenerEnviosPorEnviar()
		{
			List<ENVIO> envios = new List<ENVIO>();
			int estadoListoParaEnviar = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoEnvioListoParaEnviar"]);
			int estadoError = Convert.ToInt32(ConfigurationManager.AppSettings["EstadoEnvioError"]);
			using (ContextoBD contexto = new ContextoBD())
			{
				envios = contexto.ENVIO.Where(e => e.IDESTADOENVIO == estadoListoParaEnviar || e.IDESTADOENVIO == estadoError).ToList();
			}
			return envios;
		}
	}
}
