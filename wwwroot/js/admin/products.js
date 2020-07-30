$(function () {
    var indexCate = -1;
    var indexWire = -1;
    var pageItems = 7;
    var datas = [];
    var temp;
    // ============== Request ===============
    function getData(cate) {
        preHandler();
        $.ajax({
            method: 'GET', url: '/Admin/Product/Products',
            data: { indexCate: cate },
            dateType: 'JSON'
        }).done(data => requestSuccess(data));
    }

    function updateStatus(productID, index) {
        $.ajax({
            method: 'POST',
            url: '/Admin/Product/UpdateStatus',
            data: { productID: productID, indexStatus: index }
        });
    }

    // ============== Handler ===============
    //Define evnet
    var onChangeSwitch = function (handler) {
        $('.switch').on('change', function () {
            let el = $(this);
            let indexStatus = el.children('.switch-input').prop('checked');
            let id = el.parent().prev().children(':first-child').children(':first-child').children(':first-child').text();
            handler(id, indexStatus ? 1 : 0);
        });
    }

    // Event
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

    function findProduct(key) {
        temp = [];
        temp = datas.filter(item => item.name.includes(key));
        if (temp == null) showNotFound();
        displayData(temp);
    }

    // Support
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

    function showData(list) { // Show View list Product and attach Event on View

        $('#vt-data .dropdown').removeClass('vt-event-none');
        let container = $('#vt-container-data');
        container.empty();
        list.forEach(item => container.fadeIn(300).append(crtHTMLElement(item)));
        // Attach event change status
        onChangeSwitch(updateStatus);
    }

    function showNotFound() { // Show View Not Found
        $('#vt-pagination').hide();
        $('#vt-total').text(0);
        let container = $('#vt-container-data');
        container.empty();
        container.fadeIn(150).append(crtHTMLNotFound());
        $('#vt-data .dropdown').addClass('vt-event-none');
    }

    function findCategoryName(index) { // Get Name Category for Create Product Item
        return $(`#vt-filter-cate .dropdown-item:nth-child(${index + 1})`).text();
    }

    function findWireName(index) { // Get Name Wire for Create Product Item
        return $(`#vt-filter-wire .dropdown-item:nth-child(${index + 1})`).text();
    }
    // UI View
    function startLoader() {
        let el = $('#vt-loader');
        el.show();
        el.parent().addClass('vt-event-none');
    }

    function stopLoader() {
        let el = $('#vt-loader');
        el.hide();
        el.parent().removeClass('vt-event-none');
    }

    function crtHTMLElement(product) {
        let checked = product.show ? 'checked' : '';
        return ` <div class="row mb-5 align-items-center ">
                    <div class="col-10">
                        <a href="/Admin/Product/Detail?id=${product.id}">
                            <div class="row vt-item align-items-center text-dark">
                                <div class="col-md-1 col-4">${product.id}</div>
                                <div class="col-md-2 vt-d-none">
                                    <div class="vt-img" style="width:100px">
                                        <img src="/product/${product.image}">
                                    </div>
                                </div>
                                <div class="col-md-2 col-4">${product.name}</div>
                                <div class="col-md-2 vt-d-none" >${findCategoryName(product.categoryID)}</div>
                                <div class="col-md-2 vt-d-none" >${findWireName(product.typeWireID)}</div>
                                <div class="col-md-1 col-4">${product.price}</div>
                                <div class="col-md-1 vt-d-none" >${product.promotion} %</div>
                                <div class="col-md-1 vt-d-none" >${product.saleCount}</div>
                            </div>
                        </a>
                    </div>   
                    <div class="col-2 text-center vt-custom">
                        <label class="switch switch-icon switch-primary switch-pill vt-switch-sm form-control-label">
                            <input type="checkbox" class="switch-input form-check-input" ${checked} >
                            <span class="switch-label"></span>
                            <span class="switch-handle"></span>
                        </label>
                    </div>
                </div>`
    }

    // ============== ATTACH EVENT ===============

    $('#vt-filter-cate .dropdown-item').on('click', function () {
        indexCate = $(this).data('index');
        temp = []
        temp = datas.filter(item => item.categoryID == indexCate);
        if (temp == null) showNotFound();
        displayData(temp);
    });

    $('#vt-filter-wire .dropdown-item').on('click', function () {
        indexWire = $(this).data('index');
        temp = []
        temp = datas.filter(item => item.typeWireID == indexWire);
        if (temp == null) showNotFound();
        displayData(temp)
    });

    $('#vt-orderby-sale .dropdown-item').on('click', function () {
        let index = $(this).data('index');
        if (index == 0)
            datas.sort((a, b) => a.saleCount - b.saleCount);
        else
            datas.sort((a, b) => b.saleCount - a.saleCount);
        fisrtPage();
    });

    $('#vt-orderby-status .dropdown-item').on('click', function () {
        let index = $(this).data('index');
        if (index == 0)
            datas.sort((a, b) => a.status - b.status);
        else
            datas.sort((a, b) => b.status - a.status);
        fisrtPage();
    });

    $('#search-input').on('keydown', function (e) {
        if (e.keyCode == 13) findProduct($(this).val());
        if (e.keyCode == 46 && $(this).val() == '') {
            displayData(datas);
        }
    })

    // ============== END ===============
    getData();
    $('#loading').fadeOut(500);
})
