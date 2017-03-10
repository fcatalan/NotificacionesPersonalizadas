using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.DTO
{
	public class EnvioDTO
	{
		//public enum PaginaOrigenEnum { NUEVO_CORREO_SIN_PLANTILLA, NUEVO_CORREO_CON_PLANTILLA, RECUPERAR_BORRADOR };
		public string Para { get; set; }
		public string CC { get; set; }
		public string CCo { get; set; }
		public string Asunto { get; set; }
		public string Cuerpo { get; set; }
		public AdjuntoDTO Adjuntos { get; set; }
		public string FuenteDatos { get; set; }
		public int IdEnvio { get; set; } //sirve para cuando se redacta el correo. Al princpio viene en 0 y luego al presionar adjuntar o guardar viene con otro valor
																		 //public PaginaOrigenEnum PaginaOrigen { get; set; }
		public int IdPlantilla { get; set; } //al principio viene en 0
		//public string NombreUsuario { get; set; }
		public bool UsarDireccionesFuenteDatos { get; set; }
		public int IdSistemaEmisor { get; set; }
	}
}
