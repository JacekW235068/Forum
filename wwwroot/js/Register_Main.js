//////VARIABLES//////

//////LISTENERS//////
$(document).ready(function () {
    $('#btnregister').click(function () {
        if (!$("#registerform").valid()) return;
        var LoginData = $("#registerform").serialize();
        $.ajax({
            type: "POST",
            url: "/api/Account/Register",
            data: LoginData,
            success: function (response, textStatus, xhr) {
                setCookie("accessToken", response.data.accessToken, 1);
                setCookie("refreshToken", response.data.refreshToken, 3);
                window.location.href = "/";
            },
            error: function (response, ajaxOptions, thrownError) {
                response = response.responseJSON.value;
                servererrors = "";
                registerpassword = "";
                registerusername = "";
                registeremail = "";
                if (response.message == "Validation Problem") {
                    $('#servererrors').text("");
                    response.data.forEach(function (obj) {
                        if (obj.code == "Password") {
                            registerpassword += obj.description;
                            registerpassword += "\n";
                        } else if (obj.code == "Username") {
                            registerusername += obj.description;
                            registerusername += "\n";
                        } else if (obj.code == "Email") {
                            registeremail += obj.description;
                            registeremail += "\n";
                        } else {
                            servererrors += obj.description;
                            servererrors += "\n";
                        }
                    });
                } else if (response.message == "Problem Creating A User") {
                    response.data.forEach(function (obj) {
                        servererrors += obj.description;
                        servererrors += "\n";
                    });
                }
                $('#servererrors').text(servererrors);
                $('#registeremail').text(registeremail);
                $('#registerpassword').text(registerpassword);
                $('#registerusername').text(registerusername);
            }
        });
    });
})
//////FUNCTIONS//////


