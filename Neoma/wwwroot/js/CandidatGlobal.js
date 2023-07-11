$(function () {
    $('.make-select2').attr("disabled", "disabled");
    var len = $("#tableCandidat  .item").not(".w-100").length;
    if (len > 1) {
        $(".result").html('<b class="color-violet">' + len + '</b> co-surfeurs correspondent à votre recherche');
    }
    else {
        $(".result").html('<b class="color-violet">' + len + '</b> co-surfeur correspond à votre recherche');
    }

    $('.make-select2').select2({
        placeholder: "Spécialités"
    });

    $('.btn-grid').click(function () {
        $(this).addClass('active');
        $('.items-listing').addClass('grid');
        $('.items-listing').removeClass('list');
        $('.btn-list').removeClass('active');
    });
    $('.btn-list').click(function () {
        $(this).addClass('active');
        $('.items-listing').addClass('list');
        $('.items-listing').removeClass('grid');
        $('.btn-grid').removeClass('active');
    });    
});

function accessibilityFilterProject(index) {
    if (index === 0) {
        $('.Role').removeAttr("enabled");
        $('.Role').attr("disabled", "disabled");
        $('.make-select2').removeAttr("enabled");
        $('.make-select2').attr("disabled", "disabled");
    }
    else {
        $('.Role').removeAttr("disabled");
        $('.Role').attr("enabled", "enabled");
    }
}

function accessibilityFilterRole(index) {
    if (index === 0) {
        $('.make-select2').removeAttr("enabled");
        $('.make-select2').attr("disabled", "disabled");
    }
    else {
        $('.make-select2').removeAttr("disabled");
        $('.make-select2').attr("enabled", "enabled");
    }
}