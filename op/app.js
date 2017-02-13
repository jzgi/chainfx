
function dialog(action, size, style) {
    $('body').innerHTML(
        '<dialog style="width: 75%; height: 75%" open><iframe src="http://baidu.com" style="width: 100%; height: 100%"></iframe><menu><button id="cancel" type="reset">Cancel</button><button type="submit">Confirm</button></menu></dialog>'
    );
}