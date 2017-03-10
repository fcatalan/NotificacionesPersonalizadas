using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.DTO
{
    public class PalabraProhibidaDTO
    {
        public int IdPalabra { get; set; }
        public string IdUsuario { get; set; }
        public string Palabra { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string IdUsuarioModi { get; set; }
        public DateTime FechaModi { get; set; }
        public Boolean EstadoVigencia { get; set; }
        public int AuxTotalRegistrosBusqueda { get; set; }
    }
}
