import { Component, OnInit, Input} from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { ComunesService } from "../services/comunes.service";
import { SistemaEmisor } from "../models/sistemaEmisor";
import { Http,Headers, URLSearchParams,Response} from '@angular/http';
import { CookieService} from 'angular2-cookie/core';
import {Observable} from "rxjs/Rx";
import 'rxjs/add/operator/toPromise';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import {Util} from "../utils/util";
import {Constantes} from "../../config.global";

@Component({
    selector: "seleccionsistemaemisor",
    templateUrl: "./seleccionSistemaEmisor.component.html",
    providers: [ComunesService, CookieService, Util]
})

//export class SeleccionSistemaEmisorCorreoComponent {
export class SeleccionSistemaEmisorCorreoComponent implements OnInit{

    public titulo: string = "Seleccionar Sistemas o Programas Habitacionales";
    public sistemasEmisores: SistemaEmisor[];
    public errorMessage;
    public idSistemaEmisor:string = "-1";
    public casillaCorreoSistema:string = "";

    constructor(private _comunesService: ComunesService, private cookieService:CookieService,
                private router:Router, private constantes:Constantes ) {  
                    
                }

ngOnInit(){
    let $ = require("jQuery");
    $("#imgCargandoSistemas").show();
     //called after the constructor and called  after the first ngOnChanges() 
    this._comunesService.getSistemasEmisores()
                .subscribe(
                result => {
                    this.sistemasEmisores = result;
                    $("#imgCargandoSistemas").hide();
                },
                error => {
                    this.errorMessage = <any>error;
                    
                    if (this.errorMessage !== null) {
                        console.log(this.errorMessage);
                        Util.mostrarAlerta("Error","Error al recuperar lista de sistemas emisores");
                    }
                    $("#imgCargandoSistemas").hide();
                }
            );
    let self = this;
    Util.mostrarFuncionalidadEnviarCorreo.subscribe((value:boolean) => {
    if(value != null && !value) {
        self.router.navigate([self.constantes.PAGINA_REDIRECCION_SIN_PERMISO]);
    }
    });            
  }

  rellenarSistemaEmisor() {
      this.sistemasEmisores.forEach(element => {
          if (element.IdEmisor == parseInt(this.idSistemaEmisor)) {
              this.casillaCorreoSistema = element.CasillaCorreo;
          }
      });
  }
}
