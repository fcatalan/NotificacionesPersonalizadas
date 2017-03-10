using Minvu.Notificaciones.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Personalizadas.Entidades.RespuestasJSON
{
	public class RespuestaSistemaEditorDTO
	{
		public int CodError { get; set; }
		public string MsjError { get; set; }
		public SistemaEmisorDTO sistemaEmisor { get; set; }
	}
}
