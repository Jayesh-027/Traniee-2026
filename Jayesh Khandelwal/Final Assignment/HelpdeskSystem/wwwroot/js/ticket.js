
function addComment(ticketId) {

    const comment = $("#commentText").val().trim();

    if (comment === "") {
        $("#commentMessage").text("Comment cannot be empty.");
        return;
    }

    const token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: "/api/tickets/" + ticketId + "/comments",
        type: "POST",

        data: {
            comment: comment
        },

        headers: {
            "RequestVerificationToken": token
        },

        success: function (response) {
            $("#commentText").val("");
            $("#commentMessage").text(response.message);
            location.reload();
        },

        error: function (xhr) {

            let message = "Unable to add comment.";

            if (xhr.responseJSON &&
                xhr.responseJSON.message) {

                message = xhr.responseJSON.message;
            }

            $("#commentMessage").text(message);
        }
    });
}

function getAllTickets(page = 1) {

    const data = $("#ticketFilter").serialize() + "&page=" + page;

    $.ajax({
        url: "/Ticket/Search",
        type: "GET",
        data: data,

        success: function (result) {
            $("#ticket-list").html(result);
        },

        error: function () {
            console.log("Unable to load tickets");
        }
    });
}


$(document).ready(function () {

    let searchTimer;
    $("#ticketFilter").on("change", "select, input[type='checkbox']", function () {
        getAllTickets(1);
    });

    $('input[name="search"]').on("input", function () {
        clearTimeout(searchTimer);
        const search = $(this).val();
        if (search.length === 0 || search.length >= 3) {

            searchTimer = setTimeout(function () {
                getAllTickets(1);
            }, 300);
        }
    });

    $(document).on("click", ".pagination a", function (e) {
        e.preventDefault();
        const page = new URLSearchParams(this.search).get("page");
        getAllTickets(page || 1);
    });

});