import {Archivo} from "./Archivo";
import {Util} from "../utils/util";
import {Constantes} from "../../config.global";
export class Adjuntos {
	
	public constructor(private constantes:Constantes) {
		this.archivos = [];
	}

	public archivos:Archivo[];
	
	public TamanoArchivos(archivos:Archivo[]):number {
		let tamanoArchivos:number = 0;
		archivos.forEach(archivo => {
			tamanoArchivos+=archivo.tamanoArchivo;
		});
		return tamanoArchivos;
	}

	public AgregarAdjunto(archivos:Archivo[]) {
		let mensajes = "";
		let mensajesDetalle = "";
		let mensaje:string;
		let tamanoArchivosYaAgregados:number = this.TamanoArchivos(this.archivos);
		let tamanoArchivosParaAgregar:number = this.TamanoArchivos(archivos);

		console.log("CANT ARCHIVOS RECIBIDOS:"+archivos.length)
		/**
		 * Verificacion de que CADA archivo no sobrepase el limite de tamaño de Adjuntos
		 */
		archivos.forEach(archivo => {
			if (archivo.tamanoArchivo > this.constantes.TAMANHO_MAXIMO_ADJUNTOS) {
				mensaje = this.constantes.MENSAJE_UN_ARCHIVO_SOBREPASA_PESO_MAXIMO;
				mensaje = mensaje.replace("${nombreArchivo}", archivo.nombreArchivo);
				mensaje = mensaje.replace("${pesoMaximoAdjunto}", Util.BytesToString(this.constantes.TAMANHO_MAXIMO_ADJUNTOS));
				mensaje = mensaje.replace("${pesoArchivo}", Util.BytesToString(archivo.tamanoArchivo));
				mensajes += mensaje;
			}
		});
		if (mensajes.length > 0) {
			mensajes = this.constantes.MENSAJE_RESUMEN_ARCHIVOS_SOBREPASADOS_DE_PESO.replace("${listaAdvertencias}", mensajes);
		}
		if (tamanoArchivosYaAgregados+tamanoArchivosParaAgregar>this.constantes.TAMANHO_MAXIMO_ADJUNTOS) {
			/**
			 * Listar archivos ya agregados y por agregar indicando que se sobrepasa el limite de tamaño de adjuntos entre todos (como conjunto)
			 */
			this.archivos.forEach(archivo => {
				mensaje = this.constantes.MENSAJE_DETALLE_ARCHIVO;
				mensaje = mensaje.replace("${nombreArchivo}", archivo.nombreArchivo)
				mensaje = mensaje.replace("${pesoArchivo}", Util.BytesToString(archivo.tamanoArchivo))
				mensajesDetalle += mensaje;
			});
			archivos.forEach(archivo => {
				mensaje = this.constantes.MENSAJE_DETALLE_ARCHIVO;
				mensaje = mensaje.replace("${nombreArchivo}", archivo.nombreArchivo)
				mensaje = mensaje.replace("${pesoArchivo}", Util.BytesToString(archivo.tamanoArchivo))
				mensajesDetalle += mensaje;
			});
			mensajesDetalle = this.constantes.MENSAJE_VARIOS_ARCHIVOS_SOBREPASA_PESO_MAXIMO.replace("${listaArchivosConPeso}",mensajesDetalle)
			mensajesDetalle = mensajesDetalle.replace("${pesoMaximoAdjunto}", Util.BytesToString(this.constantes.TAMANHO_MAXIMO_ADJUNTOS));
			mensajesDetalle = mensajesDetalle.replace("${pesoConjuntoArchivos}", Util.BytesToString(tamanoArchivosYaAgregados+tamanoArchivosParaAgregar));
		} 
		if (mensajes.length == 0 && mensajesDetalle.length == 0) {
			archivos.forEach(archivo => {
				this.archivos.push(archivo);
			});
		} else {
			Util.mostrarAlerta("Error", mensajes+this.constantes.SEPARADOR_RESUMENES_ADJUNTOS+mensajesDetalle);
		}
	}
}