import { Component, OnInit} from '@angular/core';
import { ComunesService } from "../../services/comunes.service";
import { NotificacionService } from "../../services/notificaciones.service";
import { Notificacion } from "../../models/Notificacion";
import { NotificacionJson } from "../../models/NotificacionJson";
import { SistemaEmisor } from "../../models/sistemaEmisor";
import { Usuario } from "../../models/Usuario";
import { Router } from '@angular/router';
import { IMyOptions, IMyDateModel } from 'mydatepicker';
import { crearnotificacionComponent } from "../crearNotificacion/crearnotificacion.component";
import { FiltroBusquedaNotificacion } from "../../models/FiltroBusquedaNotificacion";
import {Util} from "../../utils/util";
import {Constantes} from '../../../config.global'

@Component({
    selector :'envionotificaciones',
    templateUrl :'./envionotificaciones.component.html',
    providers: [Constantes, ComunesService, NotificacionService, crearnotificacionComponent]
})
export class envionotificacionesComponent implements OnInit {
     public notificacionesLst: Notificacion[];
     public usuariosLst:Usuario[];
     public miNtoficacion:Notificacion;
     public actNotificacion:Notificacion;
     public notificacionText:NotificacionJson;
     public notificacionTextLst:NotificacionJson[];
     public filtroBusqNotificacion:FiltroBusquedaNotificacion;
     public sistemaEmisorLst:SistemaEmisor[];
     //Campos del filtro
     public b_nombre:string="";
     public b_fechainicio:string ="";
     public b_fechafin:string= "";
     public swUsaFiltroEspecifico:Boolean;

     public usuarioSeleccionado:string;
     public estadoNotifSeleccionado:string;
     public idEstadoDeNotificacion:number;
     public sistemaEmisorSeleccionado:number;
     //Variables para mostrar fecha y hora en popup
     public strFechaIni:string;
     public strHoraIni:string;
     public strFechaFin:string;
     public strHoraFin:string;

     public swEsIngresoAPagina:boolean;
     public totalPaginas:number;
     public numPaginaActual:number;
     public errorMessage;
     public registrosAMostrar:string;
     public myDatePickerOptions: IMyOptions;

    constructor( private _notificacionService:NotificacionService,
                 private _comunesService:ComunesService,private router: Router,
                 private constantes:Constantes) {
        this.myDatePickerOptions = {
        // other options...
        dateFormat: constantes.FORMATO_FECHA_VISIBLE,
		dayLabels: constantes.NOMBRES_DIAS_FECHAS,
		monthLabels: constantes.NOMBRES_MESES_FECHAS,
		todayBtnTxt: constantes.NOMBRE_HOY_FECHAS,
		editableDateField: false
    };
       this.miNtoficacion = new Notificacion();
       this.actNotificacion = new Notificacion();
       this.filtroBusqNotificacion= new FiltroBusquedaNotificacion();
       this.swUsaFiltroEspecifico = false;
    }
  /** @description Método propio de la clase es llamado después del constructor y después del primer ngOnChanges() 
    * @param sin parametros */
    ngOnInit(){
        this.registrosAMostrar = "Todos";
        this.swEsIngresoAPagina=true;
        
        this.filtroBusqNotificacion.NumeroPagina = 0;
        this.numPaginaActual = 0;
        this.totalPaginas = 0;
  
        this.ObtenerUsuarios();
        this.buscarSistemasEmisores();
        let self = this;
        Util.mostrarFuncionalidadEnviarNotificacion.subscribe((value:boolean) => {
            if (value != null && !value) {
                self.router.navigate(self.constantes.PAGINA_REDIRECCION_SIN_PERMISO);
            }
        });

     }

     /** @description Permite visualizar una notificación existente al levantar pop up
       * @param _idNotificacion:number parametro tiene id de la notificación seleccionada */
     verUnaNotificacion( _idNotificacion:number)
     { 
        var miId:number;
        var miObj;
        var keys;

      if (_idNotificacion==-1)
      {
        Util.mostrarAlerta("Error", "No se puede obtener la información");
      }
      // listado es utilizado en html templateUrl --> notificacionTextLst
      // notificacionTextLst:NotificacionJson[];
      for(var keyId in this.notificacionTextLst) {

          miObj = this.notificacionTextLst[keyId];
          miId = miObj.IdNotificacion; 

         if ( miId==_idNotificacion)
         { 
          this.miNtoficacion.IdNotificacion  = miObj.IdNotificacion; 
          this.miNtoficacion.Nombre = miObj.Nombre; 
          this.miNtoficacion.Mensaje = miObj.Mensaje ;
          this.miNtoficacion.FechaHorainicio = miObj.FechaHoraInicio;
          this.miNtoficacion.FechaHorafin = miObj.FechaHoraFin; 
          var strFechaCompleta1:string = this.miNtoficacion.FechaHorainicio.toString();
          var arrFechaIni = strFechaCompleta1.split('T');
          this.miNtoficacion.HoraInicio = arrFechaIni[1] ;
          var strFechaCompleta2:string = this.miNtoficacion.FechaHorafin.toString();
          var arrFechaFin = strFechaCompleta2.split('T');
          this.miNtoficacion.HoraFin = arrFechaFin[1];
         }
       }
     }
    
