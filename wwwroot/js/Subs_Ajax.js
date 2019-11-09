function DeleteSub(ID) {
    var Bearer = 'bearer ' + getCookie('accessToken');
    $.ajax({
        url: "/api/Forum/Delete/" + ID,
        method: 'DELETE',
        headers: {
            'Authorization': Bearer
        },
        success: function () {
            $grid.masonry('remove', $('#' + ID));
            $('.grid').masonry();
        },
        error: function (response, ajaxOptions, thrownError) {
            if (response.status == 401 && response.getAllResponseHeaders().includes("The token is expired")) {
                RefreshToken(DeleteSub, ID);
            } else {
                alert("something went wrong");
            }
        }
    });
}

function NewSub(Forum) {
    var Bearer = 'bearer ' + getCookie('accessToken');
    $.ajax({
        type: "POST",
        url: "/api/Forum/New",
        headers: {
            'Authorization': Bearer
        },
        data: Forum,
        success: function () {
            location.reload();
        },
        error: function (response, ajaxOptions, thrownError) {
            if (response.status == 401 && response.getAllResponseHeaders().includes("The token is expired")) {
                RefreshToken(NewSub, Forum);
                return; 
            }
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
}

 function EditSub(Forum) {
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
            if (response.status == 401 && response.getAllResponseHeaders().includes("The token is expired")) {
                RefreshToken(EditSub, Forum);
                return;
            }
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
}