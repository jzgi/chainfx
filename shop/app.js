
// build and open a reveal dialog
function dialog(btn, modal) {

    var sizg = modal == 1 ? 'large' : modal == 2 ? 'small' : 'tiny';
    // behave: free, submit, input
    var behave = 1;

    var html = '<div id="dynadlg" class="' + sizg + ' reveal"  data-reveal data-close-on-click="false">'
        + '<h3>' + btn.innerHTML + ' </h3>'
        + '<button class="close-button" data-close type="button"><span aria-hidden="true">&times;</span></button>'
        + '<iframe src="' + btn.name + '" style="width: 100%; height: 80%"></iframe>'
        + '<button onclick="onok(' + modal + ');">确定</botton>'
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
        var iframe = dlge.find('iframe')[0]
        var form = iframe.contents().find('form');
        if (form) {

        }
    } else if (modal == 2) { // prompt mode, merge to the parent and submit

    } else { // picker mode

    }

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

