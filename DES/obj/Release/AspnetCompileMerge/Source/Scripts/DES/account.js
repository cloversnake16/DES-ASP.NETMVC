$('#forgot_password').on("click", function () {
    $("#login_status").text('');

    if ($('#table_login #UserName').val() == '') {
        $("#login_status").text('Empty username or Email.');
        return;
    }

    $.ajax({
        url: urlSendCode,
        type: 'POST',
        dataType: 'json',
        async: true,
        success: function (result) {
            if (result) {
                if (result.Status) window.location = urlResetPassword;
                else $("#login_status").text(result.Result);
            }
            else $("#login_status").text('No result');
        },
        error: function (result) {
            $("#login_status").text('Error');
        },
        data: {
            username: $('#table_login #UserName').val()
        }
    });
});

$('#change_password_menu').on("click", function () {

    $('#old_password').val('');
    $('#new_password').val('');
    $('#confirm_password').val('');
    $('#change_password_status').text('');

    $("#change_password_dialog").modal({ backdrop: 'static', keyboard: false });
});

function change_password() {

    if ($('#new_password').val() != $('#confirm_password').val())
    {
        $("#change_password_status").text("NewPassword doesn't match with ConfirmPassword .");
        return;
    }
    $("#change_password_status").text("Changing password...");

    var changepasswordModel = {};
    changepasswordModel.OldPassword = $('#old_password').val();
    changepasswordModel.NewPassword = $('#new_password').val();

    $.ajax({
        url: urlChangePassword,
        type: 'POST',
        contentType: 'application/json',
        async: true,
        success: function (result) {

            if (result) {
                if (result.Status) {
                    $("#change_password_dialog").modal("hide");
                }
                else {
                    $("#change_password_status").text(result.Result);
                }
            }
            else {
                $("#change_password_status").text('No result');
            }
        },
        error: function (result) {
            $("#change_password_status").text('Error');
        },
        data: JSON.stringify(changepasswordModel)
    });
}

$('#user_settings_menu').on("click", function () {

    $("#user_settings_dialog").modal({ backdrop: 'static', keyboard: false });
});

function user_settings() {

    $("#user_settings_status").text("Setting user profile...");

    var user = {};
    user.FirstName = $('#user_settings_firstname').val();
    user.LastName = $('#user_settings_lastname').val();
    user.Address = $('#user_settings_address').val();
    user.Email = $('#user_settings_email').val();
    user.ContactNumber = $('#user_settings_contactnumber').val();

    $.ajax({
        url: urlUserSettings,
        type: 'POST',
        contentType: 'application/json',
        async: true,
        success: function (result) {
            if (result) {
                if (result.Status) {
                    $("#user_settings_dialog").modal("hide");
                    location.reload();
                }
                else {
                    $("#user_settings_status").text(result.Result);
                }
            }
            else {
                $("#user_settings_status").text('No result');
            }
        },
        error: function (result) {
            $("#user_settings_status").text(result.statusText);
        },
        data: JSON.stringify(user)
    });
}

