
// build and open a reveal dialog
function dialog(btn, modal) {

    var sizg = modal == 1 ? 'large' : modal == 2 ? 'small' : 'tiny';
    // behave: free, submit, input
    var behave = 1;

    var html = '<div id="dynadlg" class="' + sizg + ' reveal"  data-reveal data-close-on-click="false">'
        + '<h3>' + btn.innerHTML + ' </h3>'
        + '<button class="close-button" type="button" onclick="oncancel(this);"><span aria-hidden="true">&times;</span></button>'
        + '<div style="height: 600px"><iframe src="' + btn.name + '" style="width: 100%; height: 100%"></iframe></div>'
        + '<button onclick="onok(this,' + modal + ');" disabled>确定</botton>'
        + '</div>';

    var dive = $(html);

    $('body').append(dive);

    // initialize
    $(dive).foundation();

    // open
    $(dive).foundation('open');
}

// when clicked on the OK button
function onok(btn, modal) {

    var dlge = $('#dynadlg');

    if (modal == 1) { // standard mode, submit or return
        var iframe = dlge.find('iframe');
        var form = iframe.contents().find('form');
        if (form.length != 0) {
            if (!form[0].reportValidity()) return;
            form[0].submit();
        } else {
            location.reload();
            return;
        }
    } else if (modal == 2) { // prompt mode, merge to the parent and submit

    } else { // picker mode

    }

    // clean up the dialog
    dlge.foundation('close');
    dlge.foundation('destroy');
    dlge.remove();
}

// when clicked on the OK button
function oncancel(btn) {
    var dlge = $(btn).parent();
    // clean up the dialog
    dlge.foundation('close');
    dlge.foundation('destroy');
    dlge.remove();
}

function validate() {

    // calculate checked if and unif
    var if_;
    var unif;
    $(':checked').each(function () {
        if_ &= $(this).data('if');
        unif |= $(this).data('unif');
    });

    // enable/disable buttons
    $('button').each(function () {
        var a = $(this).data('if');
        var b = $(this).data('unif');
        this.disabled = (if_ & a == a) && (unif | b == b);
    });
}

