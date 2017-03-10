"use strict";
var http_1 = require("@angular/http");
var Observable_1 = require("rxjs/Observable");
var PalabrasProhibidasService = (function () {
    function PalabrasProhibidasService(_http) {
        this._http = _http;
    }
    PalabrasProhibidasService.prototype.CrearPalabraProhibida = function (object, url) {
        var _this = this;
        var json = JSON.stringify({ object: object });
        var params = 'json=' + json;
        var headers = new http_1.Headers();
        headers.append('Content-Type', 'application/json');
        return this._http.post(url, params, { headers: headers })
            .map(function (res) { return res.json()
            .catch(_this.handleError); });
    };
    PalabrasProhibidasService.prototype.handleError = function (error) {
        console.error(error);
        return Observable_1.Observable.throw(error.json().error || 'Server Error');
    };
    return PalabrasProhibidasService;
}());
exports.PalabrasProhibidasService = PalabrasProhibidasService;
//# sourceMappingURL=PalabrasProhibidasServices.js.map