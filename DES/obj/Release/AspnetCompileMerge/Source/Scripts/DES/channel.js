$('.retrieve-channel-site').on("click", function () {

    var row = $(this).parent().parent();
    var col_index = row.find('td').first().next();
    var index = parseInt(col_index.text());

    start_action("Retrieving parameters from remote site", "Retrieving Channel" + index + "...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "Retrieve";
    siteParaViewModel.SiteParaType = "Channel";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = false;
    siteParaViewModel.ChannelViewModel = {};
    siteParaViewModel.ChannelViewModel.ChannelIndex = index;
    load_parameter(siteParaViewModel);
})

function retrieve_all_channel_site() {

    start_action("Retrieving parameters from remote site", "Loading Channel...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "Retrieve";
    siteParaViewModel.SiteParaType = "Channel";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = true;
    siteParaViewModel.ChannelViewModel = {};
    siteParaViewModel.ChannelViewModel.ChannelIndex = 1;
    load_parameter(siteParaViewModel);
};

$('.program-channel-site').on("click", function () {

    var row = $(this).parent().parent();
    var col_index = row.find('td').first().next();
    var index = parseInt(col_index.text());

    var error = validateChannelByIndex(index);
    if (error != "") {
        error_action(error);
        return;
    }

    start_action("Programming parameters to remote site", "Programming Channel" + index + "...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "LastProgrammed";
    siteParaViewModel.SiteParaType = "Channel";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = false;
    siteParaViewModel.ChannelViewModel = getChannelViewModel(index);
    save_parameter(siteParaViewModel);
})

function program_all_channel_site() {

    var error = validateChannels();
    if (error != "") {
        error_action(error);
        return;
    }

    start_action("Programming parameters to remote site", "Programming Channel...");

    var siteParaViewModel = {}
    siteParaViewModel.SiteDataType = "LastProgrammed";
    siteParaViewModel.SiteParaType = "Channel";
    siteParaViewModel.All = false;
    siteParaViewModel.Next = true;
    siteParaViewModel.ChannelViewModel = getChannelViewModel(1);
    save_parameter(siteParaViewModel);
};

function getChannelViewModel(index) {

    var channelViewModel = {};
    channelViewModel.ChannelIndex = index;
    channelViewModel.Reserved = true;

    var row = $('#table-channels').find('tr').first().next();
    for (var i = 0; i < channelViewModel.ChannelIndex; i++) {
        
        var col = row.find('td').first()
        var checkbox = col.find('input').first();
        var selected = checkbox.prop('checked');

        col = col.next();
        if (col.text() == index) {

            channelViewModel.Selected = selected;

            col = col.next();
            channelViewModel.Flat = col.find('input').first().val();

            col = col.next();
            channelViewModel.PPP = col.find('input').first().val();

            col = col.next();
            channelViewModel.Doors = [];
            channelViewModel.DoorVisibles = [];
            var dr = col.find('div.boolspan').first();
            for (var i = 0; i < 8; i++) {

                var door = [];
                var visible = false;
                for (var j = 0; j < 16; j++) {
                    if (dr.hasClass('visible')) visible = true;
                    if (dr.hasClass('selected')) door.push(true);
                    else door.push(false);
                    dr = dr.next();
                }
                channelViewModel.Doors.push(door);
                channelViewModel.DoorVisibles.push(visible);
            }

            channelViewModel.Tags = [];
            var parent = col.next();
            var tag_color = parent.find('.tag-color').first();
            for (var i = 0; i < 8; i++) {
                channelViewModel.Tags.push($(tag_color).attr('data-tag'));
                tag_color = tag_color.next();
            }

            channelViewModel.Reserved = false;
            return channelViewModel;
        }

        row = row.next();
    }
    return channelViewModel;
}

function setChannelViewModel(channelViewModel) {

    var row = $('#table-channels').find('tr').first().next();
    for (var i = 0; i < channelViewModel.ChannelIndex; i++) {

        var col = row.find('td').first()
        col = col.next();
        if (col.text() == channelViewModel.ChannelIndex) {

            col = col.next();
            col.find('input').first().val(channelViewModel.Flat);
            col = col.next();
            col.find('input').first().val(channelViewModel.PPP);

            col = col.next();
            var dr = col.find('div.boolspan').first();
            for (var i = 0; i < 8; i++) {
                for (var j = 0; j < 16; j++) {
                    if (channelViewModel.DoorVisibles[i]) dr.addClass('visible');
                    else dr.removeClass('visible');
                    if (channelViewModel.Doors[i][j]) dr.addClass('selected');
                    else dr.removeClass('selected');
                    dr = dr.next();
                }
            }

            col = col.next();
            var tag_color = col.find('.tag-color').first();
            for (var i = 0; i < 8; i++) {
                var intVal = channelViewModel.Tags[i];
                $(tag_color).attr('data-tag', intVal)
                tag_color = tag_color.next();
            }

            col = col.next().next();
            col.text(channelViewModel.DateUpdated);

            return;
        }
        row = row.next();
    }
}

function validateChannel(channel) {

    if (channel.Reserved) return "";

    if (!validateInteger(channel.Flat, 0, 9999)) {
        return "Flat(" + channel.Flat + ") in channel" + channel.ChannelIndex +
            " is invalid. Flat should be integer between 0 and 9999";
    }

    if (!validateInteger(channel.PPP, 0, 9999999999999999)) {
        return "PPP(" + channel.PPP + ") in channel" + channel.ChannelIndex + " is invalid. PPP should be digits up to 16";
    }

    for (var j = 0; j < 8; j++) {
        if (!validateHexString(channel.Tags[j], 0, 0xFFFFFFFF)) {
            return "Tag" + (j + 1) + "(" + channel.Tags[j] + ") in channel" + channel.ChannelIndex +
                " is invalid. Tag should be integer between 0 and FFFFFFFF in hexadecimal.";
        }
    }
    return "";
};

function validateChannelByIndex(index) {

    var channel = getChannelViewModel(index);
    return validateChannel(channel);
};

function validateChannels() {

    for (var i = 0; i < 256; i++) {
        var error = validateChannelByIndex(i + 1);
        if (error != "") return error;
    }

    return exists_same_flat();
};

function exists_same_flat() {

    var flats = [];
    var flatIndics = [];

    var rows = $('#table-channels').find('tr');
    var row = rows.first().next();
    for (var i = 0; i < rows.length - 1; i++) {

        var col = row.find('td').first()

        col = col.next();
        var channelIndex = col.text();

        col = col.next();
        var flat = col.find('input').first().val();
        if (flat == 0) continue;

        var index = flats.indexOf(flat);
        if (index > -1) {
            return "Channel" + flatIndics[index] + " and Channel" + channelIndex + " have same flat."
        }

        flats.push(flat);
        flatIndics.push(channelIndex);

        row = row.next();
    }
    return "";

    return exists_same_flat();
};

$('#tag-ishexadecimal').on("click", function () {
    var edit = $("#tag_edit").find('input').first();

    if ($(this).prop('checked')) edit.val(parseInt(edit.val()).toString(16));
    else edit.val(parseInt(edit.val(),16));
})

$('.tag-color').on("click", function () {
    $("#tag-info").text("");
    $('.tag-color.sel').removeClass('sel');
    $(this).addClass('sel');

    var p = $(this).position();
    $("#tag_popup").css("display", "inline-block");
    $("#tag_popup").css("left", p.left + 10);
    $("#tag_popup").css("top", p.top + 30);

    var label = $('#tag_edit').find('span').first();
    label.text('Tag' + $(this).text());

    var edit = $("#tag_edit").find('input').first();
    var value = parseInt($(this).attr('data-tag'));

    if ($('#tag-ishexadecimal').prop('checked')) edit.val(value.toString(16));
    else edit.val(value);
});

$('.tag-set').on("click", function () {
    var edit = $("#tag_edit").find('input').first();

    if ($('#tag-ishexadecimal').prop('checked')) {
        if (!validateHexString(edit.val(), 0, 0xFFFFFFFF)) {
            $("#tag-info").text("invalid value");
            return;
        }
        var select = $('.tag-color.sel');
        select.attr('data-tag', parseInt(edit.val(), 16));

    }
    else {
        if (!validateInteger(edit.val(), 0, 4294967295)) {
            $("#tag-info").text("invalid value");
            return;
        }
        var select = $('.tag-color.sel');
        select.attr('data-tag', edit.val());
    }

    $("#tag_popup").css("display", "none");
    $('.tag-color.sel').removeClass('sel');
});

$('.tag-cancel').on("click", function () {
    $("#tag_popup").css("display", "none");
    $('.tag-color.sel').removeClass('sel');
});

$('td').on("click", $('div'), function (e) {
    if (!e.target.classList.contains('boolspan')) return;
    if (e.target.classList.contains('selected')) e.target.classList.remove('selected');
    else e.target.classList.add('selected');
})

$('.channel-door-all').on("click", function () {
    var parent = $(this).parent();
    var doors = parent.find('div.boolspan');
    doors.addClass('selected');
})

$('.channel-door-clear').on("click", function () {
    var parent = $(this).parent();
    var doors = parent.find('div.boolspan');
    doors.removeClass('selected');
})

$('.channel-door-add').on("click", function () {
    var parent = $(this).parent();
    var doors = parent.find('div.boolspan.visible');
    var len = doors.length;
    if (len < 128) {
        var firstdoor = parent.find('div.boolspan').first();
        for (var i = 0; i < len; i++) firstdoor = firstdoor.next();
        for (var i = 0; i < 16; i++) {
            firstdoor.addClass('visible');
            firstdoor = firstdoor.next();
        }
    }
})

$('.channel-door-delete').on("click", function () {
    var parent = $(this).parent();
    var doors = parent.find('div.boolspan.visible');
    var len = doors.length;
    if (len > 16) {
        var firstdoor = parent.find('div.boolspan').first();
        for (var i = 0; i < len - 16; i++) firstdoor = firstdoor.next();
        for (var i = 0; i < 16; i++) {
            firstdoor.removeClass('visible');
            firstdoor = firstdoor.next();
        }
    }
})

$('.total-attach').on("click", function () {
    if ($(this).prop('checked')) $('.channel-attach').prop('checked', true);
    else $('.channel-attach').prop('checked', false);
})

$('.channel-attach').on("click", function () {
    if (!$(this).prop('checked')) $('.total-attach').prop('checked', false);
})

