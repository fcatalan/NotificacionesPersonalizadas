using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Entidades.Entidades
{
    public class EnvioEntidad
    {
        public int IdEnvio { get; set; }
        public int IdPlantilla { get; set; }
        public int IdUsuario { get; set; }
        public string PlantillaEnvioOrio { get; set; }
        public string PlantillaAsuntoEnvioOri { get; set; }
        public string ResumenCcCco { get; set; }
        public DateTime FechaHora { get; set; }
        public int EstadoEnvio { get; set; }
    }
}
