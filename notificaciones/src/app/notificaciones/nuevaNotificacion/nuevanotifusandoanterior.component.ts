import { Component, OnInit} from '@angular/core';
import { Http,HttpModule, Headers, URLSearchParams,Response} from '@angular/http';
import { NotificacionService } from "../../services/notificaciones.service";
import { Notificacion } from "../../models/Notificacion";
import { RespuestaGenerica } from "../../models/RespuestaGenerica";
import { Observable } from "rxjs/Rx";
import { IMyOptions, IMyDateModel } from 'mydatepicker';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule} from '@angular/forms';
import { Router, ActivatedRoute, Params } from '@angular/router';
import {Util} from "../../utils/util";
import {Constantes} from '../../../config.global'

@Component({
    selector :'nuevanotificacion',
    templateUrl: './nuevanotifusandoanterior.component.html',
    providers: [Constantes, NotificacionService]
})
export class NuevaNotifUsandoAnteriorComponent implements OnInit{
  public notificacion: Notificacion;
  public notificacionPrev: Notificacion;
  public errorMessage:string;
  public _respuestaGenerica :RespuestaGenerica;
  public strFechaIni:string;
  public strFechaFin:string;
  public strMensajePrev:string;
  public streFechaIniPrev:string;
  public streFechaFinPrev:string;
  //public _notificacionesService:NotificacionService;
  public mydatepicker_fechainicio:string;
  public mydatepicker_fechafin:string;
  public myDatePickerOptions: IMyOptions;
  private sub: any;
  private id:string;
  private idNotificacion:number;
  private selectedDateInicio:Object = {};
  private selectedDateFin:Object = {};

   constructor(private _notificacionesService:NotificacionService, 
               private constantes:Constantes,
               private route: ActivatedRoute,
               private router:Router){ 
       this.myDatePickerOptions =  {
        // other options...
        dateFormat: constantes.FORMATO_FECHA_VISIBLE,
		dayLabels: constantes.NOMBRES_DIAS_FECHAS,
		monthLabels: constantes.NOMBRES_MESES_FECHAS,
		todayBtnTxt: constantes.NOMBRE_HOY_FECHAS,
		editableDateField: false
    };
       this.notificacion = new Notificacion();
       this.notificacionPrev = new Notificacion();
   }
    ngOnInit(){
        //called after the constructor and called  after the first ngOnChanges()     
        this.sub = this.route.params.subscribe(params => {
           this.id = params['id'];
           this.setearParametros(this.id);
           this.obtenerNotificacion(this.idNotificacion);
      });
  
        let self = this;
        Util.mostrarFuncionalidadEnviarNotificacion.subscribe((value:boolean) => {
            if (value != null && !value) {
                self.router.navigate(self.constantes.PAGINA_REDIRECCION_SIN_PERMISO);
            }
        });

    }
    guardarNuevaNotificacion(event)
    { 
            this.notificacion.IdNotificacion = 0;
            this.notificacion.IdUsuario ="" ;
            this.notificacion.Nombre;
            
            this.notificacion.Mensaje   //     ="Este es un mensaje de prueba." ;

            var a =  this.selectedDateInicio;
            this.notificacion.FechaHorainicio = this.generaFechaHora(this.mydatepicker_fechainicio, this.notificacion.HoraInicio + ':00'); //new Date(arrIni[2] + '-' + arrIni[1] + '-' + arrIni[0] + 'T' + this.notificacion.horaInicio + ':00') ;//date
            this.notificacion.StrFechaInicio = this.cambiarFormatoFecha(this.mydatepicker_fechainicio) + " " + this.notificacion.HoraInicio + ':00';
            this.notificacion.FechaHorafin = this.generaFechaHora(this.mydatepicker_fechainicio , this.notificacion.HoraFin + ':00') ;
            this.notificacion.StrFechaFin = this.cambiarFormatoFecha( this.mydatepicker_fechafin) + " " + this.notificacion.HoraFin + ':00';
            this.notificacion.EstadoVigencia = true;                 
            this.notificacion.FechaCreacion = new Date();   
            this.notificacion.StrAux = this.id;
    
            var nombreYMensajeNotif = this.notificacion.Nombre + " " + this.notificacion.Mensaje
            this.validarPalabrasProhibidas(nombreYMensajeNotif, this._notificacionesService);

     

    }
     guardarNotificacion( _notificacion:Notificacion, _notificacionesService:NotificacionService)
    {
  
      let _respuestaGenerica = new RespuestaGenerica();  
         
         _notificacionesService.guardarNuevaNotificacion(_notificacion)
            .subscribe(
            result => {  _respuestaGenerica = result;
                 
                       if (_respuestaGenerica.CodError == 0)
                       {
                          Util.mostrarAlerta("Información", "Notificación se guardó correctamente.");
                         //this.limpiarControles();
                       } 
                       else{
                           Util.mostrarAlerta("Error", "No se logró guardar Notificación.");
                       }

                
            },
            error => {
                this.errorMessage = <any>error;

                if (this.errorMessage !== null) {
                    console.log(this.errorMessage);
                    Util.mostrarAlerta("Error", "Error al crear notificaci�n.<br>"
                    +"Revise la consola del navegador para m�s informaci�n");
                }
            }
            );
     }
     previsualizar()
     {
          // var _strfechaIni =  this.notificacion.fechaHorainicio.toString();
          // var _strfechaFin =  this.notificacion.fechaHorafin.toString();
          // var arrIni =  _strfechaIni.split('/');
          // var arrFin = _strfechaFin.split('/');
            this.strFechaIni = this.notificacion.StrFechaInicio; //_strfechaIni;
            this.strFechaFin = this.notificacion.StrFechaFin;
            this.strMensajePrev = this.notificacion.Mensaje; 
            this.streFechaIniPrev = this.mydatepicker_fechainicio;
            this.streFechaFinPrev = this.mydatepicker_fechafin;
     }

