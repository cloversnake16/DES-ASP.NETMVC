function load_template_dialog() {

    $("#load_template_exists").css({ display: 'inline' });
    $("#load_template_status").text('');
    $("#load_template_dialog").modal({ backdrop: 'static', keyboard: false });
};

function save_template_dialog() {

    var error = isValidate();
    if (error != "") {
        err_alert("Save to database", error);
        return;
    }

    $("#save_template_overwrite").css({ display: 'none' });
    $("#save_template_overwrite_no").css({ display: 'none' });
    $("#save_template_exists").css({ display: 'inline' });
    $("#template_name").text('');
    $("#save_template_status").text('');

    $("#save_template_dialog").modal({ backdrop: 'static', keyboard: false });
};

$('#save_template_list').on("change", function () {

    $("#template_name").val($("#save_template_list").val());
});

function template_name_change() {
    $("#save_template_status").text('');
};

function exists_template() {

    if ($('#template_name').val() == '' || $('#template_name').val() == null) {
        $("#save_template_status").text('Empty templatename');
        return;
    }

    $("#save_template_status").text('Verifing templatename...');

    $.ajax({
        url: urlExistsTemplate,
        type: 'POST',
        dataType: 'json',
        async: true,
        success: function (result) {
            if (result) {
                if (result.Status) {
                    if (result.Result) {

                        $("#save_template_status").text("The template already exists.");
                        $("#save_template_overwrite").css({ display: 'inline' });
                        $("#save_template_overwrite_no").css({ display: 'inline' });
                        $("#save_template_exists").css({ display: 'none' });
                    }
                    else save_template();
                }
                else $("#save_template_status").text(result.Result);
            }
            else $("#save_template_status").text('No result');
        },
        error: function (result) {
            $("#save_template_status").text('error');
        },
        data: {
            templateName: $('#template_name').val()
        }
    });
};

function exists_templates() {
    $.ajax({
        url: urlExistsTemplate,
        type: 'POST',
        dataType: 'json',
        async: true,
        success: function (result) {
            if (result) {
                if (result.Status) {
                    if (result.Result) load_template_dialog()
                    else err_alert('Load template', 'No existing template');
                }
                else err_alert('Load template', result.Result);
            }
            else err_alert('Load template', 'No result');
        },
        error: function (result) {
            err_alert.text('Load template', 'error');
        },
        data: {
            templateName: ''
        }
    });
};

function overwrite_no() {
    $("#save_template_overwrite").css({ display: 'none' });
    $("#save_template_overwrite_no").css({ display: 'none' });
    $("#save_template_exists").css({ display: 'inline' });
    $("#template_name").text('');
    $("#save_template_status").text('');
};

$('.delete-sitedata').on("click", function () {

    var col = $(this).parent().parent().find('td').first();
    var id = $(this).attr('data-sitedataid');
    $("#delete_templatename").text(col.text());
    $("#delete_templatename").attr('data-sitedataid', id);

    $("#delete_template_dialog").modal({ backdrop: 'static', keyboard: false });
})

/**
 * remove time interval
 */
$("a").click(function () {
    if (connectionStatus) clearInterval(connectionStatus);
});


function delete_template() {
    $("#delete_template_status").text("Deleting template...");

    $.ajax({
        url: urlDeleteSiteData,
        type: 'POST',
        dataType: 'json',
        async: true,
        success: function (result) {
            if (result) {
                if (result.Status) {
                    $("#delete_template_dialog").modal("hide");
                    location.reload();
                }
                else $("#delete_template_status").text(result.Result);
            }
            else $("#delete_template_status").text('No result');
        },
        error: function (result) {
            $("#delete_template_status").text('Error');
        },
        data: {
            siteDataId: $('#delete_templatename').attr('data-sitedataid')
        }
    });
};

$('.move-site').on("click", function () {
    if (connectionStatus) clearInterval(connectionStatus);
    start_action("Loading last programmed parameters", "Preparing to load from database...");

    var siteParaViewModel = {};
    siteParaViewModel.SiteData = {};
    siteParaViewModel.SiteData.SiteId = $(this).attr('data-id');

    siteParaViewModel.SiteDataType = "MoveSite";
    siteParaViewModel.SiteParaType = "Begin";
    siteParaViewModel.All = true;
    siteParaViewModel.Next = true;
    //console.log(siteParaViewModel);
    //console.log("==========");
    load_parameter(siteParaViewModel);
    //console.log(siteParaViewModel);
});

