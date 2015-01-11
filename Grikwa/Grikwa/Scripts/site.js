jQuery(function ($) {
    //$('#ProductImage').ace_file_input({
    //    style: 'well',
    //    btn_choose: 'Drop product image here or click to choose',
    //    btn_change: null,
    //    no_icon: 'icon-cloud-upload',
    //    droppable: true,
    //    thumbnail: 'small'//large | fit
    //    //,icon_remove:null//set null, to hide remove/reset button
    //    /**,before_change:function(files, dropped) {
    //        //Check an example below
    //        //or examples/file-upload.html
    //        return true;
    //    }*/
    //    /**,before_remove : function() {
    //        return true;
    //    }*/
	//				,
    //    preview_error: function (filename, error_code) {
    //        //name of the file that failed
    //        //error_code values
    //        //1 = 'FILE_LOAD_FAILED',
    //        //2 = 'IMAGE_LOAD_FAILED',
    //        //3 = 'THUMBNAIL_FAILED'
    //        //alert(error_code);
    //    }

    //}).on('change', function () {
    //    //console.log($(this).data('ace_input_files'));
    //    //console.log($(this).data('ace_input_method'));
    //});

    $("#input-mask-price").autoNumeric('init');
    $(".price").each(function (index) {
        var price = Number($(this).html());
        $(this).autoNumeric('init');
        $(this).autoNumeric("set", price);
    })

    $(".chzn-select").chosen(
    {
        display_selected_options: false,
        placeholder_text: "Select some categories",
        search_contains : true
    });

    //$(".chzn-select").bind();
})


function getQ() {
    if ($("#InstitutionID").val() != "") {
        jQuery.ajax(
            {
                url: '/QualificationManager/Qualifications?InstitutionID=' + parseInt($("#InstitutionID").val()),
                type: 'GET',
                dataType: 'json',
                success: function (results) {
                    $("#QualificationID").html('<option value="">--select--</option>');
                    $.each(results, function (index, item) {
                        $("#QualificationID").append('<option value =' + item.QualificationID + '>(' + item.Code + ') ' + item.Name + '</option>');
                    })
                }

            });
    }
}

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
})

$("#editForm").submit(function () {
    $("#input-mask-price").val($("#input-mask-price").autoNumeric('get'));
})

$('.dialogs,.comments').slimScroll({
    height: '450px'
});



