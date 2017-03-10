export class PalabrasProhibidas
{
    constructor(
        public idPalabra: number,
        public palabra: string,
        public usuario: string,
        public fechaIngreso: any,
        public usuario_modi: string,
        public fechaModi: any,
        public estadoVigencia: boolean

    ) {

    }
}