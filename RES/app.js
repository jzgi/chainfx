


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
                                .append('<div class="row ">' +
                                    '<div class="large-2 columns">' +
                                    '<a href = "/celeb/' + value.id + '"> <img src = "http://placehold.it/150x200&amp;text=book cover" alt = "book cover" class=" thumbnail"></a>' +
                                    '</div>' +
                                    '<div class="large-8 columns">' +
                                    value.name +
                                    '</div>' +
                                    '</div>'
                                );

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
                                .append('<div class="row ">' +
                                    '<div class="large-2 columns">' +
                                    '<a href = "/celeb/' +
                                    value.id +
                                    '"> <img src = "http://placehold.it/150x200&amp;text=book cover" alt = "book cover" class=" thumbnail"></a>' +
                                    '</div>' +
                                    '<div class="large-8 columns">' +
                                    value.name +
                                    '</div>' +
                                    '</div>'
                                );

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
                                .append('<div class="row ">' +
                                    '<div class="large-2 columns">' +
                                    '<a href = "/celeb/' +
                                    value.id +
                                    '"> <img src = "http://placehold.it/150x200&amp;text=book cover" alt = "book cover" class=" thumbnail"></a>' +
                                    '</div>' +
                                    '<div class="large-8 columns">' +
                                    value.name +
                                    '</div>' +
                                    '</div>'
                                );

                        });
                }
            });
        }
    )