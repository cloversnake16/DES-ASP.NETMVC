function start_action(title, contents) {
    $('#progress-modal').attr('data-returnurl', '');
    $("#progress-titlebar").css('background-color', '#0094ff');
    $("#progress-titlebar").css('color', '#ffffff');
    $("#progress-modal").addClass("action");
    $("#progress-title").text(title);
    $("#progress-contents").text(contents);
    $("#progress-modal").modal({ backdrop: 'static', keyboard: false });
};

function stop_action() {
    $("#progress-modal").removeClass("action");
    $("#progress-modal").modal("hide");

    //added by me
    //console.log(typeof getCurrentConnectionStatus);
    if (typeof getCurrentConnectionStatus == 'function') getCurrentConnectionStatus();
    //

    var returnUrl = $('#progress-modal').attr('data-returnurl');
    if (returnUrl != undefined && returnUrl != '') window.location = returnUrl;
};

function info_action(contents) {
    $("#progress-titlebar").css('background-color', '#00ff00');
    $("#progress-titlebar").css('color', '#000000');
    $("#progress-modal").removeClass("action");
    $("#progress-contents").text(contents);
};

function error_action(contents) {
    $("#progress-titlebar").css('background-color', '#ff0000');
    $("#progress-titlebar").css('color', '#ffffff');
    $("#progress-modal").removeClass("action");
    $("#progress-contents").text(contents);
};

function info_alert(title, contents) {
    start_action(title, contents);
    info_action(contents);
};

function err_alert(title, contents) {
    start_action(title, contents);
    error_action(contents);
};

$('.div-channel').on("click", function () {

    if ($(this).hasClass('selected')) $(this).removeClass('selected');
    else $(this).addClass('selected');
})

$('.div-staffdoor').on("click", function () {

    if ($(this).hasClass('selected')) $(this).removeClass('selected');
    else $(this).addClass('selected');
})

function update_reserved_channels() {

    start_action("Reserved channels", "Updating the reserved channels...");

    var ReservedChannels = {};
    ReservedChannels.Channels = [];

    var channel = $('#div-channels').find('div.div-channel').first()
    for (var i = 0; i < 16; i++) {
        if (channel.hasClass('selected')) ReservedChannels.Channels.push(true);
        else ReservedChannels.Channels.push(false);

        channel = channel.next();
    }

    $.ajax({
        url: urlSaveReservedChannels,
        type: 'POST',
        async: true,
        contentType: 'application/json',
        success: function (result) {
            if (result) {
                if (result.Status) info_action(result.Result);
                else error_action(result.Result);
            }
            else error_action('No result');
        },
        error: function (result) {
            error_action('Error');
        },
        data: JSON.stringify(ReservedChannels)
    });
};

function update_alert_setting() {

    start_action("Alert setting", "Updating alert setting...");

    var Setting = {};
    var row = $('#table-alert-setting').find('tr').first();
    var value = row.find('input').first().val();
    if (!validateInteger(value, 1, 20000)) {
        error_action("Invalid CheckPeriod. CheckPeriod should be larger integer than zero ");
    };
    Setting.CheckPeriod = value

    row = row.next();
    value = row.find('input').first().val();
    if (!validateInteger(value, 1, 20000)) {
        error_action("Invalid MaxDataSize. MaxDataSize should be larger integer than zero ");
    };
    Setting.MaxDataSize = value

    row = row.next();
    value = row.find('input').first().val();
    if (!validateInteger(value, 1, 200000)) {
        error_action("Invalid MaxEventlogSize. MaxEventlogSize should be larger integer than zero ");
    };
    Setting.MaxEventlogSize = value

    row = row.next();
    var month = row.find('select.MonthReset').first();
    Setting.MonthReset = month.val();
    var day = row.find('select.DayReset').first();
    Setting.DayReset = day.val();


    $.ajax({
        url: urlSetAlertSetting,
        type: 'POST',
        async: true,
        contentType: 'application/json',
        success: function (result) {

            if (result) {
                if (result.Status) info_action(result.Result);
                else error_action(result.Result);
            }
            else error_action('No result');
        },
        error: function (result) {
            error_action('Error');
        },
        data: JSON.stringify(Setting)
    });
};

