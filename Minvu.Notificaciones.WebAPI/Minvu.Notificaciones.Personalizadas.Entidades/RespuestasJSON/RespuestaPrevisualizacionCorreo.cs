using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Personalizadas.Entidades.RespuestasJSON
{
	public class RespuestaPrevisualizacionCorreo
	{
		public int CodError { get; set; }
		public string MsjError { get; set; }
		public int CantCorreos { get; set; }
		public string De { get; set; }
		public string Para { get; set; }
		public string Asunto { get; set; }
		public string Cuerpo { get; set; }
	}
}