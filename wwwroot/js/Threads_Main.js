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
            $('#btneditthread').click(EditThreadListener);
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
            response = response.data;
            $div = createThreadView(response, getCookie('roles'), getCookie('username'));
            $grid.prepend($div).masonry('prepended', $div).masonry();
        },
        error: function (response, ajaxOptions, thrownError) {
       
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
            if (response === 'undefined') {
                //stop requesting
                return;
            }
            response = response.data;
            if (response.length == amount)
                start += amount;
            else { //stop requesting
            }
            generateThreads(response);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //TODO
        }
    });
});
