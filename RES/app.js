$(document).foundation();

$(".off-canvas-submenu").hide();

$(".off-canvas-submenu-call")
    .click(function() {

        var icon = $(this).parent().next(".off-canvas-submenu").is(':visible') ? '+' : '-';

        $(this).parent().next(".off-canvas-submenu").slideToggle('fast');

        $(this).find("span").text(icon);

    });

