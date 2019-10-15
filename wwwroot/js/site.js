$(document).ready(function () {
    AccountData();
});

$('#accountbutton').click(function() {
    $('#accountmodal').modal('toggle');
});

function AccountData() {
    $.ajax({
        url: "/Home/AccountInfo",
        type: 'get',
        data: { accessToken: getCookie("accessToken")},
        success: function (response, textStatus, xhr) {
            $('#accountcontent').html(response);
            if ($('#accountcontent').html().includes("form")) {
                eraseCookie("accessToken");
                eraseCookie("refreshToken");
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
        },
        error: function (response, ajaxOptions, thrownErrorr) {
            response = response.responseJSON.value;
            servererrors = "";
            registerpassword = "";
            registeremail = "";
            if (response.message == "Validation Problem") {
                $('#servererrors').text("");
                response.data.forEach(function (obj) {
                    if (obj.code == "Password") {
                        registerpassword += obj.description;
                        registerpassword += "\n";
                    } else if (obj.code == "Email") {
                        registeremail += obj.description;
                        registeremail += "\n";
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
                if (response.responseJSON.description) {
                    servererrors = "your account has beed locked out due to too many failed login attempt";
                    $('#btnlogin').attr("disabled", true);
                }
            }
            $('#servererrors').text(servererrors);
            $('#registeremail').text(registeremail);
            $('#registerpassword').text(registerpassword);
        }
    });
}

function createNewThreadView(thread) {
    var newDiv = document.createElement("div");
    newDiv.setAttribute("id", thread.id);
    newDiv.classList.add("grid-item");
    var divTitle = document.createElement("h3");
    divTitle.innerHTML = thread.title;
    var divContent = document.createElement("p");
    divContent.innerHTML = thread.text;
    newDiv.appendChild(divTitle);
    newDiv.appendChild(divContent);
    var $NewDiv = $(newDiv);
    return $NewDiv;
}

function ViewThread(id) {
    $('#threadview').html($( '#' + id).html()); 
    var postsContainer = document.createElement('div');
    var Data = {threadID: id, start: 0,amount: 10000}
    $.ajax({
        url: "/api/Post/Posts",
        type: 'get',
        data: Data,
        success: function (response, textStatus, xhr) {
            $('#threadmodal').modal('toggle');
            response.responseJSON.data.postViewModels.forEach(function (post) {
                var $div = createNewPostView(post)
                postsContainer.innerHTML += $div;
            })
            
        },
        error: function (response, ajaxOptions, thrownError) {
            alert(thrownError);
        }
    });
    $('#threadview').html($('#threadview').html() + postsContainer); 
}
function createNewPostView(post){
    var newDiv = document.createElement("div");
    newDiv.setAttribute("id", post.PostId);
    newDiv.classList.add("post-item");
    var divContent = document.createElement("p");
    divContent.innerHTML = post.Text;
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