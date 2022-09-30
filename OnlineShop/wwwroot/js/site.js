// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    mouseToggle("a.edit", "Edit", '<i class="fas fa-edit">');
    mouseToggle("a.info", "Details", '<i class="fas fa-info-circle">');
    mouseToggle("a.del", "Delete", '<i class="far fa-trash-alt">');
});
function mouseToggle(elementName, valueEnter, valueLeave) {
    var element = $(elementName);
    element.on("mouseenter", function () {
        $(this).html(valueEnter);
    });
    element.on("mouseleave", function () {
        $(this).html(valueLeave)
    });
    $("img").css("padding", "0px");
}