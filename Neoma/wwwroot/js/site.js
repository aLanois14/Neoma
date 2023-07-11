$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();

    getMessage();
    getNotification();
    function getMessage() {
        $.ajax({
            url: $("#notification-message").data('request-url'),
            method: "GET",
            success: function (result) {
                $(".notification-drop.messages").html(result.count);
                $(".notification-drop.notification-drop-big").html(result.count + "+");
            },
            error: function (error) {

            }
        });
    }

    function getNotification() {
        $.ajax({
            url: $("#notification").data('request-url'),
            method: "GET",
            success: function (result) {
                if (result.role === "Fondateur") {
                    $(".notification-drop.selection").html(result.count2);
                    $(".notification-drop.candidatureR").html(result.count1);
                }
                else {
                    $(".notification-drop.candidature").html(result.count2);
                    $(".notification-drop.candidatureT").html(result.count1);
                }
            },
            error: function (error) {

            }
        });
    }
});