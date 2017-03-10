using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApi.Models.Dto;

namespace WebApi.Models
{
    public class SistemaEmisor
    {

        public List<SistemaEmisorDTO> ObtenerLstSistemaEmisor()
        {
            List<SistemaEmisorDTO> sistemasEmisores = new List<SistemaEmisorDTO>();
            sistemasEmisores.Add(new SistemaEmisorDTO { IdEmisor = 1, CasillaCorreo = "alexiuribe@gmail.com", NombreSistema = "Sistema DS115", Vigente = true });
            sistemasEmisores.Add(new SistemaEmisorDTO { IdEmisor = 2, CasillaCorreo = "alexiuribe@hotmail.com", NombreSistema = "Sistema DS10", Vigente = true });
            sistemasEmisores.Add(new SistemaEmisorDTO { IdEmisor = 3, CasillaCorreo = "alexiuribe@Angular.com", NombreSistema = "Sistema DS20", Vigente = true });
            sistemasEmisores.Add(new SistemaEmisorDTO { IdEmisor = 4, CasillaCorreo = "alexiuribe@Angular2.com", NombreSistema = "Sistema DS239", Vigente = true });

            return sistemasEmisores;
        }
    }
}