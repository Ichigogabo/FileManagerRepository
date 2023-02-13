// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

window.ObtenerDominioApp = function () {
    var RaizGlobal;
    var Ruta = location.host;

    if (Ruta.indexOf("localhost") >= 0 && Ruta.indexOf(":") >= 0) {
        RaizGlobal = '';
    }
    else {
        var NombreRuta = window.location.pathname;
        var DirectorioVirtual = NombreRuta.split('/');
        RaizGlobal = DirectorioVirtual[1];
        RaizGlobal = '/' + RaizGlobal;
    }

    return RaizGlobal;
}