function load_parameter(siteParaViewModel) {
    if (!$("#progress-modal").hasClass("action")) return;

    if (siteParaViewModel == null) {
        info_action("Done to load.");
        return;
    }

    if (siteParaViewModel.SiteDataType == 6 && siteParaViewModel.SiteParaType == 7) {
        window.location = urlSiteData;
        return;
    }

    $.ajax({
        url: urlLoadParameter,
        type: 'POST',
        async: true,
        contentType: 'application/json',
        success: function (result) {
            if (result) {                
                //console.log("After ============"+result.Status);
                if (result.Status) {
                    siteParaViewModel = setSiteParaViewModel(result.Result);                    
                    load_parameter(siteParaViewModel);
                }
                else {                    
                    if (result.Result != null) {
                        siteParaViewModel = result.Result;
                        if (siteParaViewModel.SiteDataType == 6 && siteParaViewModel.SiteParaType == 0) {
                            $('#progress-modal').attr('data-returnurl', urlSiteData);
                            error_action('No existing last-programmed parameters.');
                        }
                    } else {
                        // modify by me
                        siteParaViewModel.SiteDataType = 6;
                        siteParaViewModel.SiteParaType = 7;
                        load_parameter(siteParaViewModel);                        
                        //-------------
                    }                                        
                }
            }
            else error_action('No result');
        },
        error: function (result) {
            error_action('Error');
        },
        data: JSON.stringify(siteParaViewModel)
    });
};

function save_parameter(siteParaViewModel) {
    if (!$("#progress-modal").hasClass("action")) return;

    if (siteParaViewModel == null) {
        info_action("Done to save");
        return;
    }

    $.ajax({
        url: urlSaveParameter,
        type: 'POST',
        async: true,
        contentType: 'application/json',
        success: function (result) {
            if (result) {
                if (result.Status) {
                    siteParaViewModel = getSiteParaViewModel(result.Result);
                    save_parameter(siteParaViewModel);
                }
                else error_action(result.Result);
            }
            else error_action('No result');
        },
        error: function (result) {
            error_action.text('Error');
        },
        data: JSON.stringify(siteParaViewModel)
    });
};

