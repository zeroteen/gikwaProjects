jQuery(function ($) {

    $("#input-mask-price").autoNumeric('init');
    $(".price").each(function (index) {
        var price = Number($(this).html());
        $(this).autoNumeric('init');
        $(this).autoNumeric("set", price);
    });

    $(".chzn-select").chosen(
    {
        display_selected_options: false,
        placeholder_text: "Select some categories",
        search_contains : true
    });

})

function getPendingUsers() {
    if ($("#ProductID").val() != "") {
        jQuery.ajax(
            {
                url: '/Store/PendingUsers?ProductID=' + parseInt($("#ProductID").val()),
                type: 'GET',
                dataType: 'json',
                success: function (results) {
                    $("#CustomerID").html('<option value="">--select--</option>');
                    $("#CustomerID").append('<option value ="OutsideGrikwa">Someone Outside Grikwa</option>');
                    $.each(results, function (index, item) {
                        $("#CustomerID").append('<option value =' + item.UserID + '>' + item.FullName + '</option>');
                    })
                }

            });
    }
}

$("#sellForm").submit(function () {
    $("#input-mask-price").val($("#input-mask-price").autoNumeric('get'));
});

$("#editProductForm").submit(function () {
    $("#input-mask-price").val($("#input-mask-price").autoNumeric('get'));
});



//$('.dialogs,.comments').slimScroll({
//    height: '450px'
//});



