// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

$(function(){
    $('.modal[data-has-remote]').on('show.bs.modal', event =>{
        $($(event.relatedTarget).data('target') + ' .modal-dialog').load($(event.relatedTarget).data("remote"));
    });
});