function getSiteParaViewModel(siteParaViewModel) {

    if (siteParaViewModel.SiteParaType == 0) {

        siteParaViewModel.SiteParaType = "Channel";
        siteParaViewModel.ChannelViewModel = {};
        siteParaViewModel.ChannelViewModel.ChannelIndex = 1;
        siteParaViewModel.ChannelViewModel = getChannelViewModel(siteParaViewModel.ChannelViewModel.ChannelIndex);
    }
    else if (siteParaViewModel.SiteParaType == 1) {

        if (siteParaViewModel.Next) {
            if (siteParaViewModel.ChannelViewModel.ChannelIndex < 256) {
                $("#progress-contents").text("Saving Channel" + siteParaViewModel.ChannelViewModel.ChannelIndex + "...");
                siteParaViewModel.ChannelViewModel.ChannelIndex++;
                siteParaViewModel.ChannelViewModel = getChannelViewModel(siteParaViewModel.ChannelViewModel.ChannelIndex);
            }
            else if (siteParaViewModel.All) {
                siteParaViewModel.ChannelViewModel = null;
                siteParaViewModel.SiteParaType = "Schedule";
                siteParaViewModel.ScheduleViewModel = {};
                siteParaViewModel.ScheduleViewModel.ScheduleIndex = 1;
                siteParaViewModel.ScheduleViewModel = getScheduleViewModel(siteParaViewModel.ScheduleViewModel.ScheduleIndex);
            }
            else siteParaViewModel = null;
        }
        else siteParaViewModel = null;
    }
    else if (siteParaViewModel.SiteParaType == 2) {

        if (siteParaViewModel.Next) {
            if (siteParaViewModel.ScheduleViewModel.ScheduleIndex < 32) {
                $("#progress-contents").text("Saving Schedule" + siteParaViewModel.ScheduleViewModel.ScheduleIndex + "...");
                siteParaViewModel.ScheduleViewModel.ScheduleIndex++;
                siteParaViewModel.ScheduleViewModel = getScheduleViewModel(siteParaViewModel.ScheduleViewModel.ScheduleIndex);
            }
            else if (siteParaViewModel.All) {
                siteParaViewModel.ScheduleViewModel = null;
                siteParaViewModel.SiteParaType = "TradeCode";
                siteParaViewModel.TradeCodeViewModel = {};
                siteParaViewModel.TradeCodeViewModel.TradeCodeIndex = 1;
                siteParaViewModel.TradeCodeViewModel = getTradeCode(siteParaViewModel.TradeCodeViewModel.TradeCodeIndex);
            }
            else siteParaViewModel = null;
        }
        else siteParaViewModel = null;
    }
    else if (siteParaViewModel.SiteParaType == 3) {

        if (siteParaViewModel.Next) {
            if (siteParaViewModel.TradeCodeViewModel.TradeCodeIndex < 16) {
                $("#progress-contents").text("Saving TradeCode" + siteParaViewModel.TradeCodeViewModel.TradeCodeIndex + "...");
                siteParaViewModel.TradeCodeViewModel.TradeCodeIndex++;
                siteParaViewModel.TradeCodeViewModel = getTradeCode(siteParaViewModel.TradeCodeViewModel.TradeCodeIndex);
            }
            else if (siteParaViewModel.All) {
                siteParaViewModel.TradeCodeViewModel = null;
                siteParaViewModel.SiteParaType = "Door";
                siteParaViewModel.DoorViewModel = {};
                siteParaViewModel.DoorViewModel.DoorIndex = 1;
                siteParaViewModel.DoorViewModel = getDoor(siteParaViewModel.DoorViewModel.DoorIndex);
            }
            else siteParaViewModel = null;
        }
        else siteParaViewModel = null;
    }
    else if (siteParaViewModel.SiteParaType == 4) {

        if (siteParaViewModel.Next) {
            if (siteParaViewModel.DoorViewModel.DoorIndex < 128) {
                $("#progress-contents").text("Saving Door" + siteParaViewModel.DoorViewModel.DoorIndex + "...");
                siteParaViewModel.DoorViewModel.DoorIndex++;
                siteParaViewModel.DoorViewModel = getDoor(siteParaViewModel.DoorViewModel.DoorIndex);
            }
            else if (siteParaViewModel.All) {
                siteParaViewModel.DoorViewModel = null;
                siteParaViewModel.SiteParaType = "StaffAccess";
                siteParaViewModel.StaffAccessViewModel = {};
                siteParaViewModel.StaffAccessViewModel.StaffAccessIndex = 1;
                siteParaViewModel.StaffAccessViewModel = getStaffAccess(siteParaViewModel.StaffAccessViewModel.StaffAccessIndex);
            }
            else siteParaViewModel = null;
        }
        else siteParaViewModel = null;
    }
    else if (siteParaViewModel.SiteParaType == 5) {

        if (siteParaViewModel.Next) {
            if (siteParaViewModel.StaffAccessViewModel.StaffAccessIndex < 32) {
                $("#progress-contents").text("Saving StaffAccess" + siteParaViewModel.StaffAccessViewModel.StaffAccessIndex + "...");
                siteParaViewModel.StaffAccessViewModel.StaffAccessIndex++;
                siteParaViewModel.StaffAccessViewModel = getStaffAccess(siteParaViewModel.StaffAccessViewModel.StaffAccessIndex);
            }
            else if (siteParaViewModel.All) {
                $("#progress-contents").text("Saving SystemOption...");
                siteParaViewModel.StaffAccessViewModel = null;
                siteParaViewModel.SiteParaType = "SystemOption";
                siteParaViewModel.SystemOptionViewModel = getSystemOption();
            }
            else siteParaViewModel = null;
        }
        else siteParaViewModel = null;
    }
    else if (siteParaViewModel.SiteParaType == 6) {

        siteParaViewModel.SystemOption = null;
        siteParaViewModel.SiteParaType = "End";
    }
    else siteParaViewModel = null;

    return siteParaViewModel;
};

