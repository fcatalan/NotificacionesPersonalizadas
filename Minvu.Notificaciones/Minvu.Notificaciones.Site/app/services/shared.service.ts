import { Injectable } from "@angular/core";
import { Http, Response, Headers } from "@angular/http";
import "rxjs/add/operator/map";
import { Observable } from "rxjs/Observable";
import { SistemaEmisor } from "../models/sistemaEmisor";

@Injectable()
export class SharedService {
    constructor(private _http: Http) { }


    getSistemasEmisores() {
        // petición por get a esa url de un api rest de pruebas
        return this._http.get("http://localhost:20620/api/SistemaEmisor")
            .map(res => res.json());
    }



}