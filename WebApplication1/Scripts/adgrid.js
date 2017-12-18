var currentPage = 1; // Default Page
var sortBy = 'Brand Name'; // Default Sort By Column Display name

// This function is called when any column header text is clicked.
// It calls the grid partial view with current page and new sort column
// then replaces the current grid
function sort(sortBy) {
    window.sortBy = sortBy;
    $.ajax({
        url: "/ads/grid/" + window.currentPage + "/" + sortBy + "/" + modelNumber
    })
        .done(function (data) {
            $('#grid').html(data);
        });
}

// This function is called when any page is clicked.
// It calls the grid partial view with new current page and current sort column
// then replaces the current grid
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