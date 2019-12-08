function LoadMoreThreads() {

    if (($(document).height() - $(window).height() - $(window).scrollTop()) < 250) {
        if (start_thread > -1) {
            LoadRecentThreads();
        } else {
            $(window).unbind("scroll")
        }
    }
}


function generateThreads(threads) {
    var username = getCookie('username');
    var roles = getCookie('roles');
    threads.forEach(function (thread) {
        var $div = createThreadView(thread, roles, username);
        $grid.masonry()
            .append($div)
            .masonry('appended', $div)
            .masonry();
    });

}

function createThreadView(thread, roles, username) {
    var threadID = thread.id;
    var userName = thread.userName;
    var postTime = thread.postTime;
    var lastPostTime = thread.lastPostTime;
    var title = thread.title;
    var text = thread.text;
    var comments = thread.comments;
    var parentID = thread.parentForumID;
    var $newThreadDiv = $($.parseHTML(`<div id="${threadID}" class="grid-item postdiv">
            <div class="item-header">
                <div class="user">
                    ${userName}
                </div>
                <div class="time">
                    created: ${postTime} <br/>
                    last post: ${lastPostTime}
                </div>
            </div>
                <a class="btn btn-primary" href="/Threads?subID=${parentID}" role="button">Link</a>
                <div class="item-body">
                    <p class="item-title">${title}</p><br/>
                    <div class="item-text">${text}</div>
                </div>
                <div class="item-footer">
                    <button class="btn btn-danger remove-thread-button" hidden>Delete</button>
                    ${comments}
                </div>
            </div>
        `));
    $newThreadDiv.click(function () {
        ViewThread($(this).attr('id'));
    });
    var $remove = $newThreadDiv.find('.remove-thread-button');
    if (roles != null && (roles.includes('Admin') || userName == username)) {
        $remove.removeAttr('hidden');
        $remove.click(RemoveThreadListener);
    }
    return $newThreadDiv;
}

