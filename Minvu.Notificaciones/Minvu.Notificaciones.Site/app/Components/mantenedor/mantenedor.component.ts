import { Component } from '@angular/core';
import { PalabrasProhibidasService } from '../../services/PalabrasProhibidasServices';
import { PalabrasProhibidas } from '../../models/PalabrasProhibidas';
import { serviceUrl } from '../../services/ServiceUrl';
@Component({
    selector: "mantenedor",
    templateUrl: 'app/Components/mantenedor/mantenedor.component.html',
    providers: [PalabrasProhibidasService]
})

export class mantenedorComponent {
    titulo: string = "Mantenedor";
    palabraProhibidas: PalabrasProhibidas;
   
    constructor(private _notificacionServices: PalabrasProhibidasService, private _url: serviceUrl)
    {
        
        this._notificacionServices.CrearPalabraProhibida(this.palabraProhibidas, this._url.urlCrearPalabraProhibida)
    }
}