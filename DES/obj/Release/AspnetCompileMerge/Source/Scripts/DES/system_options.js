function retrieve_all_system_option_site() {

    start_action("Retrieving parameters from remote site", "Retrieving SystemOption...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "Retrieve";
    siteParaViewModel.SiteParaType = "SystemOption";
    siteParaViewModel.SystemOption = {};
    load_parameter(siteParaViewModel);
}

function program_all_system_option_site() {

    start_action("Please wait for...", "");
    var error = validateSystemOption();
    if (error != "") {
        error_action(error);
        return;
    }

    start_action("Programming parameters from remote site", "Programming SystemOption...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "LastProgrammed";
    siteParaViewModel.SiteParaType = "SystemOption";
    siteParaViewModel.SystemOption = getSystemOption();
    save_parameter(siteParaViewModel);
}

function getSystemOption() {

    var systemOption = {};
    var row = $('#table_system_options').find('tr').first();
    systemOption.Option1 = row.find('td').first().next().find('input').first().val();

    row = row.next()
    systemOption.Option2 = row.find('td').first().next().find('input').first().val();

    row = row.next()
    systemOption.TradeSchedule = row.find('td').first().next().find('input').first().val();

    row = row.next()
    systemOption.RingTimeout = row.find('td').first().next().find('input').first().val();

    row = row.next()
    systemOption.AudioTimeout = row.find('td').first().next().find('input').first().val();

    row = row.next()
    systemOption.WardenChannel = row.find('td').first().next().find('input').first().val();

    row = row.next()
    systemOption.CustomerNo = row.find('td').first().next().find('input').first().val();

    row = row.next()
    systemOption.SiteNo = row.find('td').first().next().find('input').first().val();

    return systemOption;
};

function setSystemOption(systemOptions) {

    var row = $('#table_system_options').find('tr').first();

    row.find('td').first().next().find('input').first().val(systemOptions.Option1);
    row = row.next();

    row.find('td').first().next().find('input').first().val(systemOptions.Option2);
    row = row.next();

    row.find('td').first().next().find('input').first().val(systemOptions.TradeSchedule);
    row = row.next();

    row.find('td').first().next().find('input').first().val(systemOptions.RingTimeout);
    row = row.next();

    row.find('td').first().next().find('input').first().val(systemOptions.AudioTimeout);
    row = row.next();

    row.find('td').first().next().find('input').first().val(systemOptions.WardenChannel);
    row = row.next();

    row.find('td').first().next().find('input').first().val(systemOptions.CustomerNo);
    row = row.next();

    row.find('td').first().next().find('input').first().val(systemOptions.SiteNo);
    row = row.next();

    row.find('td').first().next().text(systemOptions.DateUpdated);
};

function validateSystemOption() {

    var systemOption = {};
    var row = $('#table_system_options').find('tr').first();
    systemOption.Option1 = row.find('td').first().next().find('input').first().val();
    if (!validateHexString(systemOption.Option1, 0, 0xFF)) {
        return "Option1(" + systemOption.Option1 + ") in SystemOption" +
            " is invalid. Option1 should be integer between 0 and FF";
    }

    row = row.next()
    systemOption.Option2 = row.find('td').first().next().find('input').first().val();
    if (!validateHexString(systemOption.Option2, 0, 0xFF)) {
        return "Option2(" + systemOption.Option2 + ") in SystemOption" +
            " is invalid. Option2 should be integer between 0 and FF";
    }

    row = row.next()
    systemOption.TradeSchedule = row.find('td').first().next().find('input').first().val();
    if (!validateInteger(systemOption.TradeSchedule, 0, 32)) {
        return "TradeSchedule(" + systemOption.TradeSchedule + ") in SystemOption" +
            " is invalid. TradeSchedule should be integer between 0 and 32";
    }

    row = row.next()
    systemOption.RingTimeout = row.find('td').first().next().find('input').first().val();
    if (!validateInteger(systemOption.RingTimeout, 0, 240)) {
        return "RingTimeout(" + systemOption.RingTimeout + ") in SystemOption" +
            " is invalid. RingTimeout should be integer between 0 and 240";
    }

    row = row.next()
    systemOption.AudioTimeout = row.find('td').first().next().find('input').first().val();
    if (!validateInteger(systemOption.AudioTimeout, 0, 240)) {
        return "AudioTimeout(" + systemOption.AudioTimeout + ") in SystemOption" +
            " is invalid. AudioTimeout should be integer between 0 and 240";
    }

    row = row.next()
    systemOption.WardenChannel = row.find('td').first().next().find('input').first().val();
    if (!validateHexString(systemOption.WardenChannel, 0, 0xFF)) {
        return "WardenChannel(" + systemOption.WardenChannel + ") in SystemOption" +
            " is invalid. WardenChannel should be integer between 0 and FF";
    }

    row = row.next()
    systemOption.CustomerNo = row.find('td').first().next().find('input').first().val();
    if (!validateInteger(systemOption.CustomerNo, 0, 9999)) {
        return "CustomerNo(" + systemOption.CustomerNo + ") in SystemOption" +
            " is invalid. CustomerNo should be integer between 0 and 9999";
    }

    row = row.next()
    systemOption.SiteNo = row.find('td').first().next().find('input').first().val();
    if (!validateInteger(systemOption.SiteNo, 0, 9999)) {
        return "SiteNo(" + systemOption.SiteNo + ") in SystemOption" +
            " is invalid. SiteNo should be integer between 0 and 9999";
    }

    return "";
};
