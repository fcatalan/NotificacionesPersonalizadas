import { Component, Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from "@angular/http";
import { Observable } from 'rxjs/Rx';
import { Notificacion} from '../models/Notificacion';
import { FiltroBusquedaNotificacion } from "../models/FiltroBusquedaNotificacion";
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { HttpModule } from '@angular/http';
import {Constantes} from '../../config.global';
import {Util} from "../utils/util";

@Injectable()
export class NotificacionService {
    private handleError;    
    private urlWebApi;
    
    constructor(private _http:Http, private constantes:Constantes, private util:Util){
        this.urlWebApi = constantes.urlWebApi; //"http://localhost:2292";
        
    }
    
    public guardarNuevaNotificacion( nuevaNotificacion:Notificacion) {
        debugger;
        let body = JSON.stringify( nuevaNotificacion );
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers, method: "post", withCredentials: true });
  
        console.log("NotificacionService -- POST /api/notificacion/GuardarNotificacion");
                     
        return this._http.post(this.urlWebApi + 'notificacion/GuardarNotificacion' , body, options)
        .map(res => res.json())
        .catch(error => this.util.manejarErrorPeticionHttp(error));   
    }

    obtenerNotificaciones() {

         return this._http.get(this.urlWebApi + 'notificacion/listar', {withCredentials: true}).
            map(res => res.json())
            .catch(error => this.util.manejarErrorPeticionHttp(error));   
   }
    obtenerCantTotNotificaciones() {

         return this._http.get(this.urlWebApi + '/api/notificacion/obtenerTotRegNotificaciones', {withCredentials: true}).
            map(res => res.json());
   }
    //llamada que permite actualizr una notificación
    public actualizaNotificacion( actNotificacion:Notificacion) {

        let body = JSON.stringify( actNotificacion );
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers, method: "post", withCredentials: true });
                     
        return this._http.post(this.urlWebApi + 'notificacion/Actualizar', body, options).
            map(res => res.json())
            .catch(error => this.util.manejarErrorPeticionHttp(error));
   }
   //Se obtiene listado de sistemas de notificaciones asociados a una notificaci�n
    obtenerSistemasdeNotificacion(idNotificacion:string) {

         return this._http.get(this.urlWebApi + 'notificacion/listarSistemasdeNotificacion/' + idNotificacion , {withCredentials: true}).
            map(res => res.json())
            .catch(error => this.util.manejarErrorPeticionHttp(error));
   }

   buscarNotificacionesPorUsuario(idUsuario:string)
   {
       return this._http.get(this.urlWebApi + 'notificacion/BuscarNotificacionesPorUsuario/' + idUsuario , {withCredentials: true}).
            map(res => res.json())
            .catch(error => this.util.manejarErrorPeticionHttp(error));
   }


   public buscarNotificacionesPorFiltros( filtroBusqNotif:FiltroBusquedaNotificacion) {
       
        let body = JSON.stringify( filtroBusqNotif );
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers, method: "post", withCredentials: true });
                     
        return this._http.post(this.urlWebApi + 'notificacion/BuscarNotificacionesPorFiltros' , body, options).
            map(res => res.json())
            .catch(error => this.util.manejarErrorPeticionHttp(error));
           
   }
   public buscarNotificacionPorId(idNotificacion:Number){
           return this._http.get(this.urlWebApi + 'notificacion/ObtenerNotificacion/' + idNotificacion , {withCredentials: true}).
            map(res => res.json())
            .catch(error => this.util.manejarErrorPeticionHttp(error));
   }
   public validarPalabrasProhibidas(strTexto:string){
           return this._http.get(this.urlWebApi + 'notificacion/ValidaPalabrasProhibidas/' + strTexto , {withCredentials: true}).
            map(res => res.json())
            .catch(error => this.util.manejarErrorPeticionHttp(error));
   }
}
