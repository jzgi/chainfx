

//
// ajax
//

var ajax = {};
ajax.x = function () {
    if (typeof XMLHttpRequest !== 'undefined') {
        return new XMLHttpRequest();
    }
    var versions = [
        "MSXML2.XmlHttp.6.0",
        "MSXML2.XmlHttp.5.0",
        "MSXML2.XmlHttp.4.0",
        "MSXML2.XmlHttp.3.0",
        "MSXML2.XmlHttp.2.0",
        "Microsoft.XmlHttp"
    ];

    var xhr;
    for (var i = 0; i < versions.length; i++) {
        try {
            xhr = new ActiveXObject(versions[i]);
            break;
        } catch (e) {
        }
    }
    return xhr;
};

ajax.send = function (url, callback, method, data, async) {
    if (async === undefined) {
        async = true;
    }
    var x = ajax.x();
    x.open(method, url, async);
    x.onreadystatechange = function () {
        if (x.readyState == 4) {
            callback(x.responseText)
        }
    };
    if (method == 'POST') {
        x.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
    }
    x.send(data)
};

ajax.get = function (url, data, callback, async) {
    var query = [];
    for (var key in data) {
        query.push(encodeURIComponent(key) + '=' + encodeURIComponent(data[key]));
    }
    ajax.send(url + (query.length ? '?' + query.join('&') : ''), callback, 'GET', null, async)
};

ajax.post = function (url, data, callback, async) {
    var query = [];
    for (var key in data) {
        query.push(encodeURIComponent(key) + '=' + encodeURIComponent(data[key]));
    }
    ajax.send(url, callback, 'POST', query.join('&'), async)
};

//
//

function ButtonOnClick(btn, size, doer) {

    $(btn.form).append('<div id="myModal" class="reveal-modal" data-reveal aria-labelledby="modalTitle" aria-hidden="true" role="dialog"><h2 id="modalTitle">Awesome. I have it.</h2><p id="dialog"></p><a class="button radius" onclick="$(\'#myModal\').foundation(\'reveal\', \'close\');">Close</a></div>');

    let url = doer + '?' + $(btn.form).serialize();

    $('#myModal').foundation('reveal', 'open', {
        url: url,
        dataType: 'html',
        success: function (data) {
            alert(data);
            $('#dialog').html(data);
        },
        error: function () {
            alert('failed loading modal');
        }
    });
}


function sendRequest(url, callback) {

    var xhr = new XMLHttpRequest();
    try {
        xhr = new XMLHttpRequest();
    } catch (e) {
        try {
            xhr = new ActiveXObject("Msxml2.XMLHTTP");
        } catch (e) {
            try {
                xhr = new ActiveXObject("Microsoft.XMLHTTP");
            } catch (e) {
            }
        }
    }

    xhr.open("GET", url, true);
    xhr.setRequestHeader('User-Agent', 'XMLHTTP/1.0');
    xhr.onreadystatechange = function () {
        if (xhr.readyState != 4) return;
        if (xhr.status != 200 && xhr.status != 304) {
            return;
        }
        callback(xhr);
    };
    if (xhr.readyState == 4) return;
    xhr.send();
}


// HTML 5 DOM level 3 CSS 3

function box(url, wid, hei) {
    var feat = 'center: yes; status: no;';
    if (wid >= 0) feat += ' dialogwidth: ' + wid + 'px;';
    if (hei >= 0) feat += ' dialogheight: ' + hei + 'px;';

    return showModalDialog(url, null, feat);
}

function dialog(trig, doer, wid, hei) {
    var frm = trig.form;

    // features
    var feat = 'center: yes; status: no;';
    if (wid >= 0) feat += ' dialogwidth: ' + wid + 'px;';
    if (hei >= 0) feat += ' dialogheight: ' + hei + 'px;';

    // serialize essential form inputs
    var qry = '';
    var es = frm.elements;
    for (var i = 0; i < es.length; i++) {
        var e = es[i];
        if (e.disabled) break;
        if (e.tagName == 'INPUT') {
            if (!e.checked) break;
            if (qry.length > 0) qry += '&';
            qry += e.name + '=' + e.value;
        } else if (e.tagName == 'SELECT') {
        }
    }

    var url = doer;
    if (qry.length > 0) url += '?' + qry;

    var ret = showModalDialog(url, trig, feat);

    // add extra elements
    if (Array.isArray(ret)) {
        for (i = 0; i < ret.length; i += 2) {
            var inp = document.createElement("input");
            inp.type = "hidden";
            inp.name = ret[i];
            inp.value = ret[i + 1];
            frm.appendChild(inp);
        }
    }
    return ret;
}

