using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Personalizadas.Entidades
{
	public class FiltroPlantillaBorrador
	{
		public int tipoPlantilla { get; set; }
		public int tamanoPagina { get; set; }
		public int nroPagina { get; set; }
		public string asunto { get; set; }
		public string fecha { get; set; }
		public string nombreUsuario { get; set; }
		public int idSistemaEmisor { get; set; }
	}
}
