using Minvu.Notificaciones.IData.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.IData.DAO
{
	public class EstadoDAO
	{
		public static ESTADO_ENVIO ObtenerEstadoEnvio(int idEstado)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				return contexto.ESTADO_ENVIO.Find(idEstado);
			}
		}

		public static ESTADO_CORREO ObtenerEstadoCorreo(int idEstado)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				return contexto.ESTADO_CORREO.Find(idEstado);
			}
		}

		public static ESTADO_PLANTILLA_CORREO ObtenerEstadoPlantilla(int idEstado)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				return contexto.ESTADO_PLANTILLA_CORREO.Find(idEstado);
			}
		}
	}
}
