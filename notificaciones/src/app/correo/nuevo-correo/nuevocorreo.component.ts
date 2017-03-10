import { Component, OnInit, Input} from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import {Correo} from '../../models/Correo';
import {PlantillaCorreoDTO} from '../../models/PlantillaCorreoDTO';
import {KeyedCollection} from '../../models/KeyedCollection';
import {FormsModule} from '@angular/forms';
import { CorreoService } from "../../services/correo.service";
import { CookieService} from 'angular2-cookie/core';
import { ComunesService } from "../../services/comunes.service";
import {Util} from "../../utils/util";
//import { ComunesService } from "../../services/comunes.service";
import { SistemaEmisor } from "../../models/sistemaEmisor";
import {Constantes} from "../../../config.global"
import {Adjuntos} from "../../models/Adjuntos";
import {Archivo} from "../../models/Archivo";
import {BehaviorSubject} from 'rxjs/BehaviorSubject';
//import {SeleccionSistemaEmisorCorreoComponent} from "../../correo/seleccionSistemaEmisor.component";

@Component({
    selector:'nuevocorreo',
    templateUrl: './nuevocorreo.component.html',
    providers: [Constantes, CookieService, CorreoService, Util]
 //   inputs: ['idSistemaSelec']
})

/** @description Clase que se encarga tanto de enviar correos nuevos como de enviar correos a partir de plantillas o borradores.<br>Eso se controla por los parametros get "tipoPlantilla" e "idPlantilla".<br>"tipoPlantilla" es 1 para borradores y 2 para plantillas
 */