     /** @description Permite editar una notificación existente al levantar pop up
       * @param _idNotificacion:number parametro tiene id de la notificación seleccionada */
     actualizarNotificacion(_idNotificacion:number)
     {
       var miId:number;
        var miObj;
        var keys;

      if (_idNotificacion==-1)
      {
        Util.mostrarAlerta("Error", "No se puede obtener la información");
      }

     for(var keyId in this.notificacionTextLst) {
         miObj = this.notificacionTextLst[keyId];
         keys = Object.keys(miObj);
         miId = miObj[keys[0]];
  
         if ( miId==_idNotificacion)
         { 
          this.miNtoficacion.IdNotificacion  = miObj.IdNotificacion; 
          //this.miNtoficacion.nombre = miObj.Nombre; 
          this.miNtoficacion.Mensaje = miObj.Mensaje ;
          this.miNtoficacion.FechaHorainicio = miObj.FechaHoraInicio;
          this.miNtoficacion.FechaHorafin = miObj.FechaHoraFin; 
          //Fecha Inicial
          this.strFechaIni = this.miNtoficacion.FechaHorainicio.toString();
          var arrFechaIni = this.strFechaIni.split('T');
          this.strFechaIni =arrFechaIni[0];
          var arrfechaIniSola = arrFechaIni[0].split('-');
          this.strFechaIni = arrfechaIniSola[2]+ '-' + arrfechaIniSola[1] + '-' + arrfechaIniSola[0];
          this.selectedDateInicio = {year:+arrfechaIniSola[0], month: +arrfechaIniSola[1], day:+arrfechaIniSola[2]};
          //Fecha Final
          this.strFechaFin = this.miNtoficacion.FechaHorafin.toString();
          var arrFechaFin = this.strFechaFin.split('T');
          this.strFechaFin = arrFechaFin[0];
          var arrfechaFinSola = arrFechaFin[0].split('-');
          this.strFechaFin = arrfechaFinSola[2]+ '-' + arrfechaFinSola[1] + '-' + arrfechaFinSola[0];
          this.selectedDateFin = {year:+arrfechaFinSola[0], month: +arrfechaFinSola[1], day:+arrfechaFinSola[2]};
          //hora Ini
          var arrHora = arrFechaIni[1].split(':');
          this.strHoraIni = arrHora[0] + ':' +  arrHora[1];
          //Hora Fin
          var arrHoraFin = arrFechaFin[1].split(':'); 
          this.strHoraFin =arrHoraFin[0] + ':' + arrHoraFin[1];
          //se asignan valores a variables
          this.strMensajeModalEditar =this.miNtoficacion.Mensaje; 
          this.strFechaInicioModalEditar= this.strFechaIni ;
          this.strHoraInicioModalEditar= this.strHoraIni  ;
          this.strFechaFinModalEditar= this.strFechaFin;
          this.strHoraFinModalEditar= this.strHoraFin;
         }
       }
     }

     private strMensajeModalEditar:string;
     private strFechaInicioModalEditar:string;
     private strHoraInicioModalEditar:string;
     private strFechaFinModalEditar:string;
     private strHoraFinModalEditar:string;
     private selectedDateInicio:Object = {};
     private selectedDateFin:Object = {};