$('.div-opendoor').on("click", function () {

    var index = $(this).text();
    start_action("Please wait for...", "Opening Door" + index + "...");
    $.ajax({
        url: urlOpenDoor,
        type: 'POST',
        dataType: 'json',
        async: true,
        success: function (result) {
            if (result) {
                if (result.Status) {
                    info_action(result.Result);
                }
                else {
                    error_action(result.Result);
                }
            }
            else {
                error_action("No result");
            }
        },
        error: function (result) {
            error_action("No result");
        },
        data: {
            Index: index
        }
    });
});

$('.save-door-description').on("click", function () {

    var Description = {};
    var row = $(this).parent().parent();
    var col = row.find('td').first();
    var index = parseInt(col.text());
    Description.Index = index - 1;
    col = col.next().find('input').first();
    Description.Contents = col.val();

    start_action("Please wait for...", "Saving description for door " + index + " to databse...");

    $.ajax({
        url: urlSaveDoorDescription,
        type: 'POST',
        contentType: 'application/json',
        async: true,
        success: function (result) {
            if (result) {
                if (result.Status) info_action(result.Result);
                else error_action(result.Result);
            }
            else error_action("No result");
        },
        error: function (result) {
            error_action("Error");
        },
        data: JSON.stringify(Description)
    });
});

function create_staff_group() {

    var staffGroupName = $('#staff_group_name').val();
    if (staffGroupName == undefined) {
        err_alert('Create staff group', 'Empty staffGroupName.');
        return;
    }

    start_action("Create staff group", "Creating the staff group...");

    var StaffGroupViewModel = {};
    StaffGroupViewModel.StaffGroupName = staffGroupName;

    StaffGroupViewModel.StaffIds = [];
    StaffGroupViewModel.Tags = [];
    var staff_tags = $('#staff_tags').find('tr');
    var tagRow = staff_tags.first();
    for (var i = 0; i < 8; i++) {

        var tag = tagRow.find('input').first();
        StaffGroupViewModel.Tags.push(tag.val());

        var staffId = tagRow.find('select').first();
        StaffGroupViewModel.StaffIds.push(staffId.val());

        tagRow = tagRow.next();
    }

    StaffGroupViewModel.SiteIds = '';
    var staff_sites = $('#staff_sites').find('tr');
    var site = staff_sites.first();
    for (var i = 0; i < staff_sites.length; i++) {

        var checked = site.find('input').first();
        if (checked.prop('checked')) {
            if (StaffGroupViewModel.SiteIds == '') StaffGroupViewModel.SiteIds = checked.attr('data-siteid');
            else StaffGroupViewModel.SiteIds += ',' + checked.attr('data-siteid');
        }
        site = site.next();
    }

    StaffGroupViewModel.Door1 = 0;
    var door = $('#staff_doors').find('div.div-staffdoor').first()
    for (var i = 0; i < 32; i++) {

        if (door.hasClass('selected')) StaffGroupViewModel.Door1 |= (1 << i);
        door = door.next();
    }

    StaffGroupViewModel.Door2 = 0;
    for (var i = 0; i < 32; i++) {

        if (door.hasClass('selected')) StaffGroupViewModel.Door2 |= (1 << i);
        door = door.next();
    }

    StaffGroupViewModel.Door3 = 0;
    for (var i = 0; i < 32; i++) {

        if (door.hasClass('selected')) StaffGroupViewModel.Door3 |= (1 << i);
        door = door.next();
    }

    StaffGroupViewModel.Door4 = 0;
    for (var i = 0; i < 32; i++) {

        if (door.hasClass('selected')) StaffGroupViewModel.Door4 |= (1 << i);
        door = door.next();
    }
    
    $.ajax({
        url: urlCreateStaffGroup,
        type: 'POST',
        async: true,
        contentType: 'application/json',
        success: function (result) {
            if (result) {
                if (result.Status) {    
                    var groupText = $('#staff_group_members').find('option').length + 1;
                    var zz = "";
                    if (groupText.toString().length < 4) {                        
                        for (var i = 0; i < 4 - groupText.toString().length; ++i) zz += '0';
                        groupText =zz+ groupText.toString();
                    }
                    
                    $('#staff_group_members').append($('<option>', {
                        style: "padding:5px 2px",
                        'value': result.Result.Id,
                        text: groupText + ' ' + StaffGroupViewModel.StaffGroupName,
                        'data-door1': result.Result.Door1,
                        'data-door2': result.Result.Door2,
                        'data-door3': result.Result.Door3,
                        'data-door4': result.Result.Door4,
                        'data-staffgroupname': result.Result.StaffGroupName,
                        'data-siteids': result.Result.SiteIds,
                        'data-staff1': result.Result.StaffIds[0],
                        'data-staff2': result.Result.StaffIds[1],
                        'data-staff3': result.Result.StaffIds[2],
                        'data-staff4': result.Result.StaffIds[3],
                        'data-staff5': result.Result.StaffIds[4],
                        'data-staff6': result.Result.StaffIds[5],
                        'data-staff7': result.Result.StaffIds[6],
                        'data-staff8': result.Result.StaffIds[7],
                        'data-tag1': result.Result.Tags[0],
                        'data-tag2': result.Result.Tags[1],
                        'data-tag3': result.Result.Tags[2],
                        'data-tag4': result.Result.Tags[3],
                        'data-tag5': result.Result.Tags[4],
                        'data-tag6': result.Result.Tags[5],
                        'data-tag7': result.Result.Tags[6],
                        'data-tag8': result.Result.Tags[7],
                    }));
                    // add group into staff create and update dialog 
                    $('#create_staff_group_members').append("<option style='padding:5px' value='" + result.Result.Id + "'>" + StaffGroupViewModel.StaffGroupName + "</option>");
                    $('#update_staff_group_members').append("<option style='padding:5px' value='" + result.Result.Id + "'>" + StaffGroupViewModel.StaffGroupName + "</option>");
                    $('#staff_group_name').val("");
                    //----------------------------------------------
                    stop_action();
                }
                else error_action(result.Result);
            }
            else error_action('No result');
        },
        error: function (result) {
            error_action('Error');
        },
        data: JSON.stringify(StaffGroupViewModel)
    });
};

