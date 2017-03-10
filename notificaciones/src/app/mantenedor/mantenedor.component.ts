import { Router, ActivatedRoute, Params } from '@angular/router';
import {Component, OnInit} from '@angular/core';
import { PalabraProhibidaService} from "../services/palabraprohibida.service";
import { PalabraProhibidaDTO} from "../models/PalabraProhibidaDTO";
import { FiltroPalabraProhibida} from "../models/FiltroPalabraProhibida";
import { RespuestaGenerica } from "../models/RespuestaGenerica";
import { Util} from "../utils/util"
import { Constantes} from "../../config.global"

@Component({ 
    selector : 'mantenedor',
    templateUrl : './mantenedor.component.html',
    providers: [PalabraProhibidaService, Constantes,]
})
export class mantenedorComponent implements OnInit {
    public respuestaGenerica:RespuestaGenerica;
    public palabraProhibida:PalabraProhibidaDTO;
    public palabraProhibidaEdit:PalabraProhibidaDTO;
    public nuevaPalabraProhibida:PalabraProhibidaDTO;
    private errorMessage; 
    public palabraProhibidaLst: PalabraProhibidaDTO[];
    public palabraBuscar:string;
    public filtroPalabraProhibida:FiltroPalabraProhibida;
    public totalPaginas:number;
    public numPaginaActual:number;
    public cantidadTotBusqueda:number;
    public idPalabraEditada:number;
    public palabraEditada:string;
    public nuevaPalabra:string;
    public idcmbMostrar:string;