      /** @description Obtener fecha de inicio --formato fecha de salida dd/mm/yyyy utiliza mydatepicker  
       *          Modal para modificar el valor.Se actualiza variable this.b_fechainicio
       * @param {IMyDateModel} event se activa en evento change del control */
     onDateChangedFechaInicioModalEditar(event: IMyDateModel) {
      
         var fecha = event.formatted;
		 this.strFechaInicioModalEditar = fecha.split('/').join('-');
	}
     /** @description Obtener fecha fin --formato fecha de salida dd/mm/yyyy utiliza mydatepicker  
       *          Modal para modificar el valor.Se actualiza variable this.b_fechainicio
       * @param {IMyDateModel} event se activa en evento change del control */
     onDateChangedFechaFinModalEditar(event: IMyDateModel) {
         var fecha = event.formatted;
		 this.strFechaFinModalEditar = fecha.split('/').join('-');
     }


    
     /** @description Permite guardar los cambios de una notificación.
       * @param son parámetros */
     GuardarCambiosActualizacion()
     {
       this.miNtoficacion.Mensaje = this.strMensajeModalEditar;
       //fecha inicio
       var arrayTmp = this.strFechaInicioModalEditar.split("-"); 
       this.miNtoficacion.StrFechaInicio = arrayTmp[2]+ "-" + arrayTmp[1]+ "-" + arrayTmp[0];
       //fecha fin
       var arrayTmp = this.strFechaFinModalEditar.split("-"); 
       this.miNtoficacion.StrFechaFin = arrayTmp[2]+ "-" + arrayTmp[1]+ "-" + arrayTmp[0];
       //hora inicio /hora fin
       this.miNtoficacion.HoraInicio = this.strHoraInicioModalEditar;
       this.miNtoficacion.HoraFin = this.strHoraFinModalEditar;

       //var myFormattedDate = day+"-"+month+"-"+year+" "+ hours+":"+minutes+":"+seconds;
       //this.miNtoficacion.fechaHorainicio = new Date (year,month,day,hours,minutes);
      
        this._notificacionService.actualizaNotificacion(this.miNtoficacion)
        .subscribe(
         result => {   
            },
            error => {
                this.errorMessage = <any>error;
                   
                if (this.errorMessage !== null) {
                    console.log(this.errorMessage);
                    Util.mostrarAlerta("Error", "Error al tratar de actualizar la notificación<br>."+
                    "verifique formato de los valores ingresados.");
                }
            }
            );
     }
 
  /** @description Permite obtener listado de usuarios registrados en el sistema
    * @param sin parámetros */
    private ObtenerUsuarios()
     {
       this._comunesService.obtenerUsuarios()
           .subscribe(
            result => {   
               this.usuariosLst = result;
            },
            error => {
                this.errorMessage = <any>error;
                   
                if (this.errorMessage !== null) {
                    console.log(this.errorMessage);
                    Util.mostrarAlerta("Error", "Error al obtener los usuarios<br>"+
                    "Consulte la consola del navegador para más información");
                }
            }
            );

     }
  /** @description Llamada generada al activadar el evento chande del combobox CmbUsuarios
    * @param $event obtiene valores generados en el evento $event.target.value*/
     onChangeCmbUsuario($event)
     {
       this.usuarioSeleccionado =  $event.target.value;  
       this.swUsaFiltroEspecifico = true;
     }
  /** @description Llamada generada al activadar el evento chande del combobox cmbEstadoNotificacion
    * @param $event obtiene valores generados en el evento $event.target.value*/
     onChangeCmbSistemaEmisor($event)
     { debugger
       this.sistemaEmisorSeleccionado =  $event.target.value;  
       this.swUsaFiltroEspecifico = true;
     }
  /** @description Llamada generada al activadar el evento chande del combobox cmbEstadoNotificacion
    * @param $event obtiene valores generados en el evento $event.target.value*/
     onChangeCmbEstadoNotificacion($event)
     {
       this.idEstadoDeNotificacion = $event.target.value; 
       this.swUsaFiltroEspecifico = true;
     }
    
