$('.retrieve-schedule-site').on("click", function () {

    var row = $(this).parent().parent();
    var col_index = row.find('td').first();
    var index = parseInt(col_index.text());

    start_action("Retrieving parameters from remote site", "Retrieving Schedule" + index + "...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "Retrieve";
    siteParaViewModel.SiteParaType = "Schedule";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = false;
    siteParaViewModel.ScheduleViewModel = {};
    siteParaViewModel.ScheduleViewModel.ScheduleIndex = index;
    load_parameter(siteParaViewModel);
})

function retrieve_all_schedule_site() {

    start_action("Retrieving parameters from remote site", "Retrieving Schedule...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "Retrieve";
    siteParaViewModel.SiteParaType = "Schedule";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = true;
    siteParaViewModel.ScheduleViewModel = {};
    siteParaViewModel.ScheduleViewModel.ScheduleIndex = 1;
    load_parameter(siteParaViewModel);
};

$('.program-schedule-site').on("click", function () {

    var row = $(this).parent().parent();
    var col_index = row.find('td').first();
    var index = parseInt(col_index.text());

    start_action("Programming parameters from remote site", "Programming Schedule" + index + "...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "LastProgrammed";
    siteParaViewModel.SiteParaType = "Schedule";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = false;
    siteParaViewModel.ScheduleViewModel = getScheduleViewModel(index);
    save_parameter(siteParaViewModel);
})

function program_all_schedule_site() {

    start_action("Programming parameters from remote site", "Programming Schedule...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "LastProgrammed";
    siteParaViewModel.SiteParaType = "Schedule";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = true;
    siteParaViewModel.ScheduleViewModel = getScheduleViewModel(1);;
    save_parameter(siteParaViewModel);
};

function getScheduleViewModel(index) {
    var scheduleViewModel = {};
    scheduleViewModel.ScheduleIndex = index;

    var row = $('#table-schedules').find('tr').first();
    for (var i = 0; i < scheduleViewModel.ScheduleIndex; i++) row = row.next();

    var col = row.find('td').first()

    col = col.next();
    var hm = col.find('select').first();
    scheduleViewModel.Start1Hour = hm.val();
    hm = hm.next().next();
    scheduleViewModel.Start1Minute = hm.val();

    col = col.next();
    hm = col.find('select').first();
    scheduleViewModel.End1Hour = hm.val();
    hm = hm.next().next();
    scheduleViewModel.End1Minute = hm.val();

    col = col.next();
    daycol = col.find('span').first();
    scheduleViewModel.Day1 = [];
    for (var i = 0; i < 7; i++) {
        if (daycol.hasClass('selected')) scheduleViewModel.Day1.push(true);
        else scheduleViewModel.Day1.push(false);
        daycol = daycol.next();
    }

    col = col.next();
    hm = col.find('select').first();
    scheduleViewModel.Start2Hour = hm.val();
    hm = hm.next().next();
    scheduleViewModel.Start2Minute = hm.val();

    col = col.next();
    hm = col.find('select').first();
    scheduleViewModel.End2Hour = hm.val();
    hm = hm.next().next();
    scheduleViewModel.End2Minute = hm.val();

    col = col.next();
    daycol = col.find('span').first();
    scheduleViewModel.Day2 = [];
    for (var i = 0; i < 7; i++) {
        if (daycol.hasClass('selected')) scheduleViewModel.Day2.push(true);
        else scheduleViewModel.Day2.push(false);
        daycol = daycol.next();
    }
    return scheduleViewModel;
};

function setScheduleViewModel(scheduleViewModel) {

    var row = $('#table-schedules').find('tr').first();
    for (var i = 0; i < scheduleViewModel.ScheduleIndex; i++) row = row.next();

    var col = row.find('td').first();

    col = col.next();
    var hm = col.find('select').first();
    hm.val(scheduleViewModel.Start1Hour);
    hm = hm.next().next();
    hm.val(scheduleViewModel.Start1Minute);

    col = col.next();
    hm = col.find('select').first();
    hm.val(scheduleViewModel.End1Hour);
    hm = hm.next().next();
    hm.val(scheduleViewModel.End1Minute);


    col = col.next();
    var daycol = col.find('span').first();
    for (var i = 0; i < 7; i++) {
        if (scheduleViewModel.Day1[i]) daycol.addClass('selected');
        else daycol.removeClass('selected');
        daycol = daycol.next();
    }

    col = col.next();
    col.find('input').first().val(scheduleViewModel.Start2);
    hm = col.find('select').first();
    hm.val(scheduleViewModel.Start2Hour);
    hm = hm.next().next();
    hm.val(scheduleViewModel.Start2Minute);

    col = col.next();
    hm = col.find('select').first();
    hm.val(scheduleViewModel.End2Hour);
    hm = hm.next().next();
    hm.val(scheduleViewModel.End2Minute);

    col = col.next();
    daycol = col.find('span').first();
    for (var i = 0; i < 7; i++) {
        if (scheduleViewModel.Day2[i]) daycol.addClass('selected');
        else daycol.removeClass('selected');
        daycol = daycol.next();
    }

    col = col.next().next();
    col.text(scheduleViewModel.DateUpdated);
};

