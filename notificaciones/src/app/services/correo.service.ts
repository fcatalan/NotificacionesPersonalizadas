import { Component, Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from "@angular/http";
import { Observable } from 'rxjs/Rx';
import { Correo} from '../models/Correo';
import {PrevisualizacionCorreo} from "../models/PrevisualizacionCorreo";
import {RespuestaPrevisualizacionCorreo} from "../models/RespuestaPrevisualizacionCorreo";
import {FiltroGrillaCorreos} from "../models/FiltroGrillaCorreos";
import {FiltroPlantillaBorrador} from "../models/FiltroPlantillaBorrador";
import 'rxjs/add/operator/map';
import {Constantes} from '../../config.global';
import {Util} from "../utils/util";

@Injectable()
export class CorreoService {
    private urlWebApi;
    private urlGuardarCorreo;
    private urlEnviarCorreo;
    private urlGuardarAdjunto;
    private urlPrevisualizarCorreo;
    private urlBuscarCorreosBandejaSalida;
    private urlBuscarPlantillasBorradores;

    constructor(private _http:Http, private constantes:Constantes, private util:Util) { 
        this.urlWebApi = constantes.urlWebApi;
        this.urlGuardarCorreo = this.urlWebApi + "Correo/GuardarCorreo";
        this.urlEnviarCorreo = this.urlWebApi + "Correo/EnviarCorreo";
        this.urlGuardarAdjunto = this.urlWebApi + "Correo/GuardarAdjuntos";
        this.urlPrevisualizarCorreo = this.urlWebApi + "Correo/PrevisualizarCorreo";
        this.urlBuscarCorreosBandejaSalida = this.urlWebApi + "Correo/FiltrarCorreos";
        this.urlBuscarPlantillasBorradores = this.urlWebApi + "Correo/FiltrarPlantillasBorradores";
    }

    public guardarCorreo(correo:Correo) {
      
        let body = JSON.stringify(correo);
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers, method: "post", withCredentials: true });

        console.log("POST "+this.urlGuardarCorreo);
                     
        return this._http.post(this.urlGuardarCorreo, body, options).
            map(res => res.json())
            .catch(error => this.util.manejarErrorPeticionHttp(error));
    }

    public guardarAdjunto(correo:Correo) {

        
        let body = JSON.stringify(correo);

        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers, method: "post", withCredentials:true });

        console.log("POST "+this.urlGuardarAdjunto);
        return this._http.post(this.urlGuardarAdjunto, body, options).
            map(res => res.json())
            .catch(error => this.util.manejarErrorPeticionHttp(error));
    }

    public enviarCorreo(correo:Correo) {
        let body = JSON.stringify(correo);
        console.log("Peso de json a enviar por post: "+body.length);
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers, method: "post", withCredentials: true });

        console.log("POST "+this.urlEnviarCorreo);
        return this._http.post(this.urlEnviarCorreo, body, options).
            map(res => res.json())
            .catch(error => this.util.manejarErrorPeticionHttp(error));
    }

    public previsualizarCorreo(previsualizacionCorreo:PrevisualizacionCorreo) {
        let body = JSON.stringify(previsualizacionCorreo);
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers, method: "post", withCredentials: true });

        console.log("POST "+this.urlPrevisualizarCorreo);
        return this._http.post(this.urlPrevisualizarCorreo, body, options).
            map(res => res.json())
            .catch(error => this.util.manejarErrorPeticionHttp(error));
    }

    public obtenerCorreosBandejaSalida(filtro:FiltroGrillaCorreos) {
        let body = JSON.stringify(filtro);
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers, method: "post", withCredentials: true });

        console.log("POST "+this.urlBuscarCorreosBandejaSalida);
        return this._http.post(this.urlBuscarCorreosBandejaSalida, body, options).
            map(res => { 
                console.log(res);
                return res.json(); 
            })
            .catch(error => this.util.manejarErrorPeticionHttp(error));
    }

    public obtenerPlantillasBorradores(filtro:FiltroPlantillaBorrador) {
        let body = JSON.stringify(filtro);
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers, method: "post", withCredentials: true });

        console.log("POST "+this.urlBuscarPlantillasBorradores);
        return this._http.post(this.urlBuscarPlantillasBorradores, body, options).
            map(res => { 
                console.log(res);
                return res.json(); 
            })
             .catch(error => this.util.manejarErrorPeticionHttp(error));
   }

    public ObtenerPlantilla(idPlantilla:number) {
        //console.log( this.urlWebApi + "/Correo/ObtenerPlantilla?idPlantilla="+idPlantilla );
        //let body = JSON.stringify({ 'User': 'aauribem', 'Password': 'apiux.NRR'});
        let headers = new Headers();
          //  headers.append('Content-Type','application/json' );
           // headers.append("Cookie=" + strToken );

        return this._http.get(this.urlWebApi + "/Correo/ObtenerPlantilla?idPlantilla="+idPlantilla, { withCredentials: true })
            .map(res => res.json())
            .catch(error => this.util.manejarErrorPeticionHttp(error));
      }
}