import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ModuleWithProviders } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './app.component';
import { correonuevoComponent } from './Components/correo/correo-nuevo/correonuevo.component';
import { plantillacorreoComponent } from './Components/correo/correoplantilla/correoplantilla.component';
import { enviocorreoComponent } from './Components/correo/enviocorreo/enviocorreo.component';
import { mantenedorComponent } from './Components/mantenedor/mantenedor.component';
import { crearnotificacionComponent } from './Components/notificaciones/crearNotificacion/crearnotificacion.component';
import { notificacionesComponent } from './Components/notificaciones/notificacion/notificaciones.component';
import { nuevanotificacionComponent } from './Components/notificaciones/nuevaNotificacion/nuevanotificacion.component';
import { menuComponent } from './Components/menu/menu.component';
import { routes } from './app.router';

@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        HttpModule,
        routes
    ],
  declarations: [
      AppComponent,
      menuComponent,
      correonuevoComponent,
      plantillacorreoComponent,
      enviocorreoComponent,
      mantenedorComponent,
      crearnotificacionComponent,
      notificacionesComponent,
      nuevanotificacionComponent
  ],
  
  bootstrap: [
      AppComponent
  ]
})
export class AppModule { }
