import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';

import { SharedService } from "../../../services/shared.service";
import { SistemaEmisor } from "../../../models/sistemaEmisor";

@Component({
    
    templateUrl: 'app/Components/correo/enviocorreo/enviocorreo.component.html',
    providers: [SharedService]
})
export class enviocorreoComponent{

    public titulo: string = "Envío de Correo";
    public sistemasEmisores: SistemaEmisor[];
    public status: string;
    public errorMessage: string;

    constructor(private _notificacionesService: SharedService) {

        this._notificacionesService.getSistemasEmisores()
            .subscribe(
            result => {
                this.sistemasEmisores = result;
                console.log("obteniendo objeto rest");
                console.log(result[0]);
                //this.status = result.status;
                //if (this.status !== "success") {
                //    alert("Error en el servidor");
                //}
            },
            error => {
                this.errorMessage = <any>error;

                if (this.errorMessage !== null) {
                    console.log(this.errorMessage);
                    alert("Error en la petición");
                }
            }
            );

    }



}