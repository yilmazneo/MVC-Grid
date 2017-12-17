var currentPage = 1;
var sortBy = 'Brand Name';

function sort(sortBy) {
    window.sortBy = sortBy;
    $.ajax({
        url: "/ads/grid/" + window.currentPage + "/" + sortBy + "/" + modelNumber
    })
        .done(function (data) {
            $('#grid').html(data);
        });
}

function changePage(pageNumber) {
    window.currentPage = pageNumber;
    $.ajax({
        url: "/ads/grid/" + pageNumber + "/" + window.sortBy + "/" + modelNumber,
        beforeSend: function () {            
        }
    })

        .done(function (data) {
            $('#grid').html(data);
        });
}