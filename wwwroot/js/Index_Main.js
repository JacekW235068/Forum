//////VARIABLES//////
var $grid = $('.grid').masonry({

    itemSelector: '.grid-item'
});

var start = 0;
var amount = 3;
//////LISTENERS//////
$("#LoadMore").click(function () {
    var Data = { start, amount }
    $.ajax({
        url: "/api/Thread/Recent",
        type: 'get',
        data: {start, amount},
        success: function (response) {
            response = response.data;
            if (response.threads.length == amount)
                start += amount;
            else
                $("#LoadMore").attr("disabled", true);
            response.threads.forEach(function (thread) {
                var $div = createNewThreadView(thread);
                $div.click(function (event) {
                    ViewThread($(this).attr('id'))
                })
                $grid.append($div).masonry('appended', $div);

            })
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $("#LoadMore").attr("disabled", true);
            $("#LoadMore").html("Sth went wrong");
        }
    });
});
//////FUNCTIONS//////
