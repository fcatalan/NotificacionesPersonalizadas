using Excel;
using Minvu.Notificaciones.DTO;
using Minvu.Notificaciones.IData.DAO;
using Minvu.Notificaciones.IData.Log;
using Minvu.Notificaciones.IData.ORM;
using Minvu.Notificaciones.Personalizadas.Entidades;
using Minvu.Notificaciones.Personalizadas.Entidades.RespuestasJSON;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace Minvu.Notificaciones.Domain.BLL
{
	public class CorreoBLL
	{
		public static PLANTILLA_CORREO PlantillaCorreoDAO { get; private set; }

		/// <summary>
		/// Muestra una lista filtrada de correos para la pantalla de bandeja de entrada
		/// <param name="filtro">Objeto para filtrar por asunto, fecha o nombre de usuario. Posee campos para controlar la paginación</param>
		/// <returns>Objeto con una lista de objetos para la grilla, un campo de código de error y mensaje de error 
		/// (codError=0 significa sin error) y campo de cantidad de resultados para manejar paginación</returns>
		/// </summary>
		public static RespuestaDetalleCorreoDTO FiltrarCorreos(FiltroGrillaCorreos filtro)
		{
			RespuestaDetalleCorreoDTO respuestaDetalle = new RespuestaDetalleCorreoDTO();
			try
			{
				int cantResultados = 0;
				respuestaDetalle.correos = new List<DetalleCorreoDTO>();
				Log.RegistrarInfo("filtro.fecha=" + filtro.fecha);
				Log.RegistrarInfo("Formato fecha esperado:" + ConfigurationManager.AppSettings["FormatoFechaDisplay"]);
				if (filtro.fecha.Length == 0) filtro.fecha = DateTime.MinValue.ToString(ConfigurationManager.AppSettings["FormatoFechaDisplay"], CultureInfo.InvariantCulture);
				Log.RegistrarInfo("filtro.fecha por defecto=" + filtro.fecha);
				List<CORREO> correos = CorreoDAO.FiltrarCorreos(filtro, out cantResultados);
				correos.All(c =>
				{
					DetalleCorreoDTO detalleDTO = new DetalleCorreoDTO();
					detalleDTO.id = c.IDCORREO;
					detalleDTO.para = c.DESTINATARIO.CASILLACORREO;
					detalleDTO.asunto = c.ASUNTO;
					detalleDTO.cc = c.ENVIO.CONCOPIA;
					detalleDTO.cco = c.ENVIO.CONCOPIAOCULTA;
					//detalleDTO.cuerpo = c.ENVIO.PLANTILLA_CORREO.CUERPO;
					detalleDTO.cuerpo = c.CUERPOVISUALIZACION;
					/*
					try
					{
						ReemplazarCuerpo(c.ENVIO, ref detalleDTO);
					} catch (Exception ex)
					{
						Log.RegistrarError(ex, ex.Message);
					}
					*/

					detalleDTO.usuario = c.ENVIO.IDUSUARIO;
					detalleDTO.fechaHora = c.FECHAHORA == null ? "" : ((DateTime)c.FECHAHORA).ToString(ConfigurationManager.AppSettings["FormatoFechaDisplayConHora"], CultureInfo.InvariantCulture);
					respuestaDetalle.correos.Add(detalleDTO);
					return true;
				});
				respuestaDetalle.cantResults = cantResultados;
				respuestaDetalle.codError = 0;
				respuestaDetalle.msjError = null;
			}
			catch (Exception ex)
			{
				respuestaDetalle.codError = -1;
				respuestaDetalle.msjError = MensajeBLL.ObtenerMensaje("MensajeErrorFiltrarCorreos", "MENSAJE_ERROR");
				Log.RegistrarError(ex, ex.Message);
			}
			return respuestaDetalle;
		}

		/// <summary>
		/// Método que filtra las plantillas ya sean de tipo plantilla o borrador para las pantallas de listar plantilla o borrador para enviar correo basado en ellos
		/// <param name="filtro">Filtro para buscar por fecha, asunto o usuario. Tiene campos para controlar paginacion</param>
		/// <returns>Este objeto retorna la lista de plantillas u borradores con campos para controlar paginación y errores. CodError=0 : sin error</returns>
		/// </summary>
		public static RespuestaDetallePlantillaBorradorDTO FiltrarPlantillaBorrador(FiltroPlantillaBorrador filtro)
		{
			RespuestaDetallePlantillaBorradorDTO respuestaDetalle = new RespuestaDetallePlantillaBorradorDTO();
			try
			{
				int cantResultados = 0;
				respuestaDetalle.plantillas = new List<PlantillaCorreoDTO>();
				if (filtro.fecha.Length == 0) filtro.fecha = DateTime.MinValue.ToString(ConfigurationManager.AppSettings["FormatoFechaDisplay"], CultureInfo.InvariantCulture);
				List<PLANTILLA_CORREO> plantillas = CorreoDAO.FiltrarPlantillasBorradores(filtro, out cantResultados);
				plantillas.All(p =>
				{
					PlantillaCorreoDTO detalleDTO = new PlantillaCorreoDTO();
					detalleDTO.IdPlantilla = p.IDPLANTILLACORREO;
					detalleDTO.Asunto = p.ASUNTO;
					detalleDTO.FechaCreacion = ((DateTime)p.FECHACREACION).ToString(ConfigurationManager.AppSettings["FormatoFechaDisplayConHora"], CultureInfo.InvariantCulture);
					detalleDTO.Idusuario = p.IDUSUARIO;
					detalleDTO.Nombre = p.NOMBRE;
					//detalleDTO.cuerpo = c.ENVIO.PLANTILLA_CORREO.CUERPO;
					respuestaDetalle.plantillas.Add(detalleDTO);
					return true;
				});
				respuestaDetalle.cantResults = cantResultados;
				respuestaDetalle.codError = 0;
				respuestaDetalle.msjError = null;
			}
			catch (Exception ex)
			{
				respuestaDetalle.codError = -1;
				respuestaDetalle.msjError = MensajeBLL.ObtenerMensaje("MensajeErrorFiltrarCorreos", "MENSAJE_ERROR");
				Log.RegistrarError(ex, ex.Message);
			}
			return respuestaDetalle;
		}

		/// <summary>
		/// Obtiene una plantilla de correo a partir de su clave primaria
		/// <param name="idPlantilla">La clave primaria de la tabla PLANTILLA_CORREO</param>
		/// <returns>Una respuesta con la plantilla y campos para controlar errores (CodError=0 : sin error)</returns>
		/// </summary>
		public static RespuestaPlantillaDTO ObtenerPlantilla(int idPlantilla)
		{
			RespuestaPlantillaDTO respuestaPlantillaDTO = new RespuestaPlantillaDTO();
			try
			{
				respuestaPlantillaDTO.codError = 0;
				PLANTILLA_CORREO plantillaCorreo = ManejoCorreoDAO.ObtenerPlantillaCorreo(idPlantilla);
				respuestaPlantillaDTO.plantillaCorreoDTO = new PlantillaCorreoDTO();
				respuestaPlantillaDTO.plantillaCorreoDTO.Asunto = plantillaCorreo.ASUNTO;
				respuestaPlantillaDTO.plantillaCorreoDTO.Nombre = plantillaCorreo.NOMBRE;
				respuestaPlantillaDTO.plantillaCorreoDTO.Cuerpo = HttpUtility.UrlEncode(plantillaCorreo.CUERPO);
				respuestaPlantillaDTO.plantillaCorreoDTO.FechaCreacion = ((DateTime)plantillaCorreo.FECHACREACION).ToString(ConfigurationManager.AppSettings["FormatoFechaDisplayConHora"], CultureInfo.InvariantCulture);
				respuestaPlantillaDTO.plantillaCorreoDTO.IdPlantilla = plantillaCorreo.IDPLANTILLACORREO;
				respuestaPlantillaDTO.plantillaCorreoDTO.Idusuario = plantillaCorreo.IDUSUARIO;
			} catch (Exception ex)
			{
				Log.RegistrarError(ex, ex.Message);
				respuestaPlantillaDTO.codError = -1;
				respuestaPlantillaDTO.msjError = MensajeBLL.ObtenerMensaje("MensajeErrorObtenerPlantilla", "MENSAJE_ERROR");
			}
			return respuestaPlantillaDTO;
		}
	}
}
