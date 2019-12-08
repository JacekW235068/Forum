//////VARIABLES//////
var threadID = "";
//////LISTENERS//////
$(document).ready(function () {
    if (getCookie("accessToken") != null)
        AccountData();
    else {
        $('#btnlogin').click(LoginButtonListener);
    }
});

$('#accountbutton').click(function () {
    $('#accountmodal').modal('toggle');
});

$('#threadmodal').on('hidden.bs.modal', function (e) {
    $('#threadposts').html("");
})

$('#btnnewpost').click(function () {
    if (!$("#postform").valid()) return; //why doesnt postform work????
    var Post = $("#postform").serialize();
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
            setCookie("accessToken", response.data.accessToken, 20);
            setCookie("refreshToken", response.data.refreshToken, 20);
            AccountData();
        },
        error: function (response, ajaxOptions, thrownErrorr) {
            response = response.responseJSON.value.value;
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
            if (response.message == "Login failed") {
                servererrors = "Login Failed";
            }
            $('#servererrors').text(servererrors);
            $('#loginemail').text(loginemail);
            $('#loginpassword').text(loginpassword);
        }
    });
}
//////FUNCTIONS//////
function AccountData() {
    var Bearer = 'bearer ' + getCookie('accessToken');
    $.ajax({
        url: "/api/Account/Info",
        type: 'get',
        headers: {
            'Authorization': Bearer
        },
        success: function (response, textStatus, xhr) {
            
            roles = "";
            response = response.data;
            response.roles.forEach(function (obj) {
                roles += obj;
                roles += ',';
                
            })
            setCookie("roles", roles, 20);
            setCookie("username", response.userName, 20);
            $('#postformdiv').removeAttr('hidden')
            
            generateAccountInfo(response);
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




function RefreshToken( CallBack, CallBackData) {

    var Data = { accessToken: getCookie("accessToken"), refreshToken: getCookie("refreshToken") }
    eraseCookie("accessToken");
    eraseCookie("refreshToken");
    $.ajax({
        type: "POST",
        url: "/api/Account/RefreshToken",
        contentType: "application/json; charset=UTF-8",
        data: JSON.stringify(Data),
        success: function (response, textStatus, xhr) {
            setCookie("accessToken", response.data.accessToken, 20);
            setCookie("refreshToken", response.data.refreshToken, 20);
            if (CallBack !== undefined) {
                CallBack(CallBackData);
            }
            AccountData();
       
        },
        error: function (response, ajaxOptions, thrownErrorr) {
            AccountData();
        }
    });

}


function generateAccountInfo(data) {
    userName = data.userName;
    roles = "";
    data.roles.forEach(function (role) {
        roles += role;
        roles += ' ';
    });
    $('#accountcontent').html(`<div id="${threadID}" class="grid-item postdiv">
                <div class="username">
                    ${userName}
                </div>
                <div class="roles">
                   ${roles}
                </div>
            <button class="btn btn-dark" id="logout">Logout</button>
`);
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