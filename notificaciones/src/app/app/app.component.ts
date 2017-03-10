import { Component, ViewEncapsulation, Renderer, OnInit} from '@angular/core';
import { Usuario } from "../models/Usuario";
import { ComunesService } from "../services/comunes.service";
import { UsuarioService } from "../services/usuario.service";
import { CookieService} from 'angular2-cookie/core';
import {Router, ActivatedRoute, Params } from "@angular/router";
import {Util} from "../utils/util";
import {Constantes} from "../../config.global";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  providers: [Constantes, ComunesService, CookieService, UsuarioService, Util]
})
export class AppComponent implements OnInit{
  title = 'app principal';
  public errorMessage: string;
  public nombreCompleto: string;
  public webApiOK:boolean;
  public mostrarLinkEnviarCorreo:boolean;
  public mostrarLinkEnviarNotificacion:boolean;
  public mostrarLinkAdministrarPalabrasProhibidas:boolean;
  public _usuario:Usuario;
  private verificacionUsuarioHecha:boolean = false;

 constructor(private comunesService:ComunesService, private router:Router, 
    private activatedRoute:ActivatedRoute, 
    private cookieService:CookieService,
    public constantes: Constantes,
    private renderer:Renderer,
    private usuarioService:UsuarioService
 ){
    this.nombreCompleto = "Nombre Usuario";
    this.webApiOK = false;
    /*
        renderer.listenGlobal("document", "click", (event) => {
            console.log(event);
            if (event != null && event.target != null && (event.target.nodeName == "A" 
                                                        || event.target.nodeName == "IMG"
                                                        || event.target.nodeName == "BUTTON"
                                                        || event.target.nodeName == "SPAN")) {
            }
        });
    */
    }

    ngOnInit() {
        this.verificarPermisos();
        this.router.events.subscribe( urlData => {
                Util.mostrarFuncionalidadEnviarCorreo.subscribe(result => {
                    if (result != null) {
                        Util.mostrarFuncionalidadEnviarNotificacion.subscribe(result => {
                            if (result != null) {
                                Util.mostrarFuncionalidadAdministrarPalabrasProhibidas.subscribe( result => {
                                    if (result != null) {
                                        let $ = require("jQuery");
                                        $(".imgCargandoMenus").hide();
                                        $(".navbar-nav a").removeClass("active");
                                        let url = this.router.url;
                                        let fragmentosCorreo = this.constantes.FRAGMENTOS_URL_ACTIVACION_MENU_SUPERIOR;
                                        for (var nombreLink in fragmentosCorreo) {
                                            fragmentosCorreo[nombreLink].forEach(fragmento => {
                                                if (url.indexOf("/"+fragmento) != -1) {
                                                    $("#"+nombreLink+" a").addClass("active");
                                                    return;
                                                }
                                            });
                                        }
                                        //if (this.activatedRoute.pathFromRoot.indexOf())
                                    }
                                });
                            }
                        });
                    }
                });
            
        })
    }


