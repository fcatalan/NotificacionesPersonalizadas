import {Util} from "../utils/util";

export class Archivo{
      public nombreArchivo:string;
      public tamanoArchivo:number;
      public archivo:string;
      constructor (nombreArchivo:string, tamanoArchivo:number, archivo:string) { 
            this.nombreArchivo = nombreArchivo;
            this.tamanoArchivo = tamanoArchivo;
            this.archivo = archivo;
      }

      public ObtenerPesoLegible() {
            return Util.BytesToString(this.tamanoArchivo);
      }
 } 
