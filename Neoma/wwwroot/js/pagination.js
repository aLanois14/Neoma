var pageActive = 1;
var nbPages = 1;

function getPagination(table) {
    $('#maxRows').on('change', function () {
        $('.neo-pagination-center').html('');
        var rowCount = 0;
        var maxRows = parseInt($(this).val());
        var totalRows = $(table + ' > li').length;

        pageActive = 1;

        $(table + ' > li').each(function () {
            if (maxRows === 0) {
                $(this).show();
            }
            else {
                rowCount++;
                if (rowCount > maxRows) {
                    $(this).hide();
                }
                if (rowCount <= maxRows) {
                    $(this).show();
                }
            }
            
        });
        nbPages = 1;
        if (maxRows > 0) {
            nbPages = Math.max(1, Math.ceil(totalRows / maxRows));
            if (totalRows > maxRows) {
                for (var i = 1; i <= nbPages; i++) {
                    $('.neo-pagination-center').append('<a class="neo-pagination-link" data-page="' + i + '">' + i + '</a>').show();
                }
            }
        }
        
        $('.neo-pagination-center a:first-child').addClass('active');
        $('.neo-pagination-center a').on('click', function () {
            var pageNum = $(this).attr('data-page');
            pageActive = pageNum;

            var index = 0;
            $('.neo-pagination-center a').removeClass('active');
            $(this).addClass('active');
            $(table + ' > li').each(function () {
                index++;
                if (index > maxRows * pageNum || index <= maxRows * pageNum - maxRows) {
                    $(this).hide();
                }
                else {
                    $(this).show();
                }
            });
        });

        $('.neo-pagination-left a').on('click', function () {
            var pageNum = 1;
            pageActive = pageNum;
            if (nbPages > 1) {
                var index = 0;
                $('.neo-pagination-center a').removeClass('active');
                $('.neo-pagination-center a').each(function () {
                    if ($(this).attr('data-page') === '1')
                        $(this).addClass('active');
                });

                $(table + ' > li').each(function () {
                    index++;
                    if (index > maxRows * pageNum || index <= maxRows * pageNum - maxRows) {
                        $(this).hide();
                    }
                    else {
                        $(this).show();
                    }
                });
            }
        });

        $('.neo-pagination-right a').on('click', function () {
            var pageNum = nbPages;
            pageActive = pageNum;

            if (nbPages > 1) {
                var index = 0;
                $('.neo-pagination-center a').removeClass('active');
                $('.neo-pagination-center a').each(function () {
                    if ($(this).attr('data-page') === nbPages.toString())
                        $(this).addClass('active');
                });

                $(table + ' > li').each(function () {
                    index++;
                    if (index > maxRows * pageNum || index <= maxRows * pageNum - maxRows) {
                        $(this).hide();
                    }
                    else {
                        $(this).show();
                    }
                });
            }
        });
    });
}