    verificarPermisos() {
        let $ = require("jQuery");
        let self = this;

        $(".imgCargandoMenus").show();
      
        this.webApiOK = false;
        Util.WebAPIOnline = this.webApiOK;
        this.mostrarLinkEnviarCorreo = false;
        this.mostrarLinkEnviarNotificacion = false;
        this.mostrarLinkAdministrarPalabrasProhibidas = false;
        
        console.log(self.comunesService);
        console.log(self.comunesService.WebAPIOnline());
        self.comunesService.WebAPIOnline().subscribe(
            response => {
              if (!response) {
                  self.router.navigate([self.constantes.PAGINA_ERROR_SIN_WEBAPI], {queryParams: {mostrarLink: 1}});
                  self.cookieService.put("mensaje", self.constantes.MENSAJE_ERROR_WEBAPI_NO_RESPONDE);

              } else {
                  switch (response.CodError) {
                  case 1:
                      self.router.navigate([self.constantes.PAGINA_ERROR_SIN_WEBAPI], {queryParams: { mostrarLink: 1}, });
                      self.cookieService.put("mensaje", response.MsjError);
                  break;
                  case -1:
                      self.router.navigate([self.constantes.PAGINA_ERROR_SIN_WEBAPI], {queryParams: {mostrarLink: 1}});
                      self.cookieService.put("mensaje", response.MsjError);
                  break;
                  case 0:
                      self.webApiOK = true;
                      Util.WebAPIOnline = self.webApiOK;
                  break;
                  }
              }
              if (self.webApiOK) {
                  //visibilidad de tareas dependiendo de permisos
                this.comunesService.ValidaPermisoTarea(this.constantes.NOMBRE_TAREA_CORREO).subscribe(
                response => {
                    if (response) {
                        this.mostrarLinkEnviarCorreo = true;
                        Util.mostrarFuncionalidadEnviarCorreo.next(true);
                    } else {
                        Util.mostrarFuncionalidadEnviarCorreo.next(false);
                    }
                },
                error => {
                    Util.mostrarFuncionalidadEnviarCorreo.next(false);
                    if (<any>error !== null) {
                    console.log(error);
                    Util.mostrarAlerta("Error", "Error al verificar el permiso para enviar correo");
                    }
                });
                  
                this.comunesService.ValidaPermisoTarea(this.constantes.NOMBRE_TAREA_NOTIFICACIONES).subscribe(
                response => {
                    if (response) {
                        this.mostrarLinkEnviarNotificacion = true;
                        Util.mostrarFuncionalidadEnviarNotificacion.next(true);
                    } else {
                        Util.mostrarFuncionalidadEnviarNotificacion.next(false);
                    }
                },
                error => {
                    Util.mostrarFuncionalidadEnviarNotificacion.next(false);
                    if (<any>error !== null) {
                    console.log(error);
                    Util.mostrarAlerta("Error", "Error al verificar el permiso para administrar notificaciones");
                    }
                });

                this.comunesService.ValidaPermisoTarea(this.constantes.NOMBRE_TAREA_MANTENEDOR_PALABRAS_PROHIBIDAS).subscribe(
                response => {
                    if (response) {
                        this.mostrarLinkAdministrarPalabrasProhibidas = true;
                        Util.mostrarFuncionalidadAdministrarPalabrasProhibidas.next(true);
                    } else {
                        Util.mostrarFuncionalidadAdministrarPalabrasProhibidas.next(false);
                    }
                },
                error => {
                    Util.mostrarFuncionalidadAdministrarPalabrasProhibidas.next(false);
                    if (<any>error !== null) {
                    console.log(error);
                    Util.mostrarAlerta("Error", "Error al verificar el permiso para administrar palabras prohibidas");
                    }
                });

                  
              } else {
                  self.router.navigate([self.constantes.PAGINA_ERROR_SIN_WEBAPI], {queryParams: {mostrarLink: 1}});
                  self.cookieService.put("mensaje", self.constantes.MENSAJE_ERROR_WEBAPI_NO_RESPONDE);
              }
              console.log("self.verificacionUsuarioHecha:"+self.verificacionUsuarioHecha);
              if (!self.verificacionUsuarioHecha) {
                self.verificacionUsuarioHecha = true;
                self.comunesService.VerificarUsuario()
                    .subscribe(
                    result => { 
                        self._usuario = result;
                        self.nombreCompleto = self._usuario.NombreCompleto;
                    },
                    error => {
                        self.errorMessage = <any>error;
                        if (self.errorMessage !== null) {
                            Util.mostrarAlerta("Error","Error al recuperar información del usuario");
                        }
                    }
                );
              }

          },
          error => {
              console.log("Error: Mostrando link para ir a PSSIM");
                  if (<any>error !== null) {
                      self.router.navigate([self.constantes.PAGINA_ERROR_SIN_WEBAPI], {queryParams: {mostrarLink: 1}});
                      self.cookieService.put("mensaje", self.constantes.MENSAJE_ERROR_WEBAPI_NO_RESPONDE);
                      self.cookieService.put("WEPAPIONLINE", "false");
                  }
              }
          );
    }

    cerrarSesion() {
        let self = this;
        this.usuarioService.cerrarSesionPSSIM().subscribe(
            response => {
                if (!response) {
                    console.log("[cerrarSesionPSSIM] no response. Redirecting to error page");
                    self.router.navigate([self.constantes.PAGINA_ERROR_SIN_WEBAPI], {queryParams: {mostrarLink: 1}});
                    self.cookieService.put("mensaje", self.constantes.MENSAJE_ERROR_WEBAPI_NO_RESPONDE);
                } else {
                    console.log("[cerrarSesionPSSIM] response.CodError:"+response.CodError);
                    if (response.CodError == 0) {
                        self.cookieService.removeAll();
                        self.router.navigate(["/sesionCerrada"]);
                    } else {
                        Util.mostrarAlerta("Error",response.MsjError);
                    }
                }
            },
            error => {
                console.log("[cerrarSesionPSSIM] error");
                self.cookieService.removeAll();
                self.router.navigate(["/sesionCerrada"]);
            }
        )
    }
}