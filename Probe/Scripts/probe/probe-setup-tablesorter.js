$(function () {
    $("table.tablesorter").tablesorter({
        widthFixed: true,
        sortList: [[0, 0]],
        widgets: ['zebra', 'columns'],
        usNumberFormat: false,
        sortReset: true,
        sortRestart: true
    })
    .tablesorterPager({ container: $("#pager"), size: $(".pagesize option:selected").val() });

});