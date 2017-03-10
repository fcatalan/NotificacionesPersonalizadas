export class SistemaEmisor {
    constructor(
        public IdEmisor: number,
        public CasillaCorreo: string,
        public NombreSistema: string,
        public Vigente: boolean
    ) { }
}