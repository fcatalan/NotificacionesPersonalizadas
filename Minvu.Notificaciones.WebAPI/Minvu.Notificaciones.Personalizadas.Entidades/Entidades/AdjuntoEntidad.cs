using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Entidades.Entidades
{
   public class AdjuntoEntidad
    {
        public int IdAdjunto { get; set; }
        public int IdCorreo { get; set; }
        public string Ruta { get; set; }
        public string Nombre { get; set; }
        public string TamañoArchivo { get; set; }
    }
}
