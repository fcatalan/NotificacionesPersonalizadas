using Minvu.Notificaciones.IData.ORM;
using Minvu.Notificaciones.Personalizadas.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.IData.DAO
{
	public class CorreoDAO
	{
		public static List<CORREO> FiltrarCorreos (FiltroGrillaCorreos filtro, out int cantResults)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				DateTime dateTimeFiltro = DateTime.ParseExact(filtro.fecha,
																											ConfigurationManager.AppSettings["FormatoFechaDisplay"],
																											CultureInfo.InvariantCulture);
				Log.Log.RegistrarInfo("dateTimeFiltro=" + dateTimeFiltro.ToShortTimeString());
				List<CORREO> correosSinPaginar = contexto.CORREO.Include("ENVIO").Include("DESTINATARIO").Where(c => (dateTimeFiltro == DateTime.MinValue || DbFunctions.TruncateTime(c.FECHAHORA) == dateTimeFiltro)
																															&& (filtro.nombreUsuario.Length == 0 || c.ENVIO.IDUSUARIO == filtro.nombreUsuario)
																															&& (filtro.asunto.Length == 0 || c.ASUNTO.Contains(filtro.asunto))
																															&& (filtro.idSistemaEmisor == 0 || c.ENVIO.PLANTILLA_CORREO.IDSISTEMAEMISOR == filtro.idSistemaEmisor)).OrderByDescending(cor => cor.FECHAHORA).ToList();
				cantResults = correosSinPaginar.Count;
				if (filtro.tamanoPagina > 0)
					return correosSinPaginar.Skip(filtro.nroPagina * filtro.tamanoPagina).Take(filtro.tamanoPagina).ToList();
				else
					return correosSinPaginar;
			}
		}

		/// <summary>
		/// Método que lista plantillas o borradores basado en un filtor
		/// <param name="filtro">Filtro por nombre usuario, asunto o fecha. Posee campos para controlar paginación</param>
		/// <param name="cantResults">Arroja la cantidad de resultados de la consulta</param>
		/// <returns>La lista de plantillas (pueden ser plantillas o borradores, depende del campo IDESTADOPLANTILLACORREO que indica si es plantilla (2) o borrador (1)</returns>
		/// </summary>
		public static List<PLANTILLA_CORREO> FiltrarPlantillasBorradores(FiltroPlantillaBorrador filtro, out int cantResults)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				DateTime dateTimeFiltro = DateTime.ParseExact(filtro.fecha,
																											ConfigurationManager.AppSettings["FormatoFechaDisplay"],
																											CultureInfo.InvariantCulture);
				List<PLANTILLA_CORREO> correosSinPaginar = contexto.PLANTILLA_CORREO.Where(p => (p.IDESTADOPLANTILLACORREO == filtro.tipoPlantilla)
																															&& (dateTimeFiltro == DateTime.MinValue || DbFunctions.TruncateTime(p.FECHACREACION) == dateTimeFiltro)
																															&& (filtro.nombreUsuario.Length == 0 || p.IDUSUARIO == filtro.nombreUsuario)
																															&& (filtro.asunto.Length == 0 || p.ASUNTO.Contains(filtro.asunto))
																															&& p.IDSISTEMAEMISOR == filtro.idSistemaEmisor).OrderByDescending(pla => pla.FECHACREACION).ToList();
				cantResults = correosSinPaginar.Count;
				if (filtro.tamanoPagina > 0)
					return correosSinPaginar.Skip(filtro.nroPagina * filtro.tamanoPagina).Take(filtro.tamanoPagina).ToList();
				else
					return correosSinPaginar;
			}
		}
	}
}
