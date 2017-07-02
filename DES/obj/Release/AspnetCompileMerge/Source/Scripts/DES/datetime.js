function retrieve_all_datetime_site() {
    start_action("Please wait for...", "Programming parameters from site...");
    retrieve_datetime_site();
}

function retrieve_datetime_site() {

    if (!$("#progress-modal").hasClass("action")) return;;

    $("#progress-contents").text("Retrieving datetime from site...");

    $.ajax({
        url: urlRetrieveDateTime,
        type: 'POST',
        contentType: 'application/json',
        async: true,
        success: function (result) {

            if (result) {
                if (result.Status) {

                    var row = $('#table_datetime').find('tr').first();
                    row.find('td').first().text("RemoteSite Time: " + result.Result);

                    stop_action();
                }
                else error_action(result.Result);
            }
        },
        error: function (result) { error_action(result.statusText); },
    });
};

function program_all_datetime_site() {
    start_action("Please wait for...", "Programming parameters from site...");
    program_datetime_site();
}

function program_datetime_site() {

    if (!$("#progress-modal").hasClass("action")) return;;

    $("#progress-contents").text("Programming GMT datetime to site...");

    $.ajax({
        url: urlProgramDateTime,
        type: 'POST',
        contentType: 'application/json',
        async: true,
        success: function (result) {
            if (result.Status) {

                var row = $('#table_datetime').find('tr').first();
                row = row.next().next().next();
                row.find('td').first().text("GMT Time: : " + result.Result);
                stop_action();
            }
            else error_action(result.Result);
        },
        error: function (result) {
            error_action(result.status);
        },
    });
};

function getDateTime() {

    var systemOption = {};
    var row = $('#table_datetime').find('tr').first();
    var remoteSiteDatetime = row.find('td').first().text();

    return remoteSiteDatetime;
};

$("#table-settings select[name='MonthReset']").on("change", function () {
    var daySet = $("#table-settings select[name='DayReset']")
    var days = new Date($("#table-settings").attr('data-year'), $(this).val(), 0).getDate();
    var html = "";
    for (var i = 1; i <= days; i++) {
        html += "<option value=" + i + ">" + i + "</option>"
    }
    daySet.html(html);
})