function update_staff_group() {

    var staff = $('#staff_group_members').find(':selected');
    if (staff.attr('data-staffgroupname') == undefined) {
        err_alert('Delete staff', 'No selected staff.');
        return;
    }

    start_action("Update staff", "Updating the staff...");

    var StaffGroupViewModel = {};
    StaffGroupViewModel.Id = staff.val();
    StaffGroupViewModel.StaffGroupName = $('#staff_group_name').val();

    StaffGroupViewModel.StaffIds = [];
    StaffGroupViewModel.Tags = [];
    var staff_tags = $('#staff_tags').find('tr');
    var tagRow = staff_tags.first();
    for (var i = 0; i < 8; i++) {

        var tag = tagRow.find('input').first();
        StaffGroupViewModel.Tags.push(tag.val());

        var staffId = tagRow.find('select').first();
        StaffGroupViewModel.StaffIds.push(staffId.val());

        tagRow = tagRow.next();
    }

    StaffGroupViewModel.SiteIds = '';
    var staff_sites = $('#staff_sites').find('tr');
    var site = staff_sites.first();
    for (var i = 0; i < staff_sites.length; i++) {

        var checked = site.find('input').first();
        if (checked.prop('checked')) {
            if (StaffGroupViewModel.SiteIds == '') StaffGroupViewModel.SiteIds = checked.attr('data-siteid');
            else StaffGroupViewModel.SiteIds += ',' + checked.attr('data-siteid');
        }
        site = site.next();
    }

    StaffGroupViewModel.Door1 = 0;
    var door = $('#staff_doors').find('div.div-staffdoor').first()
    for (var i = 0; i < 32; i++) {

        if (door.hasClass('selected')) StaffGroupViewModel.Door1 |= (1 << i);
        door = door.next();
    }

    StaffGroupViewModel.Door2 = 0;
    for (var i = 0; i < 32; i++) {

        if (door.hasClass('selected')) StaffGroupViewModel.Door2 |= (1 << i);
        door = door.next();
    }

    StaffGroupViewModel.Door3 = 0;
    for (var i = 0; i < 32; i++) {

        if (door.hasClass('selected')) StaffGroupViewModel.Door3 |= (1 << i);
        door = door.next();
    }

    StaffGroupViewModel.Door4 = 0;
    for (var i = 0; i < 32; i++) {

        if (door.hasClass('selected')) StaffGroupViewModel.Door4 |= (1 << i);
        door = door.next();
    }

    $.ajax({
        url: urlUpdateStaffGroup,
        type: 'POST',
        async: true,
        contentType: 'application/json',
        success: function (result) {
            if (result) {
                if (result.Status) {
                    staff.text(staff.text().split(" ")[0]+" "+StaffGroupViewModel.StaffGroupName);
                    staff.attr('data-door1', StaffGroupViewModel.Door1);
                    staff.attr('data-door2', StaffGroupViewModel.Door2);
                    staff.attr('data-door3', StaffGroupViewModel.Door3);
                    staff.attr('data-door4', StaffGroupViewModel.Door4);
                    staff.attr('data-siteids', StaffGroupViewModel.SiteIds);
                    staff.attr('data-staff1', StaffGroupViewModel.StaffIds[0]);
                    staff.attr('data-staff2', StaffGroupViewModel.StaffIds[1]);
                    staff.attr('data-staff3', StaffGroupViewModel.StaffIds[2]);
                    staff.attr('data-staff4', StaffGroupViewModel.StaffIds[3]);
                    staff.attr('data-staff5', StaffGroupViewModel.StaffIds[4]);
                    staff.attr('data-staff6', StaffGroupViewModel.StaffIds[5]);
                    staff.attr('data-staff7', StaffGroupViewModel.StaffIds[6]);
                    staff.attr('data-staff8', StaffGroupViewModel.StaffIds[7]);
                    staff.attr('data-tag1', StaffGroupViewModel.Tags[0]);
                    staff.attr('data-tag2', StaffGroupViewModel.Tags[1]);
                    staff.attr('data-tag3', StaffGroupViewModel.Tags[2]);
                    staff.attr('data-tag4', StaffGroupViewModel.Tags[3]);
                    staff.attr('data-tag5', StaffGroupViewModel.Tags[4]);
                    staff.attr('data-tag6', StaffGroupViewModel.Tags[5]);
                    staff.attr('data-tag7', StaffGroupViewModel.Tags[6]);
                    staff.attr('data-tag8', StaffGroupViewModel.Tags[7]);

                    // modify by me
                    $('#create_staff_group_members').val(staff.val());
                    $('#create_staff_group_members').find(':selected').text($('#staff_group_name').val());
                    $('#create_staff_group_members').find(':selected').prop('selected', false);
                    
                    $('#update_staff_group_members').val(staff.val());
                    $('#update_staff_group_members').find(':selected').text($('#staff_group_name').val());
                    $('#update_staff_group_members').find(':selected').prop('selected', false);

                    $("#table-staff tr").each(function () {
                        if ($(this).find('td:eq(1)').text() == staff.attr('data-staffgroupname')) {
                            $(this).find('td:eq(1)').text($('#staff_group_name').val());
                        }
                    });
                    staff.attr('data-staffgroupname', $('#staff_group_name').val());
                    $('#staff_group_name').val("");
                    //------------------------------
                    info_action(result.Result);
                }
                else error_action(result.Result);
            }
            else error_action('No result');
        },
        error: function (result) {
            error_action('Error');
        },
        data: JSON.stringify(StaffGroupViewModel)
    });
};

