$('.select-tagcolor').on("change", function () {
    var selected = $(this).val();

    var color = "";
    if (selected == 0) color = "#0000ff";
    else if (selected == 1) color = "#ffffff";
    else if (selected == 2) color = "#ffa500";
    else if (selected == 3) color = "#00ff00";
    else if (selected == 4) color = "#ffff00";
    else if (selected == 5) color = "#808080";
    else if (selected == 6) color = "#ff0000";
    else color = "#000000";
    $(this).css('background-color', color);
})
