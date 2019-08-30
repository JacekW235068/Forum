$(document).ready(function () {
   // alert("xD");

})
var $grid = $('.grid').masonry({

    itemSelector: '.grid-item'
  });
var start = 0;
var amount = 3;

$("#LoadMore").click(function () {
    
    var Data = { start, amount };

    $.ajax({
        url: "/api/Thread/Recent",
        type: 'get',
        data: {start, amount},
        success: function (response) {
            if (response.threads.length == amount)
                start += amount;
            else
                $("#LoadMore").attr("disabled", true);
            response.threads.forEach(function (thread) {
                var $div = createNewThread(thread);
                $grid.append($div).masonry('appended', $div);

            })
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $("#LoadMore").attr("disabled", true);
            $("#LoadMore").html("Sth went wrong");
        }
    });
});

function createNewThread(thread) {
    var newDiv = document.createElement("div");
    newDiv.classList.add("grid-item");
    var divTitle = document.createElement("h3");
    divTitle.innerHTML = thread.title;
    var divContent = document.createElement("p");
    divContent.innerHTML = thread.text;
    newDiv.appendChild(divTitle);
    newDiv.appendChild(divContent);
    var $NewDiv = $(newDiv);
    return $NewDiv;
}
