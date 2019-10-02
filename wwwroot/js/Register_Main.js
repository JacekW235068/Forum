$(document).ready(function () {
    $('#btnregister').click(function () {
        if (!$("#registerform").valid()) return;
        var LoginData = $("#registerform").serialize();
        $.ajax({
            type: "POST",
            url: "/api/Account/Register",
            data: LoginData,
            success: function (response, textStatus, xhr) {
                setCookie("accessToken", response.accessToken, 1);
                setCookie("refreshToken", response.refreshToken, 3);
                window.location.href = "/";
            },
            error: function (response, ajaxOptions, thrownError) {
                if (response.responseJSON.message == "Validation Problem") {
                    errordiv = document.getElementById("servererrors");
                    errordiv.innerHTML = "";
                    response.responseJSON.data.forEach(function (obj) {
                        if (obj.code == "Password") {
                            $('#registerpassword').text(obj.description + "\n");
                        } else if (obj.code == "Username") {
                            $('#registerusername').text(obj.description + "\n");
                        } else if (obj.code == "Email") {
                            $('#registeremail').text(obj.description + "\n");
                        } else {
                            errordiv.innerHTML += obj.description;
                            errordiv.innerHTML += "<br/>"
                        }

                    })
                }
            }
        });
    });
})