function remove_staff_group_modal() {

    var staff = $('#staff_group_members').find(':selected');
    if (staff.attr('data-staffgroupname') == undefined) {
        err_alert('Remove staff', 'No selected staff.');
        return;
    }

    $("#remove-staff-group-modal").modal({ backdrop: 'static', keyboard: false });
}

function remove_staff_group() {

    $("#remove-staff-group-modal").modal('hide');
    start_action("Remove staff", "Removing the staff...");

    var staff = $('#staff_group_members').find(':selected');

    $.ajax({
        url: urlRemoveStaffGroup,
        type: 'POST',
        async: true,
        dataType: 'json',
        success: function (result) {
            if (result) {
                if (result.Status) {
                    staff.remove();
                    // modify by me
                    // modify staff tag and select, site check, door
                    var tagRow = $('#staff_tags').find('tr').first();
                    for (var i = 0; i < 8; i++) {
                        // staff input
                        tagRow.find('input').first().val('');
                        // staff option
                        tagRow.find('select').first().empty();                        
                        tagRow = tagRow.next();
                    }
                    // modify site check
                    var staff_sites = $('#staff_sites').find('tr');
                    var site = staff_sites.first();
                    for (var i = 0; i < staff_sites.length; i++) {
                        var checked = site.find('input').first();                        
                        if (checked.is(":checked")) checked.prop('checked', false);
                        site = site.next();
                    }
                    // modify door status
                    var doorDiv = $('#staff_doors').find('div').first();
                    for (var i = 0; i < 128; i++) {
                        if (!doorDiv.hasClass('selected')) doorDiv.addClass('selected');
                        doorDiv = doorDiv.next();
                    }                    
                    // add group into staff create and update dialog 
                    $('#create_staff_group_members').val(staff.val());
                    $('#create_staff_group_members').find(':selected').remove();
                    $('#create_staff_group_members').find(':selected').prop('selected', false);

                    $('#update_staff_group_members').val(staff.val());
                    $('#update_staff_group_members').find(':selected').remove();
                    $('#update_staff_group_members').find(':selected').prop('selected', false);
                                        
                    $("#table-staff tr").each(function () {
                        if ($(this).find('td:eq(1)').text() == $('#staff_group_name').val()) {
                            $(this).find('td:eq(1)').text("");
                        }
                    });
                    $('#staff_group_name').val("");
                    //----------------------------------------------
                    info_action(result.Result);
                }
                else error_action(result.Result);
            }
            else error_action('No result');
        },
        error: function (result) {
            error_action('Error');
        },
        data: {
            StaffGroupId: staff.val(),
        }
    });
};

