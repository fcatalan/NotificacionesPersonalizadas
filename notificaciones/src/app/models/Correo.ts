
import {KeyedCollection} from './KeyedCollection';

export class Correo{
    constructor () {

    }
    public Para:string;
    public CC:string;
    public CCo:string;
    public Asunto:string;
    public Cuerpo:string;
    public Adjuntos:KeyedCollection<string>;
    public FuenteDatos:string;
    public IdEnvio:number;
    public IdPlantilla:number;
    public NombreUsuario:string;
    public IdSistemaEmisor:string;
    public UsarDireccionesFuenteDatos:boolean;
 } 
