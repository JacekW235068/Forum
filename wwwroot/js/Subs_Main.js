//////VARIABLES//////
var $grid = $('.grid').masonry({

    itemSelector: '.grid-item'
});
var selectedSub = "";
//////LISTENERS//////

$(document).ready(function () {
    $('#Grid').children().each(function () {
        $(this).click(function () {
            window.location.href = "/Threads?subID=" + $(this).attr('id');
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
        $grid.masonry();
    }
})

$('#btnnewsub').click(function () {
    if (!$("#subforumform").valid()) return;
    var Forum = $("#subforumform").serialize(); 
    NewSub(Forum);
})

$('#btneditsub').click(function () {
    
    if (!$("#editsubforumform").valid()) return;
    var Forum = $("#editsubforumform").serialize();
    Forum += "&subforumid=";
    Forum += selectedSub;
    EditSub(Forum);
})