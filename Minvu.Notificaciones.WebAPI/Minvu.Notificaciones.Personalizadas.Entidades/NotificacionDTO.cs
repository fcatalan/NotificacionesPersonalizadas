using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.DTO
{
  public  class NotificacionDTO
    {
        public int IdNotificacion { get; set; }
        public string IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Asunto { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public Boolean EstadoVigencia { get; set; }
        public string EstadoNotificacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string  StrFechaInicio { get; set; }
        public string StrFechaFin { get; set; }
        public int IdEstadoNotificacion { get; set; }
        public int TotalRegistrosBusqueda { get; set; }
        public int NumeroPaginacionActual { get; set; }
        public string StrAux { get; set; }

    }
}
