$(document).ready(function () {
    updateCartCount();
});

function updateCartCount() {
    $.ajax({
        url: '/Cart/GetCartCount',
        type: 'GET',
        success: function (result) {
            $('#cart-count').text(result.count);
        }
    });
}
