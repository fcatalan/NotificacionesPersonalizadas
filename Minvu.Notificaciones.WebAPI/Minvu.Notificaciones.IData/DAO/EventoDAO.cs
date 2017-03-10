using Minvu.Notificaciones.IData.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.IData.DAO
{
	public class EventoDAO
	{
		public static void GuardarEvento(EVENTO evento)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				contexto.EVENTO.Add(evento);
				contexto.SaveChanges();
			}
		}
	}
}
