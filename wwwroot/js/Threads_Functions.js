var selectedThread = "";
function generateThreads(threads) {
    var username = getCookie('username');
    var roles = getCookie('roles');
    threads.forEach(function (thread) {
        var $div = createThreadView(thread, roles, username);
        $grid.masonry()
            .append($div)
            .masonry('appended', $div)
            .masonry();
    });
    
}

function createThreadView(thread, roles, username) {
    var threadID = thread.id;
    var userName = thread.userName;
    var postTime = thread.postTime;
    var lastPostTime = thread.lastPostTime;
    var title = thread.title;
    var text = thread.text;
    var comments = thread.comments;
    var $newThreadDiv = $($.parseHTML(`<div id="${threadID}" class="grid-item postdiv">
            <div class="item-header">
                <div class="user">
                    ${userName}
                </div>
                <div class="time">
                    created: ${postTime}
                    last post: ${lastPostTime}
                </div>
                </div>
                <div class="item-body">
                    <p class="item-title">${title}</p><br/>
                    <div class="item-text">${text}</div>
                </div>
                <div class="item-footer">
                    <button class="btn btn-danger remove-thread-button" hidden>Delete</button>
                    <button class="btn btn-warning edit-thread-button" hidden>Edit</button>
                    <button class="btn btn-primary move-thread-button" hidden>Move</button>
                    ${comments}
                </div>
            </div>
        `));
    $newThreadDiv.click(function () {
        ViewThread($(this).attr('id'));
    });
    var $remove = $newThreadDiv.find('.remove-thread-button');
    var $edit = $newThreadDiv.find('.edit-thread-button');
    var $move = $newThreadDiv.find('.move-thread-button');
    if (roles != null && (roles.includes('Admin') || userName.includes(userName))) {
        $remove.removeAttr('hidden');
        $remove.click(RemoveThreadListener);
    }
    if (userName != null && username.includes(userName)) {
        $edit.removeAttr('hidden');
        $edit.click(function () {
            event.stopPropagation();
            $("#editthreadmodal").modal('toggle');
            selectedThread = $(this).parent().parent().attr('id');
            $('#edittitle').val(
                $(this).parent().parent().find('.item-title').text()
            );
            $('#edittext').val(
                $(this).parent().parent().find('.item-text').text()
            );
        });
    }
    if (roles != null && roles.includes('Admin')) {
        $move.removeAttr('hidden');
        $move.click(MoveButtonListener);
    }
    return $newThreadDiv;
}


function RemoveThreadListener() {
    event.stopPropagation();
    ID = $(this).parent().parent().attr('id');
    var Bearer = 'bearer ' + getCookie('accessToken');
    $.ajax({
        url: "/api/Thread/Delete/" + ID,
        method: 'DELETE',
        headers: {
            'Authorization': Bearer
        },
        success: function (response) {
            $grid.masonry('remove', $('#' + ID));
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log(thrownError);
        }
    });
}

function EditThreadListener(){
    if (!$("#editthreadform").valid()) return;
    var Thread = $("#editthreadform").serialize();
    Thread += "&threadid=";
    Thread += selectedThread;//???

    var Bearer = 'bearer ' + getCookie('accessToken');
    $.ajax({
        type: "POST",
        url: "/api/Thread/Edit",
        headers: {
            'Authorization': Bearer
        },
        data: Thread,
        success: function (response, textStatus, xhr) {
            response = response.data;
            $('#' + response.id).find(".item-title").text(response.title);
            $('#' + response.id).find(".item-text").text(response.text);
            $grid.masonry();
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

}

function MoveThreadListener() {
    var data = { threadID: selectedThread, subForumID: $(this).attr('id') };
    var Bearer = 'bearer ' + getCookie('accessToken');
    $.ajax({
        type: "POST",
        url: "/api/Thread/Move",
        contentType: "application/json; charset=UTF-8",
        headers: {
            'Authorization': Bearer
        },
        data: JSON.stringify(data),
        success: function (response, textStatus, xhr) {
            $('#' + selectedThread)
            $grid.masonry('remove', $('#' + selectedThread) ).masonry();
        },
        error: function (response, ajaxOptions, thrownError) {
            
        }
    });

}

function MoveButtonListener() {
    event.stopPropagation();
    $("#sublistmodal").modal('toggle');
    selectedThread = $(this).parent().parent().attr('id');
    $.ajax({
        url: "/api/Forum/AllForums",
        type: 'get',
        success: function (response) {
            response = response.data;
            subID = window.location.href.split("?subID=")[1];
            response.forEach(function (sub) {
                if (subID == sub.id) {
                    $('#sublist').append(
                        $.parseHTML(
                            `<button type="button" class="list-group-item list-group-item-action" disabled>${sub.name}</button>`
                        )
                    );
                } else {
                    var $item = $($.parseHTML(
                        `<button id="${sub.id}" type="button" class="list-group-item list-group-item-action">${sub.name}</button>`
                    ));
                    $item.click(MoveThreadListener);
                    $('#sublist').append($item);
                }
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //TODO
        }
    });
}