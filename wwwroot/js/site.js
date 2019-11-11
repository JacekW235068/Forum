//////VARIABLES//////
var threadID = "";
//////LISTENERS//////
$(document).ready(function () {
    AccountData();
});

$('#accountbutton').click(function () {
    $('#accountmodal').modal('toggle');
});

$('#threadmodal').on('hidden.bs.modal', function (e) {
    $('#threadposts').html("");
})

$('#btnnewpost').click(function () {
    if (!$("#poform").valid()) return; //why doesnt postform work????
    var Post = $("#poform").serialize();
    Post += "&parentid=";
    Post += selectedThread;
    NewPost(Post);
    
});

function LoginButtonListener() {
    if (!$("#loginform").valid()) return;
    var LoginData = $("#loginform").serialize();
    $.ajax({
        type: "POST",
        url: "/api/Account/Login",
        data: LoginData,
        success: function (response, textStatus, xhr) {
            setCookie("accessToken", response.data.accessToken, 1);
            setCookie("refreshToken", response.data.refreshToken, 30);
            AccountData();
            location.reload();
        },
        error: function (response, ajaxOptions, thrownErrorr) {
            response = response.responseJSON.value;
            servererrors = "";
            loginpassword = "";
            loginemail = "";
            if (response.message == "Validation Problem") {
                $('#servererrors').text("");
                response.data.forEach(function (obj) {
                    if (obj.code == "Password") {
                        loginpassword += obj.description;
                        loginpassword += "\n";
                    } else if (obj.code == "Email") {
                        loginemail += obj.description;
                        loginemail += "\n";
                    } else {
                        servererrors += obj.description;
                        servererrors += "\n";
                    }
                });
            } else if (response.message == "User Not Found") {
                servererrors += response.message;
                servererrors += "\n";
            } else if (response.message == "Login failed") {
                servererrors += response.message;
                servererrors += "\n";
                if (response.data.description) {
                    servererrors = "your account has beed locked out due to too many failed login attempt";
                    $('#btnlogin').attr("disabled", true);
                }
            }
            $('#servererrors').text(servererrors);
            $('#loginemail').text(loginemail);
            $('#loginpassword').text(loginpassword);
        }
    });
}
//////FUNCTIONS//////
function AccountData() {
    $.ajax({
        url: "/Home/AccountInfoAsync",
        type: 'get',
        data: { accessToken: getCookie("accessToken") },
        success: function (response, textStatus, xhr) {
            $('#accountcontent').html(response);
            if ($('#accountcontent').html().includes("form")) {
                eraseCookie("accessToken");
                eraseCookie("refreshToken");
                eraseCookie("roles");
                eraseCookie("username");
                InitAccountForm();
            } else {
                roles = "";
                $var = $('#roles > p');
                $var.each(function (obj) {
                    roles += $(this).text();
                    roles += ',';
                    setCookie("roles", roles, 1);
                })
                setCookie("username", $('#username').text(), 1);
            }
            $('#logout').click(function () {
                
                var Bearer = 'bearer ' + getCookie('accessToken');
                $.ajax({
                    url: "/api/Account/Logout",
                    method: 'POST',
                    headers: {
                        'Authorization': Bearer
                    }
                });
                eraseCookie("accessToken");
                eraseCookie("refreshToken");
                eraseCookie("roles");
                eraseCookie("username");
                location.reload();
            });
        },
        error: function (response, ajaxOptions, thrownErrorr) {
            if (response.status == 401) {
                RefreshToken()
            }
            else {
                eraseCookie("accessToken");
                eraseCookie("refreshToken");
                eraseCookie("roles");
                eraseCookie("username");
                AccountData();
            }
        }
    });
}

function InitAccountForm() {
    $("#loginform").removeData("validator");
    $("#loginform").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse("#loginform");
    $('#btnlogin').click(LoginButtonListener);
}


function RefreshToken( CallBack, CallBackData) {

    var Data = { accessToken: getCookie("accessToken"), refreshToken: getCookie("refreshToken") }
    eraseCookie("accessToken");
    eraseCookie("refreshToken");
    var result = false;
    var request = $.ajax({
        type: "POST",
        url: "/api/Account/RefreshToken",
        contentType: "application/json; charset=UTF-8",
        data: JSON.stringify(Data),
        success: function (response, textStatus, xhr) {
            setCookie("accessToken", response.data.accessToken, 1);
            setCookie("refreshToken", response.data.refreshToken, 30);
            if (CallBack !== undefined) {
                CallBack(CallBackData);
            }
            AccountData();
            result = true;
        },
        error: function (response, ajaxOptions, thrownErrorr) {
            AccountData();
        }
    });

}


//randomowe funkcje ze stacka

function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}
function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}
function eraseCookie(name) {
    document.cookie = name + '=; Max-Age=-99999999;';
}