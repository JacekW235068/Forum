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
    Post += threadID;

    var Bearer = 'bearer ' + getCookie('accessToken');
    $.ajax({
        type: "POST",
        url: "/api/Post/New",
        headers: {
            'Authorization': Bearer
        },
        data: Post,
        success: function (response, textStatus, xhr) {
            response = response.data;
            var $div = createNewPostView(response.data)
            $('#threadposts').append($div);
        },
        error: function (response, ajaxOptions, thrownError) {
            response = response.responseJSON.value;
            newsuberrors = ""
            if (response.message == "Name is not unique") {
                newsuberrors = response.message;
            } else {
                newsuberrors = response.message;
            }
            $('#newsuberrors').text(newsuberrors)
        }
    });
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
        },
        error: function (response, ajaxOptions, thrownErrorr) {
            $('#accountcontent').html(thrownErrorr);
        }
    });
}

function InitAccountForm() {
    $("#loginform").removeData("validator");
    $("#loginform").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse("#loginform");
    $('#btnlogin').click(LoginButtonListener);
}



function ViewThread(id) {
    threadID = id;
    $('#postheader').html($('#' + id).html());
    $('#threadmodal').modal('toggle');
    $.ajax({
        url: "/api/Thread/Thread",
        type: 'get',
        data: { threadID },
        success: function (response, textStatus, xhr) {
            if (xhr.status == 204)
                $('#threadheader').text("Nothing to see here :(");
            response = response.data;
            $('#threadtitle').text(response.title);
            $('#threadtext').text(response.text);
            LoadPosts(id);
        },
        error: function (response, ajaxOptions, thrownError) {
            alert(thrownError);
            $('#threadmodal').modal('toggle');
        }
    });
}

function LoadPosts(id) {
    var Data = { threadID: id, start: 0, amount: 10000 }
    $.ajax({
        url: "/api/Post/Posts",
        type: 'get',
        data: Data,
        success: function (response, textStatus, xhr) {
            if (xhr.status == 204)
                return;
            response.data.forEach(function (post) {
                var $div = createNewPostView(post)
                $('#threadposts').append($div);
            })
        },
        error: function (response, ajaxOptions, thrownError) {
            alert(thrownError);
        }
    });
}


function createNewPostView(post) {
    var newDiv = document.createElement("div");
    newDiv.setAttribute("id", post.postId);
    newDiv.classList.add("post-item");
    var divContent = document.createElement("p");
    divContent.innerHTML = post.text;
    newDiv.appendChild(divContent);
    var $NewDiv = $(newDiv);
    return $NewDiv;
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