export class nuevocorreoComponent implements OnInit{
  //constructor(_sistEmiComponente :SeleccionSistemaEmisorCorreoComponent)
  public correo:Correo;
  public tamanhoAdjuntos:number;
  public tamanhoCorreo:number;
  public errorMessage;
  /**
   * Esta variable sirve para restaurar el correo con descriptores tras reemplazarlos cuando se ve una previsualización
   */
  public cuerpoOriginal:string;
  public editorConfig;
  public tipoPlantilla:number;
  public tipoPlantillaStr:string;
  public casillaCorreoSalida:string;
  public tituloPara:string;
  private mensajeFuenteDatosCargada:string;
  private primeraVez:boolean = true;
  private adjuntos:Adjuntos;
  private flagTerminadoSubirArchivos:BehaviorSubject<boolean>;
  @Input()
  CuerpoFullHtml:string;
  public dummy:string;
  constructor(private _comunesService:ComunesService, private cookieService:CookieService, 
              private correoService:CorreoService, private activatedRoute:ActivatedRoute,
              private constantes:Constantes, private router:Router)
  {
    this.primeraVez = true;
    this.adjuntos = new Adjuntos(constantes);
    this.correo = new Correo();
    this.correo.Adjuntos = new KeyedCollection<string>();
    //var $ = require('jQuery');
  }

/** @description Metodo que se ejecuta al inicio y varias veces mas pero es el único que se encontró<br>que se ejecuta cuando ya esta cargada la página. Se controla con un flag que se ejecute 1 vez<br>No significa que no se gatillen nuevamente los subscribe pues son eventos y no métodos
 */
  ngOnInit() {
    /**
     * Acá se verifica que se ejecute una sola vez el método
     */
    if (typeof(this.primeraVez) !== "undefined" && this.primeraVez) {
      this.primeraVez = false;
      let self = this;
      this.correo = new Correo();
      this.correo.Adjuntos = new KeyedCollection<string>();
      this.cuerpoOriginal = "";
      this.correo.UsarDireccionesFuenteDatos = true;
      /**
       * Acá se queda escuchando si hay cambios en la propiedad que indica si se deben mostrar o no correos. 
       * Esto se gatilla cada vez que cambia la variable: 
       * - se ejecuta cuando es inicalizada en null en clase Util,
       * - cuando se cambia el valor a false al en el método app.component.verificarPermisos() 
       * si la WebAPI Tarea/TienePermisoTarea?nombreTarea=<nombreTarea> informa que no hay
       * permiso para la tarea o si no se puede conectar con la WebAPI
       * - cuando cambia a true si la WebAPI Tarea/TienePermisoTarea?nombreTarea=<nombreTarea>
       * reporta que si hay permiso.
       */
      Util.mostrarFuncionalidadEnviarCorreo.subscribe((value:boolean) => {
        if(value != null && !value) {
          self.router.navigate([self.constantes.PAGINA_REDIRECCION_SIN_PERMISO]);
        } else {
          self.tamanhoAdjuntos = 0;
          self.tamanhoCorreo = 0;
          self.tituloPara = self.constantes.MENSAJE_PARA_CON_FUENTE_DATOS;
          self.mensajeFuenteDatosCargada = self.constantes.MENSAJE_FUENTE_DATOS_CARGADA;
          /**
           * Leyendo parametros get para detectar si se usa plantilla o borrador o si es correo nuevo
          */
          self.activatedRoute.queryParams.map(params => params["idPlantilla"]).subscribe(valorIdPlantilla => {
            if (typeof valorIdPlantilla !== "undefined" && valorIdPlantilla != null) {
              self.activatedRoute.queryParams.map(params => params["tipoPlantilla"]).subscribe(valorTipoPlantilla => {
                if (typeof valorTipoPlantilla !== "undefined" && valorTipoPlantilla != null) {
                    self.correo.IdPlantilla = parseInt(valorIdPlantilla);
                    self.tipoPlantilla = parseInt(valorTipoPlantilla);
                    switch(self.tipoPlantilla) {
                      case 1:
                        self.tipoPlantillaStr = "borrador";
                      break;
                      case 2:
                        self.tipoPlantillaStr = "plantilla"
                      break;
                      default:
                        self.tipoPlantillaStr = "redaccion anterior"
                      break;
                    }
                  
                    /**
                     * Aca se obtiene la plantilla que contiene el cuerpo con los descriptores. 
                     * No vienen los adjuntos, solo el cuerpo y el asunto. Aqui se inicializa CKEDITOR
                     * y se debe inmediatamente setear el contenido del editor despues de inicializado
                     * ya que de otro modo no funciona (hay que estar atento a la secuencialidad con el editor
                     * ya que haciendolo con métodos asíncronos puede pasar que se llena el editor
                     * cuando todavía esta inicializando)
                     */
                    self.correoService.ObtenerPlantilla(self.correo.IdPlantilla).subscribe(      
                      result => {
                      console.log("Resultado de obtener plantilla");
                      console.log(result);
                      if (result.codError == 0) {
                        let plantillaCorreoDTO:PlantillaCorreoDTO = result.plantillaCorreoDTO;
                        self.correo.Cuerpo = decodeURIComponent(plantillaCorreoDTO.Cuerpo.replace(/\+/g," "));
                        self.cuerpoOriginal = self.correo.Cuerpo;
                        Util.inicializarCKEditor("cuerpoCorreo");
                        Util.onChangeCKEditor("cuerpoCorreo", function() {
                          self.correo.Cuerpo = self.ObtenerCuerpo();
                        });
                        Util.cambiarHTMLTextEditor("cuerpoCorreo", self.correo.Cuerpo);
                        console.log("llamando a cambiarHTMLTextEditor");
                        self.correo.Asunto = plantillaCorreoDTO.Asunto;
                      } else {
                        Util.mostrarAlerta("Error", "Error al obtener "+self.tipoPlantillaStr+"\n:" + result.MsjError);
                      }
                      
                        // this.sistemasEmisores = result;
                    },
                    error => {
                      this.errorMessage = <any>error;
                      if (self.errorMessage !== null) {
                        console.log("Error al recuperar "+self.tipoPlantillaStr+"\n"+self.errorMessage);
                        Util.mostrarAlerta("Error", "Error al recuperar "+self.tipoPlantillaStr+". Vea la consola de errores para más información");
                      }
                    });
                }
              }).unsubscribe();//se desinscribe el método para que escuche una sola vez
            } else {
              /**
               * Acá llega cuando el correo es nuevo (sin plantilla ni borrador).
              */
              Util.inicializarCKEditor("cuerpoCorreo");
              Util.onChangeCKEditor("cuerpoCorreo", function() {
                self.correo.Cuerpo = self.ObtenerCuerpo();
              });
            }
        }).unsubscribe();//se desinscribe el método para que escuche una sola vez

          /**
           * Sirve para llenar la dirección de correo de salida que se muestra en pantalla y el id del sistema emisor (que usa
           * la WebAPI para enviar el correo desde alli)
           */
          self.activatedRoute.queryParams.map(params => params["casillaCorreoSistEmisor"]).subscribe(valorCasillaSistEmisor => {
            if (typeof valorCasillaSistEmisor !== "undefined" && valorCasillaSistEmisor != null) {
              self.activatedRoute.queryParams.map(params => params["idSistemaEmisor"]).subscribe(valorIdSistemaEmisor => {
                if (typeof valorIdSistemaEmisor !== "undefined" && valorIdSistemaEmisor != null) {
                  self.casillaCorreoSalida = valorCasillaSistEmisor;
                  self.correo.IdSistemaEmisor = valorIdSistemaEmisor;
                }
              }).unsubscribe();
            }
        }).unsubscribe();
      }
      }).unsubscribe();
    }
  }

