import { Component, Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from "@angular/http";
import { Router, ActivatedRoute, Params } from '@angular/router';
import {CookieService} from 'angular2-cookie/core'
import { Observable } from 'rxjs/Rx';
import {BehaviorSubject} from 'rxjs/BehaviorSubject';
import {Constantes} from "../../config.global";

@Component({
	providers: [Router, Constantes, CookieService]
})

export class Util {
/*
	@Component({
		selector:'util',
		providers: [Constantes, CookieService, Router]
	//   inputs: ['idSistemaSelec']
	})
*/

	private static moduloJS = require("../../comunes.js");	 

	public static WebAPIOnline:boolean = false;

	constructor(private router:Router, private constantes:Constantes, private cookieService:CookieService) {
	}

	public static dateToDDMMYYYY(date:Date) {
		var mm = date.getMonth() + 1; // getMonth() is zero-based
		var dd = date.getDate();

		
		return [(dd>9 ? '' : '0') + dd,
						(mm>9 ? '' : '0') + mm,
						date.getFullYear(),
					].join('/');
	}

	public static mostrarAlerta(titulo, cuerpo) {
		this.moduloJS.mostrarAlerta(titulo, cuerpo);
	}

	public static inicializarCKEditor(idCajaTexto:string) {
		this.moduloJS.inicializarCKEditor(idCajaTexto);
	}

	public static destruirCKEditor() {
		this.moduloJS.destruirCKEditor();
	}

	public static extraerHTMLCKEDITOR(idCajaTexto:string):string {
		return Util.normalizarAlineacionImagenes(this.moduloJS.extraerHTMLCKEDITOR(idCajaTexto));
	}

	public static onChangeCKEditor(idCajaTexto:string, funcion) {
		this.moduloJS.onchangeCKEDITOR(idCajaTexto, funcion);
	}

	public static cambiarHTMLTextEditor(idCajaTexto:string, html:string) {
		html = Util.normalizarAlineacionImagenes(html);
		this.moduloJS.cambiarHTMLTextEditor(idCajaTexto, html);
	}

	public static normalizarAlineacionImagenes(html:string):string {
		return this.moduloJS.normalizarAlineacionImagenes(html);
	}

	public static readyCKEditor(idCajaTexto:string, funcion) {
		this.moduloJS.readyCKEDITOR(idCajaTexto, funcion);
	}

	public static verificarSesion(funcion) {
		this.moduloJS.verificarSesion(funcion);
	}

	/**
	 * @description Método que es invocado del catch de cada peticion GET o POST. Verifica que venga un código 200<br>y que no venga el header "SinSesion". Si no se cumple esto, se va a la página de error
	 * @param {Response} error Response con la respuesta (tiene Codigo de respuesta HTTP, headers, etc.)
	 * 
	 */
	public manejarErrorPeticionHttp(error:Response) {
		console.log("No se pudo obtener el resultado de "+error.headers.get(this.constantes.NOMBRE_HEADER_URL_ERROR));
		if (error.status != 200 || error.headers.get(this.constantes.NOMBRE_HEADER_SIN_SESION)) {
			this.router.navigate([this.constantes.PAGINA_ERROR_SIN_WEBAPI], {queryParams: {mostrarLink: 1}});
			this.cookieService.put("mensaje", this.constantes.MENSAJE_ERROR_WEBAPI_NO_RESPONDE);
		} else {
			Util.mostrarAlerta("Error", "Ha ocurrido un error al obtener información desde la WebAPI");
		}
		let $ = require("jQuery");
		$(".imgCargandoMenus").hide();
		console.log(error);
		return Observable.of(error.json());;
	}
	
	public static getBaseLog(x, y) {
        return Math.log(x) / Math.log(y);
    }

	public static IrAlInicioPagina() {
		this.moduloJS.IrAlInicioPagina();
	}

	public static BytesToString(byteCount:number):string
    {
        let suf:string[] = [ "B", "KB", "MB", "GB", "TB", "PB", "EB" ]; //Longs run out around EB
        if (byteCount == 0)
            return "0" + suf[0];
        let bytes:number = Math.abs(byteCount);
        let place:number = Math.floor(Util.getBaseLog(bytes, 1024));
        let num:number = bytes / Math.pow(1024, place);
        return (Math.sign(byteCount) * num).toFixed(1) + suf[place];
    }

  public static clearInputFile(f){
    if(f.value){
			console.log("Valor de control file");
			console.log(f.value);
      try{
        f.value = ''; //for IE11, latest Chrome/Firefox/Opera...
      }catch(err){ }
      if(f.value){ //for IE5 ~ IE10
        var form = document.createElement('form'),
            parentNode = f.parentNode, ref = f.nextSibling;
        form.appendChild(f);
        form.reset();
        parentNode.insertBefore(f,ref);
      }
    }
  }

	public static mostrarFuncionalidadEnviarCorreo = new BehaviorSubject(null);;
	public static mostrarFuncionalidadEnviarNotificacion = new BehaviorSubject(null);;
	public static mostrarFuncionalidadAdministrarPalabrasProhibidas = new BehaviorSubject(null);;
}