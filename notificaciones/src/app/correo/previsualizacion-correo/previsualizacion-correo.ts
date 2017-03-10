import { Component, OnInit, Input} from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import {Correo} from '../../models/Correo';
import {KeyedCollection} from '../../models/KeyedCollection';
import {FormsModule} from '@angular/forms';
import {DomSanitizer, SafeHtml, SafeScript} from "@angular/platform-browser";
import { CorreoService } from "../../services/correo.service";
import { CookieService} from 'angular2-cookie/core';
import { ComunesService } from "../../services/comunes.service";
import {PrevisualizacionCorreo} from "../../models/PrevisualizacionCorreo";
import {RespuestaPrevisualizacionCorreo} from "../../models/RespuestaPrevisualizacionCorreo";
import {Constantes} from "../../../config.global";
import {Util} from "../../utils/util";

@Component({
    selector:'previsualizacionCorreo',
    templateUrl: './previsualizacion-correo.html',
    providers: [CookieService, CorreoService, Util]
 //   inputs: ['idSistemaSelec']
})

export class previsualizacionCorreo implements OnInit{
	//@Input()
	correoEntrada:Correo;
	public previsualizacion:PrevisualizacionCorreo;
	public respuestaPrev:RespuestaPrevisualizacionCorreo;
	private plantillaTitulo:string = "Previsualizando correo $nroCorreo de $cantCorreos";
	private titulo:string;
	private errorMessage:string;
	private cuerpoSaneado:SafeHtml;
	private mensajeFaltaFuenteDatos:string;
	private mensajeFaltaAsunto:string;
	private mensajeFaltaCuerpo:string;
	private mensajeJsonMuyPesado:SafeHtml;
	private pesoJson:number;
	private pesoMaximoJson:number;
	private pesoJsonTxt:string;
	private pesoMaximoJsonTxt:string;
	
	constructor(private _comunesService:ComunesService, private cookieService:CookieService, 
							private correoService:CorreoService, private domSanitizer:DomSanitizer, private constantes:Constantes,
							private router:Router) 
	{
		this.previsualizacion = new PrevisualizacionCorreo();
		this.respuestaPrev = new RespuestaPrevisualizacionCorreo();
		this.respuestaPrev.CantCorreos = 0;
		this.previsualizacion.CantCorreos = 0;
		this.mensajeFaltaFuenteDatos = constantes.MENSAJE_FALTA_FUENTE_DATOS_PREVISUALIZACION;
		this.mensajeFaltaAsunto = constantes.MENSAJE_FALTA_ASUNTO_PREVISUALIZACION;
		this.mensajeFaltaCuerpo = constantes.MENSAJE_FALTA_CUERPO_PREVISUALIZACION;
		this.pesoMaximoJson = constantes.TAMANHO_MAXIMO_CORREO;
		this.mensajeJsonMuyPesado = this.domSanitizer.bypassSecurityTrustHtml(constantes.MENSAJE_JSON_MUY_PESADO_PREVISUALIZACION);
		this.pesoMaximoJsonTxt = Util.BytesToString(this.pesoMaximoJson);
	}

	Inicializar(correo:Correo, casillaCorreoSalida:string) {
		this.correoEntrada = correo;
		this.previsualizacion.NroCorreo = 0;
		this.previsualizacion.Cuerpo = this.correoEntrada.Cuerpo;
		this.previsualizacion.Asunto = this.correoEntrada.Asunto;
		this.previsualizacion.De = casillaCorreoSalida;
		this.previsualizacion.FuenteDatos = this.correoEntrada.FuenteDatos;
		this.pesoJson = JSON.stringify(correo).length;
		this.pesoJsonTxt = Util.BytesToString(this.pesoJson);
		if (this.pesoJson <= this.pesoMaximoJson) {
			this.CargarPrevisualizacion();
		} else {
			this.mensajeJsonMuyPesado = this.domSanitizer.bypassSecurityTrustHtml(this.constantes.MENSAJE_JSON_MUY_PESADO_PREVISUALIZACION.replace("${pesoMaximoJson}", this.pesoMaximoJsonTxt)
			.replace("${pesoJson}", this.pesoJsonTxt));
			/*
			+", "+Util.BytesToString(JSON.stringify(correo.Adjuntos).length)+" de adjuntos, "
			+", "+Util.BytesToString(JSON.stringify(correo.Cuerpo).length))+" de cuerpo");*/
		}
	}

	MoverCorreoActual(cantidad:number) {
		this.previsualizacion.NroCorreo+=cantidad;
		if (this.previsualizacion.NroCorreo>=this.respuestaPrev.CantCorreos)
			this.previsualizacion.NroCorreo = this.respuestaPrev.CantCorreos-1;
		else if (this.previsualizacion.NroCorreo<0)
			this.previsualizacion.NroCorreo = 0;
		this.CargarPrevisualizacion();
	}

	CargarPrevisualizacion() {
			let self = this;
			if (this.previsualizacion.FuenteDatos != undefined && this.previsualizacion.FuenteDatos.length > 0) {
				this.correoService.previsualizarCorreo(this.previsualizacion)
				.subscribe(
				result => {
							debugger;
							self.respuestaPrev = result;
							//var messageResult = result;
							if (self.respuestaPrev.CodError == 0) {
								self.respuestaPrev = result;
								self.cuerpoSaneado = self.domSanitizer.bypassSecurityTrustHtml(self.respuestaPrev.Cuerpo);
								self.previsualizacion.Para = self.respuestaPrev.Para;
								self.titulo = self.plantillaTitulo.replace("$nroCorreo", (self.previsualizacion.NroCorreo+1).toString())
								self.titulo = self.titulo.replace("$cantCorreos", (self.respuestaPrev.CantCorreos).toString());
								self.previsualizacion.CantCorreos = this.respuestaPrev.CantCorreos;
								/*
								self.$("#prevDe").text(self.respuestaPrev.De);
								self.$("#prevPara").text(self.respuestaPrev.Para);
								self.$("#prevAsunto").text(self.respuestaPrev.Asunto);
								self.$("#prevCuerpo").html(self.respuestaPrev.Cuerpo);
								let textoTituloPopup:string = self.plantillaTitulo;
								textoTituloPopup = textoTituloPopup.replace("$nroCorreo", (self.previsualizacion.NroCorreo+1).toString());
								textoTituloPopup = textoTituloPopup.replace("$cantCorreos", (self.respuestaPrev.CantCorreos).toString());
								self.$(".modal-title").text(textoTituloPopup);
								*/
								/*
								this.$(".modal-title").text(this.$(".modal-title").text().replace("$nroCorreo"), (this.previsualizacion.NroCorreo+1).toString());
								this.$(".modal-title").text(this.$(".modal-title").text().replace("$cantCorreos"), (this.respuestaPrev.CantCorreos).toString());
								*/
								//pintar previsualizacion
							} else {
								Util.mostrarAlerta("Error", "Error al generar previsualización de correo:<br>"+result.MsjError);
							}
				},
				error => {
						this.errorMessage = <any>error;
								
						if (this.errorMessage !== null) {
						
								console.log(this.errorMessage);
								Util.mostrarAlerta("Error", "Error al generar la previsualización de correos.");
						}
				}
			);
		}
	}

	ngOnInit() {
		let self = this;
      Util.mostrarFuncionalidadEnviarCorreo.subscribe((value:boolean) => {
        if(value != null && !value) {
          self.router.navigate([self.constantes.PAGINA_REDIRECCION_SIN_PERMISO]);
        }
	  });
	}
}
