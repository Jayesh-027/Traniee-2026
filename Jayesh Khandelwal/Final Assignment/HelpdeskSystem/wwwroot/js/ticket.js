
function addComment(ticketId) {

    const comment = $("#commentText").val().trim();

    if (comment === "") {
        $("#commentMessage").text("Comment cannot be empty.");
        return;
    }

    const token =
        $('input[name="__RequestVerificationToken"]').val();

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
