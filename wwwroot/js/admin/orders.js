$(function () {
    /*======== VARIABLE ========*/
    var pageItems = 10;
    // Color for dropdown
    var dropDownColor = ['btn-danger', 'btn-success', 'btn-info', 'btn-primary', 'btn-secondary'];
    var statusOrder = ['Chưa xác nhận', 'Đã xác nhận', 'Đang giao', 'Hoàn thành', 'Huỷ'];
    // Date defautl
    var start = moment().subtract(30, 'days');
    var end = moment();
    var datas = [];
    var temp;

    /*======== GET DATA ========*/
    function findWithKey(key) {
        preHandler();
        $.ajax({
            method: "GET", url: '/Admin/Order/Find',
            data: { key: key },
            dataType: "JSON"
        }).done(data => requestSuccess(data))
    };

    function getData(fromDate, toDate) {
        preHandler();
        $.ajax({
            method: "GET", url: '/Admin/Order/Orders',
            data: { start: fromDate.format("L"), end: toDate.format("L") },
            dataType: "JSON"
        }).done(data => requestSuccess(data));
    }

    function getDetail(id) {
        $.ajax({
            method: 'GET', url: "/Admin/Order/Detail",
            data: { id: id },
            dataType: 'JSON',
        }).done(data => showModelDetail(data))
    }

    function updateStatus(orderID, index) {
        $.ajax({
            method: "POST", url: '/Admin/Order/UpdateStatus',
            data: { orderID: orderID, index: index + 1 },
            dataType: "JSON"
        });
    }

    /*======== DEFINE EVENT ON VIEW ========*/
    var onChangeDate = function (start, end) {
        $('#reportrange span').html(start.format(dateFormter) + ' - ' + end.format(dateFormter));
        getData(start, end);
    }

    var onDropDownChange = function (idDropDown, handler) {
        $(`div[aria-labelledby="${idDropDown}"] > .dropdown-item`).on('click', function () {
            let el = $(this);
            let index = el.data('index');
            let parents = el.parents();
            let elPrev = $(parents[0]).prev();
            let delClass = elPrev.get(0).classList[4];
            elPrev.removeClass(delClass).addClass(`${dropDownColor[index]}`).text(el.text());
            handler($(parents[2]).prev().children().children(':first-child').text(), index);
        });
    }

    var onShowModal = function (handler) {
        $('.vt-modal-open').on('click', function (e) {
            handler($(this).children(':first-child').text());
        });
    }

    /*======== ATTACH EVENT ========*/
    // Attach event on change select date
    $('#reportrange').daterangepicker({
        startDate: start,
        endDate: end,
        ranges: {
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        }
    }, onChangeDate);

    // Attach event search
    $('#search-input').on('keydown', function (e) {
        if (e.keyCode === 13) findWithKey($(this).val());
    });

    // Attach event for Order with cost
    $('#vt-orderby-cost .dropdown-item').on('click', function () {
        let index = $(this).data('index');
        if (index == 0)
            datas.sort((a, b) => a.total - b.total);
        else
            datas.sort((a, b) => b.total - a.total);
        fisrtPage();
    });

    // Attach event for Order with status
    $('#vt-orderby-status .dropdown-item').on('click', function () {
        let index = $(this).data('index');
        if (index == 0) {
            temp = null;
            displayData(datas);
        }
        else {
            temp = datas.filter(item => item.status == index);
            displayData(temp);
        }
    });

    // Attach event for Export func
    $('#vt-export').on('click', function (e) {
        console.log("Exporting...");
    });

    /*======== UPDATE VIEW ========*/


    function preHandler() {
        startLoader();
    }

    function requestSuccess(data) {
        stopLoader();
        $('#search-input').val('');
        if (data != null && data.length > 0) {
            datas = data;
            displayData(datas);
        }
        else
            showNotFound();
    }

    // Display data on get new data.
    function displayData(data) {
        let len = data.length
        $('#vt-total').text(len);
        $('#vt-pagination').show();
        showPages(len, pageItems, number => getPage(number));
        fisrtPage();
    }

    function fisrtPage() {
        firstPageOnlyView();
        getPage(1);
    }

    function getPage(page) {
        let obj;
        temp != null ? obj = temp : obj = datas;
        let start = (page - 1) * pageItems;
        let end = start + (pageItems - 1);
        showData(obj.slice(start, end));
    }

    // Show list Order and attach Event on View
    function showData(list) {
        $('#vt-data .dropdown').removeClass('vt-event-none');
        let container = $('#vt-container-data');
        container.empty();
        list.forEach(item => container.fadeIn(300).append(crtHTMLElement(item)));
        // Attach event change status
        onDropDownChange('dropdownChangeStatus', updateStatus)
        // Attach event show order detail
        onShowModal(getDetail);
    }

    function showNotFound() {
        $('#vt-pagination').hide();
        $('#vt-total').text(0);
        let container = $('#vt-container-data');
        container.empty();
        container.fadeIn(150).append(crtHTMLNotFound());
        $('#vt-data .dropdown').addClass('vt-event-none');
    }

    function formatDateTime(datetime) {
        let arDate = datetime.split('T');
        let time = arDate[1].split(':');
        let date = arDate[0].split('-');
        return `${date[2]}/${date[1]}/${date[0]}, ${time[0]}:${time[1]}`
    }

    // Create HTML Elemetn for each Orders
    function crtHTMLElement(order) {
        let status = order.status - 1;
        return `<div class="row mb-2">
                    <div class="col-md-10 col-8">
                        <div class="row vt-modal-open" data-toggle="modal" data-target="#orderModal">
                            <div class="col-md-2 col-4">${order.id}</div>
                            <div class="col-md-2 vt-d-none">${formatDateTime(order.dateCreated)}</div>
                            <div class="col-md-4 col-8">${order.customer}</div>
                            <div class="col-md-2 vt-d-none">${order.phone}</div>
                            <div class="col-md-2 vt-d-none" >${order.total}</div>
                        </div>
                    </div>   
                    <div class="col-md-2 col-4">
                        <div class="dropdown d-inline-block mb-1">
                            <button class="btn dropdown-toggle btn-sm vt-dropdown-w ${dropDownColor[status]} "
                            type="button" id="dropdownChangeStatus" 
                            data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" data-display="static">
                                ${statusOrder[status]}
                            </button>
                            <div class="dropdown-menu" aria-labelledby="dropdownChangeStatus">
                                <a class="dropdown-item" data-index="0">Chưa xác nhận</a>
                                <a class="dropdown-item" data-index="1">Đã xác nhận</a>
                                <a class="dropdown-item" data-index="2">Đang giao</a>
                                <a class="dropdown-item" data-index="3">Hoàn thành</a>
                                <a class="dropdown-item" data-index="4">Huỷ</a>
                            </div>
                        </div> 
                    </div>
                </div>`
    }

    function crtHTMLNotFound() {
        return `<div class="mx-auto text-center pt-5">
                    <img class="rounded-circle" src="/images/img_null.jpg" width="150" alt="Not found">
                    <p class="card-text pt-3">Not found :((</p>
                </div>`
    }
    // Show Order Detail 
    function showModelDetail(order) {
        let sum = order.products.reduce((sum, item) => sum + ((item.price * item.quantity) * (1 + item.promotion)), 0);
        $('#vt-order-id').text(order.id);
        $('#vt-order-status').text(statusOrder[order.status]);
        $('#vt-order-name').text(order.customer)
        $('#vt-order-date').text(formatDateTime(order.dateCreated))
        $('#vt-order-phone').text(order.phone)
        $('#vt-order-address').text(order.address)
        $('#vt-order-note').text(order.note)
        $('#vt-order-total').text(parseInt(sum))
        $('#vt-order-discountBill').text(order.billPromotion)
        $('#vt-order-ship').text(order.transportFee)
        $('#vt-order-pay').text(order.total)
        $('#vt-order-products').empty();
        let ar = order.products;
        for (let i = 0; i < ar.length; i++) {
            let str =
                `<tr>
            <td scope="row">${i + 1}</td>
            <td class="text-center">
                <div class="vt-img mx-auto" style="width:50px">
                    <img src="/product/${ar[i].image}" alt="Product Image">
                </div>
                <p class="pt-1">${ar[i].productName}</p>
            </td>
            <td>${ar[i].quantity}</td>
            <td>${ar[i].price}</td>
            <td>${ar[i].promotion}</td>
        </tr>`;
            $('#vt-order-products').fadeIn(300).append(str);
        }
    }

    /*======== Execute========*/
    onChangeDate(start, end);
    /*======== END ========*/
});

