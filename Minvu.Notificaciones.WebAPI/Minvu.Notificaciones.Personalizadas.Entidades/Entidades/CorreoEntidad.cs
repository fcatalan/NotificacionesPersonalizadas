using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Entidades.Entidades
{
   public class CorreoEntidad
    {
        public int IdCorreo { get; set; }
        public int IdDestinatario { get; set; }
        public int IdEnvio { get; set; }
        public int IdEmisor { get; set; }
        public string Asunto { get; set; }
        //public Byte Cuerpo { get; set; }
        public DateTime FechaHora { get; set; }
        public string EstadoEnvio { get; set; }
        public string TipoEnvio { get; set; }
    }
}
