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
})

var $grid = $('.grid').masonry({

    itemSelector: '.grid-item'
});
var start = 0;
var amount = 3;

$("#loadmore").click(function () {

    var id = $('.grid').children().attr('id');

    $.ajax({
        url: "/api/Thread/Threads",
        type: 'get',
        data: { id, start, amount },
        success: function (response) {
            response = response.data;
            if (response.threads.length == amount)
                start += amount;
            else
                $("#LoadMore").attr("disabled", true);
            roles = getCookie("roles");
            response.threads.forEach(function (thread) {
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