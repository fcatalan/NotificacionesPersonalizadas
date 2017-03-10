using Minvu.Notificaciones.DTO;
using Minvu.Notificaciones.IData.DAO;
using Minvu.Notificaciones.IData.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Domain.BLL
{
	public class SistemaEmisorBL
	{

		public List<SistemaEmisorDTO> ListarSistemasEmisores()
		{
			try
			{
				SistemaEmisorDAO palabrasDAO = new SistemaEmisorDAO();
				return palabrasDAO.ObtenerListaSistemasEmisores();
			}
			catch (Exception)
			{
				return null;
				throw;
			}
		}

		public SistemaEmisorDTO ObtenerSistemaEmisor(int idSistemaEmisor)
		{
			SISTEMA_EMISOR sistEmisor = SistemaEmisorDAO.ObtenerSistemaEmisor(idSistemaEmisor);
			SistemaEmisorDTO sistEmisorDTO = new SistemaEmisorDTO();
			sistEmisorDTO.CasillaCorreo = sistEmisor.CASILLACORREO;
			sistEmisorDTO.IdEmisor = idSistemaEmisor;
			sistEmisorDTO.NombreSistema = sistEmisor.NOMBRESISTEMA;
			sistEmisorDTO.TareaPSSIM = sistEmisor.TAREAPSSIM;
			sistEmisorDTO.Vigente = (sistEmisor.VIGENTE == null ? false : (bool)sistEmisor.VIGENTE);
			return sistEmisorDTO;
		}
	}
}
