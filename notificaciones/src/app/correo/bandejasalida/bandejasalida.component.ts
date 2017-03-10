import { Component, OnInit, Input} from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { CorreoService } from "../../services/correo.service";
import { CookieService} from 'angular2-cookie/core';
import { ComunesService } from "../../services/comunes.service";
import { Ng2AutoCompleteModule } from 'ng2-auto-complete';
import {IMyOptions, IMyDateModel} from 'mydatepicker';
import { BrowserModule, SafeHtml, DomSanitizer } from '@angular/platform-browser';
import {Util} from '../../utils/util'
import {UsuarioService} from '../../services/usuario.service';
import {RespuestaDetalleCorreoDTO} from '../../models/RespuestaDetalleCorreoDTO';
import {DetalleCorreoDTO} from '../../models/DetalleCorreoDTO';
import {FiltroGrillaCorreos} from '../../models/FiltroGrillaCorreos';
import {RespuestaSistemaEmisor} from '../../models/RespuestaSistemaEmisor';
import {Constantes} from '../../../config.global'

@Component({
    selector:'bandejasalida',
    templateUrl: './bandejasalida.component.html',
    providers: [Constantes, CookieService, CorreoService, UsuarioService, Util]
 //   inputs: ['idSistemaSelec']
})

export class bandejasalidaComponent{
    public myDatePickerOptions: IMyOptions;

	
		private urlBusquedaUsuarios:string;
		public consultaBandejaEntrada:FiltroGrillaCorreos;
		public infoGrilla:RespuestaDetalleCorreoDTO;
		private cuerpoSaneado:SafeHtml;
		private detalleCorreo:DetalleCorreoDTO;
		private respuestaSistEmisor:RespuestaSistemaEmisor;
		//public comunes = require("../../../comunes.js");

	constructor(private _comunesService:ComunesService, private cookieService:CookieService, 
				private correoService:CorreoService, public usuarioService:UsuarioService,
				private constantes: Constantes, private domSanitizer:DomSanitizer,
				private router:Router, private activatedRoute:ActivatedRoute) {
		this.myDatePickerOptions = {
        // other options...
			dateFormat: constantes.FORMATO_FECHA_VISIBLE,
			dayLabels: constantes.NOMBRES_DIAS_FECHAS,
			monthLabels: constantes.NOMBRES_MESES_FECHAS,
			todayBtnTxt: constantes.NOMBRE_HOY_FECHAS,
			editableDateField: false
		};
	}

	onDateChanged(event: IMyDateModel) {
		this.consultaBandejaEntrada.fecha = event.formatted;
		console.log("fecha formateada: "+event.formatted);
			// event properties are: event.date, event.jsdate, event.formatted and event.epoc
	}

	onNameChanged(event) {
		if (typeof event !== "undefined") {
			if (typeof event.name !== "undefined") {
				this.consultaBandejaEntrada.nombreUsuario = event.name;
			}
		}
	}

    verContenidoCorreo(idCorreo:number) {
  		this.infoGrilla.correos.forEach(correo => {
				if (correo.id == idCorreo) {
					this.cuerpoSaneado = this.domSanitizer.bypassSecurityTrustHtml(correo.cuerpo);
					this.detalleCorreo = correo;
					return;
				}
		  });
    }

    separarDireccionesCorreo(direccionCorreo:string) {
		if (direccionCorreo != null) {
			let direccionesCorreo:string[] = direccionCorreo.split(",");
			let direccionesCorreoConSaltoLinea = direccionesCorreo.join("<br>");
			return direccionesCorreoConSaltoLinea;
		}
		else {
			return "";
		}
	}