  /**
   * @description Método que se gatilla cuando cambia la casilla "Usar fuente de datos"
  */
  CambiarTextoObligatorio() {
    if (this.correo.UsarDireccionesFuenteDatos) {
      this.tituloPara = this.constantes.MENSAJE_PARA_CON_FUENTE_DATOS;
      this.correo.CC = "";
      this.correo.CCo = "";
      this.correo.Para = "";
    } else {
      this.tituloPara = this.constantes.MENSAJE_PARA_SIN_FUENTE_DATOS;
      this.correo.FuenteDatos=null;
    }
  }

  EliminarAdjunto(index) {
    if (this.adjuntos.archivos.length>0) {
      let $ = require("jQuery");
      this.adjuntos.archivos.splice(index, 1);
      this.actualizarAdjuntos();
    }
  }

  /**
   * @description Método que impide que los campos Para, Cuerpo, Asunto, CC y CCo queden undefined
   */
  private normalizarCajasTexto() {
    if (typeof this.correo.Para === "undefined")
      this.correo.Para = "";
    if (typeof this.correo.Asunto === "undefined")
      this.correo.Asunto = "";
    if (typeof this.correo.Cuerpo === "undefined")
      this.correo.Cuerpo = "";
    if (typeof this.correo.CC === "undefined")
      this.correo.CC = "";
    if (typeof this.correo.CCo == "undefined")
      this.correo.CCo = "";
  }

