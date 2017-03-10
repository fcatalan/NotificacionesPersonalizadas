using Minvu.Notificaciones.IData.DAO;
using Minvu.Notificaciones.IData.ORM;
using Minvu.Notificaciones.Personalizadas.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Domain.BLL
{
	public class MensajeBLL
	{
		public static MensajeDTO ObtenerMensajeDTO(string idMensaje, string idCategoria)
		{
			MENSAJE mensaje_error = new MENSAJE();
			MensajeDTO objDto = new MensajeDTO();

			mensaje_error = MensajeDAO.ObtenerMensaje(idMensaje, idCategoria);
			if (mensaje_error != null)
			{
				objDto.IdMensaje = mensaje_error.IDMENSAJE;
				objDto.DescripcionMensaje = mensaje_error.DESCRIPCIONMENSAJE;
				objDto.CategoriaMensaje = mensaje_error.CATEGORIAMENSAJE;
			}
			return objDto;
		}

		public static string ObtenerMensaje(string idMensaje, string idCategoria)
		{
			MensajeDTO mensajeDTO = ObtenerMensajeDTO(idMensaje, idCategoria);
			return mensajeDTO.DescripcionMensaje;
		}
	}
}
