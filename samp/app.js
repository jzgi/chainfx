

function prepayfunc(trig) {
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
            WeixinJSBridge.invoke('getBrandWCPayRequest',
                data,
                function (res) {
                    if (res.err_msg == "get_brand_wcpay_request:ok") {
                        alert('成功支付');
                        location.reload();
                    }
                });
        },
        error: function (res) {
            alert('服务器访问失败');
        }
    });
    //
    return false;
}


// build and open a reveal dialog
// trig - a button, input_button or anchor element

// mode
const PROMPT = 2, SHOW = 4, OPEN = 8;

function dialog(trig, mode, pick, siz, title) {
    var sizg = siz == 1 ? 'tiny' : siz == 2 ? 'small' : siz == 3 ? 'medium' : siz == 4 ? 'large' : 'full';
    // keep the trigger info
    var formid = trig.form ? trig.form.id : '';
    var tag = trig.tagName;
    var action;
    var method = 'post';
    var src;
    var trigclass;
    if (tag == 'BUTTON') {
        action = trig.formAction || trig.name;
        method = trig.formMethod || method;
        var qstr;
        if (pick) { // if must pick form values
            qstr = $(trig.form).serialize();
            if (!qstr) return false;
        }
        if (qstr) {
            src = action.indexOf('?') == -1 ? action + '?' + qstr : action + '&' + qstr;
        } else {
            src = action;
        }
        trigclass = ' button-trig';
    } else if (tag == 'A') {
        action = trig.href;
        method = 'get';
        src = action.indexOf('?') == -1 ? action + '?inner=true' : action + '&' + 'inner=true';
        trigclass = ' anchor-trig';
    }

    title = title || trig.innerHTML;

    var bottom = mode == OPEN ? '3.5rem' : '6rem';
    var html =
        '<div id="dyndlg" class="' + sizg + ' reveal' + trigclass + '"  data-reveal data-close-on-click="false">' +
        '<div class="title-bar"><div class="title-bar-title">' + title + '</div><div class="title-bar-right"><a class="close-dlg" onclick="$(\'#dyndlg\').foundation(\'close\').foundation(\'destroy\').remove(); return false;" style="font-size: 1.5rem">&#10060;</a></div></div>' +
        '<div style="height: -webkit-calc(100% - ' + bottom + '); height: calc(100% - ' + bottom + ')"><iframe src="' + src + '" style="width: 100%; height: 100%; border: 0"></iframe></div>' + (mode == OPEN ? '' : ('<button class=\"button primary hollow\" style="display: block; margin-top: 0.625rem; margin-left: auto; margin-right: auto" onclick="ok(this,' + mode + ',\'' + formid + '\',\'' + tag + '\',\'' + action + '\',\'' + method + '\');" disabled>确定</botton>')) + '</div>';
    var dive = $(html);
    $('body').prepend(dive);
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
        iframe = dlge.find('iframe');
        form = iframe.contents().find('form');
        if (!form.length || !form[0].reportValidity()) return;
        if (tag == 'A') { // append to url and switch
            qstr = $(form[0]).serialize();
            if (qstr) {
                uri = action.indexOf('?') == -1 ? action + '?' + qstr : action + qstr;
                location.href = uri;
            }
        } else if (tag == 'BUTTON') { // merge to the parent and submit
            if (method == 'get') {
                var qstr = $(form[0]).serialize();
                if (qstr) {
                    // dispose the dialog
                    dlge.foundation('close');
                    dlge.foundation('destroy');
                    dlge.remove();
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
                dlge.foundation('close');
                dlge.foundation('destroy');
                dlge.remove();
                // submit
                theform.attr('action', action);
                theform.attr('method', method);
                theform.submit();
            }
        }
    } else if (mode == SHOW) {
        iframe = dlge.find('iframe');
        form = iframe.contents().find('form');
        if (form.length != 0) {
            if (!form[0].reportValidity()) return;
            form[0].submit();
        }
    } else {
        if (mode == OPEN) {
            iframe = dlge.find('iframe');
            form = iframe.contents().find('form');
            if (form.length) {
                if (!form[0].reportValidity()) return;
            }
        }
    }
}


function crop(trig, ordinals, siz, title) {

    var wid, hei, sizg;
    title = title || trig.innerHTML;
    var action = trig.href || trig.formAction;
    switch (siz) {
        case 1:
            wid = 120; hei = 120; sizg = 'tiny';
            break;
        case 2:
            wid = 240; hei = 240; sizg = 'small';
            break;
        case 3:
            wid = 320; hei = 320; sizg = 'medium';
            break;
        case 4:
            wid = 480; hei = 480; sizg = 'large';
            break;
        default:
            wid = 640; hei = 640; sizg = 'full';
            break;
    }

    var html =
        '<div id="dyndlg" class="' + sizg + ' reveal"  data-reveal data-close-on-click="false">' +
        '<div class="title-bar"><div class="title-bar-left">'
    if (ordinals > 0) {
        html += '<select id="ordinal" onchange="bind(\'' + action + '\', this.value, ' + wid + ', ' + hei + ')">';
        for (var i = 1; i <= ordinals; i++) {
            html += '<option value="' + i + '">' + i + '</option>';
        }
        html += '</select>';
    }
    html +=
        '<button class="button hollow" onclick="$(\'#fileinput\').click();">浏览...</button><button class="button hollow" onclick="upload(\'' + action + '\', $(\'#ordinal\').val());">上传</button>' +
        '</div>' +
        '<div class="title-bar-right">' +
        '<a onclick="$(\'#dyndlg\').foundation(\'close\').foundation(\'destroy\').remove(); return false;" style="font-size: 1.5rem">&#10060;</a>' +
        '</div>' +
        '</div>'; // title-bar
    html += '<div id="crop" style="height: -webkit-calc(100% - 6.5rem); height: calc(100% - 6.5rem); text-align: center;"><input type="file" id="fileinput" style="display: none;" onchange="bind(window.URL.createObjectURL(this.files[0]), 0,' + wid + ',' + hei + ');"></div>';
    html += '</div>'; // dyndlg

    var dive = $(html);

    $('body').prepend(dive);
    // initialize
    $(dive).foundation();
    bind(action, 1, wid, hei);
    // open
    $(dive).foundation('open');
    // abort the onclick
    return false;
}

function bind(url, ordinal, width, height) {
    if (ordinal) url = url + '-' + ordinal;
    var mc = $('#crop');
    mc.croppie('destroy');
    mc.croppie({
        url: url,
        viewport: {
            width: width,
            height: height
        },
        enforceBoundary: true
    });
}

