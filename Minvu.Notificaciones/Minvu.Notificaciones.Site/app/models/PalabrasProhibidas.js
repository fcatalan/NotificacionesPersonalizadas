"use strict";
var PalabrasProhibidas = (function () {
    function PalabrasProhibidas(idPalabra, palabra, usuario, fechaIngreso, usuario_modi, fechaModi, estadoVigencia) {
        this.idPalabra = idPalabra;
        this.palabra = palabra;
        this.usuario = usuario;
        this.fechaIngreso = fechaIngreso;
        this.usuario_modi = usuario_modi;
        this.fechaModi = fechaModi;
        this.estadoVigencia = estadoVigencia;
    }
    return PalabrasProhibidas;
}());
exports.PalabrasProhibidas = PalabrasProhibidas;
//# sourceMappingURL=palabrasprohibidas.js.map