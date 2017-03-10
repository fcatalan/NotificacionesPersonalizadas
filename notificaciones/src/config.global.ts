import { Injectable } from '@angular/core';
import { ConfigService } from 'ng2-config';
@Injectable()
export class Constantes {
	public readonly ambiente;
	public readonly urlFront;
	public readonly urlWebApi;
	public readonly urlPSSIM;
	public readonly TAMANHO_MAXIMO_ADJUNTOS:number;
	public readonly TAMANHO_MAXIMO_CORREO:number;
	public readonly CANT_MAX_SUGEERENCIAS_AUTOCOMPLETAR:number;
	public readonly FORMATO_FECHA_VISIBLE:string;
	public readonly NOMBRES_DIAS_FECHAS;
	public readonly NOMBRES_MESES_FECHAS;
	public readonly NOMBRE_HOY_FECHAS;
	public readonly POSICION_TEXTO_INICIO_CORREO:number;
	public readonly PAGINA_ERROR_SIN_WEBAPI;
	public readonly PAGINA_REDIRECCION_SIN_PERMISO;
	public readonly MENSAJE_ERROR_WEBAPI_NO_RESPONDE:string;
	public readonly MENSAJE_SESION_CERRADA:string;
	public readonly MENSAJE_PARA_CON_FUENTE_DATOS:string;
	public readonly MENSAJE_PARA_SIN_FUENTE_DATOS:string;
	public readonly MENSAJE_FALTA_FUENTE_DATOS_PREVISUALIZACION:string;
	public readonly MENSAJE_FALTA_FUENTE_DATOS_ENVIO:string;
	public readonly MENSAJE_FALTA_ASUNTO_PREVISUALIZACION:string;
	public readonly MENSAJE_FALTA_ASUNTO_ENVIO:string;
	public readonly MENSAJE_FALTA_CUERPO_PREVISUALIZACION:string;
	public readonly MENSAJE_FALTA_CUERPO_ENVIO:string;
	public readonly MENSAJE_JSON_MUY_PESADO_PREVISUALIZACION:string;
	public readonly MENSAJE_JSON_MUY_PESADO_ENVIO:string;
	public readonly MENSAJE_FUENTE_DATOS_CARGADA:string;
	public readonly MENSAJE_UN_ARCHIVO_SOBREPASA_PESO_MAXIMO:string;
	public readonly MENSAJE_VARIOS_ARCHIVOS_SOBREPASA_PESO_MAXIMO:string;
	public readonly MENSAJE_DETALLE_ARCHIVO:string;
	public readonly MENSAJE_RESUMEN_ARCHIVOS_SOBREPASADOS_DE_PESO:string;
	public readonly SEPARADOR_RESUMENES_ADJUNTOS:string;
	public readonly NOMBRE_HEADER_SIN_SESION:string;
	public readonly NOMBRE_HEADER_URL_ERROR:string;
	public readonly FRAGMENTOS_URL_ACTIVACION_MENU_SUPERIOR;
	public readonly MENSAJE_NO_EXISTE_SISTEMA_EMISOR:string;
	public readonly MENSAJE_ERROR_OBTENER_SISTEMA_EMISOR:string;
	public readonly NOMBRE_TAREA_CORREO:string;
	public readonly NOMBRE_TAREA_NOTIFICACIONES:string;
	public readonly NOMBRE_TAREA_MANTENEDOR_PALABRAS_PROHIBIDAS:string;
	
