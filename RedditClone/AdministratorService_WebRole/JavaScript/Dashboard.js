// Samo feč radi
let allReports = [];
let dailyAverage = 0;

$(document).ready(function () {
    // TODO dnevni prosek
    $.ajax({
        url: "/dashboard/getDay",
        type: "GET",
        contentType: "application/json",

        success: function (response) {
            dailyAverage = response;
            console.log(dailyAverage);
            //PopuniPrikazPrikaz(allReports);
        },
        error: function (xhr, status, error) {
            let result = JSON.parse(xhr.responseText);
            alert(result);
        }
    });

    // Slanje GET zahteva serveru - za sve adrese
    $.ajax({
        url: "/dashboard/getHour",
        type: "GET",
        contentType: "application/json",

        success: function (response) {
            allReports = response;
            PopuniPrikazIzvestaja(allReports);
        },
        error: function (xhr, status, error) {
            let result = JSON.parse(xhr.responseText);
            alert(result);
        }
    });
});


// Popunjavanje tabelarnog prikaza za mejlove
function PopuniPrikazIzvestaja(items) {
    let table = $(".users tbody");
    table.empty();
    console.log(items);

    if (items.length == 0) {
        let tr = $("<tr></tr>");
        let message = $('<td colspan=6></td>').text("No reports found!");
        tr.append(message);
        table.append(tr);
    }
    else {
        $.each(items, function (index, item) {
            let tr = $("<tr></tr>");
            tr.attr("id", "report-" + item.RowKey);

            let report = $("<td></td>").text(item.Sadrzaj);
            // substring Not-ok -> zacrveni red
            
            // Dodaj podatke u tabelu
            tr.append(report);
            table.append(tr);
        });
    }
}

function PopuniPrikazProsek(items) {
    let table = $(".users tbody");
    table.empty();
    console.log(items);

    if (items.length == 0) {
        let tr = $("<tr></tr>");
        let message = $('<td colspan=6></td>').text("No reports found!");
        tr.append(message);
        table.append(tr);
    }
    else {
        $.each(items, function (index, item) {
            let tr = $("<tr></tr>");
            tr.attr("id", "report-" + item.RowKey);

            let report = $("<td></td>").text(item.Sadrzaj);
            // substring Not-ok -> zacrveni red

            // Dodaj podatke u tabelu
            tr.append(report);
            table.append(tr);
        });
    }
}
