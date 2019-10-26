//////VARIABLES//////
var $grid = $('.grid').masonry({

    itemSelector: '.grid-item'
});
var selectedSub = "";
//////LISTENERS//////

$(document).ready(function () {
    $('#Grid').children().each(function () {
        $(this).click(function () {
            window.location.href = "/Home/Threads?subID=" + $(this).attr('id');
        })
    })
    roles = getCookie("roles");
    if (roles != null) {
        if (roles.includes("Admin")) {
            $('.edit-button').removeAttr('hidden');
            $('.remove-button').removeAttr('hidden');
            $('.add-button').removeAttr('hidden');
            $('.remove-button').click(DeleteSubButtonListener);
            $('.edit-button').click(function () {
                event.stopPropagation();
                $("#editsubmodal").modal('toggle');
                selectedSub = $(this).parent().parent().attr('id');
                $('#editsubname').val(
                    $(this).parent().parent().find('.item-title').text()
                );
            });
            $('.add-button').click(function () { $("#addsubmodal").modal('toggle'); });
        }
    }
})

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

$('#btneditsub').click(function () {
    
    if (!$("#editsubforumform").valid()) return;
    var Forum = $("#editsubforumform").serialize();
    Forum += "&subforumid=";
    Forum += selectedSub;

    var Bearer = 'bearer ' + getCookie('accessToken');
    $.ajax({
        type: "POST",
        url: "/api/Forum/Edit",
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