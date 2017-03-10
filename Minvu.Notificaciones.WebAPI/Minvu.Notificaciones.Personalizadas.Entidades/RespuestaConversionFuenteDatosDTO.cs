using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Personalizadas.Entidades
{
	public class RespuestaConversionFuenteDatosDTO
	{
		public int CodError { get; set; }
		public string MsjError { get; set; }
		public DataSet dataset { get; set; }
		public byte[] byteArray { get; set; }
	}
}
