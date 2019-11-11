function NewThread(Thread){
    var Bearer = 'bearer ' + getCookie('accessToken');
    $.ajax({
        type: "POST",
        url: "/api/Thread/New",
        headers: {
            'Authorization': Bearer
        },
        data: Thread,
        success: function (response, textStatus, xhr) {
            response = response.data;
            $div = createThreadView(response, getCookie('roles'), getCookie('username'));
            $grid.prepend($div).masonry('prepended', $div).masonry();
        },
        error: function (response, ajaxOptions, thrownError) {
            if (response.status == 401 && response.getAllResponseHeaders().includes("The token is expired")) {
                RefreshToken(NewThread, Thread);
                return;
            }
            response = response.responseJSON.value;
            if (response.status = "Fail") {
                alert(response.data)
            }
            else if (response.status = "Error") {
                alert(response.message)
            }
        }
    });
}

function LoadThreads(subForumID) {
    $.ajax({
        url: "/api/Thread/Threads",
        type: 'get',
        data: { subForumID, start: start_thread, amount: amount_thread },
        success: function (response) {
            if (response === undefined) {
                start_thread = -1;
                return;
            }
            else {
                response = response.data;
                generateThreads(response);
                if (response.length == amount_thread)
                    start_thread += amount_thread;
                else
                    start_thread = -1;
                LoadMoreThreads();
            }
            
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("sth went wrong");
        }
    });
}

function RemoveThread(ID) {
    var Bearer = 'bearer ' + getCookie('accessToken');
    $.ajax({
        url: "/api/Thread/Delete/" + ID,
        method: 'DELETE',
        headers: {
            'Authorization': Bearer
        },
        success: function (response) {
            $grid.masonry('remove', $('#' + ID));
            $grid.masonry();
        },
        error: function (response, ajaxOptions, thrownError) {
            if (response.status == 401 && response.getAllResponseHeaders().includes("The token is expired")) {
                RefreshToken(RemoveThread, ID);
                return;
            }
            if (response.status = 403) {
                alert("ur not supposed to be here");
                return;
            }
            alert("an error has occured");
        }
    });
}

function EditThread(Thread) {
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
            if (response.status == 401 && response.getAllResponseHeaders().includes("The token is expired")) {
                RefreshToken(EditThread, Thread);
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

function MoveThread(data) {
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
            $grid.masonry('remove', $('#' + selectedThread)).masonry();
        },
        error: function (response, ajaxOptions, thrownError) {
            if (response.status == 401 && response.getAllResponseHeaders().includes("The token is expired")) {
                RefreshToken(MoveThread, data);
                return;
            }
            response = response.responseJSON.value;
            alert(response.data);
        }
    });
}

function FetchSubsForMovingThreads(OriginID) {
    $.ajax({
        url: "/api/Forum/AllForums",
        type: 'get',
        success: function (response) {
            response = response.data;
            $('#sublist').html("");
            response.forEach(function (sub) {
                if (OriginID == sub.id) {
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
            alert("something went wrong");
        }
    });
}