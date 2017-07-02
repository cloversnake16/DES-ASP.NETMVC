$('.retrieve-staff-access-site').on("click", function () {

    var row = $(this).parent().parent();
    var col_index = row.find('td').first();
    var index = parseInt(col_index.text());

    start_action("Retrieving parameters from remote site", "Retrieving StaffAccess" + index + "...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "Retrieve";
    siteParaViewModel.SiteParaType = "StaffAccess";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = false;
    siteParaViewModel.StaffAccess = {};
    siteParaViewModel.StaffAccess.StaffAccessIndex = index;
    load_parameter(siteParaViewModel);
})

function retrieve_all_staff_access_site() {

    start_action("Retrieving parameters from remote site", "Retrieving StaffAccess...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "Retrieve";
    siteParaViewModel.SiteParaType = "StaffAccess";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = true;
    siteParaViewModel.StaffAccess = {};
    siteParaViewModel.StaffAccess.StaffAccessIndex = 1;
    load_parameter(siteParaViewModel);
};

$('.program-staff-access-site').on("click", function () {

    var row = $(this).parent().parent();
    var col_index = row.find('td').first();
    var index = parseInt(col_index.text());

    var error = validateStaffAccessByIndex(index);
    if (error != "") {
        error_action(error);
        return;
    }

    start_action("Programming parameters from remote site", "Programming StaffAccess" + index + "...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "LastProgrammed";
    siteParaViewModel.SiteParaType = "StaffAccess";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = false;
    siteParaViewModel.StaffAccess = getStaffAccess(index);
    save_parameter(siteParaViewModel);
})

function program_all_staff_access_site() {

    var error = validateStaffAccesses();
    if (error != "") {
        error_action(error);
        return;
    }

    start_action("Programming parameters from remote site", "Programming StaffAccess...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "LastProgrammed";
    siteParaViewModel.SiteParaType = "StaffAccess";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = true;
    siteParaViewModel.StaffAccess = getStaffAccess(1);
    save_parameter(siteParaViewModel);
};

function getStaffAccess(index) {

    var staffAccess = {};
    staffAccess.StaffAccessIndex = index;

    var row = $('#table-staffaccess').find('tr').first();
    for (var i = 0; i < staffAccess.StaffAccessIndex; i++) row = row.next();

    var col = row.find('td').first()

    col = col.next();
    staffAccess.AccessLevel = col.find('select').first().val();

    col = col.next();
    staffAccess.PassNumber = col.find('input').first().val();

    return staffAccess;
};

function setStaffAccess(staffAccess) {

    var row = $('#table-staffaccess').find('tr').first();
    for (var i = 0; i < staffAccess.StaffAccessIndex; i++) row = row.next();

    var col = row.find('td').first()
    col = col.next();
    col.find('select').first().val(staffAccess.AccessLevel);

    col = col.next();
    col.find('input').first().val(staffAccess.PassNumber);

    col = col.next().next();
    col.text(staffAccess.DateUpdated);
};

function validateStaffAccess(staffAccess) {

    if (!validateInteger(staffAccess.PassNumber, 0, 999999)) {
        return "PassNumber(" + staffAccess.PassNumber + ") in StaffAccess" + staffAccess.StaffAccessIndex +
            " is invalid. PassNumber should be integer between 0 and 999999";
    }

    return "";
};

function validateStaffAccessByIndex(index) {

    var staffAccess = getStaffAccess(index);
    return validateStaffAccess(staffAccess);
};

function validateStaffAccesses() {

    for (var i = 0; i < 32; i++) {
        var error = validateStaffAccessByIndex(i + 1);
        if (error != "") return error;
    }
    return "";
};
