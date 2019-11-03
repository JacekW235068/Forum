var selectedPost = "";
function ViewThread(id) {
    selectedThread = id;
    $('#thread-title').text($('#' + id).find('.item-title').text());
    $('#thread-text').text($('#' + id).find('.item-text').text());
    $('#threadmodal').modal('toggle');
    LoadPosts(id);
}

function LoadPosts(id) {
    var Data = { threadID: id, start: 0, amount: 10000 }
    $.ajax({
        url: "/api/Post/Posts",
        type: 'get',
        data: Data,
        success: function (response, textStatus, xhr) {
            if (response === undefined) {
                return;
            }
            generatePosts(response.data);
        },
        error: function (response, ajaxOptions, thrownError) {
            alert(thrownError);
        }
    });
}

function generatePosts(posts){
    var username = getCookie('username');
    var roles = getCookie('roles');
    posts.forEach(function (post) {
        var $div = createPostView(post, roles, username);
        $('#threadposts').append($div);
    });
}

function createPostView(post, roles, username) {
    var postID = post.postID;
    var userName = post.userName;
    var postTime = post.postTime;
    var text = post.text;
    var $newPostDiv = $($.parseHTML(
        `
        <div id="${postID}" class="postdiv">
            <div class="post-header">
                <div class="user">
                    ${userName}
                </div>
            </div>
            <div class="post-body">
                ${text}
            </div>
            <div class="post-footer">
                <button class="btn btn-danger remove-post-button" hidden>Delete</button>
                <button class="btn btn-warning edit-post-button" hidden>Edit</button>
                <button class="btn btn-warning ok-post-button" hidden>Ok</button>
                <br/>
                <div class="time">
                    created: ${postTime}
                </div>
            </div>
        </div>
        `
    ));
    var $remove = $newPostDiv.find('.remove-post-button');
    var $edit = $newPostDiv.find('.edit-post-button');
    var $ok = $newPostDiv.find('.ok-post-button');

    if (roles != null && (roles.includes('Admin') || userName.includes(userName))) {
        $remove.removeAttr('hidden');
        $remove.click(RemovePostListener);
    }
    if (userName != null && username.includes(userName)) {
        $edit.removeAttr('hidden');
        $edit.click(function () {
            event.stopPropagation();
            $('#edit-post-input').remove();
            var $postDiv = $(this).parent().parent().find('.post-body');
            var text = $postDiv.text();
            $postDiv.text("");
            $editPostInput = $(
                $.parseHTML(`
                    <input type="text" class="form-control" id="edit-post-input">
                `)
            );
            $postDiv.append($editPostInput);
            $('#edit-post-input').val(
                text
            );
            $(this).parent().find('.ok-post-button').removeAttr('hidden');

        });
        $ok.click(EditPost);
    }
    return $newPostDiv;
}

function RemovePostListener() {
    event.stopPropagation();
    ID = $(this).parent().parent().attr('id');
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
        error: function (xhr, ajaxOptions, thrownError) {
            console.log(thrownError);
        }
    });
}

function EditPost() {
    event.stopPropagation();
    var PostID = $(this).parent().parent().attr('id');
    var Text = $('#edit-post-input').val();
    var data = { PostID, Text };
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
            alert(thrownError);
        }
    });
}
$('#threadmodal').on('hidden.bs.modal', function () {
    $('#threadposts').html("");
});