     generaFechaHora( strFecha:string,  horaMinutos:string){

      var miFechaHora:Date;
       var arrfecha =  strFecha.split('/');//dd/mm/yyyy ---> //2017-06-24T15:00:00.000Z

      var myFormattedDate = strFecha + "T"+ horaMinutos+":00.000Z";
    
      var dateResult = new Date (myFormattedDate);
    
      //var utc = miFechaHora.getTime() + (miFechaHora.getTimezoneOffset() * 60000);

      return new Date (myFormattedDate);
     }

      /** @description Obtener fecha de inicio --formato fecha de salida dd/mm/yyyy utiliza mydatepicker  
       *              se actualiza variable this.b_fechainicio
       * @param {IMyDateModel} event se activa en evento change delcontrol */
     onDateChangedFechaInicio(event: IMyDateModel) {
		this.mydatepicker_fechainicio = event.formatted;
	}
     /** @description Obtener fecha de inicio --formato fecha de salida dd/mm/yyyy utiliza mydatepicker  
       *              se actualiza variable this.b_fechafin
       * @param {IMyDateModel} event se activa en evento change del control
       * @return {number}   */  
     onDateChangedFechaFin(event: IMyDateModel) {
		this.mydatepicker_fechafin = event.formatted;	
	}
     /** @description M�todo obtiene fecha en formato dd/mm/yyyy y entrega yyyy-mm-dd
       * @param {string} strfecha recibe una fecha tipos string en formato dd/mm/yyyy
       * @return {string}   */  
     cambiarFormatoFecha(strfecha:string){
         var arrFecha = strfecha.split('/');
         var strFechaSalida = arrFecha[2] + "-" + arrFecha[1] + "-" + arrFecha[0]
         return strFechaSalida;
     }

     validarHora(hora:string)
     {
       var arrHora = hora.split(":");
        if (arrHora.length!=2) {
            Util.mostrarAlerta("Error", "Formato hora incorrecta.");
            return false;
        }
        if (parseInt(arrHora[0])<0 || parseInt(arrHora[0])>23) {
            Util.mostrarAlerta("Error", "Formato hora incorrecta.");
            return false;
        }

        if (parseInt(arrHora[1])<0 || parseInt(arrHora[1])>59) {
            Util.mostrarAlerta("Error", "Formato hora incorrecta.");
            return false;
        }
        return true;
     }
     setearParametros(strParametro:string)
     {
      if (strParametro!="")
        {
          var num_sist_emi = 0;
          var arrParam = strParametro.split('_');
          var xc = arrParam[0];
          this.idNotificacion = +xc.replace('Notif','');
          this.notificacion.StrAux = strParametro;
          this.obtenerNotificacion(this.idNotificacion);

          num_sist_emi = arrParam.length;
          for (var j=0; j < num_sist_emi; j++)
          {  
            
          }
        }

     }
     obtenerNotificacion(idnotificacion:number)
     {
           this._notificacionesService.buscarNotificacionPorId(idnotificacion)
            .subscribe(
            result => {  
             this.notificacion = result;
             this.notificacionPrev = this.notificacion;
              var arrfechaInicio = this.notificacion.StrFechaInicio.split('-');
               var arrfechaFin = this.notificacion.StrFechaFin.split('-');
     
               this.mydatepicker_fechainicio= arrfechaInicio[0] + "/" + arrfechaInicio[1] + "/" + arrfechaInicio[2] ;
               this.mydatepicker_fechafin =  arrfechaFin[0] + "/" + arrfechaFin[1] + "/" + arrfechaFin[2] ;

               this.selectedDateInicio = {year:+arrfechaInicio[2], month: +arrfechaInicio[1], day:+arrfechaInicio[0]};
               this.selectedDateFin = {year:+arrfechaFin[2], month: +arrfechaFin[1], day:+arrfechaFin[0]};
  
            },
            error => {
                this.errorMessage = <any>error;

                if (this.errorMessage !== null) {
                    console.log(this.errorMessage);
                    Util.mostrarAlerta("Error", "Error al crear notificación.<br>"
                    +"Revise la consola del navegador para más información");
                }
            }
            );
     }

      validarPalabrasProhibidas( textoValidar:string, _notificacionesService:NotificacionService)
    {
  
         _notificacionesService.validarPalabrasProhibidas(textoValidar)
            .subscribe(
            result => {  
                      var respBackend = result;
                      debugger;
                       if (respBackend != "")
                       {
                          Util.mostrarAlerta("Información", "Se encontraron las siguientes palabras prohibidas en el texto.<br>" +
                                              "( " + respBackend + " )");
                 
                       } 
                       else
                       {
                          this.guardarNotificacion(this.notificacion, this._notificacionesService );
                       }
            },
            error => {
                this.errorMessage = <any>error;

                if (this.errorMessage !== null) {
                    console.log(this.errorMessage);
                    Util.mostrarAlerta("Error", "Error al crear notificaci�n.<br>"
                    +"Revise la consola del navegador para m�s informaci�n");
                }
            }
            );
     }
     /* limpiarControles()
     {
       this.notificacion.Nombre ="";
       this.mydatepicker_fechainicio = "";          
       this.notificacion.HoraInicio = "";
       this.notificacion.HoraFin = "";
       this.notificacion.Mensaje="";
       this.selectedDateInicio = {};
       this.selectedDateFin = {};
     }*/

}