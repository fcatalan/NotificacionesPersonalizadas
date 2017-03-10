using Minvu.Notificaciones.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Personalizadas.Entidades.RespuestasJSON
{
	public class RespuestaUsuarioDTO
	{
		public int CodError { get; set; }
		public string MsgError { get; set; }
		public List<ResultadoAutocompletarDTO> usuarios { get; set; }
	}
}
