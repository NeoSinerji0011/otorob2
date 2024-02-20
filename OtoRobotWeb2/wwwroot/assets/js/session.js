async function fnk_check_session() {
    $.ajax({
        type: "POST",
        url: "/Login/CheckSession",
        success: (data) => {
            if (data)
                window.location = "/";
        },
        error: (xhr, status) => { },
        complete: (xhr, status) => {
            setTimeout(function () {
                fnk_check_session();
            }, 5000);
        }
    });

}
//fnk_check_session();