import { RouterModule, Routes } from '@angular/router';
import { ModuleWithProviders } from '@angular/core';

import { AppComponent } from './app.component';
import { correonuevoComponent } from './Components/correo/correo-nuevo/correonuevo.component';
import { plantillacorreoComponent } from './Components/correo/correoplantilla/correoplantilla.component';
import { enviocorreoComponent } from './Components/correo/enviocorreo/enviocorreo.component';
import { mantenedorComponent } from './Components/mantenedor/mantenedor.component';
import { crearnotificacionComponent } from './Components/notificaciones/crearNotificacion/crearnotificacion.component';
import { notificacionesComponent } from './Components/notificaciones/notificacion/notificaciones.component';
import { nuevanotificacionComponent } from './Components/notificaciones/nuevaNotificacion/nuevanotificacion.component';
import { menuComponent } from './Components/menu/menu.component';

export const router: Routes = [
    {
        path: '', redirectTo: 'menu', pathMatch: 'full'
    },
    {
        path: 'enviocorreo', component: enviocorreoComponent
    },
    {
        path: 'menu', component: menuComponent
    },
    {
        path: 'nuevoCorreo', component: correonuevoComponent
    },
    {
        path: 'seleccionar-plantilla-correo', component: plantillacorreoComponent
    },
    {
        path: 'enviar-notificaciones', component: notificacionesComponent
    },
    {
        path: 'crear-notificaciones', component: crearnotificacionComponent
    },
    {
        path: 'mantenedor', component: mantenedorComponent
    },
    {
        path: 'nuevanotificacion', component: nuevanotificacionComponent
    }
]
export const routes: ModuleWithProviders = RouterModule.forRoot(router)
