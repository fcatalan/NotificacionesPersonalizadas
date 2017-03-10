export class PlantillaCorreo{
       constructor (
        public idPlantilla:number,
        public idEstadoPlantillaCorreo:number,
        public usuario:string,
        public nombre:string,
        public asunto:string,
        public cuerpo:string,
        public fechaCreacion:Date
      ) { } 
 } 
