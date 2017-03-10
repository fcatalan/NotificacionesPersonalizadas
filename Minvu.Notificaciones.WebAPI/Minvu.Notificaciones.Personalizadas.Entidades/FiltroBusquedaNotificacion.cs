using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Personalizadas.Entidades
{
    public class FiltroBusquedaNotificacion
    {
        public string NombreNotificacion { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public string IdUsuario { get; set; }
        public string EstadoNotificacion { get; set; } 
        public int RegistroDesde { get; set; }
        public int RegistroHasta { get; set; }
        public int NumeroPagina { get; set; }
        public int CantidadPaginacion { get; set; }
        public int IdSistemaEmisorAsoc { get; set; }
        public int IdEstadoNotificacion { get; set; }
        public string AccionAEjecutar { get; set; }
    

    }
}
