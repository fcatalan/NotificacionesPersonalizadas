using Microsoft.AspNetCore.Cors;
using Minvu.Notificaciones.Domain.BLL;
using Minvu.Notificaciones.DTO;
using Minvu.Notificaciones.Personalizadas.Entidades;
using Minvu.Notificaciones.Personalizadas.Entidades.RespuestasJSON;
using Minvu.Security;
using Minvu.Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class NotificacionController : ApiController
    {

        [HttpPost]
        [EnableCors("AllowSpecificOrigin")]
        public RespuestaGenerica GuardarNotificacion(NotificacionDTO notificacionDTO)
        {
            List<int> sistEmisoresAsoc = new List<int>();
            return NotificacionBLL.Guardar(notificacionDTO, sistEmisoresAsoc);
        }

        [HttpPost]
        [EnableCors("AllowSpecificOrigin")]
        public RespuestaGenerica Actualizar(NotificacionDTO notificacionDTO)
        {
            //IFormatProvider culture = new System.Globalization.CultureInfo("es-es", true);
            Ticket tic = SingleSignOn.Authenticate();

            notificacionDTO.IdUsuario = tic.MinvuPrincipal.MinvuIdentity.UserName;
            return NotificacionBLL.Actualizar(notificacionDTO);
        }

        [HttpGet]
        [EnableCors("AllowSpecificOrigin")]
        public List<NotificacionDTO> listar()
        {
            return NotificacionBLL.Listar();
        }

        [HttpGet]
        [EnableCors("AllowSpecificOrigin")]
        public List<SistemaEmisorDTO> listarSistemasdeNotificacion(string id)
        {
            return NotificacionBLL.listarSistemasdeNotificacion(Convert.ToInt32(id));
        }

        [HttpGet]
        [EnableCors("AllowSpecificOrigin")]
        public List<NotificacionDTO> BuscarNotificacionesPorUsuario(string id)
        {
            string idUsuario = id;
            return NotificacionBLL.BuscarNotificacionesPorUsuario(idUsuario);
        }

        [HttpPost]
        [EnableCors("AllowSpecificOrigin")]
        public List<NotificacionDTO> BuscarNotificacionesPorFiltros(FiltroBusquedaNotificacion filtro)
        {
            return NotificacionBLL.BuscarNotificacionPorFiltro(filtro);
        }

        [HttpGet]
        [EnableCors("AllowSpecificOrigin")]
        public int obtenerTotRegNotificaciones()
        {
            return NotificacionBLL.ObtenerTotRegNotificaciones();
        }
        [HttpGet]
        [EnableCors("AllowSpecificOrigin")]
        public NotificacionDTO ObtenerNotificacion(int id)
        {
            return NotificacionBLL.ObtenerNotificacionPorId(id);
        }
        [HttpGet]
        [EnableCors("AllowSpecificOrigin")]
        public string ValidaPalabrasProhibidas(string id)
        {
            var result = NotificacionBLL.ValidaTextoPalabrasProhibidas(id);
            return result;
        }
    }
}
