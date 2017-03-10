using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Personalizadas.Entidades.RespuestasJSON
{
	public class DetalleCorreoDTO
	{
		public int id { get; set; }
		public string asunto { get; set; }
		public string fechaHora { get; set; }
		public string usuario { get; set; }
		public string para { get; set; }
		public string cc { get; set; }
		public string cco { get; set; }
		public string cuerpo { get; set; }
	}
}
