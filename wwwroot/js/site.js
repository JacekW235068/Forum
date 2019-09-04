$(document).ready(AccountData());

$('#accountbutton').click(function () {
    modal = document.getElementById("accountcontent");
    if (modal.innerHTML.includes("<div")) {
        $('#accountmodal').modal('toggle');
        return;
       
    }
    $('#accountmodal').modal('show');
    AccountData();
});

function AccountData() {
    modal = document.getElementById("accountcontent");
    $.ajax({
        url: "/Home/AccountInfo",
        type: 'get',
        data: { accessToken: getCookie("accessToken")},
        success: function (response) {
            var modalContent = document.getElementById("accountcontent");
            modalContent.innerHTML = response;
            if (modal.innerHTML.includes("form")) {
                eraseCookie("accessToken");
                eraseCookie("refreshToken");
                InitForm();
            }
            if (typeof init === 'function') {
                init();
            }
        
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
    $('#btnlogin').click(LoginButtonListener);
}

function LoginButtonListener() {
    if (!$("#loginform").valid()) return;
    var LoginData = $("#loginform").serialize();
    $.ajax({
        type: "POST",
        url: "/api/Account/Login",
        data: LoginData,
        success: function (response) {
            setCookie("accessToken", response.accessToken, 1);
            setCookie("refreshToken", response.refreshToken, 30);
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
                if (thrownError.responseJSON.errors.Password)
                    document.getElementById("servererrors").innerHTML = thrownError.responseJSON.errors.Password + "\n";
                if (thrownError.responseJSON.errors.Email)
                    document.getElementById("servererrors").innerHTML = thrownError.responseJSON.errors.Email + "\n";
            }

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
    document.getElementById('threadview').innerHTML = document.getElementById(id).innerHTML; 
    var postsContainer = document.createElement('div');
    var Data = {threadID: id, start: 0,amount: 10000}
    $.ajax({
        url: "/api/Post/Posts",
        type: 'get',
        data: Data,
        success: function (response) {
            response.postViewModels.forEach(function (post) {
                var $div = createNewPostView(post)
                postsContainer.innerHTML += $div;
            })
            $('#threadmodal').modal('toggle');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(thrownError);
        }
    });
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