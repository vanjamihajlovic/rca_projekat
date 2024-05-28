let allEmails = [];

$(document).ready(function () {
    // Dugme za dodavanje
    $('#btn-add').click(AddEmail);
    //$('#btn-delete').click(DeleteEmail);

    // Slanje GET zahteva serveru - za sve adrese
    $.ajax({
        url: "/emails/all", 
        type: "GET",
        dataType: "json",
        contentType: "application/json",

        success: function (response) {
            allEmails = response;
            PopuniPrikazMejlova(allEmails);
        },
        error: function (xhr, status, error) {
            let result = JSON.parse(xhr.responseText);
            ApiPoruka(result.Message, error);
        }
    });
});






// Popunjavanje tabelarnog prikaza za mejlove
function PopuniPrikazMejlova(items) {

    let table = $(".users tbody");
    table.empty();

    if (items.length == 0) {
        let tr = $("<tr></tr>");
        let message = $('<td colspan=6></td>').text("No emails found!");
        tr.append(message);
        table.append(tr);
    }
    else {
        $.each(items, function (index, item) {
            let tr = $("<tr></tr>");
            //tr.attr("id", "user-" + item.EmailAddress);
            
            let email = $("<td></td>").text(item.EmailAddress);
            
            let action = $('<td></td>');
            let deleteBtn = $('<button>Obriši</button>');
            deleteBtn.addClass("red-btn");
            deleteBtn.click({ email: item.EmailAddress }, DeleteEmail);

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
            alert(data.responseJSON.Message);
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}

// Button events - delete
function DeleteEmail(event) {
    event.preventDefault();

    $.ajax({
        url: '/emails/delete',
        method: 'POST',
        data: JSON.stringify(event.data),
        contentType: "application/json",
        //headers: { "Authorization": token },

        success: function () {
            alert(data.responseJSON.Message);
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}