// encode required form controls into an array, each control has name and value
function enc(trig) {
    var a = [];
    var form = trig.form;
    if (form) {
        var elements = form.elements;
        for (var i = 0; i < elements.length; i++) {
            var el = elements[i];
            if (el.required) {
                switch (el.type) {
                    case 'text':
                        a.push(el.name);
                        a.push(el.value);
                        break;
                    case 'checkbox':
                }
            }
        }
    }
    return a;
}

// call server-side actions
var A = {
    trig: null,

    form: null,

    a: [],

    en: false,

    by: function (trig) {
        this.trig = trig;
        var form = trig.form;
        if (!form) {
            alert('Missing a enclosing form');
            return false;
        }
        this.form = form;
        // if any checked
        var i;
        var elements = form.elements;
        for (i = 0; i < elements.length; i++) {
            if (elements[i].checked) {
                return true;
            }
        }
        return false;
    },

    confirm: function () {

        // compose message
        var trig = this.trig;
        var msg = '您是否确定要';
        if (trig.value) {
            msg += trig.value;
            msg += '如下数据项：\n';
        }

        var i;
        var elements = this.form.elements;
        for (i = 0; i < elements.length; i++) {
            var el = elements[i];
            if (el.checked) {
                msg += '\n\t' + el.value + '\t' + el.alt;
            }
        }

        return window.confirm(msg);
    },

    show: function (sub, wid, hei, fix) {
        // show modal dialog box
        var features = 'center: yes; status: no;';
        if (wid >= 0) features += ' dialogwidth: ' + wid + 'px;';
        if (hei >= 0) features += ' dialogheight: ' + hei + 'px;';
        if (fix) features += ' resizable: no;';
        return showModalDialog(sub, this.a, features);
    },

    onsubmit: function () {
        // add extra elements to the form
        var a = this.a;
        for (i = 0; i < a.length; i = i + 2) {
            var el = document.createElement("input");
            el.type = "hidden";
            el.name = a[i];
            el.value = a[i + 1];
            this.form.appendChild(el);
        }
        return true;
    }

};

// run in dialog box
function dial(ok) {
    var a = window.dialogArguments;
    if (ok) {
        // add to the array
        var form = document.forms[0];
        if (!form) {
            var content = document.getElementById('content');
            form = content.document.forms[0];
        }
        var elements = form.elements;
        for (var i = 0; i < elements.length; i++) {
            var el = elements[i];
            a.push(el.name);
            a.push(el.value);
        }
        return true;
    } else {
        return false;
    }
}

function imgclick(trig) {
    var file = trig.parentElement.children[0];
    file.click();
}

function filechange(trig) {
    var img = trig.parentElement.children[1];
    var file = trig.files[0];
    img.src = window.URL.createObjectURL(file);
    img.onload = function (e) {
        window.URL.revokeObjectURL(img.src);
    };
}

function FileUpload(img, file) {
    var reader = new FileReader();
    this.ctrl = createThrobber(img);
    var xhr = new XMLHttpRequest();
    this.xhr = xhr;

    var self = this;
    this.xhr.upload.addEventListener("progress", function (e) {
        if (e.lengthComputable) {
            var percentage = Math.round((e.loaded * 100) / e.total);
            self.ctrl.update(percentage);
        }
    }, false);

    xhr.upload.addEventListener("load", function (e) {
        self.ctrl.update(100);
        var canvas = self.ctrl.ctx.canvas;
        canvas.parentNode.removeChild(canvas);
    }, false);
    xhr.open("POST", "http://demos.hacks.mozilla.org/paul/demos/resources/webservices/devnull.php");
    xhr.overrideMimeType('text/plain; charset=x-user-defined-binary');
    reader.onload = function (evt) {
        xhr.sendAsBinary(evt.target.result);
    };
    reader.readAsBinaryString(file);
}