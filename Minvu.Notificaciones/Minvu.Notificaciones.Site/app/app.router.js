"use strict";
var router_1 = require("@angular/router");
var correonuevo_component_1 = require("./Components/correo/correo-nuevo/correonuevo.component");
var correoplantilla_component_1 = require("./Components/correo/correoplantilla/correoplantilla.component");
var enviocorreo_component_1 = require("./Components/correo/enviocorreo/enviocorreo.component");
var mantenedor_component_1 = require("./Components/mantenedor/mantenedor.component");
var crearnotificacion_component_1 = require("./Components/notificaciones/crearNotificacion/crearnotificacion.component");
var notificaciones_component_1 = require("./Components/notificaciones/notificacion/notificaciones.component");
var nuevanotificacion_component_1 = require("./Components/notificaciones/nuevaNotificacion/nuevanotificacion.component");
var menu_component_1 = require("./Components/menu/menu.component");
exports.router = [
    {
        path: '', redirectTo: 'menu', pathMatch: 'full'
    },
    {
        path: 'enviocorreo', component: enviocorreo_component_1.enviocorreoComponent
    },
    {
        path: 'menu', component: menu_component_1.menuComponent
    },
    {
        path: 'nuevoCorreo', component: correonuevo_component_1.correonuevoComponent
    },
    {
        path: 'seleccionar-plantilla-correo', component: correoplantilla_component_1.plantillacorreoComponent
    },
    {
        path: 'enviar-notificaciones', component: notificaciones_component_1.notificacionesComponent
    },
    {
        path: 'crear-notificaciones', component: crearnotificacion_component_1.crearnotificacionComponent
    },
    {
        path: 'mantenedor', component: mantenedor_component_1.mantenedorComponent
    },
    {
        path: 'nuevanotificacion', component: nuevanotificacion_component_1.nuevanotificacionComponent
    }
];
exports.routes = router_1.RouterModule.forRoot(exports.router);
//# sourceMappingURL=app.router.js.map