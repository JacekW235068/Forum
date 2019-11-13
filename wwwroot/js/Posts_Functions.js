var start_post = 0;
var amount_post = 3;
function ViewThread(id) {
    selectedThread = id;
    $('#thread-title').text($('#' + id).find('.item-title').text());
    $('#thread-text').text($('#' + id).find('.item-text').text());
    $('#threadmodal').modal('toggle');
    $('#threadposts').scroll(function () {
        LoadMorePosts();
    });
    LoadPosts(id);
}

function LoadMorePosts() {
    if (($('#threadposts')[0].scrollHeight - $('#threadposts').height() - $('#threadposts').scrollTop())  < 250) {
        if (start_post > -1) {
            LoadPosts(selectedThread);
        } else {
            $('#threadview').unbind("scroll")
        }
    }
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

    if (roles != null && (roles.includes('Admin') || userName == username)) {
        $remove.removeAttr('hidden');
        $remove.click(RemovePostListener);
    }
    if (userName != null && userName.includes(username)) {
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
        $ok.click(EditPostListener);
    }
    return $newPostDiv;
}

function RemovePostListener() {
    event.stopPropagation();
    ID = $(this).parent().parent().attr('id');
    RemovePost(ID);
}

function EditPostListener() {
    event.stopPropagation();
    var PostID = $(this).parent().parent().attr('id');
    var Text = $('#edit-post-input').val();
    var data = { PostID, Text };
    EditPost(data);
}
$('#threadmodal').on('hidden.bs.modal', function () {
    $('#threadposts').html("");
    start_post = 0;
    amount_post = 3;
});

