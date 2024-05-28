$(document).ready(function () {
    var username = sessionStorage.getItem("user_username");

    // Ovo mislim da će možda praviti problem
    if (username == null) {
        window.location.href = "Index.html";
    }

    $('#nav-logout').click(function () {
        $.ajax({
            url: '/login/signout',
            method: 'POST',
            success: function () {
                sessionStorage.setItem("user_username", -1);
                window.location.href = "Index.html";
            },
            error: function (xhr, status, error) {
                console.log(error);
            }
        });
    });
});
