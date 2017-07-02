$('.retrieve-door-site').on("click", function () {

    var row = $(this).parent().parent();
    var col_index = row.find('td').first();
    var index = parseInt(col_index.text());

    start_action("Retrieving parameters from remote site", "Retrieving Door" + index + "...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "Retrieve";
    siteParaViewModel.SiteParaType = "Door";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = false;
    siteParaViewModel.Door = {};
    siteParaViewModel.Door.DoorIndex = index;
    load_parameter(siteParaViewModel);
})

function retrieve_all_door_site() {

    start_action("Retrieving parameters from remote site", "Retrieving Door...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "Retrieve";
    siteParaViewModel.SiteParaType = "Door";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = true;
    siteParaViewModel.Door = {};
    siteParaViewModel.Door.DoorIndex = 1;
    load_parameter(siteParaViewModel);
};

$('.program-door-site').on("click", function () {

    var row = $(this).parent().parent();
    var col_index = row.find('td').first();
    var index = parseInt(col_index.text());

    var error = validateDoorByIndex(index);
    if (error != "") {
        error_action(error);
        return;
    }

    start_action("Programming parameters from remote site", "Programming Door" + index + "...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "LastProgrammed";
    siteParaViewModel.SiteParaType = "Door";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = false;
    siteParaViewModel.Door = getDoor(index);
    save_parameter(siteParaViewModel);
})

function program_all_door_site() {

    var error = validateDoors();
    if (error != "") {
        error_action(error);
        return;
    }

    start_action("Programming parameters from remote site", "Programming Door...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "LastProgrammed";
    siteParaViewModel.SiteParaType = "Door";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = true;
    siteParaViewModel.Door = getDoor(1);
    save_parameter(siteParaViewModel);
};

function getDoor(index) {

    var door = {};
    door.DoorIndex = index;

    var row = $('#table_doors').find('tr').first();
    for (var i = 0; i < door.DoorIndex; i++) row = row.next();

    var col = row.find('td').first()

    col = col.next();
    door.LockTimeout = col.find('input').first().val();

    col = col.next();
    door.ScheduleIndex = col.find('input').first().val();

    return door;
};

function setDoor(door) {

    var row = $('#table_doors').find('tr').first();
    for (var i = 0; i < door.DoorIndex; i++) row = row.next();

    var col = row.find('td').first()
    col = col.next();
    col.find('input').first().val(door.LockTimeout);
    col = col.next();
    col.find('input').first().val(door.ScheduleIndex);
    col = col.next().next();
    col.text(door.DateUpdated);
};

function validateDoor(door) {

    if (!validateInteger(door.LockTimeout, 0, 60)) {
        return "PassNumber(" + door.LockTimeout + ") in Door" + door.DoorIndex +
            " is invalid. PassNumber should be integer between 0 and 60";
    }

    if (!validateInteger(door.ScheduleIndex, 0, 32)) {
        return "ScheduleIndex(" + door.ScheduleIndex + ") in Door" + door.DoorIndex +
            " is invalid. ScheduleIndex should be integer between 0 and 32";
    }

    return "";
};

function validateDoorByIndex(index) {

    var row = $('#table_doors').find('tr').first();

    for (var i = 0; i < index; i++) row = row.next();

    var door = {};
    var col = row.find('td').first()
    door.DoorIndex = index;

    col = col.next();
    door.LockTimeout = col.find('input').first().val();

    col = col.next();
    door.ScheduleIndex = col.find('input').first().val();

    return validateDoor(door);
};

function validateDoors() {

    var row = $('#table_doors').find('tr').first();

    for (var i = 0; i < 128; i++) {
        row = row.next();

        var door = {};
        var col = row.find('td').first()
        door.DoorIndex = i + 1;

        col = col.next();
        door.LockTimeout = col.find('input').first().val();

        col = col.next();
        door.ScheduleIndex = col.find('input').first().val();

        var error = validateDoor(door);
        if (error != "") return error;

    }
    return "";
};

