using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Personalizadas.Entidades.RespuestasJSON
{
	public class RespuestaDatosCorreo
	{
		public int IdPlantilla { get; set; }
		public int IdEnvio { get; set; }
		public List<int> IdAdjuntos { get; set; }
		public int CodError { get; set; }
		public string MsjError { get; set; }
	}
}
