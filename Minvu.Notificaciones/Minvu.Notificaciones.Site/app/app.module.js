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
var platform_browser_1 = require("@angular/platform-browser");
var core_1 = require("@angular/core");
var forms_1 = require("@angular/forms");
var http_1 = require("@angular/http");
var app_component_1 = require("./app.component");
var correonuevo_component_1 = require("./Components/correo/correo-nuevo/correonuevo.component");
var correoplantilla_component_1 = require("./Components/correo/correoplantilla/correoplantilla.component");
var enviocorreo_component_1 = require("./Components/correo/enviocorreo/enviocorreo.component");
var mantenedor_component_1 = require("./Components/mantenedor/mantenedor.component");
var crearnotificacion_component_1 = require("./Components/notificaciones/crearNotificacion/crearnotificacion.component");
var notificaciones_component_1 = require("./Components/notificaciones/notificacion/notificaciones.component");
var nuevanotificacion_component_1 = require("./Components/notificaciones/nuevaNotificacion/nuevanotificacion.component");
var menu_component_1 = require("./Components/menu/menu.component");
var app_router_1 = require("./app.router");
var AppModule = (function () {
    function AppModule() {
    }
    return AppModule;
}());
AppModule = __decorate([
    core_1.NgModule({
        imports: [
            platform_browser_1.BrowserModule,
            forms_1.FormsModule,
            http_1.HttpModule,
            app_router_1.routes
        ],
        declarations: [
            app_component_1.AppComponent,
            menu_component_1.menuComponent,
            correonuevo_component_1.correonuevoComponent,
            correoplantilla_component_1.plantillacorreoComponent,
            enviocorreo_component_1.enviocorreoComponent,
            mantenedor_component_1.mantenedorComponent,
            crearnotificacion_component_1.crearnotificacionComponent,
            notificaciones_component_1.notificacionesComponent,
            nuevanotificacion_component_1.nuevanotificacionComponent
        ],
        bootstrap: [
            app_component_1.AppComponent
        ]
    }),
    __metadata("design:paramtypes", [])
], AppModule);
exports.AppModule = AppModule;
//# sourceMappingURL=app.module.js.map