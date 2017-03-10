
import { Component } from '@angular/core';
import {Constantes} from "../../config.global"

@Component({
  selector: 'sesionCerrada',
  templateUrl: './sesionCerrada.component.html',
  providers: [Constantes]
})
export class SesionCerradaComponent {
   public mensaje: string;
   public linkPSSIM: string;

  constructor(private constantes:Constantes){
    this.linkPSSIM = constantes.urlPSSIM;
		this.mensaje = constantes.MENSAJE_SESION_CERRADA;
  }

   private ngOnInit() {
		//this.cookieService.remove("mensaje");
		let $ = require("jQuery");
		$(".navbar-nav").remove();
  }
}
