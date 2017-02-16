
function dialog(action, size, style) {

    var div = $('<div class="full reveal" id="dialog" data-reveal data-close-on-click="false"><h3></h3><button class="close-button" data-close aria-label="Close modal" type="button"><span aria-hidden="true">&times;</span></button><iframe style="width: 100%; height: 80%"></iframe></div>');
    
    $('body').append(div);
    $(div).foundation();
    $(div).foundation('open');
}