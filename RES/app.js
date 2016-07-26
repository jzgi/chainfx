


$(document)
    .ready(function() {
            // insert hot celebs
            $.ajax({
                type: "GET",
                url: "hotcelebs.json",
                data: "",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                error: function(x, e) { alert(e.responseText); },
                success: function(lst) {
                    $.each(lst,
                        function(index, value) {
                            $('#panel1')
                                .append('<input type="checkbox" id="checkbox_' +
                                    value.id +
                                    '" checked="checked" value="0" />&nbsp;<div style = "display:inline;" > ' +
                                    value.name +
                                    '</div >  < br /  > ');

                        });
                }
            });

            // insert hot brands
            $.ajax({
                type: "GET",
                url: "hotbrands.json",
                data: "",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                error: function(x, e) { alert(e.responseText); },
                success: function(lst) {
                    $.each(lst,
                        function(index, value) {
                            $('#panel2')
                                .append('<input type="checkbox" id="checkbox_' +
                                    value.id +
                                    '" checked="checked" value="0" />&nbsp;<div style = "display:inline;" > ' +
                                    value.name +
                                    '</div >  < br /  > ');

                        });
                }
            });

            $.ajax({
                type: "GET",
                url: "hotworks.json",
                data: "",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                error: function(x, e) { alert(e.responseText); },
                success: function(lst) {
                    $.each(lst,
                        function(index, value) {
                            $('#panel3')
                                .append('<input type="checkbox" id="checkbox_' +
                                    value.id +
                                    '" checked="checked" value="0" />&nbsp;<div style = "display:inline;" > ' +
                                    value.name +
                                    '</div >  < br /  > ');

                        });
                }
            });
        }
    )