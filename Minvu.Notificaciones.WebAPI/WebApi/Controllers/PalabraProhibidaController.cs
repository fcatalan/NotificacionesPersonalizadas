using Microsoft.AspNetCore.Cors;
using Minvu.Notificaciones.Domain.BLL;
using Minvu.Notificaciones.DTO;
using Minvu.Notificaciones.Personalizadas.Entidades;
using Minvu.Notificaciones.Personalizadas.Entidades.RespuestasJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class PalabraProhibidaController : ApiController
    {
        [HttpPost]
        [EnableCors("AllowSpecificOrigin")]
        public RespuestaGenerica CrearPalabra(PalabraProhibidaDTO ent)
        {
            RespuestaGenerica resp = new RespuestaGenerica();

            PalabraProhibidaBL bll = new PalabraProhibidaBL();
            if (bll.CrearPalabrasProhibidas(ent))
            {
                resp.CodError = 0;
                resp.MsjError = "";
            }
            else
            {
                resp.CodError = -1;
                resp.MsjError = "No se logró agregar la palabra.";
            }
            return resp;
        }

        [HttpGet]
        [EnableCors("AllowSpecificOrigin")]
        public List<PalabraProhibidaDTO> BuscarPorPalabra(string id)
        {
            string idUsuario = id;
            PalabraProhibidaBL bll = new PalabraProhibidaBL();
            List<PalabraProhibidaDTO> lst = new List<PalabraProhibidaDTO>();

            lst.Add(bll.ObtenerPalabraProhibida(id));
            return lst;
        }
        [HttpPost]
        [EnableCors("AllowSpecificOrigin")]
        public List<PalabraProhibidaDTO> ObtenerPalabras(FiltroPalabraProhibida filtro)
        {
            PalabraProhibidaBL bll = new PalabraProhibidaBL();
            List<PalabraProhibidaDTO> lst = new List<PalabraProhibidaDTO>();

            lst = bll.ListarPalabrasProhibidas(filtro);
            return lst;
        }

        [HttpGet]
        [EnableCors("AllowSpecificOrigin")]
        public RespuestaGenerica EliminarPalabra(string id)
        {
            string idPalabra = id;
            RespuestaGenerica resp = new RespuestaGenerica();

            PalabraProhibidaBL bll = new PalabraProhibidaBL();
            if (bll.EliminarPalabra(Convert.ToInt32(id)))
            {
                resp.CodError = 0;
                resp.MsjError = "";
            }
            else
            {
                resp.CodError = -1;
                resp.MsjError = "No se logró eliminar palabra.";
            }

            return resp;
        }
        [HttpPost]
        [EnableCors("AllowSpecificOrigin")]
        public RespuestaGenerica ModificarPalabra(PalabraProhibidaDTO objPalabraProhibida)
        {

            RespuestaGenerica resp = new RespuestaGenerica();

            PalabraProhibidaBL bll = new PalabraProhibidaBL();
            if (bll.ObtenerPalabraProhibida(objPalabraProhibida.Palabra) == null)
            {

                if (bll.ModificarPalabra(objPalabraProhibida))
                {
                    resp.CodError = 0;
                    resp.MsjError = "";
                }
                else
                {
                    resp.CodError = -1;
                    resp.MsjError = "No se logró modificar palabra.";
                }
            }
            else
            {
                resp.CodError = -2;
                resp.MsjError = "palabra ya existe.";
            }

            return resp;
        }
    }
}
