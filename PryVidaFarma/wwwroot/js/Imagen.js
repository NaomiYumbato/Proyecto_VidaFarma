$(function () {
    document.getElementById("imagen").addEventListener("change", function (event) {
        var urlImg = event.target.files[0];

        if (urlImg) {
            $("#imgPreview").removeAttr("style").attr("src", window.URL.createObjectURL(urlImg));
            $("#imgSelect").hide();  

            var reader = new FileReader();
            reader.onload = function (e) {
                $("#imagenActual").val(e.target.result);
            }
            reader.readAsDataURL(urlImg);
        } else {
            $("#imagenActual").val($("#imgSelect").attr("src"));
        }
    });
});
