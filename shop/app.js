
// build and open a reveal dialog
// trig - a button, input_button or anchor element
// mode - link, anchor, button, input
function dialog(trig, mode, siz) {

    var sizg = siz == 1 ? 'small' : siz == 2 ? 'large' : 'full';

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
        + '<strong>' + trig.innerHTML + ' </strong>'
        + '<button class="close-button" type="button" onclick="$(\'#dynadlg\').foundation(\'destroy\').remove();">&times;</button>'
        + '<div style="height: calc(100% - 3rem)"><iframe src="' + src + '" style="width: 100%; height: 100%"></iframe></div>'
        + '<button class=\"button primary float-center\" onclick="ok(this,' + mode + ',\'' + formid + '\',\'' + tag + '\',\'' + action + '\',\'' + method + '\');" disabled>确定</botton>'
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
function ok(okbtn, mode, formid, tag, action, method) {

    var dlge = $('#dynadlg');

    if (mode == 1) { // link mode
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
    } else if (mode == 2) { // anchor mode
        var iframe = dlge.find('iframe');
        var form = iframe.contents().find('form');
        if (form.length != 0) {
            if (!form[0].reportValidity()) return;
            form[0].submit();
        } else {
            location.reload();
            return;
        }
    } else if (mode == 4) { // button mode, merge to the parent and submit
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
    } else { // picker mode

    }

    // clean up the dialog
    dlge.foundation('close');
    dlge.foundation('destroy');
    dlge.remove();
}


function crop(trig, siz, wid, hei, circle) {

    var sizg = siz == 1 ? 'small' : siz == 2 ? 'large' : 'full';

    // keep the trigger info
    var formid = trig.form ? trig.form.id : '';
    var tag = trig.tagName;
    var action;
    if (tag == 'BUTTON') {
        action = trig.formAction || trig.name;
        method = trig.formMethod || method;
    } else if (tag == 'A') {
        action = trig.href
        method = 'GET';
    }

    var html = '<div id="dynadlg" class="' + sizg + ' reveal"  data-reveal data-close-on-click="false">'
        + '<strong>' + trig.innerHTML + ' </strong>'
        + '<button class="close-button" type="button" onclick="$(\'#dynadlg\').foundation(\'destroy\').remove();">&times;</button>'
        + '<div id="demo" style="height: calc(100% - 8rem)">'
        + '<input type="file" id="fileinput" style="display: none;" onchange="bind(window.URL.createObjectURL(this.files[0]));">'
        + '<a class="button" onclick="$(\'#fileinput\').click();">选择图片</a>'
        + '<div class="progress button" role="progressbar" tabindex="0" onclick="upload();"><div class="progress-meter" style="width: 50%">Upload</div></div>'
        + '</div>';
    + '</div>';

    var dive = $(html);

    $('body').append(dive);

    // initialize
    $(dive).foundation();

    $('#demo').croppie({ url: action, viewport: { width: wid, height: hei, type: circle ? 'circle' : 'square' }, });

    // open
    $(dive).foundation('open');

    // abort the onclick
    return false;
}

function bind(url, wid, height, circle) {

    var mc = $('#demo');

    mc.croppie('destroy');

    mc.croppie({
        url: url,
        viewport: {
            width: wid,
            height: height,
            type: circle ? 'circle' : 'square'
        },
    });
}

function upload() {
    $.ajax({
        xhr: function () {
            var xhr = new window.XMLHttpRequest();
            xhr.upload.addEventListener("progress", function (evt) {
                if (evt.lengthComputable) {
                    var percent = evt.loaded / evt.total;
                    //Do something with upload progress here
                }
            }, false);
            return xhr;
        },
        type: 'POST',
        url: "/",
        data: {},
        success: function (data) {
            //Do something on success
        }
    });


    $.post('', $('#demo').croppie('result', { type: 'blob' }))
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

function prepay(trig) {
    // get prepare id
    var url = trig.formAction;
    $.get(url, null, function (data) {
        WeixinJSBridge.invoke('getBrandWCPayRequest', data, function (res) {
            if (res.err_msg == "get_brand_wcpay_request:ok") { }
        });
    });
}

