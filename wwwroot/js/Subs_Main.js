var $grid = $('.grid').masonry({

    itemSelector: '.grid-item'
});
$(document).ready(function () {
    $('#Grid').children().each(function () {
        $(this).click(function () {
            window.location.href = "/Home/Threads?subID=" + $(this).attr('id');
        })
    })
    roles = getCookie("roles");
    if (roles != null) {
        if (roles.includes("Admin")) {
            addButton = document.createElement('button');
            addButton.setAttribute("id", "addbutton");
            addButton.innerHTML = "Add New Sub";
            addButton.classList.add("btn");
            addButton.classList.add("btn-primary");
            $addButton = $(addButton);
            $addButton.click(function () { $("#addsubmodal").modal('toggle'); });
            $(document.body).append($addButton);
            $('#Grid').children().each(GenerateDeleteButton);
        }
    }
})


function GenerateDeleteButton() {
    delButton = document.createElement('button');
    delButton.innerHTML = "Delete";
    delButton.classList.add("btn");
    delButton.classList.add("btn-primary");
    delButton.classList.add("btn-delete");
    $delButton = $(delButton);
    $delButton.click(DeleteButtonListener);
    $(this).append($delButton);
}
$('#btnnewsub').click(function () {
    if (!$("#subforumform").valid()) return;
    var Forum = $("#subforumform").serialize();
    var Bearer = 'bearer ' + getCookie('accessToken');
    $.ajax({
        type: "POST",
        url: "/api/Forum/New",
        headers: {
            'Authorization': Bearer
        },
        data: Forum,
        success: function (response, textStatus, xhr) {
            location.reload();
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
})

