import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { routing, appRoutingProviders } from './app.router';
import { FormsModule } from '@angular/forms';
import { HttpModule, JsonpModule } from '@angular/http';
import { MyDatePickerModule } from 'mydatepicker';
import { Ng2AutoCompleteModule } from 'ng2-auto-complete';
import { ConfigModule, ConfigLoader, ConfigStaticLoader } from 'ng2-config';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';

// Componentes 
import { menuComponent} from './menu/menu.component';
import { AppComponent} from './app/app.component';
import { CorreoPlantillaBorradorComponent} from './correo/correoplantillaborrador/correoplantillaborrador.component';
import { nuevocorreoComponent} from './correo/nuevo-correo/nuevocorreo.component';
import { envionotificacionesComponent } from './notificaciones/envioNotificacion/envionotificaciones.component';
import { crearnotificacionComponent } from './notificaciones/crearNotificacion/crearnotificacion.component';
import { nuevanotificacionComponent} from './notificaciones/nuevaNotificacion/nuevanotificacion.component';
import { NuevaNotifUsandoAnteriorComponent}  from './notificaciones/nuevaNotificacion/nuevanotifusandoanterior.component';
import { mantenedorComponent} from './mantenedor/mantenedor.component';
import { SeleccionSistemaEmisorCorreoComponent  } from './correo/seleccionSistemaEmisor.component';
import { previsualizacionCorreo} from './correo/previsualizacion-correo/previsualizacion-correo';
import { bandejasalidaComponent} from  './correo/bandejasalida/bandejasalida.component';
import { ErrorConexionBackendComponent} from  './error/errorConexionBackend.component';
import { SesionCerradaComponent} from  './sesionCerrada/sesionCerrada.component';
import { MenuNotificacionComponent} from './menu/menuNotificacion.component';
import { SeleccionarSistemaComponent} from './notificaciones/crearNotificacion/seleccionarsistema.component';

//import {Util} from './utils/util';
// route
// Add all operators to Observable
import 'rxjs/Rx';

export function configFactory() {

  return new ConfigStaticLoader('config.json'); // PATH || API ENDPOINT
}

@NgModule({
  declarations: [
   AppComponent,
   menuComponent,
   nuevocorreoComponent,
   CorreoPlantillaBorradorComponent,
   envionotificacionesComponent,
   crearnotificacionComponent,
   nuevanotificacionComponent,
   NuevaNotifUsandoAnteriorComponent,
   mantenedorComponent,
   SeleccionSistemaEmisorCorreoComponent,
   previsualizacionCorreo,
   bandejasalidaComponent,
   ErrorConexionBackendComponent,
   SesionCerradaComponent,
   MenuNotificacionComponent,
   SeleccionarSistemaComponent
   //Util
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpModule,
    routing,
    JsonpModule,
    Ng2AutoCompleteModule,
    MyDatePickerModule,
    ConfigModule.forRoot({ provide: ConfigLoader, useFactory: (configFactory)})
  ],
  providers: [appRoutingProviders, {provide: LocationStrategy, useClass: HashLocationStrategy}],
  bootstrap: [AppComponent]
})


export class AppModule { 
  constructor() {

  }
}
