using Microsoft.AspNetCore.Cors;
using Minvu.Notificaciones.Domain.BLL;
using Minvu.Notificaciones.Domain.Util;
using Minvu.Notificaciones.DTO;
using Minvu.Notificaciones.Personalizadas.Entidades;
using Minvu.Notificaciones.Personalizadas.Entidades.RespuestasJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.Models.Dto;
using WebApi.Providers;

namespace WebApi.Controllers
{
	public class CorreoController : ApiController
	{
		/// <summary>
		/// Método de WebAPI que envia el correo. Lo deja guardado en las tablas (a nivel de tabla ENVIO si
		/// es con fuente de datos y a nivel de tabla CORREO si lo es sin fuente de datos). Luego el job
		/// periodico de la clase ProcesoEnvioCorreoBL en el método ProcesarEnvios() toma eso junto con
		/// la fuente de datos y adjuntos guardados en disco y envía los correos
		/// </summary>
		/// <param name="envioDTO">Mapeo del formulario de enviar correo desde angular (nuevo, a partir de plantilla
		/// y a partir de borrador)</param>
		/// <returns></returns>
		[HttpPost]
		[EnableCors("AllowSpecificOrigin")]
		public RespuestaGenerica EnviarCorreo(EnvioDTO envioDTO)
		{
			return ManejoCorreoBL.EnviarCorreo(envioDTO);
		}

		/// <summary>
		/// Guarda sólo la plantilla del correo (no los adjuntos) como borrador
		/// </summary>
		/// <param name="envioDTO">El mismo formulario mapeado desde la página de enviar correo 
		/// (nuevo, desde plantilla o desde borrador)</param>
		/// <returns></returns>
		[HttpPost]
		[EnableCors("AllowSpecificOrigin")]
		public RespuestaPlantilla GuardarCorreo(EnvioDTO envioDTO)
		{
			return ManejoCorreoBL.GuardarCorreo(envioDTO);
		}

		/// <summary>
		/// WebAPI que sirve para previsualizar los correos de forma paginada
		/// </summary>
		/// <param name="prevCorreoDTO">Un objeto con el cuerpo sin procesar (con los css de quill editor)
		/// el asunto, la fuente de datos y el número de correo del total que se ve actualmente (internamente
		/// se maneja partiendo en 0, en pantalla parte de 1)</param>
		/// <returns>RespuestaPrevisualizacionCorreo: el correo (asunto, cuerpo, de y para) 
		/// junto con la cantidad de paginas y un codigo y mensaje de error si falla algo
		/// el codigo de error 0 significa éxito</returns>
		[HttpPost]
		[EnableCors("AllowSpecificOrigin")]
		public RespuestaPrevisualizacionCorreo PrevisualizarCorreo(PrevisualizacionCorreoDTO prevCorreoDTO)
		{
			return ManejoCorreoBL.PrevisualizarCorreo(prevCorreoDTO);
		}

		/// <summary>
		/// WebAPI que llena la grilla de la bandeja de salida.
		/// </summary>
		/// <param name="filtro">Los filtros de búsqueda de la grilla</param>
		/// <returns>Una grilla con el id del correo, asunto, fecha y hora de creacion del correo, usuario,
		/// direccion con copia, direccion con copia oculta, direccion destino y cuerpo formateado 
		/// (visible desde un popup en la pantalla de bandeja de entrada desde un icono en cada fila.
		/// Junto con eso se retorna codigo y mensaje de error (codError=0 es exito)</returns>
		[HttpPost]
		[EnableCors("AllowSpecificOrigin")]
		public RespuestaDetalleCorreoDTO FiltrarCorreos(FiltroGrillaCorreos filtro)
		{
			return CorreoBLL.FiltrarCorreos(filtro);
		}

		/// <summary>
		/// WebAPI que lista mediante filtros la grilla de las pantallas que se 
		/// invocan desde los botones "Crear usando plantilla" y "crear usando borrador"
		/// de la pantalla de selección de sistmea emisor
		/// </summary>
		/// <param name="filtro">Filtros de búsqueda</param>
		/// <returns>La grilla con un codigo de error y mensaje de error en caso
		/// que haya problemas (codigo de error 0 = exito)</returns>
		[HttpPost]
		[EnableCors("AllowSpecificOrigin")]
		public RespuestaDetallePlantillaBorradorDTO FiltrarPlantillasBorradores(FiltroPlantillaBorrador filtro)
		{
			return CorreoBLL.FiltrarPlantillaBorrador(filtro);
		}

		/// <summary>
		/// Obtiene una plantilla a partir del id de plantilla
		/// </summary>
		/// <param name="idPlantilla"></param>
		/// <returns></returns>
		[HttpGet]
		[EnableCors("AllowSpecificOrigin")]
		public RespuestaPlantillaDTO ObtenerPlantilla(int idPlantilla)
		{
			RespuestaPlantillaDTO respPlantillaDTO = CorreoBLL.ObtenerPlantilla(idPlantilla);
			return respPlantillaDTO;
		}

		/*
		[HttpGet]
		[EnableCors("AllowSpecificOrigin")]
		public RespuestaGenerica RestaurarImagenesCorreo()
		{
			RespuestaGenerica respGenerica = new RespuestaGenerica();
			try
			{
				ProcesoEnvioCorreoBL.RestaurarImagenesCorreo();
				respGenerica.CodError = 0;
				respGenerica.MsjError = null;
			} catch (Exception ex)
			{
				Utils.RegistrarError(ex, ex.Message);
				respGenerica.CodError = -1;
				respGenerica.MsjError = ex.Message;
			}
			return respGenerica;
		}
		*/
	}
}
