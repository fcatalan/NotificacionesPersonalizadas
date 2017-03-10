import { Component, Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions,HttpModule } from "@angular/http";
import { Observable } from 'rxjs/Rx';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Constantes} from '../../config.global';
import { Util} from "../utils/util";
import { FiltroPalabraProhibida} from "../models/FiltroPalabraProhibida";
import { PalabraProhibidaDTO} from "../models/PalabraProhibidaDTO";

@Injectable()
export class PalabraProhibidaService {
 private handleError;    
 private urlWebApi;
    
 constructor(private _http:Http, private constantes:Constantes){
        this.urlWebApi = constantes.urlWebApi; 
        
  }

  obtenerUnaPalabra(palabra:string) {

      let options = new RequestOptions({ withCredentials: true, method: "get" });
        return this._http.get(this.urlWebApi + "palabraprohibida/BuscarPorPalabra?id="+palabra, options).
                            map(res => res.json());
                          //  .catch(Util.manejarErrorPeticionHttp);
   }

 obtenerPalabras(filtroPalabraProhibida:FiltroPalabraProhibida) {
        let body = JSON.stringify( filtroPalabraProhibida );
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers, method: "post", withCredentials: true });

        return this._http.post(this.urlWebApi + 'palabraprohibida/ObtenerPalabras' , body, options)
            .map(res => res.json());
           // .catch(Util.manejarErrorPeticionHttp);
   }
  crearPalabra(palabraProhibida:PalabraProhibidaDTO){
        let body = JSON.stringify( palabraProhibida );
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers, method: "post", withCredentials: true });

        return this._http.post(this.urlWebApi + 'palabraprohibida/CrearPalabra' , body, options)
            .map(res => res.json());
   }
   eliminarPalabra(idPalabra:number) { 

      let options = new RequestOptions({ withCredentials: true, method: "get" });
        return this._http.get(this.urlWebApi + "palabraprohibida/eliminarpalabra?id="+idPalabra, options).
                            map(res => res.json());
                          //  .catch(Util.manejarErrorPeticionHttp);
   }
    editarPalabra(palabraProhibida:PalabraProhibidaDTO) {
        let body = JSON.stringify( palabraProhibida );
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers, method: "post", withCredentials: true });

        return this._http.post(this.urlWebApi + 'palabraprohibida/ModificarPalabra' , body, options)
            .map(res => res.json());
   }

}