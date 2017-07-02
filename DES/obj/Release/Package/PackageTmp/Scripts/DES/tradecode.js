$('.retrieve-tradecode-site').on("click", function () {

    var row = $(this).parent().parent();
    var col_index = row.find('td').first();
    var index = parseInt(col_index.text());

    start_action("Retrieving parameters from remote site", "Retrieving TradeCode" + index + "...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "Retrieve";
    siteParaViewModel.SiteParaType = "TradeCode";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = false;
    siteParaViewModel.TradeCode = {};
    siteParaViewModel.TradeCode.TradeCodeIndex = index;
    load_parameter(siteParaViewModel);
})

function retrieve_all_tradecode_site() {

    start_action("Retrieving parameters from remote site", "Retrieving TradeCode...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "Retrieve";
    siteParaViewModel.SiteParaType = "TradeCode";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = true;
    siteParaViewModel.TradeCode = {};
    siteParaViewModel.TradeCode.TradeCodeIndex = 1;
    load_parameter(siteParaViewModel);
};

$('.program-tradecode-site').on("click", function () {

    var row = $(this).parent().parent();
    var col_index = row.find('td').first();
    var index = parseInt(col_index.text());

    var error = validateTradeCodeByIndex(index);
    if (error != "") {
        error_action(error);
        return;
    }

    start_action("Programming parameters to remote site", "Programming TradeCode" + index + "...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "LastProgrammed";
    siteParaViewModel.SiteParaType = "TradeCode";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = false;
    siteParaViewModel.TradeCode = getTradeCode(index);
    save_parameter(siteParaViewModel);
})

function program_all_tradecode_site() {

    var error = validateTradeCodes();
    if (error != "") {
        error_action(error);
        return;
    }

    start_action("Programming parameters to remote site", "Programming TradeCode...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "LastProgrammed";
    siteParaViewModel.SiteParaType = "TradeCode";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = true;
    siteParaViewModel.TradeCode = getTradeCode(1);
    save_parameter(siteParaViewModel);
};

function getTradeCode(index) {
    var tradecode = {};
    tradecode.TradeCodeIndex = index;

    var row = $('#table_tradecodes').find('tr').first();
    for (var i = 0; i < tradecode.TradeCodeIndex; i++) row = row.next();

    var col = row.find('td').first()
    col = col.next();
    tradecode.PassNumber = col.find('input').first().val();
    col = col.next();
    tradecode.ScheduleIndex = col.find('input').first().val();

    return tradecode;
}

function setTradeCode(tradecode) {

    var row = $('#table_tradecodes').find('tr').first();
    for (var i = 0; i < tradecode.TradeCodeIndex; i++) row = row.next();

    var col = row.find('td').first()
    col = col.next();
    col.find('input').first().val(tradecode.PassNumber);

    col = col.next();
    col.find('input').first().val(tradecode.ScheduleIndex);

    col = col.next().next();
    col.text(tradecode.DateUpdated);
}

function validateTradeCode(tradecode) {

    if (!validateInteger(tradecode.PassNumber, 0, 999999)) {
        return "TradeCode(" + tradecode.PassNumber + ") in TradeCode" + tradecode.TradeCodeIndex +
            " is invalid. TradeCode should be integer between 100000 and 999999";
    }

    if (!validateInteger(tradecode.ScheduleIndex, 0, 32)) {
        return "ScheduleIndex(" + tradecode.ScheduleIndex + ") in Tradecode" + tradecode.TradeCodeIndex +
            " is invalid. ScheduleIndex should be integer between 0 and 32";
    }

    return "";
};

function validateTradeCodeByIndex(index) {

    var tradecode = getTradeCode(index);
    return validateTradeCode(tradecode);
};

function validateTradeCodes() {

    for (var i = 0; i < 16; i++) {
        var error = validateTradeCodeByIndex(i + 1);
        if (error != "") return error;
    }
    return "";
};
