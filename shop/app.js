
// build and open a reveal dialog
// trig - a button, input_button or anchor element

// mode
const PROMPT = 2, SHOW = 4, OPEN = 8;

function dialog(trig, mode, siz, title) {

    var sizg = siz == 1 ? 'small' : siz == 2 ? 'large' : 'full';

    // keep the trigger info
    var formid = trig.form ? trig.form.id : '';
    var tag = trig.tagName;
    var action;
    var method = 'post';
    if (tag == 'BUTTON') {
        action = trig.formAction || trig.name;
        method = trig.formMethod || method;
    } else if (tag == 'A') {
        action = trig.href
        method = 'get';
    }

    var src = action.indexOf('?') == -1 ? action + '?dyndlg=true' : action + '&dyndlg=true';

    title = title || trig.innerHTML;

    var html = '<div id="dyndlg" class="' + sizg + ' reveal"  data-reveal data-close-on-click="false">'
        + '<div class="title-bar"><div clsas="title-bar-title">' + title + '</div><div class="title-bar-right"><a class="close-button" href="#" onclick="$(\'#dyndlg\').foundation(\'close\').foundation(\'destroy\').remove();">&times;</a></div></div>'
        + '<div style="height: calc(100% - ' + (mode == OPEN ? '0rem' : '5rem') + ')"><iframe src="' + src + '" style="width: 100%; height: 100%; border: 0"></iframe></div>'
        + (mode == OPEN ? '' : ('<button class=\"button primary float-center\" onclick="ok(this,' + mode + ',\'' + formid + '\',\'' + tag + '\',\'' + action + '\',\'' + method + '\');" disabled>确定</botton>'))
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

    var dlge = $('#dyndlg');

    if (mode == PROMPT) {

        var iframe = dlge.find('iframe');
        var form = iframe.contents().find('form');
        if (form.length != 0) {

            if (!form[0].reportValidity()) return;

            if (tag == 'A') { // append to url and switch
                var qstr = $(form[0]).serialize();
                if (qstr) {
                    var uri = action.indexOf('?') == -1 ? action + '?' + qstr : action + qstr;
                    location.href = uri;
                }
            } else if (tag == 'BUTTON') { // merge to the parent and submit
                if (method == 'get') {
                    var qstr = $(form[0]).serialize();
                    if (qstr) {
                        // dispose the dialog
                        dlge.foundation('close'); dlge.foundation('destroy'); dlge.remove();
                        // load page
                        location.href = action.split("?")[0] + '?' + qstr;
                    }
                } else if (method == 'post') {
                    var theform = $('#' + formid);
                    var pairs = $(form[0]).serializeArray();
                    pairs.forEach(function (e, i) {
                        $('<input>').attr({ type: 'hidden', name: e.name, value: e.value }).appendTo(theform);
                    });

                    // dispose the dialog
                    dlge.foundation('close'); dlge.foundation('destroy'); dlge.remove();
                    // submit
                    theform.attr('action', action);
                    theform.attr('method', method);
                    theform.submit();
                }
            }
            return;
        }
    } else if (mode == SHOW) {
        var iframe = dlge.find('iframe');
        var form = iframe.contents().find('form');
        if (form.length != 0) {
            if (!form[0].reportValidity()) return;
            form[0].submit();
            return;
        }
    } else if (mode == OPEN) {
        var iframe = dlge.find('iframe');
        var form = iframe.contents().find('form');
        if (form.length != 0) {
            if (!form[0].reportValidity()) return;
        }
    }
}


function crop(trig, wid, hei, circle, title) {

    var sizg = 'full';

    title = title || trig.innerHTML;

    var action = trig.href;

    wid = wid ? wid : 120;
    hei = hei ? hei : 120;

    var html =
        '<div id="dyndlg" class="' + sizg + ' reveal"  data-reveal data-close-on-click="false">'
        + '<div class="title-bar"><div clsas="title-bar-title">' + title + '</div><div class="title-bar-right"><a class="close-button" href="#" onclick="$(\'#dyndlg\').foundation(\'close\').foundation(\'destroy\').remove();">&times;</a></div></div>'
        + '<div id="demo" style="height: calc(100% - 8rem); text-align: center;">'
        + '<input type="file" id="fileinput" style="display: none;" onchange="bind(window.URL.createObjectURL(this.files[0]),' + wid + ',' + hei + ',' + circle + ');">'
        + '<div style="text-align: center">'
        + '<a class="button success hollow" onclick="$(\'#fileinput\').click();">选择图片</a>'
        + '<a class="button success hollow" onclick="upload(\'' + action + '\',' + circle + ');">裁剪并上传</a>'
        + '</div>'
        + '</div>';

    var dive = $(html);

    $('body').append(dive);

    // initialize
    $(dive).foundation();

    bind(action, wid, hei, circle);

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
        enforceBoundary: false
    });
}

function upload(url, circle) {

    // get blob of cropped image
    $('#demo').croppie('result', { type: 'blob', size: 'viewport', format: 'jpeg', quality: 0.75, circle: circle }).then(function (blob) {

        var fd = new FormData();
        fd.append('icon', blob, 'icon.png');

        // post
        $.ajax({
            type: 'POST',
            url: url,
            data: fd,
            processData: false,
            contentType: false,
            success: function (data) {
                alert('上传成功!');
            }
        });
    });

}


function prepay(trig) {
    // get prepare id
    var action;
    var method = 'post';
    var tag = trig.tagName;
    if (tag == 'BUTTON') {
        action = trig.formAction || trig.name;
        method = trig.formMethod || method;
    } else if (tag == 'A') {
        action = trig.href
        method = 'get';
    }

    $.ajax({
        url: action,
        type: 'GET',
        dataType: 'json',
        success: function (data) {

            WeixinJSBridge.invoke('getBrandWCPayRequest', data, function (res) {
                if (res.err_msg == "get_brand_wcpay_request:ok") {
                    alert('本订单支付成功！我们一般会在几秒钟之内接到付款通知。如有异常请您联系商家。');
                    location.reload();
                }
            });

        },
        error: function (res) {
            alert('服务器访问失败');
        }
    });
}