function upload(url, ordinal) {
    if (ordinal) url = url + '-' + ordinal;
    // get blob of cropped image
    $('#crop').croppie('result',
        {
            type: 'blob',
            size: 'viewport',
            format: 'jpeg',
            quality: 0.75
        }).then(function (blob) {
            var fd = new FormData();
            fd.append('jpeg', blob, 'jpeg.jpg');
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

// click parent's close button
function closeup(reload) {
    var win = window.parent;
    var dlg = $('#dyndlg', win.document);
    var btn = dlg.hasClass('button-trig');
    dlg.find('.close-dlg').trigger('click'); // close-button click
    if (reload && btn) {
        win.location.reload(false);
    }
}

function checkit(el) {
    var rec = $(el).closest('.card')
    if (!rec.length) {
        rec = $(el).closest('tr')
    }
    if (el.checked) {
        rec.addClass('checked');
    } else {
        rec.removeClass('checked');
    }
}

/*************************
 * Croppie
 * Copyright 2017
 * Foliotek
 * Version: 2.5.0
 *************************/
(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define(['exports'], factory);
    } else if (typeof exports === 'object' && typeof exports.nodeName !== 'string') {
        // CommonJS
        factory(exports);
    } else {
        // Browser globals
        factory((root.commonJsStrict = {}));
    }
}(this,
    function (exports) {

        /* Polyfills */
        if (typeof Promise !== 'function') {
            /*! promise-polyfill 3.1.0 */
            !function (a) {
                function b(a, b) {
                    return function () {
                        a.apply(b, arguments)
                    }
                }

                function c(a) {
                    if ("object" != typeof this) throw new TypeError("Promises must be constructed via new");
                    if ("function" != typeof a) throw new TypeError("not a function");
                    this._state = null, this._value = null, this._deferreds = [], i(a, b(e, this), b(f, this))
                }

                function d(a) {
                    var b = this;
                    return null === this._state
                        ? void this._deferreds.push(a)
                        : void k(function () {
                            var c = b._state ? a.onFulfilled : a.onRejected;
                            if (null === c) return void (b._state ? a.resolve : a.reject)(b._value);
                            var d;
                            try {
                                d = c(b._value)
                            } catch (e) {
                                return void a.reject(e)
                            }
                            a.resolve(d)
                        })
                }

                function e(a) {
                    try {
                        if (a === this) throw new TypeError("A promise cannot be resolved with itself.");
                        if (a && ("object" == typeof a || "function" == typeof a)) {
                            var c = a.then;
                            if ("function" == typeof c) return void i(b(c, a), b(e, this), b(f, this))
                        }
                        this._state = !0, this._value = a, g.call(this)
                    } catch (d) {
                        f.call(this, d)
                    }
                }

                function f(a) {
                    this._state = !1, this._value = a, g.call(this)
                }

                function g() {
                    for (var a = 0, b = this._deferreds.length; b > a; a++) d.call(this, this._deferreds[a]);
                    this._deferreds = null
                }

                function h(a, b, c, d) {
                    this.onFulfilled = "function" == typeof a ? a : null, this.onRejected =
                        "function" == typeof b ? b : null, this.resolve = c, this.reject = d
                }

                function i(a, b, c) {
                    var d = !1;
                    try {
                        a(function (a) {
                            d || (d = !0, b(a))
                        },
                            function (a) {
                                d || (d = !0, c(a))
                            })
                    } catch (e) {
                        if (d) return;
                        d = !0, c(e)
                    }
                }

                var j = setTimeout,
                    k = "function" == typeof setImmediate && setImmediate ||
                        function (a) {
                            j(a, 1)
                        },
                    l = Array.isArray ||
                        function (a) {
                            return "[object Array]" === Object.prototype.toString.call(a)
                        };
                c.prototype["catch"] = function (a) {
                    return this.then(null, a)
                }, c.prototype.then = function (a, b) {
                    var e = this;
                    return new c(function (c, f) {
                        d.call(e, new h(a, b, c, f))
                    })
                }, c.all = function () {
                    var a = Array.prototype.slice.call(1 === arguments.length && l(arguments[0])
                        ? arguments[0]
                        : arguments);
                    return new c(function (b, c) {
                        function d(f, g) {
                            try {
                                if (g && ("object" == typeof g || "function" == typeof g)) {
                                    var h = g.then;
                                    if ("function" == typeof h)
                                        return void h.call(g,
                                            function (a) {
                                                d(f, a)
                                            },
                                            c)
                                }
                                a[f] = g, 0 === --e && b(a)
                            } catch (i) {
                                c(i)
                            }
                        }

                        if (0 === a.length) return b([]);
                        for (var e = a.length, f = 0; f < a.length; f++) d(f, a[f])
                    })
                }, c.resolve = function (a) {
                    return a && "object" == typeof a && a.constructor === c
                        ? a
                        : new c(function (b) {
                            b(a)
                        })
                }, c.reject = function (a) {
                    return new c(function (b, c) {
                        c(a)
                    })
                }, c.race = function (a) {
                    return new c(function (b, c) {
                        for (var d = 0, e = a.length; e > d; d++) a[d].then(b, c)
                    })
                }, c._setImmediateFn = function (a) {
                    k = a
                }, "undefined" != typeof module && module.exports ? module.exports = c : a.Promise || (a.Promise = c)
            }(this);
        }

        if (typeof window.CustomEvent !== "function") {
            (function () {
                function CustomEvent(event, params) {
                    params = params || { bubbles: false, cancelable: false, detail: undefined };
                    var evt = document.createEvent('CustomEvent');
                    evt.initCustomEvent(event, params.bubbles, params.cancelable, params.detail);
                    return evt;
                }

                CustomEvent.prototype = window.Event.prototype;
                window.CustomEvent = CustomEvent;
            }());
        }

        if (!HTMLCanvasElement.prototype.toBlob) {
            Object.defineProperty(HTMLCanvasElement.prototype,
                'toBlob',
                {
                    value: function (callback, type, quality) {
                        var binStr = atob(this.toDataURL(type, quality).split(',')[1]),
                            len = binStr.length,
                            arr = new Uint8Array(len);

                        for (var i = 0; i < len; i++) {
                            arr[i] = binStr.charCodeAt(i);
                        }

                        callback(new Blob([arr], { type: type || 'image/png' }));
                    }
                });
        }
        /* End Polyfills */

        var cssPrefixes = ['Webkit', 'Moz', 'ms'],
            emptyStyles = document.createElement('div').style,
            CSS_TRANS_ORG,
            CSS_TRANSFORM,
            CSS_USERSELECT;

        function vendorPrefix(prop) {
            if (prop in emptyStyles) {
                return prop;
            }

            var capProp = prop[0].toUpperCase() + prop.slice(1), i = cssPrefixes.length;

            while (i--) {
                prop = cssPrefixes[i] + capProp;
                if (prop in emptyStyles) {
                    return prop;
                }
            }
        }

        CSS_TRANSFORM = vendorPrefix('transform');
        CSS_TRANS_ORG = vendorPrefix('transformOrigin');
        CSS_USERSELECT = vendorPrefix('userSelect');

        // Credits to : Andrew Dupont - http://andrewdupont.net/2009/08/28/deep-extending-objects-in-javascript/
        function deepExtend(destination, source) {
            destination = destination || {};
            for (var property in source) {
                if (source[property] && source[property].constructor && source[property].constructor === Object) {
                    destination[property] = destination[property] || {};
                    deepExtend(destination[property], source[property]);
                } else {
                    destination[property] = source[property];
                }
            }
            return destination;
        }

        function debounce(func, wait, immediate) {
            var timeout;
            return function () {
                var context = this, args = arguments;
                var later = function () {
                    timeout = null;
                    if (!immediate) func.apply(context, args);
                };
                var callNow = immediate && !timeout;
                clearTimeout(timeout);
                timeout = setTimeout(later, wait);
                if (callNow) func.apply(context, args);
            };
        }

        function dispatchChange(element) {
            if ("createEvent" in document) {
                var evt = document.createEvent("HTMLEvents");
                evt.initEvent("change", false, true);
                element.dispatchEvent(evt);
            } else {
                element.fireEvent("onchange");
            }
        }

        //http://jsperf.com/vanilla-css
        function css(el, styles, val) {
            if (typeof (styles) === 'string') {
                var tmp = styles;
                styles = {};
                styles[tmp] = val;
            }

            for (var prop in styles) {
                el.style[prop] = styles[prop];
            }
        }

        function addClass(el, c) {
            if (el.classList) {
                el.classList.add(c);
            } else {
                el.className += ' ' + c;
            }
        }

        function removeClass(el, c) {
            if (el.classList) {
                el.classList.remove(c);
            } else {
                el.className = el.className.replace(c, '');
            }
        }

        function num(v) {
            return parseInt(v, 10);
        }

        /* Utilities */
        function loadImage(src, imageEl, doExif) {
            var img = imageEl || new Image();
            img.style.opacity = 0;

            return new Promise(function (resolve) {
                function _resolve() {
                    setTimeout(function () {
                        resolve(img);
                    },
                        1);
                }

                if (img.src === src) { // If image source hasn't changed resolve immediately
                    _resolve();
                    return;
                }

                img.exifdata = null;
                img.removeAttribute('crossOrigin');
                if (src.match(/^https?:\/\/|^\/\//)) {
                    img.setAttribute('crossOrigin', 'anonymous');
                }
                img.onload = function () {
                    if (doExif) {
                        EXIF.getData(img,
                            function () {
                                _resolve();
                            });
                    } else {
                        _resolve();
                    }
                };
                img.src = src;
            });
        }

        function naturalImageDimensions(img) {
            var w = img.naturalWidth;
            var h = img.naturalHeight;
            if (img.exifdata && img.exifdata.Orientation >= 5) {
                var x = w;
                w = h;
                h = x;
            }
            return { width: w, height: h };
        }

        /* CSS Transform Prototype */
        var TRANSLATE_OPTS = {
            'translate3d': {
                suffix: ', 0px'
            },
            'translate': {
                suffix: ''
            }
        };
        var Transform = function (x, y, scale) {
            this.x = parseFloat(x);
            this.y = parseFloat(y);
            this.scale = parseFloat(scale);
        };

        Transform.parse = function (v) {
            if (v.style) {
                return Transform.parse(v.style[CSS_TRANSFORM]);
            } else if (v.indexOf('matrix') > -1 || v.indexOf('none') > -1) {
                return Transform.fromMatrix(v);
            } else {
                return Transform.fromString(v);
            }
        };

        Transform.fromMatrix = function (v) {
            var vals = v.substring(7).split(',');
            if (!vals.length || v === 'none') {
                vals = [1, 0, 0, 1, 0, 0];
            }

            return new Transform(num(vals[4]), num(vals[5]), parseFloat(vals[0]));
        };

        Transform.fromString = function (v) {
            var values = v.split(') '),
                translate = values[0].substring(Croppie.globals.translate.length + 1).split(','),
                scale = values.length > 1 ? values[1].substring(6) : 1,
                x = translate.length > 1 ? translate[0] : 0,
                y = translate.length > 1 ? translate[1] : 0;

            return new Transform(x, y, scale);
        };

        Transform.prototype.toString = function () {
            var suffix = TRANSLATE_OPTS[Croppie.globals.translate].suffix || '';
            return Croppie.globals.translate +
                '(' +
                this.x +
                'px, ' +
                this.y +
                'px' +
                suffix +
                ') scale(' +
                this.scale +
                ')';
        };

        var TransformOrigin = function (el) {
            if (!el || !el.style[CSS_TRANS_ORG]) {
                this.x = 0;
                this.y = 0;
                return;
            }
            var css = el.style[CSS_TRANS_ORG].split(' ');
            this.x = parseFloat(css[0]);
            this.y = parseFloat(css[1]);
        };

        TransformOrigin.prototype.toString = function () {
            return this.x + 'px ' + this.y + 'px';
        };

        function getExifOrientation(img) {
            return img.exifdata.Orientation;
        }

        function drawCanvas(canvas, img, orientation) {
            var width = img.width, height = img.height, ctx = canvas.getContext('2d');

            canvas.width = img.width;
            canvas.height = img.height;

            ctx.save();
            switch (orientation) {
                case 2:
                    ctx.translate(width, 0);
                    ctx.scale(-1, 1);
                    break;

                case 3:
                    ctx.translate(width, height);
                    ctx.rotate(180 * Math.PI / 180);
                    break;

                case 4:
                    ctx.translate(0, height);
                    ctx.scale(1, -1);
                    break;

                case 5:
                    canvas.width = height;
                    canvas.height = width;
                    ctx.rotate(90 * Math.PI / 180);
                    ctx.scale(1, -1);
                    break;

                case 6:
                    canvas.width = height;
                    canvas.height = width;
                    ctx.rotate(90 * Math.PI / 180);
                    ctx.translate(0, -height);
                    break;

                case 7:
                    canvas.width = height;
                    canvas.height = width;
                    ctx.rotate(-90 * Math.PI / 180);
                    ctx.translate(-width, height);
                    ctx.scale(1, -1);
                    break;

                case 8:
                    canvas.width = height;
                    canvas.height = width;
                    ctx.translate(0, width);
                    ctx.rotate(-90 * Math.PI / 180);
                    break;
            }
            ctx.drawImage(img, 0, 0, width, height);
            ctx.restore();
        }

        /* Private Methods */
        function _create() {
            var self = this,
                contClass = 'croppie-container',
                customViewportClass = self.options.viewport.type ? 'cr-vp-' + self.options.viewport.type : null,
                boundary,
                img,
                viewport,
                overlay,
                bw,
                bh;

            self.options.useCanvas = self.options.enableOrientation || _hasExif.call(self);
            // Properties on class
            self.data = {};
            self.elements = {};

            boundary = self.elements.boundary = document.createElement('div');
            viewport = self.elements.viewport = document.createElement('div');
            img = self.elements.img = document.createElement('img');
            overlay = self.elements.overlay = document.createElement('div');

            if (self.options.useCanvas) {
                self.elements.canvas = document.createElement('canvas');
                self.elements.preview = self.elements.canvas;
            } else {
                self.elements.preview = self.elements.img;
            }

            addClass(boundary, 'cr-boundary');
            bw = self.options.boundary.width;
            bh = self.options.boundary.height;
            css(boundary,
                {
                    width: (bw + (isNaN(bw) ? '' : 'px')),
                    height: (bh + (isNaN(bh) ? '' : 'px'))
                });

            addClass(viewport, 'cr-viewport');
            if (customViewportClass) {
                addClass(viewport, customViewportClass);
            }
            css(viewport,
                {
                    width: self.options.viewport.width + 'px',
                    height: self.options.viewport.height + 'px'
                });
            viewport.setAttribute('tabindex', 0);

            addClass(self.elements.preview, 'cr-image');
            addClass(overlay, 'cr-overlay');

            self.element.appendChild(boundary);
            boundary.appendChild(self.elements.preview);
            boundary.appendChild(viewport);
            boundary.appendChild(overlay);

            addClass(self.element, contClass);
            if (self.options.customClass) {
                addClass(self.element, self.options.customClass);
            }

            _initDraggable.call(this);

            if (self.options.enableZoom) {
                _initializeZoom.call(self);
            }

            // if (self.options.enableOrientation) {
            //     _initRotationControls.call(self);
            // }

            if (self.options.enableResize) {
                _initializeResize.call(self);
            }
        }

        // function _initRotationControls () {
        //     var self = this,
        //         wrap, btnLeft, btnRight, iLeft, iRight;

        //     wrap = document.createElement('div');
        //     self.elements.orientationBtnLeft = btnLeft = document.createElement('button');
        //     self.elements.orientationBtnRight = btnRight = document.createElement('button');

        //     wrap.appendChild(btnLeft);
        //     wrap.appendChild(btnRight);

        //     iLeft = document.createElement('i');
        //     iRight = document.createElement('i');
        //     btnLeft.appendChild(iLeft);
        //     btnRight.appendChild(iRight);

        //     addClass(wrap, 'cr-rotate-controls');
        //     addClass(btnLeft, 'cr-rotate-l');
        //     addClass(btnRight, 'cr-rotate-r');

        //     self.elements.boundary.appendChild(wrap);

        //     btnLeft.addEventListener('click', function () {
        //         self.rotate(-90);
        //     });
        //     btnRight.addEventListener('click', function () {
        //         self.rotate(90);
        //     });
        // }

        function _hasExif() {
            return this.options.enableExif && window.EXIF;
        }

        function _initializeResize() {
            var self = this;
            var wrap = document.createElement('div');
            var isDragging = false;
            var direction;
            var originalX;
            var originalY;
            var minSize = 50;
            var maxWidth;
            var maxHeight;
            var vr;
            var hr;

            addClass(wrap, 'cr-resizer');
            css(wrap,
                {
                    width: this.options.viewport.width + 'px',
                    height: this.options.viewport.height + 'px'
                });

            if (this.options.resizeControls.height) {
                vr = document.createElement('div');
                addClass(vr, 'cr-resizer-vertical');
                wrap.appendChild(vr);
            }

            if (this.options.resizeControls.width) {
                hr = document.createElement('div');
                addClass(hr, 'cr-resizer-horisontal');
                wrap.appendChild(hr);
            }

            function mouseDown(ev) {
                if (ev.button !== undefined && ev.button !== 0) return;

                ev.preventDefault();
                if (isDragging) {
                    return;
                }

                var overlayRect = self.elements.overlay.getBoundingClientRect();

                isDragging = true;
                originalX = ev.pageX;
                originalY = ev.pageY;
                direction = ev.currentTarget.className.indexOf('vertical') !== -1 ? 'v' : 'h';
                maxWidth = overlayRect.width;
                maxHeight = overlayRect.height;

                if (ev.touches) {
                    var touches = ev.touches[0];
                    originalX = touches.pageX;
                    originalY = touches.pageY;
                }

                window.addEventListener('mousemove', mouseMove);
                window.addEventListener('touchmove', mouseMove);
                window.addEventListener('mouseup', mouseUp);
                window.addEventListener('touchend', mouseUp);
                document.body.style[CSS_USERSELECT] = 'none';
            }

            function mouseMove(ev) {
                var pageX = ev.pageX;
                var pageY = ev.pageY;

                ev.preventDefault();

                if (ev.touches) {
                    var touches = ev.touches[0];
                    pageX = touches.pageX;
                    pageY = touches.pageY;
                }

                var deltaX = pageX - originalX;
                var deltaY = pageY - originalY;
                var newHeight = self.options.viewport.height + deltaY;
                var newWidth = self.options.viewport.width + deltaX;

                if (direction === 'v' && newHeight >= minSize && newHeight <= maxHeight) {
                    css(wrap,
                        {
                            height: newHeight + 'px'
                        });

                    self.options.boundary.height += deltaY;
                    css(self.elements.boundary,
                        {
                            height: self.options.boundary.height + 'px'
                        });

                    self.options.viewport.height += deltaY;
                    css(self.elements.viewport,
                        {
                            height: self.options.viewport.height + 'px'
                        });
                } else if (direction === 'h' && newWidth >= minSize && newWidth <= maxWidth) {
                    css(wrap,
                        {
                            width: newWidth + 'px'
                        });

                    self.options.boundary.width += deltaX;
                    css(self.elements.boundary,
                        {
                            width: self.options.boundary.width + 'px'
                        });

                    self.options.viewport.width += deltaX;
                    css(self.elements.viewport,
                        {
                            width: self.options.viewport.width + 'px'
                        });
                }

                _updateOverlay.call(self);
                _updateZoomLimits.call(self);
                _updateCenterPoint.call(self);
                _triggerUpdate.call(self);
                originalY = pageY;
                originalX = pageX;
            }

            function mouseUp() {
                isDragging = false;
                window.removeEventListener('mousemove', mouseMove);
                window.removeEventListener('touchmove', mouseMove);
                window.removeEventListener('mouseup', mouseUp);
                window.removeEventListener('touchend', mouseUp);
                document.body.style[CSS_USERSELECT] = '';
            }

            if (vr) {
                vr.addEventListener('mousedown', mouseDown);
            }

            if (hr) {
                hr.addEventListener('mousedown', mouseDown);
            }

            this.elements.boundary.appendChild(wrap);
        }

        function _setZoomerVal(v) {
            if (this.options.enableZoom) {
                var z = this.elements.zoomer, val = fix(v, 4);

                z.value = Math.max(z.min, Math.min(z.max, val));
            }
        }

        function _initializeZoom() {
            var self = this,
                wrap = self.elements.zoomerWrap = document.createElement('div'),
                zoomer = self.elements.zoomer = document.createElement('input');

            addClass(wrap, 'cr-slider-wrap');
            addClass(zoomer, 'cr-slider');
            zoomer.type = 'range';
            zoomer.step = '0.0001';
            zoomer.value = 1;
            zoomer.style.display = self.options.showZoomer ? '' : 'none';

            self.element.appendChild(wrap);
            wrap.appendChild(zoomer);

            self._currentZoom = 1;

            function change() {
                _onZoom.call(self,
                    {
                        value: parseFloat(zoomer.value),
                        origin: new TransformOrigin(self.elements.preview),
                        viewportRect: self.elements.viewport.getBoundingClientRect(),
                        transform: Transform.parse(self.elements.preview)
                    });
            }

            function scroll(ev) {
                var delta, targetZoom;

                if (ev.wheelDelta) {
                    delta = ev.wheelDelta / 1200; //wheelDelta min: -120 max: 120 // max x 10 x 2
                } else if (ev.deltaY) {
                    delta = ev.deltaY / 1060; //deltaY min: -53 max: 53 // max x 10 x 2
                } else if (ev.detail) {
                    delta = ev.detail / -60; //delta min: -3 max: 3 // max x 10 x 2
                } else {
                    delta = 0;
                }

                targetZoom = self._currentZoom + (delta * self._currentZoom);

                ev.preventDefault();
                _setZoomerVal.call(self, targetZoom);
                change.call(self);
            }

            self.elements.zoomer.addEventListener('input', change); // this is being fired twice on keypress
            self.elements.zoomer.addEventListener('change', change);

            if (self.options.mouseWheelZoom) {
                self.elements.boundary.addEventListener('mousewheel', scroll);
                self.elements.boundary.addEventListener('DOMMouseScroll', scroll);
            }
        }

        function _onZoom(ui) {
            var self = this,
                transform = ui ? ui.transform : Transform.parse(self.elements.preview),
                vpRect = ui ? ui.viewportRect : self.elements.viewport.getBoundingClientRect(),
                origin = ui ? ui.origin : new TransformOrigin(self.elements.preview);

            function applyCss() {
                var transCss = {};
                transCss[CSS_TRANSFORM] = transform.toString();
                transCss[CSS_TRANS_ORG] = origin.toString();
                css(self.elements.preview, transCss);
            }

            self._currentZoom = ui ? ui.value : self._currentZoom;
            transform.scale = self._currentZoom;
            applyCss();

            if (self.options.enforceBoundary) {
                var boundaries = _getVirtualBoundaries.call(self, vpRect),
                    transBoundaries = boundaries.translate,
                    oBoundaries = boundaries.origin;

                if (transform.x >= transBoundaries.maxX) {
                    origin.x = oBoundaries.minX;
                    transform.x = transBoundaries.maxX;
                }

                if (transform.x <= transBoundaries.minX) {
                    origin.x = oBoundaries.maxX;
                    transform.x = transBoundaries.minX;
                }

                if (transform.y >= transBoundaries.maxY) {
                    origin.y = oBoundaries.minY;
                    transform.y = transBoundaries.maxY;
                }

                if (transform.y <= transBoundaries.minY) {
                    origin.y = oBoundaries.maxY;
                    transform.y = transBoundaries.minY;
                }
            }
            applyCss();
            _debouncedOverlay.call(self);
            _triggerUpdate.call(self);
        }

        function _getVirtualBoundaries(viewport) {
            var self = this,
                scale = self._currentZoom,
                vpWidth = viewport.width,
                vpHeight = viewport.height,
                centerFromBoundaryX = self.elements.boundary.clientWidth / 2,
                centerFromBoundaryY = self.elements.boundary.clientHeight / 2,
                imgRect = self.elements.preview.getBoundingClientRect(),
                curImgWidth = imgRect.width,
                curImgHeight = imgRect.height,
                halfWidth = vpWidth / 2,
                halfHeight = vpHeight / 2;

            var maxX = ((halfWidth / scale) - centerFromBoundaryX) * -1;
            var minX = maxX - ((curImgWidth * (1 / scale)) - (vpWidth * (1 / scale)));

            var maxY = ((halfHeight / scale) - centerFromBoundaryY) * -1;
            var minY = maxY - ((curImgHeight * (1 / scale)) - (vpHeight * (1 / scale)));

            var originMinX = (1 / scale) * halfWidth;
            var originMaxX = (curImgWidth * (1 / scale)) - originMinX;

            var originMinY = (1 / scale) * halfHeight;
            var originMaxY = (curImgHeight * (1 / scale)) - originMinY;

            return {
                translate: {
                    maxX: maxX,
                    minX: minX,
                    maxY: maxY,
                    minY: minY
                },
                origin: {
                    maxX: originMaxX,
                    minX: originMinX,
                    maxY: originMaxY,
                    minY: originMinY
                }
            };
        }

        function _updateCenterPoint() {
            var self = this,
                scale = self._currentZoom,
                data = self.elements.preview.getBoundingClientRect(),
                vpData = self.elements.viewport.getBoundingClientRect(),
                transform = Transform.parse(self.elements.preview.style[CSS_TRANSFORM]),
                pc = new TransformOrigin(self.elements.preview),
                top = (vpData.top - data.top) + (vpData.height / 2),
                left = (vpData.left - data.left) + (vpData.width / 2),
                center = {},
                adj = {};

            center.y = top / scale;
            center.x = left / scale;

            adj.y = (center.y - pc.y) * (1 - scale);
            adj.x = (center.x - pc.x) * (1 - scale);

            transform.x -= adj.x;
            transform.y -= adj.y;

            var newCss = {};
            newCss[CSS_TRANS_ORG] = center.x + 'px ' + center.y + 'px';
            newCss[CSS_TRANSFORM] = transform.toString();
            css(self.elements.preview, newCss);
        }

        function _initDraggable() {
            var self = this, isDragging = false, originalX, originalY, originalDistance, vpRect, transform;

            function assignTransformCoordinates(deltaX, deltaY) {
                var imgRect = self.elements.preview.getBoundingClientRect(),
                    top = transform.y + deltaY,
                    left = transform.x + deltaX;

                if (self.options.enforceBoundary) {
                    if (vpRect.top > imgRect.top + deltaY && vpRect.bottom < imgRect.bottom + deltaY) {
                        transform.y = top;
                    }

                    if (vpRect.left > imgRect.left + deltaX && vpRect.right < imgRect.right + deltaX) {
                        transform.x = left;
                    }
                } else {
                    transform.y = top;
                    transform.x = left;
                }
            }

            function keyDown(ev) {
                var LEFT_ARROW = 37, UP_ARROW = 38, RIGHT_ARROW = 39, DOWN_ARROW = 40;

                if (ev.shiftKey && (ev.keyCode == UP_ARROW || ev.keyCode == DOWN_ARROW)) {
                    var zoom = 0.0;
                    if (ev.keyCode == UP_ARROW) {
                        zoom = parseFloat(self.elements.zoomer.value, 10) + parseFloat(self.elements.zoomer.step, 10)
                    } else {
                        zoom = parseFloat(self.elements.zoomer.value, 10) - parseFloat(self.elements.zoomer.step, 10)
                    }
                    self.setZoom(zoom);
                } else if (self.options.enableKeyMovement && (ev.keyCode >= 37 && ev.keyCode <= 40)) {
                    ev.preventDefault();
                    var movement = parseKeyDown(ev.keyCode);

                    transform = Transform.parse(self.elements.preview);
                    document.body.style[CSS_USERSELECT] = 'none';
                    vpRect = self.elements.viewport.getBoundingClientRect();
                    keyMove(movement);
                };

                function parseKeyDown(key) {
                    switch (key) {
                        case LEFT_ARROW:
                            return [1, 0];
                        case UP_ARROW:
                            return [0, 1];
                        case RIGHT_ARROW:
                            return [-1, 0];
                        case DOWN_ARROW:
                            return [0, -1];
                    };
                };
            }

            function keyMove(movement) {
                var deltaX = movement[0], deltaY = movement[1], newCss = {};

                assignTransformCoordinates(deltaX, deltaY);

                newCss[CSS_TRANSFORM] = transform.toString();
                css(self.elements.preview, newCss);
                _updateOverlay.call(self);
                document.body.style[CSS_USERSELECT] = '';
                _updateCenterPoint.call(self);
                _triggerUpdate.call(self);
                originalDistance = 0;
            }

            function mouseDown(ev) {
                if (ev.button !== undefined && ev.button !== 0) return;

                ev.preventDefault();
                if (isDragging) return;
                isDragging = true;
                originalX = ev.pageX;
                originalY = ev.pageY;

                if (ev.touches) {
                    var touches = ev.touches[0];
                    originalX = touches.pageX;
                    originalY = touches.pageY;
                }

                transform = Transform.parse(self.elements.preview);
                window.addEventListener('mousemove', mouseMove);
                window.addEventListener('touchmove', mouseMove);
                window.addEventListener('mouseup', mouseUp);
                window.addEventListener('touchend', mouseUp);
                document.body.style[CSS_USERSELECT] = 'none';
                vpRect = self.elements.viewport.getBoundingClientRect();
            }

            function mouseMove(ev) {
                ev.preventDefault();
                var pageX = ev.pageX, pageY = ev.pageY;

                if (ev.touches) {
                    var touches = ev.touches[0];
                    pageX = touches.pageX;
                    pageY = touches.pageY;
                }

                var deltaX = pageX - originalX, deltaY = pageY - originalY, newCss = {};

                if (ev.type == 'touchmove') {
                    if (ev.touches.length > 1) {
                        var touch1 = ev.touches[0];
                        var touch2 = ev.touches[1];
                        var dist = Math.sqrt((touch1.pageX - touch2.pageX) * (touch1.pageX - touch2.pageX) +
                            (touch1.pageY - touch2.pageY) * (touch1.pageY - touch2.pageY));

                        if (!originalDistance) {
                            originalDistance = dist / self._currentZoom;
                        }

                        var scale = dist / originalDistance;

                        _setZoomerVal.call(self, scale);
                        dispatchChange(self.elements.zoomer);
                        return;
                    }
                }

                assignTransformCoordinates(deltaX, deltaY);

                newCss[CSS_TRANSFORM] = transform.toString();
                css(self.elements.preview, newCss);
                _updateOverlay.call(self);
                originalY = pageY;
                originalX = pageX;
            }

            function mouseUp() {
                isDragging = false;
                window.removeEventListener('mousemove', mouseMove);
                window.removeEventListener('touchmove', mouseMove);
                window.removeEventListener('mouseup', mouseUp);
                window.removeEventListener('touchend', mouseUp);
                document.body.style[CSS_USERSELECT] = '';
                _updateCenterPoint.call(self);
                _triggerUpdate.call(self);
                originalDistance = 0;
            }

            self.elements.overlay.addEventListener('mousedown', mouseDown);
            self.elements.viewport.addEventListener('keydown', keyDown);
            self.elements.overlay.addEventListener('touchstart', mouseDown);
        }

        function _updateOverlay() {
            var self = this,
                boundRect = self.elements.boundary.getBoundingClientRect(),
                imgData = self.elements.preview.getBoundingClientRect();

            css(self.elements.overlay,
                {
                    width: imgData.width + 'px',
                    height: imgData.height + 'px',
                    top: (imgData.top - boundRect.top) + 'px',
                    left: (imgData.left - boundRect.left) + 'px'
                });
        }

        var _debouncedOverlay = debounce(_updateOverlay, 500);

        function _triggerUpdate() {
            var self = this, data = self.get(), ev;

            if (!_isVisible.call(self)) {
                return;
            }

            self.options.update.call(self, data);
            if (self.$ && typeof Prototype == 'undefined') {
                self.$(self.element).trigger('update', data);
            } else {
                var ev;
                if (window.CustomEvent) {
                    ev = new CustomEvent('update', { detail: data });
                } else {
                    ev = document.createEvent('CustomEvent');
                    ev.initCustomEvent('update', true, true, data);
                }

                self.element.dispatchEvent(ev);
            }
        }

        function _isVisible() {
            return this.elements.preview.offsetHeight > 0 && this.elements.preview.offsetWidth > 0;
        }

        function _updatePropertiesFromImage() {
            var self = this,
                initialZoom = 1,
                cssReset = {},
                img = self.elements.preview,
                imgData = self.elements.preview.getBoundingClientRect(),
                transformReset = new Transform(0, 0, initialZoom),
                originReset = new TransformOrigin(),
                isVisible = _isVisible.call(self);

            if (!isVisible || self.data.bound) {
                // if the croppie isn't visible or it doesn't need binding
                return;
            }

            self.data.bound = true;
            cssReset[CSS_TRANSFORM] = transformReset.toString();
            cssReset[CSS_TRANS_ORG] = originReset.toString();
            cssReset['opacity'] = 1;
            css(img, cssReset);

            self._originalImageWidth = imgData.width;
            self._originalImageHeight = imgData.height;

            if (self.options.enableZoom) {
                _updateZoomLimits.call(self, true);
            } else {
                self._currentZoom = initialZoom;
            }

            transformReset.scale = self._currentZoom;
            cssReset[CSS_TRANSFORM] = transformReset.toString();
            css(img, cssReset);

            if (self.data.points.length) {
                _bindPoints.call(self, self.data.points);
            } else {
                _centerImage.call(self);
            }

            _updateCenterPoint.call(self);
            _updateOverlay.call(self);
        }

        function _updateZoomLimits(initial) {
            var self = this,
                minZoom = 0,
                maxZoom = 1.5,
                initialZoom,
                defaultInitialZoom,
                zoomer = self.elements.zoomer,
                scale = parseFloat(zoomer.value),
                boundaryData = self.elements.boundary.getBoundingClientRect(),
                imgData = self.elements.preview.getBoundingClientRect(),
                vpData = self.elements.viewport.getBoundingClientRect(),
                minW,
                minH;

            if (self.options.enforceBoundary) {
                minW = vpData.width / (initial ? imgData.width : imgData.width / scale);
                minH = vpData.height / (initial ? imgData.height : imgData.height / scale);
                minZoom = Math.max(minW, minH);
            }

            if (minZoom >= maxZoom) {
                maxZoom = minZoom + 1;
            }

            zoomer.min = fix(minZoom, 4);
            zoomer.max = fix(maxZoom, 4);

            if (initial) {
                defaultInitialZoom =
                    Math.max((boundaryData.width / imgData.width), (boundaryData.height / imgData.height));
                initialZoom = self.data.boundZoom !== null ? self.data.boundZoom : defaultInitialZoom;
                _setZoomerVal.call(self, initialZoom);
            }

            dispatchChange(zoomer);
        }

        function _bindPoints(points) {
            if (points.length != 4) {
                throw "Croppie - Invalid number of points supplied: " + points;
            }
            var self = this,
                pointsWidth = points[2] - points[0], // pointsHeight = points[3] - points[1],
                vpData = self.elements.viewport.getBoundingClientRect(),
                boundRect = self.elements.boundary.getBoundingClientRect(),
                vpOffset = {
                    left: vpData.left - boundRect.left,
                    top: vpData.top - boundRect.top
                },
                scale = vpData.width / pointsWidth,
                originTop = points[1],
                originLeft = points[0],
                transformTop = (-1 * points[1]) + vpOffset.top,
                transformLeft = (-1 * points[0]) + vpOffset.left,
                newCss = {};

            newCss[CSS_TRANS_ORG] = originLeft + 'px ' + originTop + 'px';
            newCss[CSS_TRANSFORM] = new Transform(transformLeft, transformTop, scale).toString();
            css(self.elements.preview, newCss);

            _setZoomerVal.call(self, scale);
            self._currentZoom = scale;
        }

        function _centerImage() {
            var self = this,
                imgDim = self.elements.preview.getBoundingClientRect(),
                vpDim = self.elements.viewport.getBoundingClientRect(),
                boundDim = self.elements.boundary.getBoundingClientRect(),
                vpLeft = vpDim.left - boundDim.left,
                vpTop = vpDim.top - boundDim.top,
                w = vpLeft - ((imgDim.width - vpDim.width) / 2),
                h = vpTop - ((imgDim.height - vpDim.height) / 2),
                transform = new Transform(w, h, self._currentZoom);

            css(self.elements.preview, CSS_TRANSFORM, transform.toString());
        }

        function _transferImageToCanvas(customOrientation) {
            var self = this,
                canvas = self.elements.canvas,
                img = self.elements.img,
                ctx = canvas.getContext('2d'),
                exif = _hasExif.call(self),
                customOrientation = self.options.enableOrientation && customOrientation;

            ctx.clearRect(0, 0, canvas.width, canvas.height);
            canvas.width = img.width;
            canvas.height = img.height;

            if (exif && !customOrientation) {
                var orientation = getExifOrientation(img);
                drawCanvas(canvas, img, num(orientation || 0, 10));
            } else if (customOrientation) {
                drawCanvas(canvas, img, customOrientation);
            }
        }

        function _getCanvas(data) {
            var self = this,
                points = data.points,
                left = num(points[0]),
                top = num(points[1]),
                right = num(points[2]),
                bottom = num(points[3]),
                width = right - left,
                height = bottom - top,
                circle = data.circle,
                canvas = document.createElement('canvas'),
                ctx = canvas.getContext('2d'),
                outWidth = width,
                outHeight = height,
                startX = 0,
                startY = 0,
                canvasWidth = outWidth,
                canvasHeight = outHeight,
                customDimensions = (data.outputWidth && data.outputHeight),
                outputRatio = 1;

            if (customDimensions) {
                canvasWidth = data.outputWidth;
                canvasHeight = data.outputHeight;
                outputRatio = canvasWidth / outWidth;
            }

            canvas.width = canvasWidth;
            canvas.height = canvasHeight;

            if (data.backgroundColor) {
                ctx.fillStyle = data.backgroundColor;
                ctx.fillRect(0, 0, outWidth, outHeight);
            }

            // start fixing data to send to draw image for enforceBoundary: false
            if (!self.options.enforceBoundary) {
                if (left < 0) {
                    startX = Math.abs(left);
                    left = 0;
                }
                if (top < 0) {
                    startY = Math.abs(top);
                    top = 0;
                }
                if (right > self._originalImageWidth) {
                    width = self._originalImageWidth - left;
                    outWidth = width;
                }
                if (bottom > self._originalImageHeight) {
                    height = self._originalImageHeight - top;
                    outHeight = height;
                }
            }

            if (outputRatio !== 1) {
                startX *= outputRatio;
                startY *= outputRatio;
                outWidth *= outputRatio;
                outHeight *= outputRatio;
            }

            ctx.drawImage(this.elements.preview,
                left,
                top,
                Math.min(width, self._originalImageWidth),
                Math.min(height, self._originalImageHeight),
                startX,
                startY,
                outWidth,
                outHeight);
            if (circle) {
                ctx.fillStyle = '#fff';
                ctx.globalCompositeOperation = 'destination-in';
                ctx.beginPath();
                ctx.arc(outWidth / 2, outHeight / 2, outWidth / 2, 0, Math.PI * 2, true);
                ctx.closePath();
                ctx.fill();
            }
            return canvas;
        }

        function _getHtmlResult(data) {
            var points = data.points,
                div = document.createElement('div'),
                img = document.createElement('img'),
                width = points[2] - points[0],
                height = points[3] - points[1];

            addClass(div, 'croppie-result');
            div.appendChild(img);
            css(img,
                {
                    left: (-1 * points[0]) + 'px',
                    top: (-1 * points[1]) + 'px'
                });
            img.src = data.url;
            css(div,
                {
                    width: width + 'px',
                    height: height + 'px'
                });

            return div;
        }

        function _getBase64Result(data) {
            return _getCanvas.call(this, data).toDataURL(data.format, data.quality);
        }

        function _getBlobResult(data) {
            var self = this;
            return new Promise(function (resolve, reject) {
                _getCanvas.call(self, data).toBlob(function (blob) {
                    resolve(blob);
                },
                    data.format,
                    data.quality);
            });
        }

        function _bind(options, cb) {
            var self = this, url, points = [], zoom = null, hasExif = _hasExif.call(self);;

            if (typeof (options) === 'string') {
                url = options;
                options = {};
            } else if (Array.isArray(options)) {
                points = options.slice();
            } else if (typeof (options) == 'undefined' && self.data.url) { //refreshing
                _updatePropertiesFromImage.call(self);
                _triggerUpdate.call(self);
                return null;
            } else {
                url = options.url;
                points = options.points || [];
                zoom = typeof (options.zoom) === 'undefined' ? null : options.zoom;
            }

            self.data.bound = false;
            self.data.url = url || self.data.url;
            self.data.boundZoom = zoom;

            return loadImage(url, self.elements.img, hasExif).then(function (img) {
                if (!points.length) {
                    var natDim = naturalImageDimensions(img);
                    var rect = self.elements.viewport.getBoundingClientRect();
                    var aspectRatio = rect.width / rect.height;
                    var imgAspectRatio = natDim.width / natDim.height;
                    var width, height;

                    if (imgAspectRatio > aspectRatio) {
                        height = natDim.height;
                        width = height * aspectRatio;
                    } else {
                        width = natDim.width;
                        height = width / aspectRatio;
                    }

                    var x0 = (natDim.width - width) / 2;
                    var y0 = (natDim.height - height) / 2;
                    var x1 = x0 + width;
                    var y1 = y0 + height;

                    self.data.points = [x0, y0, x1, y1];
                } else if (self.options.relative) {
                    points = [
                        points[0] * img.naturalWidth / 100, points[1] * img.naturalHeight / 100,
                        points[2] * img.naturalWidth / 100, points[3] * img.naturalHeight / 100
                    ];
                }

                self.data.points = points.map(function (p) {
                    return parseFloat(p);
                });
                if (self.options.useCanvas) {
                    _transferImageToCanvas.call(self, options.orientation || 1);
                }
                _updatePropertiesFromImage.call(self);
                _triggerUpdate.call(self);
                cb && cb();
            });
        }

        function fix(v, decimalPoints) {
            return parseFloat(v).toFixed(decimalPoints || 0);
        }

        function _get() {
            var self = this,
                imgData = self.elements.preview.getBoundingClientRect(),
                vpData = self.elements.viewport.getBoundingClientRect(),
                x1 = vpData.left - imgData.left,
                y1 = vpData.top - imgData.top,
                widthDiff = (vpData.width - self.elements.viewport.offsetWidth) / 2, //border
                heightDiff = (vpData.height - self.elements.viewport.offsetHeight) / 2,
                x2 = x1 + self.elements.viewport.offsetWidth + widthDiff,
                y2 = y1 + self.elements.viewport.offsetHeight + heightDiff,
                scale = self._currentZoom;

            if (scale === Infinity || isNaN(scale)) {
                scale = 1;
            }

            var max = self.options.enforceBoundary ? 0 : Number.NEGATIVE_INFINITY;
            x1 = Math.max(max, x1 / scale);
            y1 = Math.max(max, y1 / scale);
            x2 = Math.max(max, x2 / scale);
            y2 = Math.max(max, y2 / scale);

            return {
                points: [fix(x1), fix(y1), fix(x2), fix(y2)],
                zoom: scale
            };
        }

        var RESULT_DEFAULTS = {
            type: 'canvas',
            format: 'png',
            quality: 1
        },
            RESULT_FORMATS = ['jpeg', 'webp', 'png'];

        function _result(options) {
            var self = this,
                data = _get.call(self),
                opts = deepExtend(RESULT_DEFAULTS, deepExtend({}, options)),
                resultType = (typeof (options) === 'string' ? options : (opts.type || 'base64')),
                size = opts.size || 'viewport',
                format = opts.format,
                quality = opts.quality,
                backgroundColor = opts.backgroundColor,
                circle = typeof opts.circle === 'boolean' ? opts.circle : (self.options.viewport.type === 'circle'),
                vpRect = self.elements.viewport.getBoundingClientRect(),
                ratio = vpRect.width / vpRect.height,
                prom;

            if (size === 'viewport') {
                data.outputWidth = vpRect.width;
                data.outputHeight = vpRect.height;
            } else if (typeof size === 'object') {
                if (size.width && size.height) {
                    data.outputWidth = size.width;
                    data.outputHeight = size.height;
                } else if (size.width) {
                    data.outputWidth = size.width;
                    data.outputHeight = size.width / ratio;
                } else if (size.height) {
                    data.outputWidth = size.height * ratio;
                    data.outputHeight = size.height;
                }
            }

            if (RESULT_FORMATS.indexOf(format) > -1) {
                data.format = 'image/' + format;
                data.quality = quality;
            }

            data.circle = circle;
            data.url = self.data.url;
            data.backgroundColor = backgroundColor;

            prom = new Promise(function (resolve, reject) {
                switch (resultType.toLowerCase()) {
                    case 'rawcanvas':
                        resolve(_getCanvas.call(self, data));
                        break;
                    case 'canvas':
                    case 'base64':
                        resolve(_getBase64Result.call(self, data));
                        break;
                    case 'blob':
                        _getBlobResult.call(self, data).then(resolve);
                        break;
                    default:
                        resolve(_getHtmlResult.call(self, data));
                        break;
                }
            });
            return prom;
        }

        function _refresh() {
            _updatePropertiesFromImage.call(this);
        }

        function _rotate(deg) {
            if (!this.options.useCanvas) {
                throw 'Croppie: Cannot rotate without enableOrientation';
            }

            var self = this, canvas = self.elements.canvas, copy = document.createElement('canvas'), ornt = 1;

            copy.width = canvas.width;
            copy.height = canvas.height;
            var ctx = copy.getContext('2d');
            ctx.drawImage(canvas, 0, 0);

            if (deg === 90 || deg === -270) ornt = 6;
            if (deg === -90 || deg === 270) ornt = 8;
            if (deg === 180 || deg === -180) ornt = 3;

            drawCanvas(canvas, copy, ornt);
            _onZoom.call(self);
            copy = null;
        }

        function _destroy() {
            var self = this;
            self.element.removeChild(self.elements.boundary);
            removeClass(self.element, 'croppie-container');
            if (self.options.enableZoom) {
                self.element.removeChild(self.elements.zoomerWrap);
            }
            delete self.elements;
        }

        if (window.jQuery) {
            var $ = window.jQuery;
            $.fn.croppie = function (opts) {
                var ot = typeof opts;

                if (ot === 'string') {
                    var args = Array.prototype.slice.call(arguments, 1);
                    var singleInst = $(this).data('croppie');

                    if (opts === 'get') {
                        return singleInst.get();
                    } else if (opts === 'result') {
                        return singleInst.result.apply(singleInst, args);
                    } else if (opts === 'bind') {
                        return singleInst.bind.apply(singleInst, args);
                    }

                    return this.each(function () {
                        var i = $(this).data('croppie');
                        if (!i) return;

                        var method = i[opts];
                        if ($.isFunction(method)) {
                            method.apply(i, args);
                            if (opts === 'destroy') {
                                $(this).removeData('croppie');
                            }
                        } else {
                            throw 'Croppie ' + opts + ' method not found';
                        }
                    });
                } else {
                    return this.each(function () {
                        var i = new Croppie(this, opts);
                        i.$ = $;
                        $(this).data('croppie', i);
                    });
                }
            };
        }

        function Croppie(element, opts) {
            this.element = element;
            this.options = deepExtend(deepExtend({}, Croppie.defaults), opts);

            if (this.element.tagName.toLowerCase() === 'img') {
                var origImage = this.element;
                addClass(origImage, 'cr-original-image');
                var replacementDiv = document.createElement('div');
                this.element.parentNode.appendChild(replacementDiv);
                replacementDiv.appendChild(origImage);
                this.element = replacementDiv;
                this.options.url = this.options.url || origImage.src;
            }

            _create.call(this);
            if (this.options.url) {
                var bindOpts = {
                    url: this.options.url,
                    points: this.options.points
                };
                delete this.options['url'];
                delete this.options['points'];
                _bind.call(this, bindOpts);
            }
        }

        Croppie.defaults = {
            viewport: {
                width: 100,
                height: 100,
                type: 'square'
            },
            boundary: {},
            orientationControls: {
                enabled: true,
                leftClass: '',
                rightClass: ''
            },
            resizeControls: {
                width: true,
                height: true
            },
            customClass: '',
            showZoomer: true,
            enableZoom: true,
            enableResize: false,
            mouseWheelZoom: true,
            enableExif: false,
            enforceBoundary: true,
            enableOrientation: false,
            enableKeyMovement: true,
            update: function () {
            }
        };

        Croppie.globals = {
            translate: 'translate3d'
        };

        deepExtend(Croppie.prototype,
            {
                bind: function (options, cb) {
                    return _bind.call(this, options, cb);
                },
                get: function () {
                    var data = _get.call(this);
                    var points = data.points;
                    if (this.options.relative) {
                        points[0] /= this.elements.img.naturalWidth / 100;
                        points[1] /= this.elements.img.naturalHeight / 100;
                        points[2] /= this.elements.img.naturalWidth / 100;
                        points[3] /= this.elements.img.naturalHeight / 100;
                    }
                    return data;
                },
                result: function (type) {
                    return _result.call(this, type);
                },
                refresh: function () {
                    return _refresh.call(this);
                },
                setZoom: function (v) {
                    _setZoomerVal.call(this, v);
                    dispatchChange(this.elements.zoomer);
                },
                rotate: function (deg) {
                    _rotate.call(this, deg);
                },
                destroy: function () {
                    return _destroy.call(this);
                }
            });

        exports.Croppie = window.Croppie = Croppie;

        if (typeof module === 'object' && !!module.exports) {
            module.exports = Croppie;
        }
    }));







/**
* @fileoverview
* - Using the 'QRCode for Javascript library'
* - Fixed dataset of 'QRCode for Javascript library' for support full-spec.
* - this library has no dependencies.
* 
* @author davidshimjs
* @see <a href="http://www.d-project.com/" target="_blank">http://www.d-project.com/</a>
* @see <a href="http://jeromeetienne.github.com/jquery-qrcode/" target="_blank">http://jeromeetienne.github.com/jquery-qrcode/</a>
*/
var QRCode;

(function () {
    //---------------------------------------------------------------------
    // QRCode for JavaScript
    //
    // Copyright (c) 2009 Kazuhiko Arase
    //
    // URL: http://www.d-project.com/
    //
    // Licensed under the MIT license:
    //   http://www.opensource.org/licenses/mit-license.php
    //
    // The word "QR Code" is registered trademark of 
    // DENSO WAVE INCORPORATED
    //   http://www.denso-wave.com/qrcode/faqpatent-e.html
    //
    //---------------------------------------------------------------------
    function QR8bitByte(data) {
        this.mode = QRMode.MODE_8BIT_BYTE;
        this.data = data;
        this.parsedData = [];

        // Added to support UTF-8 Characters
        for (var i = 0, l = this.data.length; i < l; i++) {
            var byteArray = [];
            var code = this.data.charCodeAt(i);

            if (code > 0x10000) {
                byteArray[0] = 0xF0 | ((code & 0x1C0000) >>> 18);
                byteArray[1] = 0x80 | ((code & 0x3F000) >>> 12);
                byteArray[2] = 0x80 | ((code & 0xFC0) >>> 6);
                byteArray[3] = 0x80 | (code & 0x3F);
            } else if (code > 0x800) {
                byteArray[0] = 0xE0 | ((code & 0xF000) >>> 12);
                byteArray[1] = 0x80 | ((code & 0xFC0) >>> 6);
                byteArray[2] = 0x80 | (code & 0x3F);
            } else if (code > 0x80) {
                byteArray[0] = 0xC0 | ((code & 0x7C0) >>> 6);
                byteArray[1] = 0x80 | (code & 0x3F);
            } else {
                byteArray[0] = code;
            }

            this.parsedData.push(byteArray);
        }

        this.parsedData = Array.prototype.concat.apply([], this.parsedData);

        if (this.parsedData.length != this.data.length) {
            this.parsedData.unshift(191);
            this.parsedData.unshift(187);
            this.parsedData.unshift(239);
        }
    }

    QR8bitByte.prototype = {
        getLength: function (buffer) {
            return this.parsedData.length;
        },
        write: function (buffer) {
            for (var i = 0, l = this.parsedData.length; i < l; i++) {
                buffer.put(this.parsedData[i], 8);
            }
        }
    };

    function QRCodeModel(typeNumber, errorCorrectLevel) {
        this.typeNumber = typeNumber;
        this.errorCorrectLevel = errorCorrectLevel;
        this.modules = null;
        this.moduleCount = 0;
        this.dataCache = null;
        this.dataList = [];
    }

    QRCodeModel.prototype = {
        addData: function (data) { var newData = new QR8bitByte(data); this.dataList.push(newData); this.dataCache = null; }, isDark: function (row, col) {
            if (row < 0 || this.moduleCount <= row || col < 0 || this.moduleCount <= col) { throw new Error(row + "," + col); }
            return this.modules[row][col];
        }, getModuleCount: function () { return this.moduleCount; }, make: function () { this.makeImpl(false, this.getBestMaskPattern()); }, makeImpl: function (test, maskPattern) {
            this.moduleCount = this.typeNumber * 4 + 17; this.modules = new Array(this.moduleCount); for (var row = 0; row < this.moduleCount; row++) { this.modules[row] = new Array(this.moduleCount); for (var col = 0; col < this.moduleCount; col++) { this.modules[row][col] = null; } }
            this.setupPositionProbePattern(0, 0); this.setupPositionProbePattern(this.moduleCount - 7, 0); this.setupPositionProbePattern(0, this.moduleCount - 7); this.setupPositionAdjustPattern(); this.setupTimingPattern(); this.setupTypeInfo(test, maskPattern); if (this.typeNumber >= 7) { this.setupTypeNumber(test); }
            if (this.dataCache == null) { this.dataCache = QRCodeModel.createData(this.typeNumber, this.errorCorrectLevel, this.dataList); }
            this.mapData(this.dataCache, maskPattern);
        }, setupPositionProbePattern: function (row, col) { for (var r = -1; r <= 7; r++) { if (row + r <= -1 || this.moduleCount <= row + r) continue; for (var c = -1; c <= 7; c++) { if (col + c <= -1 || this.moduleCount <= col + c) continue; if ((0 <= r && r <= 6 && (c == 0 || c == 6)) || (0 <= c && c <= 6 && (r == 0 || r == 6)) || (2 <= r && r <= 4 && 2 <= c && c <= 4)) { this.modules[row + r][col + c] = true; } else { this.modules[row + r][col + c] = false; } } } }, getBestMaskPattern: function () {
            var minLostPoint = 0; var pattern = 0; for (var i = 0; i < 8; i++) { this.makeImpl(true, i); var lostPoint = QRUtil.getLostPoint(this); if (i == 0 || minLostPoint > lostPoint) { minLostPoint = lostPoint; pattern = i; } }
            return pattern;
        }, createMovieClip: function (target_mc, instance_name, depth) {
            var qr_mc = target_mc.createEmptyMovieClip(instance_name, depth); var cs = 1; this.make(); for (var row = 0; row < this.modules.length; row++) { var y = row * cs; for (var col = 0; col < this.modules[row].length; col++) { var x = col * cs; var dark = this.modules[row][col]; if (dark) { qr_mc.beginFill(0, 100); qr_mc.moveTo(x, y); qr_mc.lineTo(x + cs, y); qr_mc.lineTo(x + cs, y + cs); qr_mc.lineTo(x, y + cs); qr_mc.endFill(); } } }
            return qr_mc;
        }, setupTimingPattern: function () {
            for (var r = 8; r < this.moduleCount - 8; r++) {
                if (this.modules[r][6] != null) { continue; }
                this.modules[r][6] = (r % 2 == 0);
            }
            for (var c = 8; c < this.moduleCount - 8; c++) {
                if (this.modules[6][c] != null) { continue; }
                this.modules[6][c] = (c % 2 == 0);
            }
        }, setupPositionAdjustPattern: function () {
            var pos = QRUtil.getPatternPosition(this.typeNumber); for (var i = 0; i < pos.length; i++) {
                for (var j = 0; j < pos.length; j++) {
                    var row = pos[i]; var col = pos[j]; if (this.modules[row][col] != null) { continue; }
                    for (var r = -2; r <= 2; r++) { for (var c = -2; c <= 2; c++) { if (r == -2 || r == 2 || c == -2 || c == 2 || (r == 0 && c == 0)) { this.modules[row + r][col + c] = true; } else { this.modules[row + r][col + c] = false; } } }
                }
            }
        }, setupTypeNumber: function (test) {
            var bits = QRUtil.getBCHTypeNumber(this.typeNumber); for (var i = 0; i < 18; i++) { var mod = (!test && ((bits >> i) & 1) == 1); this.modules[Math.floor(i / 3)][i % 3 + this.moduleCount - 8 - 3] = mod; }
            for (var i = 0; i < 18; i++) { var mod = (!test && ((bits >> i) & 1) == 1); this.modules[i % 3 + this.moduleCount - 8 - 3][Math.floor(i / 3)] = mod; }
        }, setupTypeInfo: function (test, maskPattern) {
            var data = (this.errorCorrectLevel << 3) | maskPattern; var bits = QRUtil.getBCHTypeInfo(data); for (var i = 0; i < 15; i++) { var mod = (!test && ((bits >> i) & 1) == 1); if (i < 6) { this.modules[i][8] = mod; } else if (i < 8) { this.modules[i + 1][8] = mod; } else { this.modules[this.moduleCount - 15 + i][8] = mod; } }
            for (var i = 0; i < 15; i++) { var mod = (!test && ((bits >> i) & 1) == 1); if (i < 8) { this.modules[8][this.moduleCount - i - 1] = mod; } else if (i < 9) { this.modules[8][15 - i - 1 + 1] = mod; } else { this.modules[8][15 - i - 1] = mod; } }
            this.modules[this.moduleCount - 8][8] = (!test);
        }, mapData: function (data, maskPattern) {
            var inc = -1; var row = this.moduleCount - 1; var bitIndex = 7; var byteIndex = 0; for (var col = this.moduleCount - 1; col > 0; col -= 2) {
                if (col == 6) col--; while (true) {
                    for (var c = 0; c < 2; c++) {
                        if (this.modules[row][col - c] == null) {
                            var dark = false; if (byteIndex < data.length) { dark = (((data[byteIndex] >>> bitIndex) & 1) == 1); }
                            var mask = QRUtil.getMask(maskPattern, row, col - c); if (mask) { dark = !dark; }
                            this.modules[row][col - c] = dark; bitIndex--; if (bitIndex == -1) { byteIndex++; bitIndex = 7; }
                        }
                    }
                    row += inc; if (row < 0 || this.moduleCount <= row) { row -= inc; inc = -inc; break; }
                }
            }
        }
    }; QRCodeModel.PAD0 = 0xEC; QRCodeModel.PAD1 = 0x11; QRCodeModel.createData = function (typeNumber, errorCorrectLevel, dataList) {
        var rsBlocks = QRRSBlock.getRSBlocks(typeNumber, errorCorrectLevel); var buffer = new QRBitBuffer(); for (var i = 0; i < dataList.length; i++) { var data = dataList[i]; buffer.put(data.mode, 4); buffer.put(data.getLength(), QRUtil.getLengthInBits(data.mode, typeNumber)); data.write(buffer); }
        var totalDataCount = 0; for (var i = 0; i < rsBlocks.length; i++) { totalDataCount += rsBlocks[i].dataCount; }
        if (buffer.getLengthInBits() > totalDataCount * 8) {
            throw new Error("code length overflow. ("
                + buffer.getLengthInBits()
                + ">"
                + totalDataCount * 8
                + ")");
        }
        if (buffer.getLengthInBits() + 4 <= totalDataCount * 8) { buffer.put(0, 4); }
        while (buffer.getLengthInBits() % 8 != 0) { buffer.putBit(false); }
        while (true) {
            if (buffer.getLengthInBits() >= totalDataCount * 8) { break; }
            buffer.put(QRCodeModel.PAD0, 8); if (buffer.getLengthInBits() >= totalDataCount * 8) { break; }
            buffer.put(QRCodeModel.PAD1, 8);
        }
        return QRCodeModel.createBytes(buffer, rsBlocks);
    }; QRCodeModel.createBytes = function (buffer, rsBlocks) {
        var offset = 0; var maxDcCount = 0; var maxEcCount = 0; var dcdata = new Array(rsBlocks.length); var ecdata = new Array(rsBlocks.length); for (var r = 0; r < rsBlocks.length; r++) {
            var dcCount = rsBlocks[r].dataCount; var ecCount = rsBlocks[r].totalCount - dcCount; maxDcCount = Math.max(maxDcCount, dcCount); maxEcCount = Math.max(maxEcCount, ecCount); dcdata[r] = new Array(dcCount); for (var i = 0; i < dcdata[r].length; i++) { dcdata[r][i] = 0xff & buffer.buffer[i + offset]; }
            offset += dcCount; var rsPoly = QRUtil.getErrorCorrectPolynomial(ecCount); var rawPoly = new QRPolynomial(dcdata[r], rsPoly.getLength() - 1); var modPoly = rawPoly.mod(rsPoly); ecdata[r] = new Array(rsPoly.getLength() - 1); for (var i = 0; i < ecdata[r].length; i++) { var modIndex = i + modPoly.getLength() - ecdata[r].length; ecdata[r][i] = (modIndex >= 0) ? modPoly.get(modIndex) : 0; }
        }
        var totalCodeCount = 0; for (var i = 0; i < rsBlocks.length; i++) { totalCodeCount += rsBlocks[i].totalCount; }
        var data = new Array(totalCodeCount); var index = 0; for (var i = 0; i < maxDcCount; i++) { for (var r = 0; r < rsBlocks.length; r++) { if (i < dcdata[r].length) { data[index++] = dcdata[r][i]; } } }
        for (var i = 0; i < maxEcCount; i++) { for (var r = 0; r < rsBlocks.length; r++) { if (i < ecdata[r].length) { data[index++] = ecdata[r][i]; } } }
        return data;
    }; var QRMode = { MODE_NUMBER: 1 << 0, MODE_ALPHA_NUM: 1 << 1, MODE_8BIT_BYTE: 1 << 2, MODE_KANJI: 1 << 3 }; var QRErrorCorrectLevel = { L: 1, M: 0, Q: 3, H: 2 }; var QRMaskPattern = { PATTERN000: 0, PATTERN001: 1, PATTERN010: 2, PATTERN011: 3, PATTERN100: 4, PATTERN101: 5, PATTERN110: 6, PATTERN111: 7 }; var QRUtil = {
        PATTERN_POSITION_TABLE: [[], [6, 18], [6, 22], [6, 26], [6, 30], [6, 34], [6, 22, 38], [6, 24, 42], [6, 26, 46], [6, 28, 50], [6, 30, 54], [6, 32, 58], [6, 34, 62], [6, 26, 46, 66], [6, 26, 48, 70], [6, 26, 50, 74], [6, 30, 54, 78], [6, 30, 56, 82], [6, 30, 58, 86], [6, 34, 62, 90], [6, 28, 50, 72, 94], [6, 26, 50, 74, 98], [6, 30, 54, 78, 102], [6, 28, 54, 80, 106], [6, 32, 58, 84, 110], [6, 30, 58, 86, 114], [6, 34, 62, 90, 118], [6, 26, 50, 74, 98, 122], [6, 30, 54, 78, 102, 126], [6, 26, 52, 78, 104, 130], [6, 30, 56, 82, 108, 134], [6, 34, 60, 86, 112, 138], [6, 30, 58, 86, 114, 142], [6, 34, 62, 90, 118, 146], [6, 30, 54, 78, 102, 126, 150], [6, 24, 50, 76, 102, 128, 154], [6, 28, 54, 80, 106, 132, 158], [6, 32, 58, 84, 110, 136, 162], [6, 26, 54, 82, 110, 138, 166], [6, 30, 58, 86, 114, 142, 170]], G15: (1 << 10) | (1 << 8) | (1 << 5) | (1 << 4) | (1 << 2) | (1 << 1) | (1 << 0), G18: (1 << 12) | (1 << 11) | (1 << 10) | (1 << 9) | (1 << 8) | (1 << 5) | (1 << 2) | (1 << 0), G15_MASK: (1 << 14) | (1 << 12) | (1 << 10) | (1 << 4) | (1 << 1), getBCHTypeInfo: function (data) {
            var d = data << 10; while (QRUtil.getBCHDigit(d) - QRUtil.getBCHDigit(QRUtil.G15) >= 0) { d ^= (QRUtil.G15 << (QRUtil.getBCHDigit(d) - QRUtil.getBCHDigit(QRUtil.G15))); }
            return ((data << 10) | d) ^ QRUtil.G15_MASK;
        }, getBCHTypeNumber: function (data) {
            var d = data << 12; while (QRUtil.getBCHDigit(d) - QRUtil.getBCHDigit(QRUtil.G18) >= 0) { d ^= (QRUtil.G18 << (QRUtil.getBCHDigit(d) - QRUtil.getBCHDigit(QRUtil.G18))); }
            return (data << 12) | d;
        }, getBCHDigit: function (data) {
            var digit = 0; while (data != 0) { digit++; data >>>= 1; }
            return digit;
        }, getPatternPosition: function (typeNumber) { return QRUtil.PATTERN_POSITION_TABLE[typeNumber - 1]; }, getMask: function (maskPattern, i, j) { switch (maskPattern) { case QRMaskPattern.PATTERN000: return (i + j) % 2 == 0; case QRMaskPattern.PATTERN001: return i % 2 == 0; case QRMaskPattern.PATTERN010: return j % 3 == 0; case QRMaskPattern.PATTERN011: return (i + j) % 3 == 0; case QRMaskPattern.PATTERN100: return (Math.floor(i / 2) + Math.floor(j / 3)) % 2 == 0; case QRMaskPattern.PATTERN101: return (i * j) % 2 + (i * j) % 3 == 0; case QRMaskPattern.PATTERN110: return ((i * j) % 2 + (i * j) % 3) % 2 == 0; case QRMaskPattern.PATTERN111: return ((i * j) % 3 + (i + j) % 2) % 2 == 0; default: throw new Error("bad maskPattern:" + maskPattern); } }, getErrorCorrectPolynomial: function (errorCorrectLength) {
            var a = new QRPolynomial([1], 0); for (var i = 0; i < errorCorrectLength; i++) { a = a.multiply(new QRPolynomial([1, QRMath.gexp(i)], 0)); }
            return a;
        }, getLengthInBits: function (mode, type) { if (1 <= type && type < 10) { switch (mode) { case QRMode.MODE_NUMBER: return 10; case QRMode.MODE_ALPHA_NUM: return 9; case QRMode.MODE_8BIT_BYTE: return 8; case QRMode.MODE_KANJI: return 8; default: throw new Error("mode:" + mode); } } else if (type < 27) { switch (mode) { case QRMode.MODE_NUMBER: return 12; case QRMode.MODE_ALPHA_NUM: return 11; case QRMode.MODE_8BIT_BYTE: return 16; case QRMode.MODE_KANJI: return 10; default: throw new Error("mode:" + mode); } } else if (type < 41) { switch (mode) { case QRMode.MODE_NUMBER: return 14; case QRMode.MODE_ALPHA_NUM: return 13; case QRMode.MODE_8BIT_BYTE: return 16; case QRMode.MODE_KANJI: return 12; default: throw new Error("mode:" + mode); } } else { throw new Error("type:" + type); } }, getLostPoint: function (qrCode) {
            var moduleCount = qrCode.getModuleCount(); var lostPoint = 0; for (var row = 0; row < moduleCount; row++) {
                for (var col = 0; col < moduleCount; col++) {
                    var sameCount = 0; var dark = qrCode.isDark(row, col); for (var r = -1; r <= 1; r++) {
                        if (row + r < 0 || moduleCount <= row + r) { continue; }
                        for (var c = -1; c <= 1; c++) {
                            if (col + c < 0 || moduleCount <= col + c) { continue; }
                            if (r == 0 && c == 0) { continue; }
                            if (dark == qrCode.isDark(row + r, col + c)) { sameCount++; }
                        }
                    }
                    if (sameCount > 5) { lostPoint += (3 + sameCount - 5); }
                }
            }
            for (var row = 0; row < moduleCount - 1; row++) { for (var col = 0; col < moduleCount - 1; col++) { var count = 0; if (qrCode.isDark(row, col)) count++; if (qrCode.isDark(row + 1, col)) count++; if (qrCode.isDark(row, col + 1)) count++; if (qrCode.isDark(row + 1, col + 1)) count++; if (count == 0 || count == 4) { lostPoint += 3; } } }
            for (var row = 0; row < moduleCount; row++) { for (var col = 0; col < moduleCount - 6; col++) { if (qrCode.isDark(row, col) && !qrCode.isDark(row, col + 1) && qrCode.isDark(row, col + 2) && qrCode.isDark(row, col + 3) && qrCode.isDark(row, col + 4) && !qrCode.isDark(row, col + 5) && qrCode.isDark(row, col + 6)) { lostPoint += 40; } } }
            for (var col = 0; col < moduleCount; col++) { for (var row = 0; row < moduleCount - 6; row++) { if (qrCode.isDark(row, col) && !qrCode.isDark(row + 1, col) && qrCode.isDark(row + 2, col) && qrCode.isDark(row + 3, col) && qrCode.isDark(row + 4, col) && !qrCode.isDark(row + 5, col) && qrCode.isDark(row + 6, col)) { lostPoint += 40; } } }
            var darkCount = 0; for (var col = 0; col < moduleCount; col++) { for (var row = 0; row < moduleCount; row++) { if (qrCode.isDark(row, col)) { darkCount++; } } }
            var ratio = Math.abs(100 * darkCount / moduleCount / moduleCount - 50) / 5; lostPoint += ratio * 10; return lostPoint;
        }
    }; var QRMath = {
        glog: function (n) {
            if (n < 1) { throw new Error("glog(" + n + ")"); }
            return QRMath.LOG_TABLE[n];
        }, gexp: function (n) {
            while (n < 0) { n += 255; }
            while (n >= 256) { n -= 255; }
            return QRMath.EXP_TABLE[n];
        }, EXP_TABLE: new Array(256), LOG_TABLE: new Array(256)
    }; for (var i = 0; i < 8; i++) { QRMath.EXP_TABLE[i] = 1 << i; }
    for (var i = 8; i < 256; i++) { QRMath.EXP_TABLE[i] = QRMath.EXP_TABLE[i - 4] ^ QRMath.EXP_TABLE[i - 5] ^ QRMath.EXP_TABLE[i - 6] ^ QRMath.EXP_TABLE[i - 8]; }
    for (var i = 0; i < 255; i++) { QRMath.LOG_TABLE[QRMath.EXP_TABLE[i]] = i; }
    function QRPolynomial(num, shift) {
        if (num.length == undefined) { throw new Error(num.length + "/" + shift); }
        var offset = 0; while (offset < num.length && num[offset] == 0) { offset++; }
        this.num = new Array(num.length - offset + shift); for (var i = 0; i < num.length - offset; i++) { this.num[i] = num[i + offset]; }
    }
    QRPolynomial.prototype = {
        get: function (index) { return this.num[index]; }, getLength: function () { return this.num.length; }, multiply: function (e) {
            var num = new Array(this.getLength() + e.getLength() - 1); for (var i = 0; i < this.getLength(); i++) { for (var j = 0; j < e.getLength(); j++) { num[i + j] ^= QRMath.gexp(QRMath.glog(this.get(i)) + QRMath.glog(e.get(j))); } }
            return new QRPolynomial(num, 0);
        }, mod: function (e) {
            if (this.getLength() - e.getLength() < 0) { return this; }
            var ratio = QRMath.glog(this.get(0)) - QRMath.glog(e.get(0)); var num = new Array(this.getLength()); for (var i = 0; i < this.getLength(); i++) { num[i] = this.get(i); }
            for (var i = 0; i < e.getLength(); i++) { num[i] ^= QRMath.gexp(QRMath.glog(e.get(i)) + ratio); }
            return new QRPolynomial(num, 0).mod(e);
        }
    }; function QRRSBlock(totalCount, dataCount) { this.totalCount = totalCount; this.dataCount = dataCount; }
    QRRSBlock.RS_BLOCK_TABLE = [[1, 26, 19], [1, 26, 16], [1, 26, 13], [1, 26, 9], [1, 44, 34], [1, 44, 28], [1, 44, 22], [1, 44, 16], [1, 70, 55], [1, 70, 44], [2, 35, 17], [2, 35, 13], [1, 100, 80], [2, 50, 32], [2, 50, 24], [4, 25, 9], [1, 134, 108], [2, 67, 43], [2, 33, 15, 2, 34, 16], [2, 33, 11, 2, 34, 12], [2, 86, 68], [4, 43, 27], [4, 43, 19], [4, 43, 15], [2, 98, 78], [4, 49, 31], [2, 32, 14, 4, 33, 15], [4, 39, 13, 1, 40, 14], [2, 121, 97], [2, 60, 38, 2, 61, 39], [4, 40, 18, 2, 41, 19], [4, 40, 14, 2, 41, 15], [2, 146, 116], [3, 58, 36, 2, 59, 37], [4, 36, 16, 4, 37, 17], [4, 36, 12, 4, 37, 13], [2, 86, 68, 2, 87, 69], [4, 69, 43, 1, 70, 44], [6, 43, 19, 2, 44, 20], [6, 43, 15, 2, 44, 16], [4, 101, 81], [1, 80, 50, 4, 81, 51], [4, 50, 22, 4, 51, 23], [3, 36, 12, 8, 37, 13], [2, 116, 92, 2, 117, 93], [6, 58, 36, 2, 59, 37], [4, 46, 20, 6, 47, 21], [7, 42, 14, 4, 43, 15], [4, 133, 107], [8, 59, 37, 1, 60, 38], [8, 44, 20, 4, 45, 21], [12, 33, 11, 4, 34, 12], [3, 145, 115, 1, 146, 116], [4, 64, 40, 5, 65, 41], [11, 36, 16, 5, 37, 17], [11, 36, 12, 5, 37, 13], [5, 109, 87, 1, 110, 88], [5, 65, 41, 5, 66, 42], [5, 54, 24, 7, 55, 25], [11, 36, 12], [5, 122, 98, 1, 123, 99], [7, 73, 45, 3, 74, 46], [15, 43, 19, 2, 44, 20], [3, 45, 15, 13, 46, 16], [1, 135, 107, 5, 136, 108], [10, 74, 46, 1, 75, 47], [1, 50, 22, 15, 51, 23], [2, 42, 14, 17, 43, 15], [5, 150, 120, 1, 151, 121], [9, 69, 43, 4, 70, 44], [17, 50, 22, 1, 51, 23], [2, 42, 14, 19, 43, 15], [3, 141, 113, 4, 142, 114], [3, 70, 44, 11, 71, 45], [17, 47, 21, 4, 48, 22], [9, 39, 13, 16, 40, 14], [3, 135, 107, 5, 136, 108], [3, 67, 41, 13, 68, 42], [15, 54, 24, 5, 55, 25], [15, 43, 15, 10, 44, 16], [4, 144, 116, 4, 145, 117], [17, 68, 42], [17, 50, 22, 6, 51, 23], [19, 46, 16, 6, 47, 17], [2, 139, 111, 7, 140, 112], [17, 74, 46], [7, 54, 24, 16, 55, 25], [34, 37, 13], [4, 151, 121, 5, 152, 122], [4, 75, 47, 14, 76, 48], [11, 54, 24, 14, 55, 25], [16, 45, 15, 14, 46, 16], [6, 147, 117, 4, 148, 118], [6, 73, 45, 14, 74, 46], [11, 54, 24, 16, 55, 25], [30, 46, 16, 2, 47, 17], [8, 132, 106, 4, 133, 107], [8, 75, 47, 13, 76, 48], [7, 54, 24, 22, 55, 25], [22, 45, 15, 13, 46, 16], [10, 142, 114, 2, 143, 115], [19, 74, 46, 4, 75, 47], [28, 50, 22, 6, 51, 23], [33, 46, 16, 4, 47, 17], [8, 152, 122, 4, 153, 123], [22, 73, 45, 3, 74, 46], [8, 53, 23, 26, 54, 24], [12, 45, 15, 28, 46, 16], [3, 147, 117, 10, 148, 118], [3, 73, 45, 23, 74, 46], [4, 54, 24, 31, 55, 25], [11, 45, 15, 31, 46, 16], [7, 146, 116, 7, 147, 117], [21, 73, 45, 7, 74, 46], [1, 53, 23, 37, 54, 24], [19, 45, 15, 26, 46, 16], [5, 145, 115, 10, 146, 116], [19, 75, 47, 10, 76, 48], [15, 54, 24, 25, 55, 25], [23, 45, 15, 25, 46, 16], [13, 145, 115, 3, 146, 116], [2, 74, 46, 29, 75, 47], [42, 54, 24, 1, 55, 25], [23, 45, 15, 28, 46, 16], [17, 145, 115], [10, 74, 46, 23, 75, 47], [10, 54, 24, 35, 55, 25], [19, 45, 15, 35, 46, 16], [17, 145, 115, 1, 146, 116], [14, 74, 46, 21, 75, 47], [29, 54, 24, 19, 55, 25], [11, 45, 15, 46, 46, 16], [13, 145, 115, 6, 146, 116], [14, 74, 46, 23, 75, 47], [44, 54, 24, 7, 55, 25], [59, 46, 16, 1, 47, 17], [12, 151, 121, 7, 152, 122], [12, 75, 47, 26, 76, 48], [39, 54, 24, 14, 55, 25], [22, 45, 15, 41, 46, 16], [6, 151, 121, 14, 152, 122], [6, 75, 47, 34, 76, 48], [46, 54, 24, 10, 55, 25], [2, 45, 15, 64, 46, 16], [17, 152, 122, 4, 153, 123], [29, 74, 46, 14, 75, 47], [49, 54, 24, 10, 55, 25], [24, 45, 15, 46, 46, 16], [4, 152, 122, 18, 153, 123], [13, 74, 46, 32, 75, 47], [48, 54, 24, 14, 55, 25], [42, 45, 15, 32, 46, 16], [20, 147, 117, 4, 148, 118], [40, 75, 47, 7, 76, 48], [43, 54, 24, 22, 55, 25], [10, 45, 15, 67, 46, 16], [19, 148, 118, 6, 149, 119], [18, 75, 47, 31, 76, 48], [34, 54, 24, 34, 55, 25], [20, 45, 15, 61, 46, 16]]; QRRSBlock.getRSBlocks = function (typeNumber, errorCorrectLevel) {
        var rsBlock = QRRSBlock.getRsBlockTable(typeNumber, errorCorrectLevel); if (rsBlock == undefined) { throw new Error("bad rs block @ typeNumber:" + typeNumber + "/errorCorrectLevel:" + errorCorrectLevel); }
        var length = rsBlock.length / 3; var list = []; for (var i = 0; i < length; i++) { var count = rsBlock[i * 3 + 0]; var totalCount = rsBlock[i * 3 + 1]; var dataCount = rsBlock[i * 3 + 2]; for (var j = 0; j < count; j++) { list.push(new QRRSBlock(totalCount, dataCount)); } }
        return list;
    }; QRRSBlock.getRsBlockTable = function (typeNumber, errorCorrectLevel) { switch (errorCorrectLevel) { case QRErrorCorrectLevel.L: return QRRSBlock.RS_BLOCK_TABLE[(typeNumber - 1) * 4 + 0]; case QRErrorCorrectLevel.M: return QRRSBlock.RS_BLOCK_TABLE[(typeNumber - 1) * 4 + 1]; case QRErrorCorrectLevel.Q: return QRRSBlock.RS_BLOCK_TABLE[(typeNumber - 1) * 4 + 2]; case QRErrorCorrectLevel.H: return QRRSBlock.RS_BLOCK_TABLE[(typeNumber - 1) * 4 + 3]; default: return undefined; } }; function QRBitBuffer() { this.buffer = []; this.length = 0; }
    QRBitBuffer.prototype = {
        get: function (index) { var bufIndex = Math.floor(index / 8); return ((this.buffer[bufIndex] >>> (7 - index % 8)) & 1) == 1; }, put: function (num, length) { for (var i = 0; i < length; i++) { this.putBit(((num >>> (length - i - 1)) & 1) == 1); } }, getLengthInBits: function () { return this.length; }, putBit: function (bit) {
            var bufIndex = Math.floor(this.length / 8); if (this.buffer.length <= bufIndex) { this.buffer.push(0); }
            if (bit) { this.buffer[bufIndex] |= (0x80 >>> (this.length % 8)); }
            this.length++;
        }
    }; var QRCodeLimitLength = [[17, 14, 11, 7], [32, 26, 20, 14], [53, 42, 32, 24], [78, 62, 46, 34], [106, 84, 60, 44], [134, 106, 74, 58], [154, 122, 86, 64], [192, 152, 108, 84], [230, 180, 130, 98], [271, 213, 151, 119], [321, 251, 177, 137], [367, 287, 203, 155], [425, 331, 241, 177], [458, 362, 258, 194], [520, 412, 292, 220], [586, 450, 322, 250], [644, 504, 364, 280], [718, 560, 394, 310], [792, 624, 442, 338], [858, 666, 482, 382], [929, 711, 509, 403], [1003, 779, 565, 439], [1091, 857, 611, 461], [1171, 911, 661, 511], [1273, 997, 715, 535], [1367, 1059, 751, 593], [1465, 1125, 805, 625], [1528, 1190, 868, 658], [1628, 1264, 908, 698], [1732, 1370, 982, 742], [1840, 1452, 1030, 790], [1952, 1538, 1112, 842], [2068, 1628, 1168, 898], [2188, 1722, 1228, 958], [2303, 1809, 1283, 983], [2431, 1911, 1351, 1051], [2563, 1989, 1423, 1093], [2699, 2099, 1499, 1139], [2809, 2213, 1579, 1219], [2953, 2331, 1663, 1273]];

    function _isSupportCanvas() {
        return typeof CanvasRenderingContext2D != "undefined";
    }

    // android 2.x doesn't support Data-URI spec
    function _getAndroid() {
        var android = false;
        var sAgent = navigator.userAgent;

        if (/android/i.test(sAgent)) { // android
            android = true;
            var aMat = sAgent.toString().match(/android ([0-9]\.[0-9])/i);

            if (aMat && aMat[1]) {
                android = parseFloat(aMat[1]);
            }
        }

        return android;
    }

    var svgDrawer = (function () {

        var Drawing = function (el, htOption) {
            this._el = el;
            this._htOption = htOption;
        };

        Drawing.prototype.draw = function (oQRCode) {
            var _htOption = this._htOption;
            var _el = this._el;
            var nCount = oQRCode.getModuleCount();
            var nWidth = Math.floor(_htOption.width / nCount);
            var nHeight = Math.floor(_htOption.height / nCount);

            this.clear();

            function makeSVG(tag, attrs) {
                var el = document.createElementNS('http://www.w3.org/2000/svg', tag);
                for (var k in attrs)
                    if (attrs.hasOwnProperty(k)) el.setAttribute(k, attrs[k]);
                return el;
            }

            var svg = makeSVG("svg", { 'viewBox': '0 0 ' + String(nCount) + " " + String(nCount), 'width': '100%', 'height': '100%', 'fill': _htOption.colorLight });
            svg.setAttributeNS("http://www.w3.org/2000/xmlns/", "xmlns:xlink", "http://www.w3.org/1999/xlink");
            _el.appendChild(svg);

            svg.appendChild(makeSVG("rect", { "fill": _htOption.colorLight, "width": "100%", "height": "100%" }));
            svg.appendChild(makeSVG("rect", { "fill": _htOption.colorDark, "width": "1", "height": "1", "id": "template" }));

            for (var row = 0; row < nCount; row++) {
                for (var col = 0; col < nCount; col++) {
                    if (oQRCode.isDark(row, col)) {
                        var child = makeSVG("use", { "x": String(col), "y": String(row) });
                        child.setAttributeNS("http://www.w3.org/1999/xlink", "href", "#template")
                        svg.appendChild(child);
                    }
                }
            }
        };
        Drawing.prototype.clear = function () {
            while (this._el.hasChildNodes())
                this._el.removeChild(this._el.lastChild);
        };
        return Drawing;
    })();

    var useSVG = document.documentElement.tagName.toLowerCase() === "svg";

    // Drawing in DOM by using Table tag
    var Drawing = useSVG ? svgDrawer : !_isSupportCanvas() ? (function () {
        var Drawing = function (el, htOption) {
            this._el = el;
            this._htOption = htOption;
        };

		/**
		 * Draw the QRCode
		 * 
		 * @param {QRCode} oQRCode
		 */
        Drawing.prototype.draw = function (oQRCode) {
            var _htOption = this._htOption;
            var _el = this._el;
            var nCount = oQRCode.getModuleCount();
            var nWidth = Math.floor(_htOption.width / nCount);
            var nHeight = Math.floor(_htOption.height / nCount);
            var aHTML = ['<table style="border:0;border-collapse:collapse;">'];

            for (var row = 0; row < nCount; row++) {
                aHTML.push('<tr>');

                for (var col = 0; col < nCount; col++) {
                    aHTML.push('<td style="border:0;border-collapse:collapse;padding:0;margin:0;width:' + nWidth + 'px;height:' + nHeight + 'px;background-color:' + (oQRCode.isDark(row, col) ? _htOption.colorDark : _htOption.colorLight) + ';"></td>');
                }

                aHTML.push('</tr>');
            }

            aHTML.push('</table>');
            _el.innerHTML = aHTML.join('');

            // Fix the margin values as real size.
            var elTable = _el.childNodes[0];
            var nLeftMarginTable = (_htOption.width - elTable.offsetWidth) / 2;
            var nTopMarginTable = (_htOption.height - elTable.offsetHeight) / 2;

            if (nLeftMarginTable > 0 && nTopMarginTable > 0) {
                elTable.style.margin = nTopMarginTable + "px " + nLeftMarginTable + "px";
            }
        };

		/**
		 * Clear the QRCode
		 */
        Drawing.prototype.clear = function () {
            this._el.innerHTML = '';
        };

        return Drawing;
    })() : (function () { // Drawing in Canvas
        function _onMakeImage() {
            this._elImage.src = this._elCanvas.toDataURL("image/png");
            this._elImage.style.display = "block";
            this._elCanvas.style.display = "none";
        }

        // Android 2.1 bug workaround
        // http://code.google.com/p/android/issues/detail?id=5141
        if (this._android && this._android <= 2.1) {
            var factor = 1 / window.devicePixelRatio;
            var drawImage = CanvasRenderingContext2D.prototype.drawImage;
            CanvasRenderingContext2D.prototype.drawImage = function (image, sx, sy, sw, sh, dx, dy, dw, dh) {
                if (("nodeName" in image) && /img/i.test(image.nodeName)) {
                    for (var i = arguments.length - 1; i >= 1; i--) {
                        arguments[i] = arguments[i] * factor;
                    }
                } else if (typeof dw == "undefined") {
                    arguments[1] *= factor;
                    arguments[2] *= factor;
                    arguments[3] *= factor;
                    arguments[4] *= factor;
                }

                drawImage.apply(this, arguments);
            };
        }

		/**
		 * Check whether the user's browser supports Data URI or not
		 * 
		 * @private
		 * @param {Function} fSuccess Occurs if it supports Data URI
		 * @param {Function} fFail Occurs if it doesn't support Data URI
		 */
        function _safeSetDataURI(fSuccess, fFail) {
            var self = this;
            self._fFail = fFail;
            self._fSuccess = fSuccess;

            // Check it just once
            if (self._bSupportDataURI === null) {
                var el = document.createElement("img");
                var fOnError = function () {
                    self._bSupportDataURI = false;

                    if (self._fFail) {
                        self._fFail.call(self);
                    }
                };
                var fOnSuccess = function () {
                    self._bSupportDataURI = true;

                    if (self._fSuccess) {
                        self._fSuccess.call(self);
                    }
                };

                el.onabort = fOnError;
                el.onerror = fOnError;
                el.onload = fOnSuccess;
                el.src = "data:image/gif;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg=="; // the Image contains 1px data.
                return;
            } else if (self._bSupportDataURI === true && self._fSuccess) {
                self._fSuccess.call(self);
            } else if (self._bSupportDataURI === false && self._fFail) {
                self._fFail.call(self);
            }
        };

		/**
		 * Drawing QRCode by using canvas
		 * 
		 * @constructor
		 * @param {HTMLElement} el
		 * @param {Object} htOption QRCode Options 
		 */
        var Drawing = function (el, htOption) {
            this._bIsPainted = false;
            this._android = _getAndroid();

            this._htOption = htOption;
            this._elCanvas = document.createElement("canvas");
            this._elCanvas.width = htOption.width;
            this._elCanvas.height = htOption.height;
            el.appendChild(this._elCanvas);
            this._el = el;
            this._oContext = this._elCanvas.getContext("2d");
            this._bIsPainted = false;
            this._elImage = document.createElement("img");
            this._elImage.alt = "Scan me!";
            this._elImage.style.display = "none";
            this._el.appendChild(this._elImage);
            this._bSupportDataURI = null;
        };

		/**
		 * Draw the QRCode
		 * 
		 * @param {QRCode} oQRCode 
		 */
        Drawing.prototype.draw = function (oQRCode) {
            var _elImage = this._elImage;
            var _oContext = this._oContext;
            var _htOption = this._htOption;

            var nCount = oQRCode.getModuleCount();
            var nWidth = _htOption.width / nCount;
            var nHeight = _htOption.height / nCount;
            var nRoundedWidth = Math.round(nWidth);
            var nRoundedHeight = Math.round(nHeight);

            _elImage.style.display = "none";
            this.clear();

            for (var row = 0; row < nCount; row++) {
                for (var col = 0; col < nCount; col++) {
                    var bIsDark = oQRCode.isDark(row, col);
                    var nLeft = col * nWidth;
                    var nTop = row * nHeight;
                    _oContext.strokeStyle = bIsDark ? _htOption.colorDark : _htOption.colorLight;
                    _oContext.lineWidth = 1;
                    _oContext.fillStyle = bIsDark ? _htOption.colorDark : _htOption.colorLight;
                    _oContext.fillRect(nLeft, nTop, nWidth, nHeight);

                    // 안티 앨리어싱 방지 처리
                    _oContext.strokeRect(
                        Math.floor(nLeft) + 0.5,
                        Math.floor(nTop) + 0.5,
                        nRoundedWidth,
                        nRoundedHeight
                    );

                    _oContext.strokeRect(
                        Math.ceil(nLeft) - 0.5,
                        Math.ceil(nTop) - 0.5,
                        nRoundedWidth,
                        nRoundedHeight
                    );
                }
            }

            this._bIsPainted = true;
        };

		/**
		 * Make the image from Canvas if the browser supports Data URI.
		 */
        Drawing.prototype.makeImage = function () {
            if (this._bIsPainted) {
                _safeSetDataURI.call(this, _onMakeImage);
            }
        };

		/**
		 * Return whether the QRCode is painted or not
		 * 
		 * @return {Boolean}
		 */
        Drawing.prototype.isPainted = function () {
            return this._bIsPainted;
        };

		/**
		 * Clear the QRCode
		 */
        Drawing.prototype.clear = function () {
            this._oContext.clearRect(0, 0, this._elCanvas.width, this._elCanvas.height);
            this._bIsPainted = false;
        };

		/**
		 * @private
		 * @param {Number} nNumber
		 */
        Drawing.prototype.round = function (nNumber) {
            if (!nNumber) {
                return nNumber;
            }

            return Math.floor(nNumber * 1000) / 1000;
        };

        return Drawing;
    })();

	/**
	 * Get the type by string length
	 * 
	 * @private
	 * @param {String} sText
	 * @param {Number} nCorrectLevel
	 * @return {Number} type
	 */
    function _getTypeNumber(sText, nCorrectLevel) {
        var nType = 1;
        var length = _getUTF8Length(sText);

        for (var i = 0, len = QRCodeLimitLength.length; i <= len; i++) {
            var nLimit = 0;

            switch (nCorrectLevel) {
                case QRErrorCorrectLevel.L:
                    nLimit = QRCodeLimitLength[i][0];
                    break;
                case QRErrorCorrectLevel.M:
                    nLimit = QRCodeLimitLength[i][1];
                    break;
                case QRErrorCorrectLevel.Q:
                    nLimit = QRCodeLimitLength[i][2];
                    break;
                case QRErrorCorrectLevel.H:
                    nLimit = QRCodeLimitLength[i][3];
                    break;
            }

            if (length <= nLimit) {
                break;
            } else {
                nType++;
            }
        }

        if (nType > QRCodeLimitLength.length) {
            throw new Error("Too long data");
        }

        return nType;
    }

    function _getUTF8Length(sText) {
        var replacedText = encodeURI(sText).toString().replace(/\%[0-9a-fA-F]{2}/g, 'a');
        return replacedText.length + (replacedText.length != sText ? 3 : 0);
    }

	/**
	 * @class QRCode
	 * @constructor
	 * @example 
	 * new QRCode(document.getElementById("test"), "http://jindo.dev.naver.com/collie");
	 *
	 * @example
	 * var oQRCode = new QRCode("test", {
	 *    text : "http://naver.com",
	 *    width : 128,
	 *    height : 128
	 * });
	 * 
	 * oQRCode.clear(); // Clear the QRCode.
	 * oQRCode.makeCode("http://map.naver.com"); // Re-create the QRCode.
	 *
	 * @param {HTMLElement|String} el target element or 'id' attribute of element.
	 * @param {Object|String} vOption
	 * @param {String} vOption.text QRCode link data
	 * @param {Number} [vOption.width=256]
	 * @param {Number} [vOption.height=256]
	 * @param {String} [vOption.colorDark="#000000"]
	 * @param {String} [vOption.colorLight="#ffffff"]
	 * @param {QRCode.CorrectLevel} [vOption.correctLevel=QRCode.CorrectLevel.H] [L|M|Q|H] 
	 */
    QRCode = function (el, vOption) {
        this._htOption = {
            width: 256,
            height: 256,
            typeNumber: 4,
            colorDark: "#000000",
            colorLight: "#ffffff",
            correctLevel: QRErrorCorrectLevel.H
        };

        if (typeof vOption === 'string') {
            vOption = {
                text: vOption
            };
        }

        // Overwrites options
        if (vOption) {
            for (var i in vOption) {
                this._htOption[i] = vOption[i];
            }
        }

        if (typeof el == "string") {
            el = document.getElementById(el);
        }

        if (this._htOption.useSVG) {
            Drawing = svgDrawer;
        }

        this._android = _getAndroid();
        this._el = el;
        this._oQRCode = null;
        this._oDrawing = new Drawing(this._el, this._htOption);

        if (this._htOption.text) {
            this.makeCode(this._htOption.text);
        }
    };

	/**
	 * Make the QRCode
	 * 
	 * @param {String} sText link data
	 */
    QRCode.prototype.makeCode = function (sText) {
        this._oQRCode = new QRCodeModel(_getTypeNumber(sText, this._htOption.correctLevel), this._htOption.correctLevel);
        this._oQRCode.addData(sText);
        this._oQRCode.make();
        this._el.title = sText;
        this._oDrawing.draw(this._oQRCode);
        this.makeImage();
    };

	/**
	 * Make the Image from Canvas element
	 * - It occurs automatically
	 * - Android below 3 doesn't support Data-URI spec.
	 * 
	 * @private
	 */
    QRCode.prototype.makeImage = function () {
        if (typeof this._oDrawing.makeImage == "function" && (!this._android || this._android >= 3)) {
            this._oDrawing.makeImage();
        }
    };

	/**
	 * Clear the QRCode
	 */
    QRCode.prototype.clear = function () {
        this._oDrawing.clear();
    };

	/**
	 * @name QRCode.CorrectLevel
	 */
    QRCode.CorrectLevel = QRErrorCorrectLevel;
})();