using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Entidades.Entidades
{
  public  class PlantillaCorreoEntidad
    {
        public int IdPlantilla { get; set; }
        public string Nombre { get; set; }
        public string Asunto { get; set; }
        public string Cuerpo { get; set; } //BLOB BYNARY
        public DateTime FechaCreacion { get; set; }
        public int Idusuario { get; set; }
    }
}
