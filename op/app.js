
function dialog(btn, modal) {

    var sizg = modal == 1 ? 'small' : modal == 2 ? 'large' : 'full';

    var okbtn = modal <= 2 ? '<button onclick="btn.submit()">OK</botton>' : '';

    var html =
        '<div class="' + sizg + ' reveal" id="dialog" data-reveal data-close-on-click="false">' +
        '<h3></h3>' +
        '<button class="close-button" data-close aria-label="Close modal" type="button"><span aria-hidden="true">&times;</span></button>' +
        okbtn +
        '<iframe src="' + btn.name + '" style="width: 100%; height: 80%"></iframe>' +
        '</div>';

    var div = $(html);

    $('body').append(div);

    // initialize
    $(div).foundation();

    // open
    $(div).foundation('open');
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