using Minvu.Notificaciones.IData.DAO.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Domain.BLL
{
    public class PlantillaCorreoBLL
    {
        #region "Properties" 
        public int IdPlantilla { get; set; }
        public string Nombre { get; set; }
        public string Asunto { get; set; }
        public string Cuerpo { get; set; }//BLOB BINARY
        public DateTime FechaCreacion { get; set; }
        public int IdUsuario { get; set; }
        List<PlantillaCorreoDto> Plantillas { get; set; }
        #endregion

        #region "Methods" 
        public Boolean Crear()
        {
            Boolean result = false;

            return result;
        }
        public PlantillaCorreoDto Obtener()
        {
            return new PlantillaCorreoDto();
        }
        public Boolean ValidarConsistencia()
        {
            Boolean result = false;

            return result;

        }
        #endregion
    }
}
