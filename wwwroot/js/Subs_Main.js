var $grid = $('.grid').masonry({

    itemSelector: '.grid-item'
});
$(document).ready(function () {
    $('#Grid').children().each(function () {
        $(this).click(function () {
            window.location.href = "/Home/Threads?subID=" + $(this).attr('id');
        })
    })
})
function init() {
    if (document.getElementById("roles")) {
        if (document.getElementById("roles").innerHTML.includes("Admin")) {
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

}

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
        success: function (response) {
            location.reload();
        },
        error: function (thrownError) {
            if (thrownError.responseJSON.value) {
                document.getElementById("newsuberrors").innerHTML = thrownError.responseJSON.value.error;
                if (thrownError.responseJSON.value.lockedout) {
                }
            }
            if (thrownError.responseJSON.errors) {
                if (thrownError.responseJSON.errors.Name)
                    document.getElementById("newsuberrors").innerHTML = thrownError.responseJSON.errors.Name + "\n";
            }

        }
    });
})

function DeleteButtonListener() {
    event.stopPropagation();
    ID = $(this).parent().attr('id') ;
    var Bearer = 'bearer ' + getCookie('accessToken');
    $.ajax({
        url: "/api/Forum/Delete/"+ ID,
        method: 'DELETE',
        headers: {
            'Authorization': Bearer
        },
        success: function (response) {
            location.reload();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log(thrownError);
            //if (thrownError.responseJSON.value) {
            //    document.getElementById("newsuberrors").innerHTML = thrownError.responseJSON.value.error;
            //    if (thrownError.responseJSON.value.lockedout) {
            //    }
            //}
            //if (thrownError.responseJSON.errors) {
            //    if (thrownError.responseJSON.errors.Name)
            //        document.getElementById("newsuberrors").innerHTML = thrownError.responseJSON.errors.Name + "\n";
            //}

        }
    });
}