import { Component, OnInit} from '@angular/core';
import { ComunesService } from "../services/comunes.service";
import {Router, ActivatedRoute, Params } from "@angular/router";
import {Constantes} from "../../config.global"
import { SistemaEmisor } from "../models/sistemaEmisor";
import { Usuario } from "../models/Usuario";
import { Observable }       from 'rxjs/Observable';
import { CookieService} from 'angular2-cookie/core';
import {Util} from '../utils/util';

@Component({
    selector : 'menu',
    templateUrl:'../menu/menu.component.html',
     providers: [Constantes, ComunesService, CookieService, Util]
})

export class menuComponent implements OnInit
{
 titulo:string = "Bienvenido";
  public tienePermisoEnviarCorreo:boolean;
  public tienePermisoEnviarNotificacion:boolean;
  public tienePermisoAdministrarPalabrasProhibidas:boolean;
  public errorMessage;

  constructor(private comunesService:ComunesService, private cookieService:CookieService,
              private constantes:Constantes, private router:Router, 
              private activatedRoute:ActivatedRoute){
  }

  ngOnInit() {
    console.log()
    Util.mostrarFuncionalidadEnviarCorreo.subscribe((value:boolean) => {
      this.tienePermisoEnviarCorreo = value
    });
    Util.mostrarFuncionalidadEnviarNotificacion.subscribe((value:boolean) => {
      this.tienePermisoEnviarNotificacion = value
    });
    Util.mostrarFuncionalidadAdministrarPalabrasProhibidas.subscribe((value: boolean) => {
      this.tienePermisoAdministrarPalabrasProhibidas = value
  });
  }
}
