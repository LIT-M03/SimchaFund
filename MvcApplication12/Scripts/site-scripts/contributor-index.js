$(function() {
    $("#new-contributor").on('click', function() {
        $(".new-contrib").modal();
    });

    $(".deposit-button").on('click', function () {
        var contribId = $(this).data('contribid');
        $('[name="contributorId"]').val(contribId);

        var tr = $(this).parent().parent();
        var name = tr.find('td:eq(1)').text();
        $("#deposit-name").text(name);  

        $(".deposit").modal();
    });

    $("#search").on('keyup', function() {
        var text = $(this).val();
        $("table tr:gt(0)").each(function() {
            var tr = $(this);
            var name = tr.find('td:eq(1)').text();
            if (name.toLowerCase().indexOf(text.toLowerCase()) !== -1) {
                tr.show();
            } else {
                tr.hide();
            }
        });
    });

    $("#clear").on('click', function() {
        $("#search").val('');
        $("tr").show();
    });
});