function setSiteParaViewModel(siteParaViewModel) {
    //console.log(siteParaViewModel);
    if (siteParaViewModel.SiteParaType == 0) {
        siteParaViewModel.SiteParaType = "Channel";
        siteParaViewModel.ChannelViewModel = {};
        siteParaViewModel.ChannelViewModel.ChannelIndex = 1;
    }
    else if (siteParaViewModel.SiteParaType == 1) {
        if (siteParaViewModel.SiteDataType != 6) setChannelViewModel(siteParaViewModel.ChannelViewModel);

        if (siteParaViewModel.Next) {
            if (siteParaViewModel.ChannelViewModel.ChannelIndex < 256) {
                //console.log(siteParaViewModel.ChannelViewModel.ChannelIndex + "========");
                $("#progress-contents").text("Loading Channel" + siteParaViewModel.ChannelViewModel.ChannelIndex + "...");
                siteParaViewModel.ChannelViewModel.ChannelIndex++;
            }
            else if (siteParaViewModel.All) {
                siteParaViewModel.ChannelViewModel = null;
                siteParaViewModel.SiteParaType = "Schedule";
                siteParaViewModel.ScheduleViewModel = {};
                siteParaViewModel.ScheduleViewModel.ScheduleIndex = 1;
            }
            else siteParaViewModel = null;
        }
        else {
            siteParaViewModel = null;
        }
    }
    else if (siteParaViewModel.SiteParaType == 2) {
        if (siteParaViewModel.SiteDataType != 6) setScheduleViewModel(siteParaViewModel.ScheduleViewModel);

        if (siteParaViewModel.Next) {
            if (siteParaViewModel.ScheduleViewModel.ScheduleIndex < 32) {
                $("#progress-contents").text("Loading Schedule" + siteParaViewModel.ScheduleViewModel.ScheduleIndex + "...");
                siteParaViewModel.ScheduleViewModel.ScheduleIndex++;
            }
            else if (siteParaViewModel.All) {
                siteParaViewModel.ScheduleViewModel = null;
                siteParaViewModel.SiteParaType = "TradeCode";
                siteParaViewModel.TradeCodeViewModel = {};
                siteParaViewModel.TradeCodeViewModel.TradeCodeIndex = 1;
            }
            else siteParaViewModel = null;
        }
        else siteParaViewModel = null;
    }
    else if (siteParaViewModel.SiteParaType == 3) {
        if (siteParaViewModel.SiteDataType != 6) setTradeCode(siteParaViewModel.TradeCodeViewModel);

        if (siteParaViewModel.Next) {
            if (siteParaViewModel.TradeCodeViewModel.TradeCodeIndex < 16) {
                $("#progress-contents").text("Loading TradeCode" + siteParaViewModel.TradeCodeViewModel.TradeCodeIndex + "...");
                siteParaViewModel.TradeCodeViewModel.TradeCodeIndex++;
            }
            else if (siteParaViewModel.All) {
                siteParaViewModel.TradeCodeViewModel = null;
                siteParaViewModel.SiteParaType = "Door";
                siteParaViewModel.DoorViewModel = {};
                siteParaViewModel.DoorViewModel.DoorIndex = 1;
            }
            else siteParaViewModel = null;
        }
        else siteParaViewModel = null;
    }
    else if (siteParaViewModel.SiteParaType == 4) {
        if (siteParaViewModel.SiteDataType != 6) setDoor(siteParaViewModel.DoorViewModel);

        if (siteParaViewModel.Next) {
            if (siteParaViewModel.DoorViewModel.DoorIndex < 128) {
                $("#progress-contents").text("Loading Door" + siteParaViewModel.DoorViewModel.DoorIndex + "...");
                siteParaViewModel.DoorViewModel.DoorIndex++;
            }
            else if (siteParaViewModel.All) {
                siteParaViewModel.DoorViewModel = null;
                siteParaViewModel.SiteParaType = "StaffAccess";
                siteParaViewModel.StaffAccessViewModel = {};
                siteParaViewModel.StaffAccessViewModel.StaffAccessIndex = 1;
            }
            else siteParaViewModel = null;
        }
        else siteParaViewModel = null;
    }
    else if (siteParaViewModel.SiteParaType == 5) {
        if (siteParaViewModel.SiteDataType != 6) setStaffAccess(siteParaViewModel.StaffAccessViewModel);

        if (siteParaViewModel.Next) {
            if (siteParaViewModel.StaffAccessViewModel.StaffAccessIndex < 32) {
                $("#progress-contents").text("Loading StaffAccess" + siteParaViewModel.StaffAccessViewModel.StaffAccessIndex + "...");
                siteParaViewModel.StaffAccessViewModel.StaffAccessIndex++;
            }
            else if (siteParaViewModel.All) {
                $("#progress-contents").text("Loading SystemOption...");
                siteParaViewModel.StaffAccessViewModel = null;
                siteParaViewModel.SiteParaType = "SystemOption";
                siteParaViewModel.SystemOptionViewModel = {};
            }
            else siteParaViewModel = null;
        }
        else siteParaViewModel = null;
    }
    else if (siteParaViewModel.SiteParaType == 6) {
        if (siteParaViewModel.SiteDataType != 6) setSystemOption(siteParaViewModel.SystemOptionViewModel);

        siteParaViewModel.SystemOptionViewModel = null;
        siteParaViewModel.SiteParaType = "End";
    }
    else siteParaViewModel = null;
    return siteParaViewModel;
};

