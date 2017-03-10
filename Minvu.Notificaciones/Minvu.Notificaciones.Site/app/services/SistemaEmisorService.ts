import { Component, Injectable } from '@angular/core';
import { Http, Response, Headers } from "@angular/http";
@Injectable()
export class SistemasEmisorService {
    private url = "http://localhost:20619/api/SistemaEmisor";

    constructor(private _http: Http) { }

    getSistemasEmisores() {
        return this._http.get(this.url).
            map(res => res.json());
        
    }
    
}