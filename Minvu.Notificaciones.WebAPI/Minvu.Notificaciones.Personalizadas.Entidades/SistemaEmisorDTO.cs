using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.DTO
{
	public class SistemaEmisorDTO
	{
		public int IdEmisor { get; set; }
		public string CasillaCorreo { get; set; }
		public string NombreSistema { get; set; }
		public Boolean Vigente { get; set; }
		public string TareaPSSIM { get; set; }
	}
}