	recargarGrilla(resetearNroPagina:boolean = false) {
		let self = this;
		let $ = require("jQuery");
		$("#imgCargandoBandejaSalida").show();
		$("#divGrilla").hide();
		if (resetearNroPagina) this.consultaBandejaEntrada.nroPagina = 0;
		this.correoService.obtenerCorreosBandejaSalida(this.consultaBandejaEntrada).subscribe(
			resp => {
				this.infoGrilla = resp;
				this.ObtenerNombreSistemaEmisor();
				$("#imgCargandoBandejaSalida").hide();
				$("#divGrilla").show();
			},
			error => {
				this.ObtenerNombreSistemaEmisor();
				$("#imgCargandoBandejaSalida").hide();
			}
		);
	}

	ObtenerNombreSistemaEmisor() {
		this._comunesService.ObtenerSistemaEmisor(this.consultaBandejaEntrada.idSistemaEmisor).subscribe(
			resp => {
				this.respuestaSistEmisor = resp;
				if (this.respuestaSistEmisor.CodError == 0) {
					if (this.respuestaSistEmisor.sistemaEmisor == null) {
						Util.mostrarAlerta("Advertencia", this.constantes.MENSAJE_NO_EXISTE_SISTEMA_EMISOR);
					}
				} else {
					Util.mostrarAlerta("Advertencia", this.respuestaSistEmisor.MsjError);
				}
			},
			error => {
				Util.mostrarAlerta("Advertencia", this.constantes.MENSAJE_NO_EXISTE_SISTEMA_EMISOR);
			}
		);
	}

	moverPagina(cantPaginas:number) {
		let totalPaginas = this.getTotalPaginas();
		this.consultaBandejaEntrada.nroPagina+=cantPaginas;
		if (this.consultaBandejaEntrada.nroPagina+1 > totalPaginas) {
			this.consultaBandejaEntrada.nroPagina=totalPaginas-1;
		}
		if (this.consultaBandejaEntrada.nroPagina<0) {
			this.consultaBandejaEntrada.nroPagina=0;
		}
		this.recargarGrilla();
	}
	moverPaginaInicial() {
		this.consultaBandejaEntrada.nroPagina=0;
		this.recargarGrilla();
	}
	moverPaginaFinal() {
		let totalPaginas = this.getTotalPaginas();
		this.consultaBandejaEntrada.nroPagina=totalPaginas-1;
		this.recargarGrilla();
	}
	getTotalPaginas() {
		if (this.consultaBandejaEntrada.tamanoPagina != 0 && !isNaN(this.infoGrilla.cantResults))
			return isNaN(Math.ceil(this.infoGrilla.cantResults/this.consultaBandejaEntrada.tamanoPagina)) ? 1 : Math.ceil(this.infoGrilla.cantResults/this.consultaBandejaEntrada.tamanoPagina);
		else
			return 1
	}

	ngOnInit() {
		let self = this;
      Util.mostrarFuncionalidadEnviarCorreo.subscribe((value:boolean) => {
        if(value != null && !value) {
          self.router.navigate([self.constantes.PAGINA_REDIRECCION_SIN_PERMISO]);
        }
	  });
		this.activatedRoute.queryParams.map(params => params["idSistemaEmisor"]).subscribe(valorIdSistEmisor => {
			if (valorIdSistEmisor != undefined && valorIdSistEmisor > 0) {
				self.urlBusquedaUsuarios = self.constantes.urlWebApi + "User/ConsultarInfoUsuarios?parteNombreUsuario=:my_own_keyword";
				self.infoGrilla = new RespuestaDetalleCorreoDTO();
				self.infoGrilla.correos = [];
				self.consultaBandejaEntrada = new FiltroGrillaCorreos();
				self.consultaBandejaEntrada.nroPagina = 0;
				self.consultaBandejaEntrada.asunto = "";
				self.consultaBandejaEntrada.tamanoPagina = 5;
				self.consultaBandejaEntrada.nombreUsuario = "";
				self.consultaBandejaEntrada.fecha = "";//Util.dateToDDMMYYYY(date);
				self.detalleCorreo = new DetalleCorreoDTO();
				self.consultaBandejaEntrada.idSistemaEmisor = valorIdSistEmisor;
				self.recargarGrilla();
			}
		});
	}
}