function program_staff_group_modal() {

    $("#program-staff-group-modal").modal({ backdrop: 'static', keyboard: false });
}

function program_staff_group() {

    $("#program-staff-group-modal").modal('hide');
    start_action("Program staff", "Programming the staff group...");

    $.ajax({
        url: urlProgramStaffGroup,
        type: 'POST',
        async: true,
        success: function (result) {
            if (result) {
                if (result.Status) {
                    info_action(result.Result);
                }
                else error_action(result.Result);
            }
            else error_action('No result');
        },
        error: function (result) {
            error_action('Error');
        },
    });
};

$('#staff_group_members').on("change", function () {    
    var staff = $(this).find(':selected');
    if (staff == null) return;

    // update staff member select
    var tagRow = $('#staff_tags').find('tr').first();
    for (var i = 0; i < 8; i++) {
        // staff input

        // staff option
        tagRow.find('select').first().empty();
        for (var j = 0; j < 8; ++j){
            if (staff.attr('data-staff' + (j + 1)) > 0) {
                var staffFullName = '';
                $("#table-staff tr").each(function () {
                    if (staff.attr('data-staff' + (j + 1)) == $(this).find('td').last().find('span').first().attr('data-id')) {
                        staffFullName = $(this).find('td:eq(2)').text() + " " + $(this).find('td:eq(3)').text();
                    }
                });
                tagRow.find('select').first().append("<option style='padding:5px 2px' value='" + staff.attr('data-staff' + (j + 1)) + "'>" + staffFullName + "</option>");
            }                
        }        
        tagRow = tagRow.next();
    }
    //---------------------------

    $('#staff_group_name').val(staff.attr('data-staffgroupname'));

    var staff_tags = $('#staff_tags').find('tr');
    var tagRow = staff_tags.first();
    for (var i = 0; i < 8; i++) {

        var tag = tagRow.find('input').first();
        tag.val(staff.attr('data-tag' + (i + 1)));

        var tag = tagRow.find('select').first();
        tag.val(staff.attr('data-staff' + (i + 1)));

        tagRow = tagRow.next();
    }

    var ids = staff.attr('data-siteids').split(",");
    var staff_sites = $('#staff_sites').find('tr');
    var site = staff_sites.first();
    for (var i = 0; i < staff_sites.length; i++) {

        var checked = site.find('input').first();
        var index = ids.indexOf(checked.attr('data-siteid'));
        if (index == -1) checked.prop('checked', false)
        else checked.prop('checked', true)

        site = site.next();
    }

    var dr = staff.attr('data-door1');
    var door = $('#staff_doors').find('div.div-staffdoor').first()
    for (var i = 0; i < 32; i++) {

        if ((dr & (1 << i)) != 0) door.addClass('selected');
        else door.removeClass('selected');
        door = door.next();
    }

    dr = staff.attr('data-door2');
    for (var i = 0; i < 32; i++) {

        if ((dr & (1 << i)) != 0) door.addClass('selected');
        else door.removeClass('selected');
        door = door.next();
    }

    dr = staff.attr('data-door3');
    for (var i = 0; i < 32; i++) {

        if ((dr & (1 << i)) != 0) door.addClass('selected');
        else door.removeClass('selected');
        door = door.next();
    }

    dr = staff.attr('data-door4');
    for (var i = 0; i < 32; i++) {

        if ((dr & (1 << i)) != 0) door.addClass('selected');
        else door.removeClass('selected');
        door = door.next();
    }
});

