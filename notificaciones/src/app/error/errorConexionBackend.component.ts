import { Component } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { ComunesService } from "../services/comunes.service";
import { CookieService} from 'angular2-cookie/core';
import {Constantes} from "../../config.global"

@Component({
  selector: 'errorbackend',
  templateUrl: './errorConexionBackend.component.html',
  providers: [Constantes, ComunesService, CookieService]
})
export class ErrorConexionBackendComponent {
   public mensaje: string;
   public linkPSSIM: string;
   public mostrarLink:boolean;

  constructor(private route: ActivatedRoute, private cookieService:CookieService, private constantes:Constantes){
    this.linkPSSIM = constantes.urlPSSIM;
  }

   private ngOnInit() {
     let $ = require("jQuery");
     $(".imgCargandoMenus").hide();
    this.route.queryParams.map(params => params["mostrarLink"]).subscribe(valorMostrarLink => {
    if (typeof valorMostrarLink !== "undefined" && valorMostrarLink != null) {
        this.mostrarLink = valorMostrarLink != "0";
        this.mensaje = this.cookieService.get("mensaje");
        //this.cookieService.remove("mensaje");
        /*
        let $ = require("jQuery");
        $(".navbar-nav").remove();
        */
      }
    });
  }
}
