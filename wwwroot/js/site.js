
$('#accountbutton').click(function () {
    modal = document.getElementById("accountcontent");
    if (modal.innerHTML.includes("<div")) {
        $('#accountmodal').modal('toggle');
        return;
       
    }
    AccountData();

});

function AccountData() {
    var accessToken = getCookie("accessToken");
    $.ajax({
        url: "/Home/AccountInfo",
        type: 'get',
        data: accessToken,
        success: function (response) {
            var modalContent = document.getElementById("accountcontent");
            modalContent.innerHTML = response;
            InitForm();
            $('#accountmodal').modal('toggle');

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(thrownError);
        }
    });
}

function InitForm() {
    $("#loginform").removeData("validator");
    $("#loginform").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse("#loginform");
    $('#btnlogin').click(function () {
        if (!$("#loginform").valid()) return;
        var LoginData = $("#loginform").serialize();
        $.ajax({
            type: "POST",
            url: "/api/Account/Login",
            data: LoginData,
            success: function (response) {
                setCookie(accessToken, response.responseJSON.AccessToken, 10);
                setCookie(refreshToken, response.responseJSON.RefreshToken, 10);
                AccountData();
            },
            error: function (thrownError) {
                if (thrownError.responseJSON.value) {
                    document.getElementById("servererrors").innerHTML = thrownError.responseJSON.value.error;
                    if (thrownError.responseJSON.value.lockedout) {
                        //disable form
                    }
                }
                if (thrownError.responseJSON.errors) {
                    if (thrownError.responseJSON.errors.Password )
                        document.getElementById("servererrors").innerHTML = thrownError.responseJSON.errors.Password + "\n";
                    if (thrownError.responseJSON.errors.Email )
                    document.getElementById("servererrors").innerHTML = thrownError.responseJSON.errors.Email + "\n"; 
                }

            }
        });
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