import { Component, Injectable } from "@angular/core";
import { Http, Response,Jsonp,JsonpModule, Headers,RequestOptions,URLSearchParams  } from "@angular/http";
import "rxjs/add/operator/map";
//import { Observable } from 'rxjs/Rx';
import { Observable }       from 'rxjs/Observable';
import { SistemaEmisor } from "../models/sistemaEmisor";
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import {Router} from "@angular/router";
import {Util} from "../utils/util";
import {Constantes} from '../../config.global';

@Injectable()
export class ComunesService {
    private urlPSSIM:string;
    private urlWebApi:string;

    constructor(private _http: Http, private _jsonp: Jsonp, private constantes:Constantes, private util:Util) { 
        this.urlPSSIM = constantes.urlPSSIM;
        this.urlWebApi  = constantes.urlWebApi;
    }

    VerificarUsuario() {
        console.log( this.urlWebApi + "User/ObtenerInfoUsuario" );
        //let body = JSON.stringify({ 'User': 'aauribem', 'Password': 'apiux.NRR'});
        let headers = new Headers();
          //  headers.append('Content-Type','application/json' );
           // headers.append("Cookie=" + strToken );

        return this._http.get(this.urlWebApi + 'User/ObtenerInfoUsuario', { withCredentials: true })
            .map(res => res.json())
            .catch(error => this.util.manejarErrorPeticionHttp(error));
      }

     getSistemasEmisores() {
        // petición por get a esa url de un api rest de pruebas  
        return this._http.get(this.urlWebApi + "SistemaEmisor/GetSistemasEmisores", { withCredentials: true })
            .map(res => res.json())
            .catch(error => this.util.manejarErrorPeticionHttp(error));
       }
    ObtenerSistemaEmisor(idSistemaEmisor:number) {
        console.log("GET: "+this.urlWebApi + "SistemaEmisor/ObtenerSistemaEmisor?idSistemaEmisor="+idSistemaEmisor);
        return this._http.get(this.urlWebApi + "SistemaEmisor/ObtenerSistemaEmisor?idSistemaEmisor="+idSistemaEmisor, { withCredentials: true })
            .map(res => res.json())
            .catch(error => this.util.manejarErrorPeticionHttp(error));
    }
     obtenerUsuarios(){
        return this._http.get(this.urlWebApi + "user/ListarUsuarios", { withCredentials: true })
            .map(res => res.json())
            .catch(error => this.util.manejarErrorPeticionHttp(error));
     }
    ValidaPermisoTarea(nombreTarea:string) {
        
        let options = new RequestOptions({ withCredentials: true, method: "get" });
        return this._http.get(this.urlWebApi + "Tarea/TienePermisoTarea?nombreTarea="+nombreTarea, options).
                            map(res => res.json())
                            .catch(error => this.util.manejarErrorPeticionHttp(error));
    }

/*
  public VerificarPermiso(nombreTarea:string, selector:string, redirigirAMenu:boolean = false)
  {

    var $ = require('jQuery');
    $(selector).hide();
    this.ValidaPermisoTarea(nombreTarea)
    .subscribe(
      response => {
        if (!response) {
            if (redirigirAMenu) {
                
            } else {
                $(selector).remove();
            }
        } else {
            $(selector).show();
        }
      },
      error => {
        if (<any>error !== null) {
          $(selector).remove();
          console.log(error);
          Util.mostrarAlerta("Error", "Error al verificar el permiso para la tarea \""+nombreTarea+"\"");
        }
      }
    );
  }
*/
  public WebAPIOnline() {
    return this._http.get(this.urlWebApi + "SistemaEmisor/WebAPIOnline", { withCredentials: true })
    .map(res => res.json())
    .catch(error => this.util.manejarErrorPeticionHttp(error));
  }

    extractData(res: Response) {
        let body = res.json();
        return body.data || { };
    }    
}