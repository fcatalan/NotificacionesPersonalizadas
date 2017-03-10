using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minvu.Notificaciones.IData.ORM;
using Minvu.Notificaciones.DTO;
//dd
namespace Minvu.Notificaciones.IData.DAO
{
	public class SistemaEmisorDAO
	{
		public List<SistemaEmisorDTO> ObtenerListaSistemasEmisores()
		{
			///  dddd
			try
			{
				using (ContextoBD context = new ContextoBD())
				{
					List<SistemaEmisorDTO> ListSistemaEmisorDTO = new List<SistemaEmisorDTO>();
					var list = context.SISTEMA_EMISOR.ToList();

					ListSistemaEmisorDTO = list.Select(l => new SistemaEmisorDTO
					{
						IdEmisor = l.IDSISTEMAEMISOR,
						CasillaCorreo = l.CASILLACORREO,
						NombreSistema = l.NOMBRESISTEMA,
						Vigente = (bool)l.VIGENTE,
						TareaPSSIM = l.TAREAPSSIM

					}).ToList();

					return ListSistemaEmisorDTO;

				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return null;
				throw;
			}
		}

		public static SISTEMA_EMISOR ObtenerSistemaEmisor(int idSistemaEmisor)
		{
			using (ContextoBD contexto = new ContextoBD())
			{
				return contexto.SISTEMA_EMISOR.Find(idSistemaEmisor);
			}
		}
	}
}
