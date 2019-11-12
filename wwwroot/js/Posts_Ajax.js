function LoadPosts(id) {
    var Data = { threadID: id, start: start_post, amount: amount_post }
    $.ajax({
        url: "/api/Post/Posts",
        type: 'get',
        data: Data,
        success: function (response, textStatus, xhr) {
            if (response === undefined) {
                start_post = -1;
                return;
            }
            else {
                response = response.data;
                generatePosts(response);
                if (response.length == amount_post)
                    start_post += amount_post;
                //else
                //    start_post = -1;
                LoadMorePosts();
            }
            
        },
        error: function (response, ajaxOptions, thrownError) {
            $('#threadmodal').modal('hide');
            alert(thrownError);
        }
    });
}

function RemovePost(ID) {
    var Bearer = 'bearer ' + getCookie('accessToken');
    $.ajax({
        url: "/api/Post/Delete/" + ID,
        method: 'DELETE',
        headers: {
            'Authorization': Bearer
        },
        success: function (response) {
            $('#' + ID).remove();
        },
        error: function (response, ajaxOptions, thrownError) {
            if (response.status == 401 && response.getAllResponseHeaders().includes("The token is expired")) {
                RefreshToken(RemovePost, ID);
                return;
            }
            alert("sth went wrong");
        }
    });
}

function EditPost(data) {
    var Bearer = 'bearer ' + getCookie('accessToken');
    $.ajax({
        type: "POST",
        url: "/api/Post/Edit",
        headers: {
            'Authorization': Bearer
        },
        contentType: "application/json; charset=UTF-8",
        data: JSON.stringify(data),
        success: function (response, textStatus, xhr) {
            response = response.data;
            $('.ok-post-button').attr('hidden', true);
            $('#' + response.postID).find(".post-body").html("");
            $('#' + response.postID).find(".post-body").text(response.text);
        },
        error: function (response, ajaxOptions, thrownError) {
            if (response.status == 401 && response.getAllResponseHeaders().includes("The token is expired")) {
                RefreshToken(EditPost, data);
                return;
            }
            alert(thrownError);
        }
    });
}

function NewPost(Post) {
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
            var username = getCookie('username');
            var roles = getCookie('roles');
            var $div = createPostView(response, roles, username);
            $('#threadposts').append($div);
        },
        error: function (response, ajaxOptions, thrownError) {
            if (response.status == 401 && response.getAllResponseHeaders().includes("The token is expired")) {
                RefreshToken(NewPost, Post);
                return;
            }
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
}