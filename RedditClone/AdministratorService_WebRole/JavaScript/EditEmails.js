$(document).ready(function () {
    // Dodavanje
    $('#btn-add').click(function () {
        event.preventDefault();
        var mail = {
            "EmailAddress": $('#email_addr').val(),
        };

        $.ajax({
            url: '/emails/add',
            method: 'POST',
            data: mail,
            success: function () {
                alert(data.responseJSON.Message);
            },
            error: function (xhr, status, error) {
                console.log(error);
            }
        });
    });

    // Brisanje
    $('#btn-delete').click(function () {
        event.preventDefault();
        var mail = {
            "EmailAddress": $('#email_addr').val(),
        };

        $.ajax({
            url: '/emails/delete',
            method: 'POST',
            data: mail,
            success: function () {
                alert(data.responseJSON.Message);
            },
            error: function (xhr, status, error) {
                console.log(error);
            }
        });
    });
});
