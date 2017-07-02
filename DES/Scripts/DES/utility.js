function validateInteger(value, min, max) {
    if (isNaN(value)) return false;
    var number = parseInt(value);
    if ((min < max) && (number < min || number > max)) return false;
    return true;
}

function validateHexString(value, min, max) {
    var number = parseInt(value, 16);
    if (number.toString(16) === value.toLowerCase()) {
        if ((min < max) && (number < min || number > max)) return false;
        else return true;
    }
    return false;
}

$('.span-retry-task').on("click", function () {

    var obj = $(this);
    var id = obj.attr('data-id');

    $.ajax({
        url: urlRetryTask,
        type: 'POST',
        async: true,
        dataType: 'json',
        success: function (result) {
            if (result) {
                if (result.Status) {
                    var td = obj.parent().parent().find('td').first().next().next();
                    td.text('Task is waiting for its turn.')
                    info_alert("Info", result.Result);
                }
                else err_alert("Error", result.Result);
            }
            else err_alert("Error", 'No result');
        },
        error: function (result) {
            err_alert("Error", 'Error');
        },
        data: {
            Id: id
        }
    });
})

$('.span-cancel-task').on("click", function () {

    var obj = $(this);
    var id = obj.attr('data-id');

    $.ajax({
        url: urlCancelTask,
        type: 'POST',
        async: true,
        dataType: 'json',
        success: function (result) {
            if (result) {
                if (result.Status) {
                    obj.parent().parent().remove();
                }
                else err_alert("Error", result.Result);
            }
            else err_alert("Error", 'No result');
        },
        error: function (result) {
            err_alert("Error", 'Error');
        },
        data: {
            Id: id
        }
    });
})

$('.span-remove-notification').on("click", function () {

    var obj = $(this);
    var id = obj.attr('data-id');

    $.ajax({
        url: urlRemoveNotification,
        type: 'POST',
        async: true,
        dataType: 'json',
        success: function (result) {
            if (result) {
                if (result.Status) {
                    obj.parent().parent().remove();
                }
                else error_action("Error", result.Result);
            }
            else error_action("Error", 'No result');
        },
        error: function (result) {
            error_action("Error", 'Error');
        },
        data: {
            Id: id
        }
    });
})

