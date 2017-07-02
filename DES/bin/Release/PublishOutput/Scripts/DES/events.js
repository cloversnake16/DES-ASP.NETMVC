function program_all_events_enable_site() {
    start_action("Please wait for...", "");
    program_events_enable_site();
}

function program_events_enable_site() {

    if (!$("#progress-modal").hasClass("action")) return;;

    $("#progress-contents").text("Programming events enable to site...");

    var eventsEnable = {};
    eventsEnable.DoorOpenTag = $('#check_door_opentag').prop('checked');
    eventsEnable.DoorOpenHS = $('#check_door_openhs').prop('checked');
    eventsEnable.DoorOpenTrade = $('#check_door_opentrade').prop('checked');
    eventsEnable.DoorOpenExit = $('#check_door_openexit').prop('checked');
    eventsEnable.DoorOpenForsed = $('#check_door_openforsed').prop('checked');
    eventsEnable.DoorOpen = $('#check_door_open').prop('checked');
    eventsEnable.DoorOpenRemote = $('#check_door_openremote').prop('checked');
    eventsEnable.DoorClosed = $('#check_door_closed').prop('checked');
    eventsEnable.MainOn = $('#check_door_mainon').prop('checked');
    eventsEnable.MainOff = $('#check_door_mainoff').prop('checked');
    eventsEnable.ProgChanged = $('#check_door_progchanged').prop('checked');

    $.ajax({
        url: urlProgramEventsEnable,
        type: 'POST',
        contentType: 'application/json',
        async: true,
        success: function (result) {
            if (result) {
                if (result.Status) {
                    $("#progress-contents").text("Programmed events enable to site successfully.");
                }
                else error_action(result.Result);
            }
            else error_action('No result');
        },
        error: function (result) {
            error_action('Error');
        },
        data: JSON.stringify(eventsEnable)
    });
};

