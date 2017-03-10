import {PlantillaCorreoDTO} from "./PlantillaCorreoDTO";
export class RespuestaPlantillaCorreoDTO {
	public codError:number;
	public msjError:string;
	public cantResults:number;
	public plantillas:PlantillaCorreoDTO[];
}