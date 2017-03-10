import { Component, OnInit, Input} from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { CorreoService } from "../../services/correo.service";
import { CookieService} from 'angular2-cookie/core';
import { ComunesService } from "../../services/comunes.service";
import { Ng2AutoCompleteModule } from 'ng2-auto-complete';
import {IMyOptions, IMyDateModel} from 'mydatepicker';
import { BrowserModule } from '@angular/platform-browser';
import {UsuarioService} from '../../services/usuario.service';
import {RespuestaPlantillaCorreoDTO} from '../../models/RespuestaPlantillaCorreoDTO';
import {FiltroPlantillaBorrador} from '../../models/FiltroPlantillaBorrador';
import {RespuestaSistemaEmisor} from '../../models/RespuestaSistemaEmisor';
import {Constantes} from '../../../config.global'
import {Util} from '../../utils/util';

@Component({
    selector:'correoPlantillaBorrador',
    templateUrl: './correoplantillaborrador.component.html',
    providers: [Constantes, CookieService, CorreoService, UsuarioService, Util]
 //   inputs: ['idSistemaSelec']
})

export class CorreoPlantillaBorradorComponent implements OnInit{
    public myDatePickerOptions: IMyOptions;
	
		private urlBusquedaUsuarios:string
		public consultaPlantillaBorrador:FiltroPlantillaBorrador;
		public infoGrilla:RespuestaPlantillaCorreoDTO;
        public title:string;
		private casillaCorreoSistEmisor:string;
		private idSistemaEmisor:number;
		private respuestaSistEmisor:RespuestaSistemaEmisor;

	constructor(private _comunesService:ComunesService, private cookieService:CookieService, 
				private correoService:CorreoService, public usuarioService:UsuarioService,
                private _routeParams: ActivatedRoute, private constantes:Constantes,
				private router:Router) {
		this.myDatePickerOptions = {
        // other options...
			dateFormat: constantes.FORMATO_FECHA_VISIBLE,
			dayLabels: constantes.NOMBRES_DIAS_FECHAS,
			monthLabels: constantes.NOMBRES_MESES_FECHAS,
			todayBtnTxt: constantes.NOMBRE_HOY_FECHAS,
			editableDateField: false
		};
		let self = this;
          this._routeParams.queryParams.map(params => params["casillaCorreoSistEmisor"]).subscribe(valorCasillaCorreoSistEmisor => {
            if (typeof valorCasillaCorreoSistEmisor !== "undefined" && valorCasillaCorreoSistEmisor != null) {
				self._routeParams.queryParams.map(params => params["idSistemaEmisor"]).subscribe(valorIdSistemaEmisor => {
					if (typeof valorIdSistemaEmisor !== "undefined" && valorIdSistemaEmisor != null) {
						self.casillaCorreoSistEmisor = valorCasillaCorreoSistEmisor
						self.idSistemaEmisor = valorIdSistemaEmisor;
						self.urlBusquedaUsuarios = constantes.urlWebApi + "User/ConsultarInfoUsuarios?parteNombreUsuario=:my_own_keyword";
						self.infoGrilla = new RespuestaPlantillaCorreoDTO();
						self.infoGrilla.plantillas = [];
						self.consultaPlantillaBorrador = new FiltroPlantillaBorrador();
						self.consultaPlantillaBorrador.nroPagina = 0;
						self.consultaPlantillaBorrador.asunto = "";
						self.consultaPlantillaBorrador.tamanoPagina = 5;
						self.consultaPlantillaBorrador.nombreUsuario = "";
						self.consultaPlantillaBorrador.fecha = "";//Util.dateToDDMMYYYY(date);
						self.consultaPlantillaBorrador.idSistemaEmisor = this.idSistemaEmisor;
						self._routeParams.queryParams.map(params => params["tipoPlantilla"])
																		.subscribe(valor => {
							self.consultaPlantillaBorrador.tipoPlantilla = parseInt(valor);
							switch(self.consultaPlantillaBorrador.tipoPlantilla) {
								case 1:
									self.title = "Selección de borrador";
								break;
								case 2:
									self.title = "Selección de plantilla";
								break;
								default:
									self.title = "Selección de borradores y plantillas";
								break;
							}
							self.recargarGrilla();
						});
					}
				});
			}
		  }).unsubscribe();
        
	}

	onDateChanged(event: IMyDateModel) {
		this.consultaPlantillaBorrador.fecha = event.formatted;
		console.log("fecha formateada: "+event.formatted);
			// event properties are: event.date, event.jsdate, event.formatted and event.epoc
	}

	onNameChanged(event) {
		if (typeof event !== "undefined") {
			if (typeof event.name !== "undefined") {
				this.consultaPlantillaBorrador.nombreUsuario = event.name;
			}
		}
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
		let $ = require("jQuery");
		$("#imgCargandoPlantillaBorrador").show();
		$("#divGrilla").hide();
		if (resetearNroPagina) this.consultaPlantillaBorrador.nroPagina = 0;
		this.consultaPlantillaBorrador.idSistemaEmisor = this.idSistemaEmisor;
		this.correoService.obtenerPlantillasBorradores(this.consultaPlantillaBorrador).subscribe(
			resp => {
				this.ObtenerNombreSistemaEmisor();
				this.infoGrilla = resp;
				$("#divGrilla").show();
			}
		)
	}

	ObtenerNombreSistemaEmisor() {
		this._comunesService.ObtenerSistemaEmisor(this.idSistemaEmisor).subscribe(
			resp => {
				let $ = require("jQuery");
				$("#imgCargandoPlantillaBorrador").hide();
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
		//Util.mostrarAlerta("<b>Titulo</b>","<i>Cuerpo</i>");
		let totalPaginas = this.getTotalPaginas();
		this.consultaPlantillaBorrador.nroPagina+=cantPaginas;
		if (this.consultaPlantillaBorrador.nroPagina+1 > totalPaginas) {
			this.consultaPlantillaBorrador.nroPagina=totalPaginas-1;
		}
		if (this.consultaPlantillaBorrador.nroPagina<0) {
			this.consultaPlantillaBorrador.nroPagina=0;
		}
		this.recargarGrilla();
	}
	moverPaginaInicial() {
		this.consultaPlantillaBorrador.nroPagina=0;
		this.recargarGrilla();
	}
	moverPaginaFinal() {
		let totalPaginas = this.getTotalPaginas();
		this.consultaPlantillaBorrador.nroPagina=totalPaginas-1;
		this.recargarGrilla();
	}
	getTotalPaginas() {
		if (this.consultaPlantillaBorrador.tamanoPagina > 0)
			return Math.ceil(this.infoGrilla.cantResults/this.consultaPlantillaBorrador.tamanoPagina);
		else	
			return 1;
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
