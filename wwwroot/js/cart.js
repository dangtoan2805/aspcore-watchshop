$(".vt-addition").on("click", function () {
    let obj = $(this).next();
    let quantity = parseInt(obj.text()) + 1;
    if (quantity > 1)
        $(this).next().next().removeClass("vt-none-event")
    obj.text(quantity);
    cal();
})

$(".vt-reduction").on("click", function () {
    let obj = $(this).prev();
    let quantity = parseInt(obj.text()) - 1;
    if (quantity <= 0)
        $(this).addClass("vt-none-event")
    else
        obj.text(quantity);
    cal();
})


$(".vt-remove").on("click", function () {
    let productId = $(this).data("itemid");
    let parent = $(this).parents()[1];
    $(parent).fadeOut(300);
    $(parent).remove();
    $.ajax({ method: "POST", url: "/Cart/RemoveItem", data: { productID: productId } })
    cal()
})


$('#add-cart').on('click', function (e) {
    let productId = $(this).data('itemid');
    $.ajax({
        method: "POST", url: "/Cart/AddItem", data: { productID: productId }
    })
})

function cal() {
    let cart = $(".cart-item");
    let ar = cart.toArray();
    let total = ar.reduce((sum, item) => {
        var price = item.querySelector(".vt-price :first-child").innerText;
        var prom = item.querySelector(".vt-promotion :first-child").innerText;
        var quantity = item.querySelector(".vt-quantity").innerText;
        return sum + ((parseInt(price) * (1 - (parseFloat(prom) / 100)) * parseInt(quantity)));
    }, 0);
    $('#vt-total-cart').text(total);
    let tranpost;
    let tax = $("#vt-tax").text()
    if (total > 2000000)
        tranpost = 0;
    else
        tranpost = 30000;
    $("#vt-tranpost").text(tranpost);
    let pay = (total - tranpost) * (1 - parseFloat(tax));
    $('#vt-total-bill').text(pay <= 0 ? 0 : pay);

}
cal();

$('#vt-form').on('submit', function (e) {
    let tranpost = $("#vt-tranpost").text();
    let tax = $("#vt-tax").text();
    let pay = $('#vt-total-bill').text();
    $(`<input type='hidden' name='Total' value='${pay}'`);
    $(`<input type='hidden' name='TransportFee' value='${tranpost}'`);
    $(`<input type='hidden' name='Tax' value='${tax}'`);
    return true;
});



