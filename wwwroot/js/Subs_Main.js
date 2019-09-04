var $grid = $('.grid').masonry({

    itemSelector: '.grid-item'
});
function init() {
    if (document.getElementById("roles") && document.getElementById("roles").innerHTML.includes("Admin")) {
        addButton = document.createElement('button');
        addButton.setAttribute("id", "addbutton");
        addButton.innerHTML = "Add New Sub";
        addButton.classList.add("btn");
        addButton.classList.add("btn-primary");
        $addButton = $(addButton);
        $addButton.click(function () { $("#addsubmodal").modal('toggle'); });
        $(document.body).append(addButton);
    }

}
$('#btnnewsub').click(function () {
    if (!$("#subforumform").valid()) return;
    var Forum = $("#subforumform").serialize();
    $.ajax({
        type: "POST",
        url: "/api/Forum/New",
        data: Forum,
        success: function (response) {
            window.Location.reload();
        },
        error: function (thrownError) {
            if (thrownError.responseJSON.value) {
                document.getElementById("newsuberrors").innerHTML = thrownError.responseJSON.value.error;
                if (thrownError.responseJSON.value.lockedout) {
                    //disable form
                }
            }
            if (thrownError.responseJSON.errors) {
                if (thrownError.responseJSON.errors.Name)
                    document.getElementById("newsuberrors").innerHTML = thrownError.responseJSON.errors.Name + "\n";
            }

        }
    });
})