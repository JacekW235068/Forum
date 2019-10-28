function DeleteSubButtonListener() {
    event.stopPropagation();
    ID = $(this).parent().parent().attr('id');
    var Bearer = 'bearer ' + getCookie('accessToken');
    $.ajax({
        url: "/api/Forum/Delete/" + ID,
        method: 'DELETE',
        headers: {
            'Authorization': Bearer
        },
        success: function (response) {
            $grid.masonry('remove', $('#' + ID));
            $('.grid').masonry();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(thrownError);
        }
    });
};
