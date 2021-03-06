﻿using Minvu.Notificaciones.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Personalizadas.Entidades.RespuestasJSON
{
	public class RespuestaDetallePlantillaBorradorDTO
	{
		public int codError { get; set; }
		public string msjError { get; set; }
		public int cantResults { get; set; }
		public List<PlantillaCorreoDTO> plantillas { get; set; }
	}
}
