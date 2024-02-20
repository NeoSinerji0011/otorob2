$(document).ready(function () {
    $("#btn_login").click(() => {
        $("#alert_success").hide();
        $("#alert_danger").hide();

        var username = $("#username-field").val();
        var password = $("#password-field").val();
        $.ajax({
            type: "GET",
            url: "CheckLogin/?username=" + username + "&password=" + password,
            success: (data) => {
                if (data) {
                    $("#alert_success").fadeIn();
                    setTimeout(() => {
                        window.location.href = "/Home/Index";
                    }, 2000)
                } else {
                    $("#alert_danger").fadeIn();
                }
            },
            error: (xhr, status) => { },
            complete: (xhr, status) => {
            }
        });
    })
});

(function ($) {

    "use strict";

    $(".toggle-password").click(function () {

        $(this).toggleClass("fa-eye fa-eye-slash");
        var input = $($(this).attr("toggle"));
        if (input.attr("type") == "password") {
            input.attr("type", "text");
        } else {
            input.attr("type", "password");
        }
    });

})(jQuery);