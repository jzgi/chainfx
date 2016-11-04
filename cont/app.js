

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

function dialog(btn, size) {

    let frm = btn.form;

    let lbl = btn.innerText;
    let meth = btn.formMethod || frm.method;
    let act = btn.formAction || frm.action;
    let iframeid = 'iframe' + act;

    let html =
        '<dialog>' +
        '<header>' + lbl + '</header>' +
        '<section>' +
        '<iframe src="' + act + '">' +
        '</iframe>' +
        '</section>' +
        '</dialog>';

    let wrap = document.createElement('div')
    wrap.innerHTML = html;
    let dlg = wrap.firstChild;
    document.getElementsByTagName('body')[0].appendChild(dlg);

    dlg.showModal();

    // if (meth == 'get') {
    //     let url = act + '?' + serialize(frm);
    //     document.getElementById(iframeid)
    //     window.location = url;
    // }

}

function serialize(frm) {
//     // serialize essential form inputs
//     var qry = '';
//     var es = frm.elements;
//     for (var i = 0; i < es.length; i++) {
//         var e = es[i];
//         if (e.disabled) break;
//         if (e.tagName == 'INPUT') {
//             if (!e.checked) break;
//             if (qry.length > 0) qry += '&';
//             qry += e.name + '=' + e.value;
//         } else if (e.tagName == 'SELECT') {
//         }
//     }

//     var url = doer;
//     if (qry.length > 0) url += '?' + qry;

//     var ret = showModalDialog(url, trig, feat);

//     // add extra elements
//     if (Array.isArray(ret)) {
//         for (i = 0; i < ret.length; i += 2) {
//             var inp = document.createElement("input");
//             inp.type = "hidden";
//             inp.name = ret[i];
//             inp.value = ret[i + 1];
//             frm.appendChild(inp);
//         }
//     }
    return 'name=michael&skill=3';
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


//
// file upload
//

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

