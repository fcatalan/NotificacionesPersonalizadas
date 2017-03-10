using Microsoft.AspNetCore.Cors;
using Minvu.Notificaciones.Domain.BLL;
using Minvu.Notificaciones.Domain.Util;
using Minvu.Notificaciones.DTO;
using Minvu.Notificaciones.Personalizadas.Entidades.RespuestasJSON;
using Minvu.Security;
using Minvu.Security.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
	public class SistemaEmisorController : ApiController
	{
		/// <summary>
		/// Obtiene la lista de sistemas Emisores
		/// </summary>
		/// <returns>IEnumerable<Minvu.Notificaciones.DTO.SistemaEmisorDTO>: Lista de objetos de tipo Sistema Emisor</returns>
		[HttpGet]
		[EnableCors("AllowSpecificOrigin")]
		public List<SistemaEmisorDTO> getSistemasEmisores()
		{
			Ticket ticket = SingleSignOn.Authenticate();
			SistemaEmisorBL sistemaEmisorBL = new SistemaEmisorBL();
			var sistemasEmisores = sistemaEmisorBL.ListarSistemasEmisores(); ;
			List<SistemaEmisorDTO> sistemasAutorizados = new List<SistemaEmisorDTO>();
			foreach (SistemaEmisorDTO sistemaEmisorDTO in sistemasEmisores)
			{
				string usuario = "<nulo>";
				if (ticket != null && ticket.MinvuPrincipal != null & ticket.MinvuPrincipal.MinvuIdentity != null)
				{
					usuario = ticket.MinvuPrincipal.MinvuIdentity.UserName;
				}
				Utils.RegistrarInfo(string.Format("Usuario {1} posee tarea {1} para sistema {2}",
																					usuario,
																					sistemaEmisorDTO.TareaPSSIM == null ? "<nulo>" : sistemaEmisorDTO.TareaPSSIM,
																					sistemaEmisorDTO.NombreSistema == null ? "<nulo>" : sistemaEmisorDTO.NombreSistema));
				if (sistemaEmisorDTO.TareaPSSIM != null && ticket.MinvuPrincipal.HasTarea(sistemaEmisorDTO.TareaPSSIM)) sistemasAutorizados.Add(sistemaEmisorDTO);
			}
			return sistemasAutorizados;
		}

		/// <summary>
		/// Verifica si la WebAPI está online. Si angular no logra invocar este método 
		/// lo captura con el listener error del subscribe y responde que no esta online 
		/// y muestra un mensaje de error en la web de angular. Este método se invoca
		/// desde el app.component antes que cualquier otra cosa en angular para evitar
		/// hacer llamadas a otras webapi y motrar el mensaje de error de inmediato
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[EnableCors("AllowSpecificOrigin")]
		public RespuestaGenerica WebAPIOnline()
		{
			RespuestaGenerica respuesta = new RespuestaGenerica();
			try
			{
				Ticket tic = SingleSignOn.Authenticate();
				//si puede obtener el ticket significa que esta online
				if (tic != null)
				{
					respuesta.CodError = 0;
					respuesta.MsjError = null;
				}
				else
				{
					//si no, responde que falta iniciar sesión (este flujo al parecer no lleagaría nunca 
					//ya que sin sesión no se logra entrar al método de la WebAPI
					respuesta.CodError = 1;
					respuesta.MsjError = MensajeBLL.ObtenerMensaje("MensajeErrorSinSesion", "MENSAJE_ERROR");
				}
			}
			catch (Exception ex)
			{
				//arroja un mensaje de error de invocacion si se cae la llamada
				Utils.RegistrarError(ex, ex.Message);
				respuesta.CodError = -1;
				respuesta.MsjError = MensajeBLL.ObtenerMensaje("MensajeErrorInvocacionWebAPI", "MENSAJE_ERROR");
			}
			return respuesta;
		}

		[HttpGet]
		[EnableCors("AllowSpecificOrigin")]
		public RespuestaSistemaEditorDTO ObtenerSistemaEmisor(int idSistemaEmisor)
		{
			RespuestaSistemaEditorDTO resp = new RespuestaSistemaEditorDTO();
			try
			{
				SistemaEmisorBL sistBL = new SistemaEmisorBL();
				SistemaEmisorDTO sistEmisor = sistBL.ObtenerSistemaEmisor(idSistemaEmisor);
				resp.CodError = 0;
				resp.MsjError = null;
				resp.sistemaEmisor = sistEmisor;
			}
			catch (Exception ex)
			{
				Utils.RegistrarError(ex, ex.Message);
				resp.CodError = -1;
				resp.MsjError = MensajeBLL.ObtenerMensaje("MensajeErrorObtenerSistEmisor", "MENSAJE_ERROR");
			}
			return resp;
		}
	}
}