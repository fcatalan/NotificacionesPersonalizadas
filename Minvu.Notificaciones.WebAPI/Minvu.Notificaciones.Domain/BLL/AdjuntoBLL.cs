using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Domain.BLL
{
    public class AdjuntoBLL
    {
        #region "Properties" 
        public int IdAdjunto { get; set; }
        public string Nombre { get; set; }
        public string TamañoArchivo { get; set; }
        #endregion

        #region "Methods" 
        public Boolean CopiarAdjunto()
        {
            Boolean result = false;

            return result;
        }
        public Boolean ValidarTamaño()
        {
            Boolean result = false;

            return result;
        }


        #endregion
    }
}
