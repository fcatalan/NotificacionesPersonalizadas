using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Personalizadas.Entidades.RespuestasJSON
{
	public class RespuestaDetalleCorreoDTO
	{
		public int codError { get; set; }
		public string msjError { get; set; }
		public int cantResults { get; set; }
		public List<DetalleCorreoDTO> correos { get; set; }
	}
}
