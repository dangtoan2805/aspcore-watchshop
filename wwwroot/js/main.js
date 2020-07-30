var pageItems = 12;
var datas = [];
var temp;
// ============== Request ===============
function prom() {
  $.ajax({
    method: 'GET', url: '/Product/GetPromotions',
    dateType: 'JSON'
  }).done(data => requestSuccess(data));
}

function category(cateId) {
  $.ajax({
    method: 'GET', url: '/Product/GetCategory',
    data: { cateID: cateId },
    dateType: 'JSON'
  }).done(data => requestSuccess(data));
}

function findWithKey(key) {
  $('#result').show();
  $('#vt-loader-min').show();
  $.ajax({
    method: 'GET', url: '/Product/Search',
    data: { key: key },
    dataType: 'JSON'
  }).done(data => showSearchResult(data));
}

function getRelationProduct(key) {
  $.ajax({
    method: 'GET', url: "/Product/GetRelations",
    data: { key: key },
    dataType: "JSON"
  }).done(data => {
    if (data != null)
      showData(data);
  });
}
// ============== Handler ===============
// Paging 
function onChangePage(handler) {
  $('.page-link').on('click', function () {
    $("html,body").animate({ scrollTop: "0" }, 500);
    $('#vt-pagination').children('.active').removeClass('active');
    $(this).parent().addClass('active');
    handler($(this).text());
  });
}

function calPages(length, pageItems) {
  if (length % pageItems != 0)
    return parseInt(length / pageItems) + 1;
  return length / pageItems;
}

var showPages = function (length, pageItems, handler) {
  let containter = $('#vt-pagination');
  containter.empty();
  let pages = calPages(length, pageItems);
  for (let index = 1; index <= pages; index++) {
    containter.append(`<li class="page-item"><a class="page-link py-2 px-3" >${index}</a</li>`);
  }
  //Attach when click page
  onChangePage(handler);
}
// Event
function requestSuccess(data) {
  $("#loading").fadeOut(500);
  $('#search-input').val('');
  datas.length = 0;
  datas = data;
  displayData(datas);
}

function onChangeFilter(wireID) { // Filter
  console.log(wireID);
  if (wireID == 0) {
    temp = null;
    //Refresh root data on View
    displayData(datas);
  }
  else {
    temp = datas.filter(item => item.typeWireID == wireID);
    //Display filter data on View
    displayData(temp);
  }
}

function onChangeOrderBy(orderIndex) { // Orderby
  let obj;
  temp != null ? obj = temp : obj = datas;
  if (orderIndex == 1) //Index = 0 => Ascending
    // datas.reverse();
    obj.sort((a, b) => ((a.price * (1 - a.promotion)) - (b.price * (1 - b.promotion))));
  else //Index = 1 => Decreasing
    obj.sort((a, b) => ((b.price * (1 - b.promotion)) - (a.price * (1 - a.promotion))));
  fisrtPage();
}

// Suport
function displayData(data) {
  if (data != null && data.length > 0) {
    let len = data.length
    $('#vt-total').text(len);
    $('#vt-pagination').show();
    showPages(len, pageItems, number => getPage(number));
    fisrtPage();
  }
  else
    showNotFound();
}

function fisrtPage() {
  $('#vt-pagination').children('.active').removeClass('active');
  $(`#vt-pagination .page-item:first-child`).addClass('active');
  getPage(1);
}

function getPage(page) {
  let obj;
  temp != null ? obj = temp : obj = datas;
  let start = (page - 1) * pageItems;
  let end = start + pageItems;
  showData(obj.slice(start, end));
}

function showData(list) { // Show View list Product and attach Event on View
  $('.dropdown').removeClass('vt-event-none');
  let container = $('#vt-container-data');
  container.empty();
  list.forEach(item => container.fadeIn(300).append(crtHTMLElement(item)));
}

function showNotFound() { // Show View Not Found
  $('#vt-pagination').hide();
  $('#vt-total').text(0);
  let container = $('#vt-container-data');
  container.empty();
  container.fadeIn(150).append(crtHTMLNotFound());
}

function showSearchResult(data) {
  $('#vt-loader-min').hide();
  var result = $('#result');
  result.empty();
  data.forEach(item => result.append(crtHTMLResult(item)))
}
// UI View
function crtHTMLElement(product) {
  let prom = product.promotion;
  let price = product.price;
  let isHide = prom == 0 ? "vt-hidden" : "";
  if (!isHide) {
    prom = new Intl.NumberFormat().format(Math.round(price * (1 - prom)));
    price = new Intl.NumberFormat().format(price);
  }
  else
    prom = new Intl.NumberFormat().format(price);
  return `<div class="col-lg-3 col-6">
            <div class="vt-product">
            <a href="/Product/ProductDetail?id=${product.id}">
                <div class="vt-product-img">
                    <img src="/product/${product.image}" alt="Image product">
                        <img class="${isHide}" src="/images/sale.png" />
                    </div>
                    <div class="vt-product-content">
                        <h5 class="vt-product-name pt-1">${product.name}</h5>
                        <div>
                            <p class="vt-price">${prom} đ</p>
                            <del class=${isHide} > ${price} đ</del>
                        </div>
                    </div>
                </a>
            </div>
          </div >`;
}

function crtHTMLResult(product) {
  let prom = product.promotion;
  let price = product.price;
  let isHide = prom == 0 ? "vt-hidden" : "";
  if (!isHide)
    prom = new Intl.NumberFormat().format(Math.round(price * (1 - prom)));
  else
    prom = new Intl.NumberFormat().format(price);
  return `<a class="vt-link mb-3" href="/Product/ProductDetail?id=${product.id}">
            <div class="d-flex vt-result-item">
                <div>
                    <img src="/product/${product.image}" width="100">
                </div>
                <div class="pl-4">
                    <p>${product.name}</p>
                    <span class="pr-4 vt-price">${prom} đ</span><del class="${isHide}">${price}</del>
                </div>
            </div>
          </a>`

}

function crtHTMLNotFound() {
  return `<div class="mx-auto text-center pt-5">
              <img class="rounded-circle" src="/images/img_null.jpg" width="150" alt="Not found">
              <p class="card-text pt-3">Not found :((</p>
          </div>`
}
// ============== ATTACH EVENT ===============

$(document).ready(() => {
  let req = $('#vt-res-js').text();
  if (!req)
    $("#loading").fadeOut(500);
  switch (req) {
    case 'promotions': prom(); break;
    case 'men': category(1); break;
    case 'women': category(2); break;
    case 'accessories': category(3); break;
  }
});

$("#products").ready(() => {
  //*******Add Event Filter*******/
  $("#select-wire").on("click", ".dropdown-item", function () {
    $("#vt-selectFilter").text($(this).text());
    onChangeFilter($(this).data("wire"))
  });
  //*******Add Event Filter Order*******/
  $("#select-order").on("click", ".dropdown-item", function () {
    $("#vt-orderFilter").text($(this).text());
    onChangeOrderBy($(this).data("order"));
  });
});

$("#vt-searchBar").on("keyup", function (e) {
  if (e.keyCode === 13) findWithKey($(this).val());
})

$('#result').hide();
$('#vt-loader-min').hide();









