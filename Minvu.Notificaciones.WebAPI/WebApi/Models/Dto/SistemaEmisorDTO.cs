using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Dto
{
    public class SistemaEmisorDTO
    {
        #region "Properties" 
        public int IdEmisor { get; set; }
        public string CasillaCorreo { get; set; }
        public string NombreSistema { get; set; }
        public Nullable<Boolean> Vigente { get; set; }
        //public int CountReg { get; set; }
        //public string ErrorStr { get; set; }
        #endregion
    }
}