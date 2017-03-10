

import { Component } from '@angular/core';
import { SistemasEmisorService} from '../../../services/SistemaEmisorservice';
import { SistemaEmisor } from '../../../models/sistemaEmisor';
@Component({
    selector: "crear-notificaciones",
    templateUrl: 'app/Components/notificaciones/crearNotificacion/crearnotificacion.component.html',
    providers: [SistemasEmisorService]
})

export class crearnotificacionComponent {
    titulo: string = "Crear Notificaciones";
    public sistemaEmisor: SistemaEmisor[];
    public status: string;
    public errorMessage: string;
    public confirmado: string;

    constructor(private _notificacionService: SistemasEmisorService) {
        this._notificacionService.getSistemasEmisores().
            subscribe(
            result => {
                this.sistemaEmisor = result;
                console.log(result[0]);

            },
            error => {
                this.errorMessage = <any>error;
                if (this.errorMessage !== null) {
                    console.log(this.errorMessage);
                    alert("Error  en la petición");
                }
            });
    }

}