  /**
   * @description Método que se gatilla cuando se adjunta un archivo. Queda solemente en memoria en el modelo (propiedad correo.Adjuntos)
   */
  public adjuntarArchivo(event) {
    let $ = require("jQuery");
    if (typeof this.correo === "undefined") return;
    
    let input = event.target;
    //this.tamanhoAdjuntos = 0;
    let archivos:Archivo[] = [];
    let count:number = 0;
    this.normalizarCajasTexto();
    console.log("CANTIDAD DE ARCHIVOS A ADJUNTAR AHORA:"+input.files.length);
    if (input.files.length>0) $("#imgCargandoAdjuntos").show();
    this.flagTerminadoSubirArchivos = new BehaviorSubject(null);
    for (var index = 0; index < input.files.length; index++) {
        let self = this;
        let reader = new FileReader();
        /*
        let name = input.files[index].name;
        this.tamanhoAdjuntos+=input.files[index].size; 
        this.tamanhoCorreo=this.tamanhoAdjuntos;
        this.tamanhoCorreo+=this.correo.Asunto.length+this.correo.Cuerpo.length;
        this.tamanhoCorreo+=this.correo.Para.length+this.correo.CC.length+this.correo.CCo.length;
        */
        reader.readAsDataURL(input.files[index]);
        reader.onload = () => {
          // this 'text' is the content of the file
          
          var limitador = "base64,";
          var posBase64 = reader.result.indexOf(limitador);
          var content = reader.result.substr(posBase64+limitador.length);
          let archivo:Archivo = new Archivo(input.files[count].name, input.files[count].size, content);
          archivos.push(archivo);
          count++;
          if (count == input.files.length) {
            self.flagTerminadoSubirArchivos.next(true);
          }
          //this.correo.Adjuntos.Add(name, content);
          //this.adjuntoJson[input.files[index]]=this.arrayBufferToBase64(content);
          //console.log(name+" = "+content);
        }
    };
    this.flagTerminadoSubirArchivos.subscribe(resp => {
      if (resp != null && resp) {
        this.adjuntos.AgregarAdjunto(archivos);
        
        this.actualizarAdjuntos();
        $("#imgCargandoAdjuntos").hide();
        $("#adjunto").val("");
        $("#adjunto").each(function() {
            Util.clearInputFile($(this).get());
        })
        //this.flagTerminadoSubirArchivos.next(false);
      }
    });
    
    /*
    if (this.tamanhoAdjuntos> this.constantes.TAMANHO_MAXIMO_ADJUNTOS) {
      Util.mostrarAlerta("Error", "No se pueden agregar adjuntos que pesen más de "
      +Util.BytesToString(this.constantes.TAMANHO_MAXIMO_ADJUNTOS)
      +"\nLos adjuntos agregados en este correo pesan en total "+Util.BytesToString(this.tamanhoAdjuntos));
    }
    */
  }

  private actualizarAdjuntos() {
    let i:number = 0;
    //let keys:string[] = [];
    /*
    console.log("cantidad de adjuntos en objeto correo:"+this.correo.Adjuntos.Keys.length);
    for (i=0;i<this.correo.Adjuntos.Count();i++) {
      keys.push(this.correo.Adjuntos.Keys[i]);
    }
    console.log("cantidad de adjuntos en objeto correo recuperados:"+keys.length);
    for(i=0;i<keys.length;i++) {
      this.correo.Adjuntos.Remove(keys[i]);
    }
    console.log("cantidad de adjuntos en objeto correo despues de limpiarse:"+this.correo.Adjuntos.Count());
    console.log("cantidad de adjuntos en objeto adjuntos:"+this.adjuntos.archivos.length);
    */
    this.correo.Adjuntos = new KeyedCollection<string>();
    this.adjuntos.archivos.forEach(archivo => {
      this.correo.Adjuntos.Add(archivo.nombreArchivo, archivo.archivo);
    })
    /*
    console.log("cantidad de adjuntos en objeto correo despues de llenarse:"+this.correo.Adjuntos.Count()
    +" peso adjuntos en objeto correo:"+Util.BytesToString(JSON.stringify(this.correo.Adjuntos).length));
    */
  }

  /**
   * @description Adjunta la fuente de datos (excel xls y xlsx) en memoria en el objeto correo en campo FuenteDatos
   */
  public adjuntarFuenteDatos(event) {
    let input = event.target;
    let reader = new FileReader();
    let name = input.files[0].name;
    reader.onload = () => {
        // this 'text' is the content of the file
        var limitador = "base64,";
        var posBase64 = reader.result.indexOf(limitador);
        var content = reader.result.substr(posBase64+limitador.length);
        this.correo.FuenteDatos=content;
        //this.adjuntoJson[input.files[index]]=this.arrayBufferToBase64(content);
        let $ = require("jQuery");
        $("#fileFuenteDatos").val("");
        $("#fileFuenteDatos").each(function() {
            Util.clearInputFile($(this).get());
        })
    }
    reader.readAsDataURL(input.files[0]);
  }

