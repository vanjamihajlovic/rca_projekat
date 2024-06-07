let allReports = [];
let dailyAverage = 0;

$(document).ready(function () {
    $.ajax({
        url: "/dashboard/getDay",
        type: "GET",
        contentType: "application/json",

        success: function (response) {
            dailyAverage = response;
            console.log(dailyAverage);
            document.getElementById("daily-avg").innerHTML = dailyAverage;
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

// Popunjavanje tabelarnog prikaza za izvestaje
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

            // Sadrzaj
            let report = $("<td></td>").text(item.Sadrzaj);
            if (item.Sadrzaj.includes("NOT")) {
                $(tr).css('color', 'red');
            }
            else {
                $(tr).css('color', 'green');
            }

            // Timestamp
            let ts = $("<td></td>").text(item.Vreme);

            // Dodaj podatke u tabelu
            tr.append(report, ts);
            table.append(tr);
        });
    }
}
