import { PalabrasProhibidas} from '../models/palabrasprohibidas';
import { Component, Injectable } from '@angular/core';
import { Http, Response, Headers } from "@angular/http";
import { Observable } from 'rxjs/Observable';


export class PalabrasProhibidasService {
    constructor(private _http: Http) { }
    

    CrearPalabraProhibida(object:any,url:string)
    {
        var json = JSON.stringify({ object });
        var params = 'json=' + json;
        var headers = new Headers();
        headers.append('Content-Type', 'application/json');
        return this._http.post(url,
            params,
            { headers: headers })
            .map(res => res.json()
            .catch(this.handleError));
    }
    private handleError(error: Response) {
        console.error(error);
        return Observable.throw(error.json().error || 'Server Error');
    }
    
}