  /**
   * @description Método que obtiene el html desde el editor. Los que lo invocan lo hacen para actualizar el modelo (correo.Cuerpo)<br>debido a que CKEDITOR es un editor por javascript y no angular. El textarea no guarda el html que se ve en pantalla por lo que no se puede usar ngModel
   * @returns El cuerpo del correo en html desde el editor CKEDITOR
   */
  ObtenerCuerpo():string {
    return Util.extraerHTMLCKEDITOR("cuerpoCorreo");
  }
  
  /**
   * @description Método que se llama cuando se aprieta el botón de guardar el correo
   */
  CoordinarGuardarCorreo()
  {
    this.correo.Cuerpo = Util.normalizarAlineacionImagenes(this.correo.Cuerpo);
    this.cuerpoOriginal = this.correo.Cuerpo;
    this.tamanhoCorreo=JSON.stringify(this.correo).length;
    if (this.tamanhoCorreo> this.constantes.TAMANHO_MAXIMO_CORREO) {
      Util.mostrarAlerta("Error", this.constantes.MENSAJE_JSON_MUY_PESADO_ENVIO.replace("${pesoMaximoJson}", Util.BytesToString(this.constantes.TAMANHO_MAXIMO_CORREO))
          .replace("${pesoJson}", Util.BytesToString(this.tamanhoCorreo)));
      return;
    }
    this.GuardarCorreo();
  }

  /**
   * @description Método que guarda el correo. Se guarda como borrador (en tabla PLANTILLA con IDTIPOPLANTILLA = 1)
   */
  GuardarCorreo() {
    this.correoService.guardarCorreo(this.correo)
    .subscribe(
      result => {
        if (result.CodError == 0) {
          this.correo.IdPlantilla = result.IdPlantilla;
          this.correo.Cuerpo = this.cuerpoOriginal;
          Util.mostrarAlerta("Información", "Borrador guardado con éxito");
          //Util.mostrarAlerta("Información", "Borrador guardado con Id:" + result.IdPlantilla);
        } else {
          Util.mostrarAlerta("Error", "Error al guardar borrador\n:" + result.MsjError);
        }
        
          // this.sistemasEmisores = result;
      },
      error => {
        this.errorMessage = <any>error;
        this.correo.Cuerpo = this.cuerpoOriginal;
        if (this.errorMessage !== null) {
          console.log("Error al guardar borrador:\n"+this.errorMessage);
          Util.mostrarAlerta("Error", "Error al guardar borrador. Vea la consola de errores para más información");
        }
      }
    );
  }  

