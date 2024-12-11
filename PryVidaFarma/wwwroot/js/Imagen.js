$(function () {
    document.getElementById("imagen").addEventListener("change", function (event) {
        var urlImg = event.target.files[0];
        $("#imgSelect").removeAttr("hidden");
        $("#imgSelect").attr("src", window.URL.createObjectURL(urlImg));
        event.preventDefault();
    });
});