    constructor(private palabraprohibidaservice:PalabraProhibidaService, 
                private router:Router, 
                private constantes:Constantes) {

       this.palabraProhibida = new PalabraProhibidaDTO();
       this.palabraProhibidaLst = [];
       this.filtroPalabraProhibida = new FiltroPalabraProhibida();
        var self = this;
        Util.mostrarFuncionalidadAdministrarPalabrasProhibidas.subscribe((value:boolean) => {
            if (!value) {
                self.router.navigate(self.constantes.PAGINA_REDIRECCION_SIN_PERMISO);
            }
        });
        
    }
    ngOnInit(){
        this.palabraBuscar = "";

        this.numPaginaActual = 0;
        this.totalPaginas = 0;
        this.filtroPalabraProhibida.Palabra = "";
        this.filtroPalabraProhibida.NumeroPagina = 1;
        this.filtroPalabraProhibida.CantidadRegPorPaginacion =5;
        //this.obtenerPalabras(this.filtroPalabraProhibida);
        this.idcmbMostrar="Todos";

    }
    CrearPalabra(palabraNueva:string)
    { 
       this.nuevaPalabraProhibida = new PalabraProhibidaDTO();
       this.nuevaPalabraProhibida.Idpalabra = 0;
       this.nuevaPalabraProhibida.Palabra = palabraNueva;
       this.nuevaPalabraProhibida.Estadovigencia = true;
       debugger;
       this.GuardarPalabra(this.nuevaPalabraProhibida);
    }
    setearPalabraSeleccionadaAEditar(idPalabra:number,palabra:string)
    {
 
     this.palabraProhibidaEdit = new PalabraProhibidaDTO();
     this.palabraProhibidaEdit.Idpalabra = idPalabra;
     this.palabraProhibidaEdit.Palabra = palabra;
     this.palabraEditada = palabra;
    
     
    }
    guardarPalabraEditada()
    {
     //this.palabraProhibidaEdit.Idpalabra = idPalabra;
     this.palabraProhibidaEdit.Palabra = this.palabraEditada;
     this.editarPalabraService(this.palabraProhibidaEdit)
     //28-02-2017 se agrega 2 funciones. --> Se debe probar.

      this.obtenerPalabras(this.filtroPalabraProhibida);
    }
    borrarPalabra(idPalabra:number){
  
       this.palabraprohibidaservice.eliminarPalabra(idPalabra)
         .subscribe(
            result => { this.respuestaGenerica = result;  
           
                if (this.respuestaGenerica.CodError ==0){
                     Util.mostrarAlerta("Información", "Palabra Eliminada correctamente.");
                    this.obtenerPalabras(this.filtroPalabraProhibida )
                }
            },
            error => {
                this.errorMessage = <any>error;

                if (this.errorMessage !== null) {
                    console.log(this.errorMessage);
                    Util.mostrarAlerta("Error", "No fue posible eliminar la palabra.<br>"+
                    "Consulte al encargado del sistema.");
                }
            }
            );
    }
    buscarPalabra(palabra:string){
        debugger
        var valorCantidadAMostrar = this.idcmbMostrar;

      if (this.palabraBuscar!=""){
          this.palabraprohibidaservice.obtenerUnaPalabra(palabra)
         .subscribe(
            result => { this.palabraProhibidaLst = result; 
                        
                       if(this.palabraProhibidaLst[0]==null){
                           this.resultadoPalabraNoEncontrada();
                           Util.mostrarAlerta("Información","No se encontró palabra en el sistema.");
                       }
                       else
                        this.actualizaPaginacion();
            },
            error => {
                this.errorMessage = <any>error;

                if (this.errorMessage !== null) {
                    
                    Util.mostrarAlerta("Error", "No fue posible obtener palabras.");
                }
            }
            );
          }
          else{
              if (this.idcmbMostrar =="Todos"){
                  this.filtroPalabraProhibida.NumeroPagina = 1;
                  this.filtroPalabraProhibida.CantidadRegPorPaginacion = 0;
                  this.obtenerPalabras(this.filtroPalabraProhibida);
                  this.numPaginaActual = 1;
                  this.totalPaginas = 1;
              }
              else{
                  this.filtroPalabraProhibida.NumeroPagina = 1;
                  //this.filtroPalabraProhibida.CantidadRegPorPaginacion = 5;
                  this.obtenerPalabras(this.filtroPalabraProhibida);
              }
          }
    }
    GuardarPalabra( objPalabra:PalabraProhibidaDTO){
            this.palabraprohibidaservice.crearPalabra(objPalabra)
             .subscribe(
            result => { this.respuestaGenerica = result;  
                       if (this.respuestaGenerica.CodError == 0)
                       {
                          this.obtenerPalabras(this.filtroPalabraProhibida);
                          Util.mostrarAlerta("Información", "Palabra guardada correctamente.");
                       }
                       else
                       {
                          Util.mostrarAlerta("Error", this.respuestaGenerica.MsjError);
                       }
            },
            error => {
                this.errorMessage = <any>error;

                if (this.errorMessage !== null) {
                    console.log(this.errorMessage);
                    Util.mostrarAlerta("Error", "No fue posible guardar palabra.");
                }
            }
            );
    }
    editarPalabraService(objPalabra:PalabraProhibidaDTO){
          this.palabraprohibidaservice.editarPalabra(objPalabra)
             .subscribe(
            result => { this.respuestaGenerica = result;
                        if (this.respuestaGenerica.CodError==0)
                        {
                          this.obtenerPalabras(this.filtroPalabraProhibida);
                          this.actualizaPaginacion();
                        }
                        else
                        {
                          Util.mostrarAlerta("Información", "No fue posible modificar la palabra.");
                        }
            },
            error => {
                this.errorMessage = <any>error;

                if (this.errorMessage !== null) {
                    console.log(this.errorMessage);
                    Util.mostrarAlerta("Error", "No fue posible modificar la palabra.");
                }
            }
            );

    }
   
