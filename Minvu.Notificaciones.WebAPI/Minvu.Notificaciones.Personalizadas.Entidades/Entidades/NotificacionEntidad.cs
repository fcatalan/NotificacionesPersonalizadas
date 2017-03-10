using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Entidades.Entidades
{
  public  class NotificacionEntidad
    {
        public int IdNotificacion { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Asunto { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public Boolean EstadoVigencia { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
