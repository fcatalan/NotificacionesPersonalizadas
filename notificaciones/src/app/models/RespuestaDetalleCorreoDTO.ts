import {DetalleCorreoDTO} from "./DetalleCorreoDTO";
export class RespuestaDetalleCorreoDTO {
	public codError:number;
	public msjError:string;
	public cantResults:number;
	public correos:DetalleCorreoDTO[];
}