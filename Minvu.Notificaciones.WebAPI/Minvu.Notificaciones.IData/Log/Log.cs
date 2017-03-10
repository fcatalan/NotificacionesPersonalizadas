//using Minvu.Notificaciones.IData.DAO;
using Minvu.Notificaciones.IData.DAO;
using Minvu.Notificaciones.IData.ORM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.IData.Log
{
	public class Log
	{
		private enum TipoLog { Error, Info, Warning };
		private static readonly log4net.ILog log =
			log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private static void RegistrarLog(TipoLog tipoLog, Exception ex, string message, string idUsuario = null)
		{
			EVENTO evento = new EVENTO();
			switch (tipoLog)
			{
				case TipoLog.Error:
					log.Error(message, ex);
					evento.IDUSUARIO = idUsuario;
					evento.FECHA = DateTime.Now;
					evento.DESCRIPCION = message;
					EventoDAO.GuardarEvento(evento);
					//evento.
					break;
				case TipoLog.Info:
					log.Info(message);
					evento.IDUSUARIO = idUsuario;
					evento.FECHA = DateTime.Now;
					evento.DESCRIPCION = message;
					EventoDAO.GuardarEvento(evento);
					break;
				case TipoLog.Warning:
					evento.IDUSUARIO = idUsuario;
					evento.FECHA = DateTime.Now;
					evento.DESCRIPCION = message;
					EventoDAO.GuardarEvento(evento);
					log.Warn(message);
					break;
			}
			if (ex != null && ex.InnerException != null)
			{
				RegistrarLog(tipoLog, ex.InnerException, message, idUsuario);
			}
		}

		public static void RegistrarError(Exception ex, string message, string idUsuario = null)
		{
			RegistrarLog(TipoLog.Error, ex, message, idUsuario);
		}

		public static void RegistrarWarning(string message, string idUsuario = null)
		{
			RegistrarLog(TipoLog.Warning, null, message, idUsuario);
		}

		public static void RegistrarInfo(string message, string idUsuario = null)
		{
			RegistrarLog(TipoLog.Info, null, message, idUsuario);
		}
	}
}
