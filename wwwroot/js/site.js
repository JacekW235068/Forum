// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$('#accountbutton').click(function () {

    var accessToken = null;
    $.ajax({
        url: "/Home/AccountInfo",
        type: 'get',
        data: accessToken,
        success: function (response) {
            var modalContent = document.getElementById("accountcontent");
            modalContent.innerHTML = response;
            $("#loginform").removeData("validator");
            $("#loginform").removeData("unobtrusiveValidation");
            $.validator.unobtrusive.parse("#loginform");
            $('#accountmodal').modal('toggle');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(thrownError);
        }
    });
});