  /** @description Se busca las notificaciones de un usuario
    * @param strIdUsuario:string id del usuario a buscar.*/
     buscarNotificacionesPorUsuario(strIdUsuario:string)
     {
        strIdUsuario = this.usuarioSeleccionado;
        this.notificacionTextLst 
        this._notificacionService.buscarNotificacionesPorUsuario(strIdUsuario)
            .subscribe(
            result => {  
                         this.notificacionTextLst = result;
            },
            error => {
                this.errorMessage = <any>error;

                if (this.errorMessage !== null) {
                    console.log(this.errorMessage);
                    Util.mostrarAlerta("Error", "Error al obtener las notificaciones del usuario \""+strIdUsuario+"\"<br>"+
                    "Consulte la consola del navegador para más información");
                }
            }
            );
     }
  /** @description Se busca listado de sistemas emisores
    * @param sin parámetros.*/
     buscarSistemasEmisores()
     {
        this._comunesService.getSistemasEmisores()
            .subscribe(
            result => {  
                          this.sistemaEmisorLst = result;
            },
            error => {
                this.errorMessage = <any>error;

                if (this.errorMessage !== null) {
                    console.log(this.errorMessage);
                    Util.mostrarAlerta("Error", "Error al buscar notificaciones con filtros<br>"+
                    "Consulte la consola del navegador para más información");
                }
            }
            );

     }
     cambiarFormatoFecha(strfecha){
         if (strfecha=="")
         {
             return strfecha;
         }
         else
         {
             var arrFecha = strfecha.split('/');
             var strFechaSalida = arrFecha[2] + "-" + arrFecha[1] + "-" + arrFecha[0]
             return strFechaSalida;
         }
     }

    
     /** @description Obtener fecha de inicio --formato fecha de salida dd/mm/yyyy utiliza mydatepicker  
       *              se actualiza variable this.b_fechainicio
       * @param {IMyDateModel} event se activa en evento change delcontrol */
     onDateChangedFechaInicio(event: IMyDateModel) {
		this.b_fechainicio = event.formatted;
	}
     /** @description Obtener fecha de inicio --formato fecha de salida dd/mm/yyyy utiliza mydatepicker  
       *              se actualiza variable this.b_fechafin
       * @param {IMyDateModel} event se activa en evento change del control
       * @return {number}   */  
     onDateChangedFechaFin(event: IMyDateModel) {
		this.b_fechafin = event.formatted;	
	}
    //manejo de paginacion de registros.
    irPaginaInicio()
    { 
        this.filtroBusqNotificacion.NumeroPagina = 1;
        this.obtenerNotificaciones("MoverPaginaInicio");
    }
    irPaginaFinal()
    {
        this.filtroBusqNotificacion.NumeroPagina = this.totalPaginas;
        this.obtenerNotificaciones("MoverPaginaFinal");
    }
    paginaSiguiente(cantPaginas:number)
    {
       Util.mostrarAlerta("titulo","cuerpo");
		//let totalPaginas = this.ObtenerTotalPaginas();

		this.filtroBusqNotificacion.NumeroPagina+=cantPaginas;
		if (this.filtroBusqNotificacion.NumeroPagina+1 > this.totalPaginas) {
			this.filtroBusqNotificacion.NumeroPagina=this.totalPaginas-1;
		}
		this.numPaginaActual=2;

		this.obtenerNotificaciones("MoverPaginacion");
    }
    paginaAnterior(cantPaginas:number)
    {
       Util.mostrarAlerta("titulo","cuerpo");
		//let totalPaginas = this.ObtenerTotalPaginas();
		this.filtroBusqNotificacion.NumeroPagina+=cantPaginas;
		
		if (this.filtroBusqNotificacion.NumeroPagina<0) {
			this.filtroBusqNotificacion.NumeroPagina=0;
		}
        this.numPaginaActual=2;
		this.obtenerNotificaciones("MoverPaginacion");
    }
   
    ObtenerTotalPaginas(totalRegistros:number,cantidadPorPaginacion) {
        debugger;
        if (cantidadPorPaginacion !="Todos")
        {
		  this.totalPaginas = Math.ceil( totalRegistros/cantidadPorPaginacion);
        }
        else
          this.totalPaginas = 1;
          
        return this.totalPaginas;
	}
    /** @description obtener notificaciones de paginación siguiente.
      * @param {number} numeropagina número de página solicitada (paginación) */
    paginacionSiguiente(numeropaginaSolicitada:number)
    { 
      if(this.numPaginaActual<this.totalPaginas)
        {
         this.filtroBusqNotificacion.NumeroPagina = numeropaginaSolicitada;
         this.obtenerNotificaciones("MoverPaginacionSiguiente");
         this.numPaginaActual = numeropaginaSolicitada
        }

    }
 /** @description obtener notificaciones de paginación anterior.
   * @param {number} numeropagina número de página solicitada (paginación) */
    paginacionAnterior(numeropaginaSolicitada:number)
    { 
     if(numeropaginaSolicitada > 0)
        {
         this.filtroBusqNotificacion.NumeroPagina = numeropaginaSolicitada;
         this.obtenerNotificaciones("MoverPaginacionAnterior");
         this.numPaginaActual = numeropaginaSolicitada 
        }
    }
      
