import { Component, OnInit} from '@angular/core';
import { Notificacion } from "../../models/Notificacion";
import { RespuestaGenerica } from "../../models/RespuestaGenerica";
import { ComunesService } from "../../services/comunes.service";
import { NotificacionService } from "../../services/notificaciones.service";
import { SistemaEmisor } from "../../models/sistemaEmisor";
import { Router, ActivatedRoute, Params } from '@angular/router';
import { FormsModule} from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { IMyOptions, IMyDateModel } from 'mydatepicker';
import {Util} from "../../utils/util";
import {Constantes} from '../../../config.global';

@Component({
    selector:'seleccionarSistema',
    templateUrl: './seleccionarsistema.component.html',
    providers: [ComunesService, NotificacionService, Constantes]
})
export class SeleccionarSistemaComponent implements OnInit{
   public notificacion: Notificacion;
   public sistemasEmisoresLst: SistemaEmisor[];
   public sistemasEmiAsociadosLst: SistemaEmisor[];
   public sistEmiSelect: boolean[];
   public listaChecked:number[];
   private errorMessage; 
   private sub: any; 
   private id:number;
   private switchCrearNotifPart1:boolean;
   private switchCrearNotifPart2:boolean;
   public nombrePrueba:string;
   private hayAlgunSistSeleccionado:boolean;
  
   public myDatePickerOptions: IMyOptions;
   public valorParametro:string;

    constructor(private _notificacionesService:NotificacionService, 
                private _comunesService:ComunesService,
                private route: ActivatedRoute,
                private router:Router,
                private constantes:Constantes) {
       this.sistEmiSelect = [];
      
       this.myDatePickerOptions = {
        // other options...
        dateFormat: constantes.FORMATO_FECHA_VISIBLE,
		dayLabels: constantes.NOMBRES_DIAS_FECHAS,
		monthLabels: constantes.NOMBRES_MESES_FECHAS,
		todayBtnTxt: constantes.NOMBRE_HOY_FECHAS,
		editableDateField: false
       };
       this.hayAlgunSistSeleccionado = false;
       this.sistEmiSelect = [];
       this.sistemasEmisoresLst =[];
       this.notificacion = new Notificacion();
    }

    ngOnInit(){
     //called after the constructor and called  after the first ngOnChanges() 
         this.switchCrearNotifPart1 = true;
         this.switchCrearNotifPart2 = true;

        this._comunesService.ValidaPermisoTarea("EnviarNotificacion");

      this._comunesService.getSistemasEmisores()
         .subscribe(
            result => { this.sistemasEmisoresLst = result;  
            },
            error => {
                this.errorMessage = <any>error;

                if (this.errorMessage !== null) {
                    console.log(this.errorMessage);
                    Util.mostrarAlerta("Error", "No fue posible obtener la lista de sistemas receptores.<br>"+
                    "Consulte la cosola de javascript de su navegador");
                }
            }
            );

            
    }
     public guardarNotificacion(_notificacionesService:NotificacionService, _notificacion:Notificacion)
    {
 
    
     _notificacionesService.guardarNuevaNotificacion(_notificacion)
            .subscribe(
            result => { 
              
            },
            error => {
                this.errorMessage = <any>error;

                if (this.errorMessage !== null) {
                    console.log(this.errorMessage);
                    Util.mostrarAlerta("Error", "Error al guardar la notificación<br>."+
                    "Consulte la consola del navegador para más información");
                }
            }
            );
    
    }

    check_uncheck(e, idSistema:number, estadoChecked:boolean){

        this.valorParametro = "";
        if(e.target.checked){
          this.sistEmiSelect[e.target.id] = true; 
        }
        else
        {
          this.sistEmiSelect[e.target.id] = false; 
        }

         for(var keyId in this.sistEmiSelect) {
           this.valorParametro = this.valorParametro + "_" + keyId ;
        }
          
        for (var i = 0; i < this.sistEmiSelect.length; i++) {
            var item = this.sistEmiSelect[i];
            this.valorParametro = this.valorParametro + "_" + item ;
        }
      
     }


    procesarListadoSistEmisores()
    {
      //dejar listado en false
     for (var i = 0; i < this.sistemasEmisoresLst.length; i++) {
          var item = this.sistemasEmisoresLst[i];
          item.Vigente = false;
    }
     //Se utiliza lista de sistema Emisores asociados a la notificación.
     for (var j=0; j < this.sistemasEmiAsociadosLst.length; j++)
       {   
           var itemAsoc =  this.sistemasEmiAsociadosLst[j];

        for (var i = 0; i < this.sistemasEmisoresLst.length; i++) {
            var item = this.sistemasEmisoresLst[i];
          
            if (itemAsoc.IdEmisor == item.IdEmisor )
            {
               item.Vigente = true;
            }
        }
     }
        
    }

  //ngOnDestroy() {
  //      this.sub.unsubscribe();
  //}
  activarCrearNotificacion()
  {
    //valor del primer campo es de idNotificación, en este caso es cero
    //porque la notificacion es nueva,    
   var seleccionados:string;
       seleccionados = "Notif_0";
       
   for(var key in this.sistemasEmisoresLst) {
     var valor =  this.sistemasEmisoresLst[key];
     var chk = <HTMLInputElement> document.getElementById(valor.IdEmisor.toString());
     var isChecked = chk.checked;

     if (isChecked) 
      {
         this.hayAlgunSistSeleccionado = true;
         seleccionados = seleccionados + "_" + valor.IdEmisor.toString();
      }
    }
     //Se valida si hay a lo menos un sistema emisor seleccionados
    if (this.hayAlgunSistSeleccionado){
         this.router.navigate(['/nuevanotificacion', seleccionados]);
     }
     else
     {
       Util.mostrarAlerta("Información", "Debe seleccionar al menos un sistema receptor.<br>.");
     }
  }
   obtenerNotificacion( _idNotificacion:number):any
  {
      this._notificacionesService.buscarNotificacionPorId(_idNotificacion)
            .subscribe(
            result => {  this.notificacion = result;
            },
            error => {
                this.errorMessage = <any>error;

                if (this.errorMessage !== null) {
                    console.log(this.errorMessage);
                    Util.mostrarAlerta("Error", "Error al obtener la notificación<br>."+
                    "Puede consultar al administrador del sistema.");
                }
            }
            );
   }
  

}
