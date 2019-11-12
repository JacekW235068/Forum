//////VARIABLES//////
var $grid = $('.grid').masonry({

    itemSelector: '.grid-item'
});
//TODO: like everything
var start_thread = 0;
var amount_thread = 3;
//////LISTENERS//////
$(document).ready(function () {
    $(window).scroll(function () {
        LoadMoreThreads();
    });
    LoadMoreThreads();
})


function RemoveThreadListener() {
    event.stopPropagation();
    ID = $(this).parent().parent().attr('id');
    RemoveThread(ID);
}