    irPaginaInicio()
    {   
        this.filtroPalabraProhibida.NumeroPagina = 0;
    }
    irPaginaFinal()
    {   
        this.filtroPalabraProhibida.NumeroPagina = this.totalPaginas;
    }
   /** @description obtener notificaciones de paginación siguiente.
      * @param {number} numeropagina número de página solicitada (paginación) */
    paginacionSiguiente(numeropaginaSolicitada:number)
    {  debugger;
      if(this.numPaginaActual<this.totalPaginas)
        { 
          this.filtroPalabraProhibida.NumeroPagina = numeropaginaSolicitada;
          this.obtenerPalabras(this.filtroPalabraProhibida);
          this.numPaginaActual = numeropaginaSolicitada
        }
        else
        {
           this.filtroPalabraProhibida.NumeroPagina = this.totalPaginas;
           this.obtenerPalabras(this.filtroPalabraProhibida);
           this.numPaginaActual = this.totalPaginas;
        }
    }
    /** @description obtener notificaciones de paginación anterior.
      * @param {number} numeropagina número de página solicitada (paginación) */
    paginacionAnterior(numeropaginaSolicitada:number)
    {  debugger
     if(numeropaginaSolicitada > 0)
        {
         this.filtroPalabraProhibida.NumeroPagina = numeropaginaSolicitada;
         this.obtenerPalabras(this.filtroPalabraProhibida);
         this.numPaginaActual = numeropaginaSolicitada 
        }
    }

    /** @description Método permite obtener listado de palabras.
      * @param {FiltroPalabraProhibida} nuevoFiltro objeto con propiedades para generar filtro de palabras */
   obtenerPalabras(nuevoFiltro:FiltroPalabraProhibida )
   {
     debugger;
     this.palabraprohibidaservice.obtenerPalabras(nuevoFiltro)
         .subscribe(
            result => { this.palabraProhibidaLst = result;  
                        
                        this.actualizaPaginacion();
                //this.cantidadTotBusqueda = this.palabraProhibidaLst.length;
            },
            error => {
                this.errorMessage = <any>error;

                if (this.errorMessage !== null) {
                    console.log(this.errorMessage);
                    Util.mostrarAlerta("Error", "No fue posible obtener lista de palabras.<br>");
                }
            }
            );

   }
    actualizaPaginacion(){

       var miObj;
         //for(var indice in this.notificacionTextLst) {
           miObj=  this.palabraProhibidaLst[0];
           var totalRegistrosDeBusqueda = miObj.TotalRegistrosBusqueda;
               totalRegistrosDeBusqueda = miObj.AuxTotalRegistrosBusqueda;
           if (this.numPaginaActual==0) this.numPaginaActual=1;

           this.totalPaginas = this.ObtenerTotalPaginas(totalRegistrosDeBusqueda ,this.filtroPalabraProhibida.CantidadRegPorPaginacion);
         //}
     }
      ObtenerTotalPaginas(totalRegistros:number,cantidadPorPaginacion) {
       if (totalRegistros > 0 && cantidadPorPaginacion >0)
        {
          return Math.ceil( totalRegistros/cantidadPorPaginacion);
        }
        else
        { 
            if (cantidadPorPaginacion==0)
            {
               return 1;
            }
            if (totalRegistros ==0)
            {
                return 0;
            }
        }

		
	}
      resultadoPalabraNoEncontrada(){
           this.palabraProhibidaLst=null;
           this.totalPaginas = 0;
           this.numPaginaActual = 0;
           this.totalPaginas = 0;
      }
 /** @description Llamada generada al activadar el evento chande del combobox que contiene la paginacion
   * @param la variable $event obtiene valores generados en el evento change*/
     onChangeCmbPaginacion($event)
     { debugger;
        var cantRegistrosPaginacion =  $event.target.value;  
       if (cantRegistrosPaginacion !="Todos")
       { 
         this.filtroPalabraProhibida.CantidadRegPorPaginacion = parseInt(cantRegistrosPaginacion);  
       }
       else
       { 
         this.filtroPalabraProhibida.CantidadRegPorPaginacion = 0;
       }
     }


    
   
}