function load_last_programed() {

    start_action("Loading last programmed parameters", "Preparing to load from database...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "LastProgrammed";
    siteParaViewModel.SiteParaType = "Begin";
    siteParaViewModel.All = true;
    siteParaViewModel.Next = true;
    load_parameter(siteParaViewModel);
};

function load_last_saved() {

    start_action("Loading last saved parameters", "Preparing to load from database...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "LastSaved";
    siteParaViewModel.SiteParaType = "Begin";
    siteParaViewModel.All = true;
    siteParaViewModel.Next = true;
    load_parameter(siteParaViewModel);
};

function load_template() {

    var templateName = $('#load_template_list').val();
    $("#load_template_dialog").modal("hide");

    start_action("Loading template", "Preparing to load from database...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "Template";
    siteParaViewModel.SiteParaType = "Begin";
    siteParaViewModel.All = true;
    siteParaViewModel.Next = true;
    siteParaViewModel.SiteData = {};
    siteParaViewModel.SiteData.TemplateName = templateName;
    load_parameter(siteParaViewModel);
};

function retrieve_remote_site() {

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "Retrieve";
    siteParaViewModel.SiteParaType = "Begin";
    siteParaViewModel.All = true;
    siteParaViewModel.Next = true;
    load_parameter(siteParaViewModel);
};

function save_last_saved() {

    var error = isValidate();
    if (error != "") {
        err_alert("Save to database", error);
        return;
    }

    start_action("Saving parameters to database", "Preparing to save to database...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "LastSaved";
    siteParaViewModel.SiteParaType = "Begin";
    siteParaViewModel.All = true;
    siteParaViewModel.Next = true;
    save_parameter(siteParaViewModel);
};

function save_template() {

    var templateName = $('#template_name').val();
    $("#save_template_dialog").modal("hide");

    start_action("Saving parameters to " + templateName, "");
    var error = isValidate();
    if (error != "") {
        error_action(error);
        return;
    }

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "Template";
    siteParaViewModel.SiteParaType = "Begin";
    siteParaViewModel.All = true;
    siteParaViewModel.Next = true;
    siteParaViewModel.SiteData = {};
    siteParaViewModel.SiteData.TemplateName = templateName;
    save_parameter(siteParaViewModel);
};

function exists_back_task() {

    start_action("Programing parameters", "Verifing background task...");

    $.ajax({
        url: urlExistsBackTask,
        type: 'POST',
        dataType: 'json',
        async: true,
        success: function (result) {
            if (result) {
                if (result.Status) {
                    if (result.Result) error_action("Background task is progressing on.");
                    else {
                        var error = isValidate();
                        if (error != "") {
                            error_action(error);
                            return;
                        }
                        stop_action();
                        $("#prepare-parameters-modal").modal({ backdrop: 'static', keyboard: false });
                    }
                }
                else error_action(result.Result);
            }
            else error_action('No result');
        },
        error: function (result) {
            error_action('error');
        },
        data: {
        }
    });
};

function prepare_remote_site() {

    $("#prepare-parameters-modal").modal('hide');
    start_action("Programing parameters to remote site", "Preparing parameter to database...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "BackWork";
    siteParaViewModel.SiteParaType = "Begin";
    siteParaViewModel.All = true;
    siteParaViewModel.Next = true;
    save_parameter(siteParaViewModel);
};

function isValidate() {

    var error = validateChannels();
    if (error != "") return error;

    error = validateTradeCodes();
    if (error != "") return error;

    error = validateDoors();
    if (error != "") return error;

    error = validateStaffAccesses();
    if (error != "") return error;

    error = validateSystemOption();
    if (error != "") return error;
    return "";
}
