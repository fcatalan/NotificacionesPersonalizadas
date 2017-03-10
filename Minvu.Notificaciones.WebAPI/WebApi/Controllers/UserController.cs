using Microsoft.AspNetCore.Cors;
using Minvu.Notificaciones.Domain.BLL;
using Minvu.Notificaciones.Domain.Util;
using Minvu.Notificaciones.DTO;
using Minvu.Notificaciones.Personalizadas.Entidades;
using Minvu.Notificaciones.Personalizadas.Entidades.RespuestasJSON;
using Minvu.Security;
using Minvu.Security.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace WebApi.Controllers
{
	public class UserController : ApiController
	{
		/// <summary>
		///		Obtiene información del nombre de usuario y nombre complete de éste basado en la conexión actual de PSSIM
		///		<returns>Un objeto de tipo UsuarioDTO que tiene el nombre de usuario, nombre completo, un código de error y un mensaje de error</returns>
		/// </summary>
		[HttpGet]
		[EnableCors("AllowSpecificOrigin")]
		[AllowAnonymous]
		public UsuarioDTO ObtenerInfoUsuario()
		{
			UsuarioDTO usuarioDTO = new UsuarioDTO();
			try
			{
				Ticket tic = SingleSignOn.Authenticate();
				usuarioDTO.NombreCompleto = tic.MinvuPrincipal.CompleteName;
				usuarioDTO.NombreUsuario = tic.MinvuPrincipal.MinvuIdentity.UserName;

				//busca usuario y si no existe lo agrega en nuestra tabla usuario
				UsuarioBLL.ObtenerUsuarioBD(usuarioDTO.NombreUsuario);
			}
			catch (Exception ex)
			{
				usuarioDTO.CodError = -1;
				usuarioDTO.MsgError = ex.Message;
				Utils.RegistrarError(ex, ex.Message);
			}
			return usuarioDTO;
		}

		/// <summary>
		/// Lista los usuarios que han ingresado al sistema de notificaciones
		/// <returns>Una lista de objetos de tipo UsuarioDTO (que posee nombre de usuario, nombre completo. Código de error y mensaje de error no se usan en cada elemento</returns>
		/// </summary>
		[HttpGet]
		[EnableCors("AllowSpecificOrigin")]
		public List<UsuarioDTO> ListarUsuarios()
		{
			return UsuarioBLL.ObtenerUsuarios();
		}

		/// <summary>
		///		Método que entrega una lista de usuarios con su nombre de usuario y nombre completo. Se invoca desde cajas de texto con autocompletar
		///		<param name="parteNombreUsuario">String con parte del nombre de usuario (OJO, no busca por nombre completo)</param>
		///		<returns>Una estructura con los usuarios y un codigo y mensaje de error</returns>
		/// </summary>
		[HttpGet]
		[EnableCors("AllowSpecificOrigin")]
		public RespuestaUsuarioDTO ConsultarInfoUsuarios(string parteNombreUsuario)
		{
			RespuestaUsuarioDTO respuesta = new RespuestaUsuarioDTO();
			try
			{
				List<ResultadoAutocompletarDTO> usuarios = new List<ResultadoAutocompletarDTO>();
				usuarios = UsuarioBLL.BuscarUsuariosPorNombreUsuario(parteNombreUsuario);
				respuesta.CodError = 0;
				respuesta.MsgError = null;
				respuesta.usuarios = usuarios;
			}
			catch (Exception ex)
			{
				Utils.RegistrarError(ex, ex.Message);
				respuesta.CodError = ex.HResult;
				respuesta.MsgError = MensajeBLL.ObtenerMensaje("MensajeErrorObtencionUsuarios", "MENSAJE_ERROR");
			}
			return respuesta;
		}

		/// <summary>
		///		Cierra la sesión de PSSIM
		/// <returns>Arroja una respuesta genérica con un código de error y mensaje de error. Código de error = 0 significa que no hay error</returns>
		/// </summary>
		/// 
		[HttpGet]
		[EnableCors("AllowSpecificOrigin")]
		public RespuestaGenerica CerrarSesion()
		{
			RespuestaGenerica respuesta = new RespuestaGenerica();
			try
			{
				Ticket ticket = SingleSignOn.Authenticate();
				if (ticket != null)
				{
					SingleSignOn.SignOffCurrentSession();
				}
				respuesta.CodError = 0;
			} catch (Exception ex)
			{
				Utils.RegistrarError(ex, ex.Message);
				respuesta.CodError = -1;
				respuesta.MsjError = MensajeBLL.ObtenerMensaje("MensajeErrorCerrarSesion", "MENSAJE_ERROR");
			}
			return respuesta;
		}
	}
}
