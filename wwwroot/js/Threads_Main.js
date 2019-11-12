//////VARIABLES//////
var $grid = $('.grid').masonry({

    itemSelector: '.grid-item'
});
var start_thread = 0;
var amount_thread = 3;
var SubID = "";
var selectedThread = "";
//////LISTENERS//////
$(document).ready(function () {
    SubID =  window.location.href.split("?subID=")[1];
    var roles = getCookie("roles");
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
    $(window).scroll(function () {
        LoadMoreThreads();
    });
    LoadMoreThreads();

});

$('#btnnewthread').click(function () {
    if (!$("#threadform").valid()) return;
    var Thread = $("#threadform").serialize();
    Thread += "&subforumid=";
    Thread += SubID;
    NewThread(Thread);
});





function RemoveThreadListener() {
    event.stopPropagation();
    ID = $(this).parent().parent().attr('id');
    RemoveThread(ID);
}

function EditThreadListener() {
    if (!$("#editthreadform").valid()) return;
    var Thread = $("#editthreadform").serialize();
    Thread += "&threadid=";
    Thread += selectedThread;//???
    EditThread(Thread);
}

function MoveThreadListener() {
    var data = { threadID: selectedThread, subForumID: $(this).attr('id') };
    MoveThread(data);
}

function MoveButtonListener() {
    event.stopPropagation();
    $("#sublistmodal").modal('toggle');
    selectedThread = $(this).parent().parent().attr('id');
    FetchSubsForMovingThreads(SubID);
}

