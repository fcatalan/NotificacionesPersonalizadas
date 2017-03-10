import { Component, Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from "@angular/http";
import { Observable } from 'rxjs/Rx';
import {ConsultaBusquedaUsuarios} from '../models/ConsultaBusquedaUsuarios';
import 'rxjs/add/operator/map';
import {Constantes} from '../../config.global';
import {Util} from "../utils/util";

@Injectable()
export class UsuarioService {
    private urlConsultarUsuarios;
    private urlCerrarSesion;

    constructor(private _http:Http, private constantes:Constantes, private util:Util) { 
        this.urlConsultarUsuarios = constantes.urlWebApi + "User/ConsultarInfoUsuarios";;
        this.urlCerrarSesion = constantes.urlWebApi + "User/CerrarSesion";
    }

    obtenerListaSugerenciasUsuarios = (consultaBusquedaUsuarios:ConsultaBusquedaUsuarios):Observable<any[]> => {
      
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers, method: "get", withCredentials: true });

        console.log("GET "+this.urlConsultarUsuarios+"?parteNombreUsuario="+consultaBusquedaUsuarios);
                     
        return this._http.get(this.urlConsultarUsuarios+"?parteNombreUsuario="+consultaBusquedaUsuarios, options).
            map(res => res.json())
            .catch(error => this.util.manejarErrorPeticionHttp(error));
    }

    cerrarSesionPSSIM()	{
        //console.log( this.urlWebApi + "/Correo/ObtenerPlantilla?idPlantilla="+idPlantilla );
        //let body = JSON.stringify({ 'User': 'aauribem', 'Password': 'apiux.NRR'});
        let headers = new Headers();
          //  headers.append('Content-Type','application/json' );
           // headers.append("Cookie=" + strToken );

        return this._http.get(this.urlCerrarSesion, { withCredentials: true })
            .map(res => res.json())
            .catch(error => this.util.manejarErrorPeticionHttp(error));
    }
}
