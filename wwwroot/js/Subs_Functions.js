function DeleteSubButtonListener() {
    event.stopPropagation();

    DeleteSub(
        $(this).parent().parent().attr('id')
    );
};
