let allEmails = [];

$(document).ready(function () {
    // Dugme za dodavanje
    $('#btn-add').click(AddEmail);

    // Slanje GET zahteva serveru - za sve adrese
    $.ajax({
        url: "/emails/all", 
        type: "GET",
        contentType: "application/json",

        success: function (response) {
            allEmails = response;
            PopuniPrikazMejlova(allEmails);
        },
        error: function (xhr, status, error) {
            let result = JSON.parse(xhr.responseText);
            alert(result);
        }
    });
});

// Popunjavanje tabelarnog prikaza za mejlove
function PopuniPrikazMejlova(items) {
    let table = $(".users tbody");
    table.empty();
    console.log(items);

    if (items.length == 0) {
        let tr = $("<tr></tr>");
        let message = $('<td colspan=6></td>').text("No emails found!");
        tr.append(message);
        table.append(tr);
    }
    else {
        $.each(items, function (index, item) {
            let tr = $("<tr></tr>");
            tr.attr("id", "user-" + item.RowKey);
            
            let email = $("<td></td>").text(item.EmailAdresa);

            let action = $('<td></td>');
            let deleteBtn = $('<button>Delete</button>');
            deleteBtn.addClass("red-btn");
            deleteBtn.click({ email: item.EmailAdresa }, DeleteEmail);

            action.append(deleteBtn);
            
            // Dodaj podatke u tabelu
            tr.append(email, action);
            table.append(tr);
        });
    }
}

// Button events - add
function AddEmail(event) {
    event.preventDefault();
    var mail = {
        "EmailAddress": $('#email_addr').val(),
    };

    $.ajax({
        url: '/emails/add',
        method: 'POST',
        data: mail,
        //headers: { "Authorization": token },

        success: function () {
            alert("New email address sucessfully added!");
            location.reload();
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}

// Button events - delete
function DeleteEmail(event) {
    event.preventDefault();

    let email = event.data.email;
    let emailDelete = {
        EmailAddress: email
    }

    $.ajax({
        url: '/emails/delete',
        method: 'POST',
        data: JSON.stringify(emailDelete),
        contentType: "application/json",
        //headers: { "Authorization": token },

        success: function () {
            alert("Email address has been removed.");
            location.reload();
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}
