$(document).ready(function () {
    $('#btn-Login').click(function () {
        event.preventDefault();
        var user = {
            "Username": $('#username').val(),
            "Password": $('#password').val()
        };

        $.ajax({
            type: "POST",
            url: '/login/signin',
            data: user,
            success: function (data) {
                sessionStorage.setItem("user_username", user.Username);
                window.location.href = 'Dashboard.html';
            },
            error: function (data) {
                alert(data.responseJSON.Message);
            }
        });
    });
});
