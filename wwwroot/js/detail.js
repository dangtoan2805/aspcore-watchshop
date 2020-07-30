$.ajax({
    method: 'GET', url: "/Product/GetRelations",
    data: { key: $('#relation').text() },
    dataType: "JSON"
}).done(data => {
    if (data != null)
        showData(data);
});

$('#add-cart').on('click', function (e) {
    let productId = $(this).data('itemid');
    $.ajax({
        method: "POST", url: "/Cart/AddItem", data: { productID: productId }
    }).done(() => alert("Thêm sản phẩm vào giỏ hàng thành công"))
})

$('.vt-gallery .vt-img').on('click', function () {
    let src = $(this).children().prop("src");
    $("#vt-photo-show").prop("src", src);
});

