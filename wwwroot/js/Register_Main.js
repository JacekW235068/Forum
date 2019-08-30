$(document).ready(function () {
    $('#btnregister').click(function () {
        if (!$("#registerform").valid()) return;
        var LoginData = $("#registerform").serialize();
        $.ajax({
            type: "POST",
            url: "/api/Account/Register",
            data: LoginData,
            success: function (response) {
                setCookie(accessToken, response.responseJSON.AccessToken, 10);
                setCookie(refreshToken, response.responseJSON.RefreshToken, 10);
                //redirect
            },
            error: function (thrownError) {
                errordiv = document.getElementById("servererrors");
                errordiv.innerHTML = "";
                if (thrownError.responseJSON.value.errors) {
                    thrownError.responseJSON.value.errors.forEach(function (obj) {
                        errordiv.innerHTML += obj.description;
                        errordiv.innerHTML += "<br/>"
                    })
                }
                if (thrownError.responseJSON.errors) {
                    if (thrownError.responseJSON.errors.Password)
                        errordiv.innerHTML += thrownError.responseJSON.errors.Password + "\n";
                    if (thrownError.responseJSON.errors.Email)
                        errordiv.innerHTML += thrownError.responseJSON.errors.Email + "\n";
                }
            }
        });
    });
})