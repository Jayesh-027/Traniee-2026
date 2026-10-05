// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function goToAssign(id) {
    window.location.href = "/tickets/" + id + "/Assign";
}

function goToStatus(id) {
    window.location.href = "/tickets/" + id + "/Status";
}

