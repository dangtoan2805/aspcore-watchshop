$(function () {
    var pageItems = 10;
    var datas = [];
    var temp;
    var product = [];
    /*======== Request ========*/
    function getData() {
        preHandler();
        $.ajax({
            method: 'GET', url: '/Admin/Promotion/GetPromotion',
            dateType: 'JSON'
        }).done(data => requestSuccess(data));
    }

    /*======== ATTACH EVENT ========*/
    // $('#search-input').on('keydown', function (e) {
    //     if (e.keyCode == 13) findCustomter($(this).val());
    //     if (e.keyCode == 46 && $(this).val() == '') {
    //         displayData(datas);
    //     }
    // })

    /*======== UPDATE VIEW ========*/
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

    function preHandler() {
        startLoader();
    }

    function requestSuccess(data) {
        stopLoader();
        // $('#search-input').val('');
        datas = data;
        if (datas != null && data.length > 0) {
            product = data.map(item => item.id);
            displayData(datas);
        }
        else
            showNotFound();
    }

    function displayData(data) {
        let len = data.length
        $('#vt-total').text(len);
        $('#vt-pagination').show();
        showPages(len, pageItems, number => getPage(number));
        fisrtPage();
    }

    function getPage(page) {
        let obj = datas;
        let start = (page - 1) * pageItems;
        let end = start + (pageItems - 1);
        showData(obj.slice(start, end));
    }

    function fisrtPage() {
        firstPageOnlyView();
        getPage(1);
    }

    // function findCustomter(key) {
    //     temp = datas.filter(item => item.phone.includes(key) || item.name.includes(key));
    //     if (temp == null) showNotFound();
    //     displayData(temp);
    // }

    function showNotFound() {
        $('#vt-pagination').hide();
        $('#vt-total').text(0);
        let container = $('#vt-container-data');
        container.empty();
        container.fadeIn(150).append(crtHTMLNotFound());
        $('#vt-data .dropdown').addClass('vt-event-none');
    }

    function showData(data) {
        let container = $('#vt-container-data');
        container.empty();
        data.forEach(item => container.fadeIn(300).append(crtHTMLElement(item)));
    }


    function crtHTMLElement(prom) {
        let checked = prom.status ? 'checked' : '';
        return `<tr>
                    
                    <td>${prom.id}</td>
                    <td>
                        <a class="text-dark" href="/Admin/Promotion/Detail?promID=${prom.id}">
                        ${prom.name}
                        </a>
                    </td>
                    <td class="d-none d-lg-table-cell">${prom.fromDate}</td>
                    <td class="d-none d-lg-table-cell">${prom.toDate}</td>
                    <td>${ parseInt(prom.discount * 100)} %</td>
                    <td class="d-none d-lg-table-cell">
                        <label class="switch switch-icon switch-primary switch-pill form-control-label">
                            <input type="checkbox" class="switch-input form-check-input" ${checked}>
                            <span class="switch-label"></span>
                            <span class="switch-handle"></span>
                        </label>
                    </td>  
                </tr>`
    }

    getData();
    // Show page and attach Event on View
    $('#loading').fadeOut(500);
});