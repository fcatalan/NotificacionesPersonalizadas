using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.DTO
{
    public class UsuarioDTO
    {
		public string NombreUsuario { get; set; }
		public string NombreCompleto { get; set; }
		public int CodError { get; set; }
		public string MsgError { get; set; }
    }
}
