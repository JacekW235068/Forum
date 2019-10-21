//////VARIABLES//////
var $grid = $('.grid').masonry({

    itemSelector: '.grid-item'
});
var start = 0;
var amount = 3;
//////LISTENERS//////
$(document).ready(function () {
    roles = getCookie("roles");
    if (roles != null) {
        if (roles.includes("Admin") || roles.includes("NormalUser")) {
            addButton = document.createElement('button');
            addButton.setAttribute("id", "addbutton");
            addButton.innerHTML = "Add New Thread";
            addButton.classList.add("btn");
            addButton.classList.add("btn-primary");
            $addButton = $(addButton);
            $addButton.click(function () { $("#addthreadmodal").modal('toggle'); });
            $(document.body).append($addButton);
            $('#Grid').children().each(GenerateDeleteButton);
        }
    }
});

$('#btnnewthread').click(function () {
    if (!$("#threadform").valid()) return;
    var Thread = $("#threadform").serialize();
    Thread += "&subforumid=";
    Thread += window.location.href.split("?subID=")[1];//???

    var Bearer = 'bearer ' + getCookie('accessToken');
    $.ajax({
        type: "POST",
        url: "/api/Thread/New",
        headers: {
            'Authorization': Bearer
        },
        data: Thread,
        success: function (response, textStatus, xhr) {
            response = response.value;
            $div = createNewThreadView(response.data);
            $div.click(function (event) {
                ViewThread($(this).attr('id'))
            })
            $grid.append($div).masonry('appended', $div);
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

$("#loadmore").click(function () {

    var subForumID = window.location.href.split("?subID=")[1];

    $.ajax({
        url: "/api/Thread/Threads",
        type: 'get',
        data: { subForumID, start, amount },
        success: function (response) {
            response = response.data;
            if (response.length == amount)
                start += amount;
            else
                $("#LoadMore").attr("disabled", true);
            roles = getCookie("roles");
            response.forEach(function (thread) {
                var $div = createNewThreadView(thread);
                if (roles != null)
                    if (roles.includes("Admin") || (roles.includes("NormalUser") && getCookie("username") == thread.Username))
                        GenerateDeleteButton($div);
                $div.click(function (event) {
                    ViewThread($(this).attr('id'))
                })
                $grid.append($div).masonry('appended', $div);

            })
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $("#loadmore").attr("disabled", true);
            $("#loadmore").html("Sth went wrong");
        }
    });
});
//////FUNCTIONS//////
function GenerateDeleteButton(element) {
    delButton = document.createElement('button');
    delButton.innerHTML = "Delete";
    delButton.classList.add("btn");
    delButton.classList.add("btn-primary");
    delButton.classList.add("btn-delete");
    $delButton = $(delButton);
    $delButton.click(DeleteButtonListener);
    element.append($delButton);
}

function DeleteButtonListener() {
    event.stopPropagation();
    ID = $(this).parent().attr('id');
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