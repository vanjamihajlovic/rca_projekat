$(document).ready(function () {
    var username = sessionStorage.getItem("user_username");

    if (username == null) {
        $.ajax({
            url: '/login/signin',
            method: 'GET',
            success: function (data) {
                sessionStorage.setItem("user_username", data.Username);
                Administrator();
            }
        });
    } else {
        Undefined();
    }

    $('#nav-logout').click(function () {
        $.ajax({
            url: '/api/login/signout',
            method: 'GET',
            success: function () {
                Undefined();
                sessionStorage.setItem("user_username", -1);
                window.location.href = "Index.html";
            },
            error: function (xhr, status, error) {
                console.log(error);
            }
        });
    });
});


function Administrator() {
    $('#nav-dashboard').show();
    $('#nav-admin-emails').show();
    $('#nav-login').hide();
    $('#nav-logout').show();
}

function Undefined() {
    $('#nav-dashboard').hide();
    $('#nav-admin-emails').hide();
    $('#nav-login').show();
    $('#nav-logout').hide();
}