	public constructor(private config:ConfigService) {

		this.ambiente = config.getSettings("configSystem", "ambActivo");
		//this.urlFront = config.getSettings("urlFront", this.ambiente);
		this.urlWebApi = config.getSettings("urlWebApi", this.ambiente);
		this.urlPSSIM = config.getSettings("urlPSSIM", this.ambiente);
		this.NOMBRE_TAREA_CORREO = config.getSettings("nombreTareaCorreo", this.ambiente);
		this.NOMBRE_TAREA_NOTIFICACIONES = config.getSettings("nombreTareaNotificaciones", this.ambiente);
		this.NOMBRE_TAREA_MANTENEDOR_PALABRAS_PROHIBIDAS = config.getSettings("nombreTareaMantenedorPalabrasProhibidas", this.ambiente);
		this.TAMANHO_MAXIMO_ADJUNTOS = parseInt(config.getSettings("configArchivoAdjunto", "TAMANHO_MAXIMO_ADJUNTOS"));
		this.TAMANHO_MAXIMO_CORREO = parseInt(config.getSettings("configArchivoAdjunto", "TAMANHO_MAXIMO_CORREO"));
		this.CANT_MAX_SUGEERENCIAS_AUTOCOMPLETAR = parseInt(config.getSettings("configAutocompletar", "CANT_MAX_SUGEERENCIAS_AUTOCOMPLETAR"));
		this.FORMATO_FECHA_VISIBLE = config.getSettings("configDatePicker", "FORMATO_FECHA_VISIBLE");
		this.NOMBRES_DIAS_FECHAS = config.getSettings("configDatePicker", "NOMBRES_DIAS_FECHAS");
		this.NOMBRES_MESES_FECHAS = config.getSettings("configDatePicker", "NOMBRES_MESES_FECHAS");
		this.NOMBRE_HOY_FECHAS = config.getSettings("configDatePicker", "NOMBRE_HOY_FECHAS");
		this.POSICION_TEXTO_INICIO_CORREO = parseInt(config.getSettings("configCorreo","POSICION_TEXTO_INICIO_CORREO"));
		this.PAGINA_ERROR_SIN_WEBAPI = config.getSettings("errorConfig","PAGINA_ERROR_SIN_WEBAPI");
		this.MENSAJE_ERROR_WEBAPI_NO_RESPONDE = config.getSettings("errorConfig","MENSAJE_ERROR_WEBAPI_NO_RESPONDE");
		this.MENSAJE_SESION_CERRADA = config.getSettings("cerrarSesionConfig", "mensaje");
		this.MENSAJE_PARA_CON_FUENTE_DATOS = config.getSettings("tituloPara", "usarFuenteDatos");
		this.MENSAJE_PARA_SIN_FUENTE_DATOS = config.getSettings("tituloPara", "noUsarFuenteDatos");
		this.MENSAJE_FALTA_FUENTE_DATOS_PREVISUALIZACION = config.getSettings("mensajeFaltaFuenteDatos", "previsualizacion")
		this.MENSAJE_FALTA_FUENTE_DATOS_ENVIO = config.getSettings("mensajeFaltaFuenteDatos", "envio")
		this.MENSAJE_JSON_MUY_PESADO_PREVISUALIZACION = config.getSettings("mensajeJsonMuyPesado", "previsualizacion")
		this.MENSAJE_JSON_MUY_PESADO_ENVIO = config.getSettings("mensajeJsonMuyPesado", "envio")
		this.MENSAJE_FUENTE_DATOS_CARGADA = config.getSettings("mensajeFuenteDatosCargada", "mensaje");
		this.PAGINA_REDIRECCION_SIN_PERMISO = config.getSettings("errorConfig", "PAGINA_ERROR_SIN_PERMISO");
		this.MENSAJE_FALTA_ASUNTO_PREVISUALIZACION = config.getSettings("mensajeFaltaAsunto", "previsualizacion")
		this.MENSAJE_FALTA_ASUNTO_ENVIO = config.getSettings("mensajeFaltaAsunto", "envio")
		this.MENSAJE_FALTA_CUERPO_PREVISUALIZACION = config.getSettings("mensajeFaltaCuerpo", "previsualizacion")
		this.MENSAJE_FALTA_CUERPO_ENVIO = config.getSettings("mensajeFaltaCuerpo", "envio")
		this.MENSAJE_UN_ARCHIVO_SOBREPASA_PESO_MAXIMO = config.getSettings("mensajesArchivos","unAdjuntoSobrepasaLimitePeso");
		this.MENSAJE_VARIOS_ARCHIVOS_SOBREPASA_PESO_MAXIMO = config.getSettings("mensajesArchivos","variosAdjuntosSobrepasanLimitePeso");
		this.MENSAJE_DETALLE_ARCHIVO = config.getSettings("mensajesArchivos","detalleArchivo");
		this.MENSAJE_RESUMEN_ARCHIVOS_SOBREPASADOS_DE_PESO = config.getSettings("mensajesArchivos","resumenArchivosSobrepasadosDePeso");
		this.SEPARADOR_RESUMENES_ADJUNTOS = config.getSettings("mensajesArchivos","separadorResumenes");
		this.NOMBRE_HEADER_SIN_SESION = config.getSettings("errorConfig", "nombreHeaderSinSesion");
		this.NOMBRE_HEADER_URL_ERROR = config.getSettings("errorConfig", "nombreHeaderUrl");
		this.FRAGMENTOS_URL_ACTIVACION_MENU_SUPERIOR = config.getSettings("menuConfig", "fragmentos");
		this.MENSAJE_NO_EXISTE_SISTEMA_EMISOR = config.getSettings("mensajeErrorSistEmisor", "nulo");
		this.MENSAJE_ERROR_OBTENER_SISTEMA_EMISOR = config.getSettings("mensajeErrorSistEmisor", "error");
	}
}