// modify by me
$(document).ready(function () {
    $('#staff_group_members').trigger('change');
});
//-------------

function create_staff_modal() {

    $("#create-staff-info").text('');
    $("#create-staff-error").text('');

    var row = $('#create-staff-modal').find('table tr').first();
    row.find('input').first().val('');

    row = row.next();
    value = row.find('input').first().val('');

    row = row.next();
    value = row.find('input').first().val('');

    row = row.next();
    value = row.find('input').first().val('');

    $("#create-staff-modal").modal({ backdrop: 'static', keyboard: false });
};

function create_staff() {

    var staff = {};

    var row = $('#create-staff-modal').find('table tr').first();
    var value = row.find('input').first().val();
    if (value == '') {
        $("#create-staff-error").text('Empty FirstName!');
        return;
    };
    staff.FirstName = value;

    row = row.next();
    value = row.find('input').first().val();
    if (value == '') {
        $("#create-staff-error").text('Empty LastName!');
        return;
    };
    staff.LastName = value;

    row = row.next();
    value = row.find('input').first().val();
    if (value == '') {
        $("#create-staff-error").text('Empty Payroll Number!');
        return;
    };
    if (!validateInteger(value, 1, 0x7fffffff)) {
        $("#create-staff-error").text('Invalid Payroll Number!');
        return;
    };
    staff.PayrollNumber = value;

    row = row.next();
    value = row.find('input').first().val();

    if (value == '') {
        $("#create-staff-error").text('Empty DateExpire!');
        return;
    };
    staff.DateExpire = value;

    staff.StaffGroupId = $("#create_staff_group_members").val();

    $("#create-staff-info").text('Creating staff...');
    $("#create-staff-error").text('');    

    $.ajax({
        url: urlCreateStaff,
        type: 'POST',
        async: true,
        contentType: 'application/json',
        success: function (result) {
            $("#create-staff-info").text('');
            if (result) {
                if (result.Status) {
                    $("#create-staff-modal").modal('hide');

                    staff = result.Result;                    
                    var date = new Date(staff.DateExpire);
                    $('#create_staff_group_members').val(staff.StaffGroupId);
                    var groupName = $('#create_staff_group_members').find(':selected').text();
                    $('#create_staff_group_members').val(0);

                    var rows = $('#table-staff').find('tr');
                    $('#table-staff').append(
                        '<tr><td align="center" style="padding: 6px;">' + rows.length + '</td>' +
                        '<td align="center" style="padding: 6px;">' + groupName + '</td>' +
                        '<td align="center" style="padding: 6px;">' + staff.FirstName + '</td>' +
                        '<td align="center" style="padding: 6px;">' + staff.LastName + '</td>' +
                        '<td align="center" style="padding: 6px;">' + staff.PayrollNumber + '</td>' +
                        '<td align="center" style="padding: 6px;">' + staff.DateExpire + '</td>' +
                        '<td align="center" style="padding: 6px;">' + staff.DateUpdated + '</td>' +
                        '<td align="center" valign="middle">' +
                        '<span class="glyphicon glyphicon-pencil update-staff" title="Update staff" data-id="' + staff.Id + '" style="color: #428bca;margin-right:20px"></span>' +
                        '<span class="glyphicon glyphicon-trash remove-staff" title="Remove staff" data-id="' + staff.Id + '" style="color: #428bca;"></span>' +
                        '</td></tr>');

                    // update staff group information
                    $('#staff_group_members').val(staff.StaffGroupId);
                    $('#staff_group_members').find(':selected').attr(result.Command, staff.Id);
                    $('#staff_group_members').find(':selected').prop('selected', false);
                    //-------------------------------
                }
                else $("#create-staff-error").text(result.Result);
            }
            else $("#create-staff-error").text('No result');
        },
        error: function (result) {
            $("#create-staff-error").text('Error');
        },
        data: JSON.stringify(staff)
    });
};

