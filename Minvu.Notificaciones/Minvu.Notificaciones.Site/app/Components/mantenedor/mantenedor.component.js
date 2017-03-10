"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require("@angular/core");
var PalabrasProhibidasServices_1 = require("../../services/PalabrasProhibidasServices");
var ServiceUrl_1 = require("../../services/ServiceUrl");
var mantenedorComponent = (function () {
    function mantenedorComponent(_notificacionServices, _url) {
        this._notificacionServices = _notificacionServices;
        this._url = _url;
        this.titulo = "Mantenedor";
        this._notificacionServices.CrearPalabraProhibida(this.palabraProhibidas, this._url.urlCrearPalabraProhibida);
    }
    return mantenedorComponent;
}());
mantenedorComponent = __decorate([
    core_1.Component({
        selector: "mantenedor",
        templateUrl: 'app/Components/mantenedor/mantenedor.component.html',
        providers: [PalabrasProhibidasServices_1.PalabrasProhibidasService]
    }),
    __metadata("design:paramtypes", [PalabrasProhibidasServices_1.PalabrasProhibidasService, ServiceUrl_1.serviceUrl])
], mantenedorComponent);
exports.mantenedorComponent = mantenedorComponent;
//# sourceMappingURL=mantenedor.component.js.map