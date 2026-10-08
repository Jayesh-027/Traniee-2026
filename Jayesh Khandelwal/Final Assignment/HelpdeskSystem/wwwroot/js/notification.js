function loadNotifications() {

    $.ajax({
        url: "/api/notifications",
        type: "GET",

        success: function (notifications) {
            $("#notificationCount").text(notifications.length);
            showNotifications(notifications);
        },
        error: function () {
            $("#notificationCount").text("0");
        }
    });
}


function showNotifications(notifications) {

    var notificationList = $("#notificationList");

    notificationList.empty();

    if (notifications.length === 0) {

        notificationList.html(`
           <div class="no-notifications">No new notifications</div>
           <a href="/Notification/Index" class="view-all-notifications">
                View all notifications
            </a>
        `);

        return;
    }

    notifications.forEach(function (notification) {

        var item = `
            <div class="notification-item"
                 data-id="${notification.id}">

                ${notification.message}

                <span class="notification-time">
                    ${notification.createdAt}
                </span>

            </div>
        `;

        notificationList.append(item);
    });
    notificationList.append(`
        <a href="/Notification/Index" class="view-all-notifications">
            View all notifications
        </a>
    `);
}


function markNotificationAsRead(notificationId) {

    $.ajax({
        url: "/api/notifications/" + notificationId + "/read",
        type: "POST",

        success: function () {
            
            window.location.href = "/Notification/Index";
        }
    });
}


$(document).ready(function () {

    loadNotifications();

    $("#notificationBell").click(function (event) {

        event.preventDefault();

        $("#notificationPanel").toggle();
    });


    $(document).on("click", ".notification-item", function () {

        var notificationId = $(this).data("id");

        markNotificationAsRead(notificationId);
    });


    setInterval(function () {

        loadNotifications();

    }, 10000);
});