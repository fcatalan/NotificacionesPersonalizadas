using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.DTO
{
  public  class PlantillaCorreoDTO
    {
        public int IdPlantilla { get; set; }
        public string Nombre { get; set; }
        public string Asunto { get; set; }
        public string Cuerpo { get; set; } //BLOB BYNARY
        public string FechaCreacion { get; set; }
        public string Idusuario { get; set; }
    }
}
