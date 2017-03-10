using Minvu.Notificaciones.IData.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.IData.DAO
{
	public class MensajeDAO
	{
		public static MENSAJE ObtenerMensaje(string idMensaje, string idCategoria)
		{
			MENSAJE msjeError = new MENSAJE();
			try
			{
				using (ContextoBD contexto = new ContextoBD())
				{
					msjeError = contexto.MENSAJE.Where(msj => msj.IDMENSAJE == idMensaje && msj.CATEGORIAMENSAJE == idCategoria).FirstOrDefault();
				}
			}
			catch (Exception ex)
			{
				Log.Log.RegistrarError(ex, ex.Message);
				msjeError = null;
			}
			return msjeError;
		}
	}
}
