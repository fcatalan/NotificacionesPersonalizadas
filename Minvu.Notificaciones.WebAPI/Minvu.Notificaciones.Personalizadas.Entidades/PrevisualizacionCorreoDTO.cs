using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Personalizadas.Entidades
{
	public class PrevisualizacionCorreoDTO
	{
		public string De { get; set; }
		public int NroCorreo { get; set; }
		//public string NombreUsuario { get; set; }
		public string Asunto { get; set; }
		public string Cuerpo { get; set; }
		public string FuenteDatos { get; set; }
	}
}
