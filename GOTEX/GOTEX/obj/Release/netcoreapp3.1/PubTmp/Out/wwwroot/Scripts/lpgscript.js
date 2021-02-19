$(document).ready(function () {
    $('#loading').hide()
        .ajaxStart(function () {
            $(this).show();
        })
        .ajaxStop(function () {
            $(this).hide();
        });
})