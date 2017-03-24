
// build and open a reveal dialog
// trig - a button, input_button or anchor element
// mode - STANDARD, PROMPT or PICKER
function dialog(trig, mode, siz) {

    var sizg = siz == 1 ? 'tiny' : mode == 2 ? 'small' : 'large';

    // keep the trigger info
    var formid = trig.form ? trig.form.id : '';
    var tag = trig.tagName;
    var action;
    var method = 'POST';
    if (tag == 'BUTTON') {
        action = trig.formAction || trig.name;
        method = trig.formMethod || method;
    } else if (tag == 'A') {
        action = trig.href
        method = 'GET';
    }

    var src = action.split("?")[0] + '?dlg=true';

    var html = '<div id="dynadlg" class="' + sizg + ' reveal"  data-reveal data-close-on-click="false">'
        + '<h3>' + trig.innerHTML + ' </h3>'
        + '<button class="close-button" type="button" onclick="oncancel(this);"><span aria-hidden="true">&times;</span></button>'
        + '<div style=""><iframe src="' + src + '" style="width: 100%; height: 100%"></iframe></div>'
        + '<button onclick="onok(this,' + mode + ',\'' + formid + '\',\'' + tag + '\',\'' + action + '\',\'' + method + '\');" disabled>确定</botton>'
        + '</div>';

    var dive = $(html);

    $('body').append(dive);

    // initialize
    $(dive).foundation();

    // open
    $(dive).foundation('open');

    // abort the onclick
    return false;
}

// when clicked on the OK button
function onok(okbtn, mode, formid, tag, action, method) {

    var dlge = $('#dynadlg');

    if (mode == 1) { // standard mode
        var iframe = dlge.find('iframe');
        var form = iframe.contents().find('form');
        if (form.length != 0) {
            if (!form[0].reportValidity()) return;
            form[0].submit();
        } else {
            location.reload();
            return;
        }
    } else if (mode == 2) { // prompt mode, merge to the parent and submit
        if (tag == 'A') {
            var iframe = dlge.find('iframe');
            var form = iframe.contents().find('form');
            if (form.length != 0) {
                if (!form[0].reportValidity()) return;
                var qstr = $(form[0]).serialize();
                if (qstr) {
                    location.href = action.split("?")[0] + '?' + qstr;
                }
                return;
            }
        } else if (tag == 'BUTTON') {
            var iframe = dlge.find('iframe');
            var form = iframe.contents().find('form');
            if (form.length != 0) {
                if (!form[0].reportValidity()) return;
                if (method == 'GET') {
                    var qstr = $(form[0]).serialize();
                    if (qstr) {
                        location.href = action.split("?")[0] + '?' + qstr;
                    }
                } else if (method == 'POST') {
                    var pairs = $(form[0]).serializeArray();
                    pairs.forEach(function (e, i) {
                        $('<input>').attr({ type: 'hidden', name: e.name, value: e.value }).appendTo(form);
                    });

                    $(formid).submit;
                }
                return;
            }
        }
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