  /**
   * @description Método que envía en correo que hacer ciertas validaciones antes de disparar la WebAPI que encola el correo<br>(queda guardado en tabla ENVIO y CORREO si es un correo sin fuente de datos)
   */
  CoordinarEnviarCorreo()
  {
      if ((this.correo.Para != undefined && this.correo.Para.length > 0 
      || this.correo.UsarDireccionesFuenteDatos) 
      && this.correo.Asunto != undefined && this.correo.Asunto.length > 0
      && this.correo.Cuerpo != undefined && this.correo.Cuerpo.length > 0) {
        this.correo.Cuerpo = Util.normalizarAlineacionImagenes(this.correo.Cuerpo);
        this.cuerpoOriginal = this.correo.Cuerpo;
        if (this.correo.UsarDireccionesFuenteDatos) {
          if (!this.correo.FuenteDatos || this.correo.FuenteDatos.length == 0) {
            Util.mostrarAlerta("Error", this.constantes.MENSAJE_FALTA_FUENTE_DATOS_ENVIO)
            return;
          }
          /*
          if (!this.correo.Asunto || this.correo.Asunto.length == 0) {
            Util.mostrarAlerta("Error", this.constantes.MENSAJE_FALTA_ASUNTO_ENVIO)
            return;
          }
          if (!this.correo.Cuerpo || this.correo.Cuerpo.length == 0) {
            Util.mostrarAlerta("Error", this.constantes.MENSAJE_FALTA_CUERPO_ENVIO)
            return;
          }
          */
        }
        this.tamanhoAdjuntos = this.adjuntos.TamanoArchivos(this.adjuntos.archivos);
        if (typeof this.correo === "undefined") return;
        this.normalizarCajasTexto();
        if (this.tamanhoAdjuntos> this.constantes.TAMANHO_MAXIMO_ADJUNTOS) {
          Util.mostrarAlerta("Error", "No se pueden agregar adjuntos que pesen más de "
          +Util.BytesToString(this.constantes.TAMANHO_MAXIMO_ADJUNTOS)
          +"\nLos adjuntos agregados en este correo pesan en total "+Util.BytesToString(this.tamanhoAdjuntos));
          return;
        }
        this.tamanhoCorreo=JSON.stringify(this.correo).length;
        if (this.tamanhoCorreo> this.constantes.TAMANHO_MAXIMO_CORREO) {
          Util.mostrarAlerta("Error", this.constantes.MENSAJE_JSON_MUY_PESADO_ENVIO.replace("${pesoMaximoJson}", Util.BytesToString(this.constantes.TAMANHO_MAXIMO_CORREO))
          .replace("${pesoJson}", Util.BytesToString(this.tamanhoCorreo)));
          return;
        }

        this.GenerarEnvioDeCorreo();
      } else {
        let conjuntoMensajes = "<ul>${listaErrores}</ul>";
        let mensajes = "";
        if ((this.correo.Para == undefined || this.correo.Para.length == 0) && !this.correo.UsarDireccionesFuenteDatos)  {
          mensajes+="<li>Debe ingresar el destinatario del correo.</li>"
        } 
        if (this.correo.Asunto == undefined || this.correo.Asunto.length == 0) {
          mensajes+="<li>Debe ingresar el asunto</li>"
        }
        if (this.correo.Cuerpo == undefined || this.correo.Cuerpo.length == 0) {
          mensajes+="<li>Debe ingresar el cuerpo</li>"
        }
        Util.mostrarAlerta("Advertencia", conjuntoMensajes.replace("${listaErrores}", mensajes));
      }
  } 

  /**
   * @description Método que invoca el método de WebAPI para enviar correo. Solamente muestra los errores de la WebAPI. Los errores en envío de correo quedan registrado en log.txt en WebAPI y en tabla EVENTO
   */
    GenerarEnvioDeCorreo()
    {
      this.correoService.enviarCorreo(this.correo)
      .subscribe(
        result => {
          this.correo.Cuerpo = this.cuerpoOriginal;
              //var messageResult = result;
              if (result.CodError == 0) {
                Util.mostrarAlerta("Información", "Envio de correo generado con éxito.\nDentro de un momento el correo será enviado");
                this.BorrarDatosCorreo();
              } else {
                Util.mostrarAlerta("Error", "Error al generar envío de correo:<br>"+result.MsjError);
              }
        },
        error => {
            this.errorMessage = <any>error;
            this.correo.Cuerpo = this.cuerpoOriginal;
            if (this.errorMessage !== null) {
            
                console.log(this.errorMessage);
                Util.mostrarAlerta("Error", "Error al enviar correo. Vea la consola de javascript para más información");
            }
            //this.BorrarDatosCorreo();
        }
      );
    }

    private BorrarDatosCorreo() {
      this.correo.Para = "";
      this.correo.CC = "";
      this.correo.CCo = "";
      this.correo.Asunto = "";
      this.correo.Cuerpo = "";
      this.correo.FuenteDatos = "";
      this.correo.Adjuntos = new KeyedCollection<string>();
      this.adjuntos.archivos = [];
      Util.cambiarHTMLTextEditor("cuerpoCorreo","");
      let $ = require("jQuery");
      $("input[type='file']").val("");
      $("input[type='file']").each(function() {
          Util.clearInputFile($(this).get());
      })
    }    
}