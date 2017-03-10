using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Entidades.Entidades
{
    public class UsuarioEntidad
    {
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaUltIngreso { get; set; }
    }
}