     /** @description Permite buscar notificaciones por distintos parametros de busqueda.
       *              Listado debe estar relacionado con la paginación. Se actualiza this.notificacionTextLst
       * @param {number} numeropagina número de página (paginación) */
    obtenerNotificaciones(AccionAEjecutar:string){
            debugger;
        if (this.swEsIngresoAPagina){
            this.filtroBusqNotificacion.NumeroPagina = 0;
            this.filtroBusqNotificacion.FechaInicio  = "";
            this.filtroBusqNotificacion.FechaFin     = "";
            this.filtroBusqNotificacion.CantidadPaginacion = 0; 
            this.filtroBusqNotificacion.AccionAEjecutar = "TraerTodos";
            this.filtroBusqNotificacion.IdUsuario = this.usuarioSeleccionado;
            this.filtroBusqNotificacion.IdSistemaEmisorAsoc = this.sistemaEmisorSeleccionado;
            this.filtroBusqNotificacion.IdEstadoNotificacion = this.idEstadoDeNotificacion;

            this.swEsIngresoAPagina = false;
            if(this.swUsaFiltroEspecifico)
             {
                this.filtroBusqNotificacion.AccionAEjecutar = "TraerTodosUsaFiltro";
                if (this.registrosAMostrar == "Todos"){
                    this.filtroBusqNotificacion.CantidadPaginacion = 0; 
                }
             }
        }
        else
        { 
            if (AccionAEjecutar=="NuevaBusqueda"){
                if (this.registrosAMostrar == "Todos"){
                    this.filtroBusqNotificacion.CantidadPaginacion = 0; 
                }
                else
                {
                     this.filtroBusqNotificacion.CantidadPaginacion = parseInt(this.registrosAMostrar)
                }

                this.filtroBusqNotificacion.AccionAEjecutar =AccionAEjecutar;
                this.filtroBusqNotificacion.NombreNotificacion = this.b_nombre;
                this.filtroBusqNotificacion.FechaInicio = this.cambiarFormatoFecha(this.b_fechainicio);
                this.filtroBusqNotificacion.FechaFin =  this.cambiarFormatoFecha(this.b_fechafin);
                this.filtroBusqNotificacion.IdUsuario = this.usuarioSeleccionado;
                this.filtroBusqNotificacion.EstadoNotificacion = this.estadoNotifSeleccionado;
                this.filtroBusqNotificacion.IdSistemaEmisorAsoc = this.sistemaEmisorSeleccionado;
                this.filtroBusqNotificacion.IdEstadoNotificacion= this.idEstadoDeNotificacion;
                this.filtroBusqNotificacion.IdUsuario = this.usuarioSeleccionado;
            }
           if (AccionAEjecutar=="MoverPaginacionSiguiente" || AccionAEjecutar=="MoverPaginacionAnterior" ){
               this.filtroBusqNotificacion.CantidadPaginacion =  parseInt(this.registrosAMostrar)
               this.filtroBusqNotificacion.AccionAEjecutar = AccionAEjecutar;
           }
           if (AccionAEjecutar=="MoverPaginaInicio"){
               this.filtroBusqNotificacion.CantidadPaginacion =  parseInt(this.registrosAMostrar)
               this.filtroBusqNotificacion.AccionAEjecutar = AccionAEjecutar;
               this.numPaginaActual = 1;
           }
           if (AccionAEjecutar=="MoverPaginaFinal"){
               this.filtroBusqNotificacion.CantidadPaginacion =  parseInt(this.registrosAMostrar)
               this.filtroBusqNotificacion.AccionAEjecutar = AccionAEjecutar;
               this.numPaginaActual = this.totalPaginas;
           }
        }

        this._notificacionService.buscarNotificacionesPorFiltros(this.filtroBusqNotificacion)
            .subscribe(
            result => {  
                         this.notificacionTextLst = result;
                         debugger
                         this.actualizaPaginacion();
            },
            error => {
                this.errorMessage = <any>error;

                if (this.errorMessage !== null) {
                    console.log(this.errorMessage);
                    Util.mostrarAlerta("Error", "Error al buscar notificaciones con filtros<br>"+
                    "Consulte la consola del navegador para más información");
                }
            }
            );
     }
    
  /** @description Función permite actualizar la paginación de registros.
    * @param sin parámetro*/
     actualizaPaginacion(){
 
       var miObj;
         //for(var indice in this.notificacionTextLst) {
           miObj=  this.notificacionTextLst[0];
           miObj.TotalRegistrosBusqueda;
           if (this.numPaginaActual==0) this.numPaginaActual=1;

           this.totalPaginas = this.ObtenerTotalPaginas( miObj.TotalRegistrosBusqueda,this.registrosAMostrar);//
         //}
     }
  /** @description Llamada generada al activadar el evento chande del combobox CmbUsuarios
    * @param $event obtiene valores generados en el evento $event.target.value*/
     onChangecmbMostrarRegistros($event)
     { 
       this.registrosAMostrar =  $event.target.value;  

       if (this.registrosAMostrar !="Todos")
       { 
         this.filtroBusqNotificacion.CantidadPaginacion = parseInt(this.registrosAMostrar);  
       }
       else
       { 
         this.filtroBusqNotificacion.CantidadPaginacion = 0;
       }


     }

}