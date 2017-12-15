var currentPage = 1;
var sortBy = 'Ad Id';

function t(sortBy) {
    window.sortBy = sortBy;
    $.ajax({
        url: "/home/grid/" + window.currentPage + "/" + sortBy
    })
        .done(function (data) {
            $('#grid').html(data);
        });
}

function changePage(pageNumber) {
    window.currentPage = pageNumber;
    $.ajax({
        url: "/home/grid/" + pageNumber + "/" + window.sortBy,
        beforeSend: function () {
            $('#grid').popover('show');
        }
    })

        .done(function (data) {
            $('#grid').html(data);
        });
}