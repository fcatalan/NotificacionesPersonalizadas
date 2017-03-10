using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Entidades.Entidades
{
  public  class SistemaEmisorEntidad
    {
        public int IdEmisor { get; set; }
        public string CasillaCorreo { get; set; }
        public string NombreSistema { get; set; }
        public Boolean Vigente { get; set; }
    }
}
