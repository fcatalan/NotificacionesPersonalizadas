import { RouterModule, Routes } from '@angular/router';
import { ModuleWithProviders } from '@angular/core';
import { AppComponent } from './app/app.component';
import { menuComponent} from './menu/menu.component';
import { CorreoPlantillaBorradorComponent} from './correo/correoplantillaborrador/correoplantillaborrador.component';
import { nuevocorreoComponent} from './correo/nuevo-correo/nuevocorreo.component';
import { envionotificacionesComponent } from './notificaciones/envioNotificacion/envionotificaciones.component'
import { crearnotificacionComponent } from './notificaciones/crearNotificacion/crearnotificacion.component';
import { nuevanotificacionComponent} from './notificaciones/nuevaNotificacion/nuevanotificacion.component';
import { NuevaNotifUsandoAnteriorComponent}  from './notificaciones/nuevaNotificacion/nuevanotifusandoanterior.component';
import { mantenedorComponent} from './mantenedor/mantenedor.component';
import { SeleccionSistemaEmisorCorreoComponent  } from './correo/seleccionSistemaEmisor.component';
import { bandejasalidaComponent} from  './correo/bandejasalida/bandejasalida.component';
import { ErrorConexionBackendComponent} from  './error/errorConexionBackend.component';
import { SesionCerradaComponent} from  './sesionCerrada/sesionCerrada.component';
import { MenuNotificacionComponent} from './menu/menuNotificacion.component';
import { SeleccionarSistemaComponent} from './notificaciones/crearNotificacion/seleccionarsistema.component';


export const router: Routes = [
    { path: '', redirectTo:'menu',pathMatch:'full'},
    { path: 'menu' , component :menuComponent },
    { path: 'correoPlantillaBorrador', component: CorreoPlantillaBorradorComponent},
    { path: 'correonuevo', component: nuevocorreoComponent },
    { path: 'envionotificaciones', component:envionotificacionesComponent},
    { path: 'crearNotificacion/:id', component:crearnotificacionComponent},     
    { path: 'nuevanotificacion/:id', component:nuevanotificacionComponent},
    { path: 'nuevaNotificacionusandoanterior/:id', component:NuevaNotifUsandoAnteriorComponent},
    { path: 'mantenedor',component: mantenedorComponent},    
    { path: 'seleccionsistemaemisor',component: SeleccionSistemaEmisorCorreoComponent},
    { path: 'bandejasalida', component: bandejasalidaComponent},
    { path: 'errorconexion', component: ErrorConexionBackendComponent},
    { path: 'sesionCerrada', component: SesionCerradaComponent},
    { path: 'menuNotificacion', component:MenuNotificacionComponent},
    { path: 'seleccionarsistema', component:SeleccionarSistemaComponent}
]

export const appRoutingProviders: any[] = [];
export const routing = RouterModule.forRoot(router);