$('#table-staff').on('click', '.update-staff', function () {

    $("#update-staff-info").text('');
    $("#update-staff-error").text('');

    var col = $(this).parent().parent().find('td').first();      
    col = col.next();
    $("#update_staff_group_members option").each(function () {
        // Add $(this).val() to your list
        if ($(this).text() == col.text()) {
            $(this).prop('selected', true);
        } else {
            $(this).prop('selected', false);
        }
    });
        
    $('#update-staff-modal').attr('data-no', col.text());
    $('#update-staff-modal').attr('data-id', $(this).attr('data-id'));

    col = col.next();
    var row = $('#update-staff-modal').find('table tr').first();
    row.find('input').first().val(col.text());

    col = col.next();
    row = row.next();
    row.find('input').first().val(col.text());

    col = col.next();
    row = row.next();
    row.find('input').first().val(col.text());

    col = col.next();
    row = row.next();
    row.find('input').first().val(col.text());     

    $("#update-staff-modal").modal({ backdrop: 'static', keyboard: false });
})

function update_staff() {

    var staff = {};
    staff.Id = $('#update-staff-modal').attr('data-id');

    var row = $('#update-staff-modal').find('table tr').first();
    var value = row.find('input').first().val();
    if (value == '') {
        $("#update-staff-error").text('Empty FirstName!');
        return;
    };
    staff.FirstName = value;

    row = row.next();
    value = row.find('input').first().val();
    if (value == '') {
        $("#update-staff-error").text('Empty LastName!');
        return;
    };
    staff.LastName = value;

    row = row.next();
    value = row.find('input').first().val();
    if (value == '') {
        $("#update-staff-error").text('Empty Payroll Number!');
        return;
    };
    if (!validateInteger(value, 1, 0x7fffffff)) {
        $("#update-staff-error").text('Invalid Payroll Number!');
        return;
    };
    staff.PayrollNumber = value;

    row = row.next();
    value = row.find('input').first().val();

    if (value == '') {
        $("#update-staff-error").text('Empty DateExpire!');
        return;
    };
    staff.DateExpire = value;

    staff.StaffGroupId = $("#update_staff_group_members").val();

    $("#update-staff-info").text('Updating staff...');
    $("#update-staff-error").text('');

    $.ajax({
        url: urlUpdateStaff,
        type: 'POST',
        async: true,
        contentType: 'application/json',
        success: function (result) {
            $("#update-staff-info").text('');
            if (result) {
                if (result.Status) {
                    var number = parseInt($('#update-staff-modal').attr('data-id'));
                    $("#update-staff-modal").modal('hide');

                    staff = result.Result;
                    $("#table-staff tr").each(function () {
                        if ($(this).find('td').last().find('span').attr('data-id') == number) {
                            var col = $(this).find('td').first().next();
                            col.text($("#update_staff_group_members option[value=" + staff.StaffGroupId + "]").text());

                            col = col.next();
                            col.text(staff.FirstName);

                            col = col.next();
                            col.text(staff.LastName);

                            col = col.next();
                            col.text(staff.PayrollNumber);

                            col = col.next();
                            col.text(staff.DateExpire);

                            col = col.next();
                            col.text(staff.DateUpdated);
                        }
                    });

                    // update staff group information
                    if (result.Command != 'undefined') {
                        var prev_option = result.Command.split(",")[0];
                        if (prev_option != 'undefined') {
                            var prev_option_attr = prev_option.split("=")[0].split(":")[1];
                            var prev_option_val = prev_option.split("=")[1];
                            $('#staff_group_members').val(prev_option_val);
                            $('#staff_group_members').find(':selected').attr(prev_option_attr, 0);
                            $('#staff_group_members').find(':selected').prop('selected', false);
                        }

                        if (result.Command.split(",").length == 2 && result.Command.split(",")[1] != 'undefined') {
                            var cur_option_attr = result.Command.split(",")[1].split(":")[1];
                            $('#staff_group_members').val(staff.StaffGroupId);
                            $('#staff_group_members').find(':selected').attr(cur_option_attr, staff.Id);
                            $('#staff_group_members').find(':selected').prop('selected', false);
                        }  
                    }                                      
                    //-------------------------------
                }
                else $("#update-staff-error").text(result.Result);
            }
            else $("#update-staff-error").text('No result');
        },
        error: function (result) {
            $("#update-staff-error").text('Error');
        },
        data: JSON.stringify(staff)
    });
};

$('#table-staff').on('click', '.remove-staff', function () {

    $("#remove-staff-info").text('');
    $("#remove-staff-error").text('');

    var col = $(this).parent().parent().find('td').first();
    $('#remove-staff-modal').attr('data-no', col.text());
    $('#remove-staff-modal').attr('data-id', $(this).attr('data-id'));

    col = col.next();
    var name = col.text();

    col = col.next();
    name += " " + col.text();

    $("#remove-staff-message").text("Are you sure to remove '" + name + "'?");
    $("#remove-staff-modal").modal({ backdrop: 'static', keyboard: false });
})

function remove_staff() {

    var staff = {};
    staff.Id = $('#remove-staff-modal').attr('data-id');

    $("#remove-staff-info").text('Removing staff...');
    $("#remove-staff-error").text('');

    $.ajax({
        url: urlRemoveStaff,
        type: 'POST',
        async: true,
        contentType: 'application/json',
        success: function (result) {
            $("#remove-staff-info").text('');
            if (result) {
                if (result.Status) {
                    var number = parseInt($('#remove-staff-modal').attr('data-no'));
                    $("#remove-staff-modal").modal('hide');

                    var rows = $('#table-staff').find('tr');
                    var row = rows.first();
                    for (var i = 0; i < number; i++) row = row.next();
                    var nextRow = row.next();
                    row.remove();

                    for (var i = number + 1; i < rows.length; i++) {
                        var col = nextRow.find('td').first();
                        col.text(parseInt(col.text()) - 1);
                        nextRow = nextRow.next();
                    }

                    // update staff group information
                    if (result.Command != "no") {
                        $('#staff_group_members').val(result.Command.split("=")[1]);
                        $('#staff_group_members').find(':selected').attr(result.Command.split("=")[0], 0);
                        $('#staff_group_members').find(':selected').prop('selected', false);                         
                    }
                    //-------------------------------
                }
                else $("#remove-staff-error").text(result.Result);
            }
            else $("#remove-staff-error").text('No result');
        },
        error: function (result) {
            $("#remove-staff-error").text('Error');
        },
        data: JSON.stringify(staff)
    });
};