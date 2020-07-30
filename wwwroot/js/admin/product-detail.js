$(function () {
    var arImages = [];
    var defaultImg = $('#vt-default-img');
    /*======== REQUEST ========*/
    function getImages() {
        let id = $("input[name='ID']").val();
        $.ajax({
            method: 'GET', url: '/Admin/Product/GetImages', data: { id: id },
            dataType: 'JSON'
        }).done(data => showImages(data));
    }

    function uploadImage(img) {
        let formData = new FormData();
        let imgName = img.name.split('.');
        formData.append('img', img, `product_${Date.now()}.${imgName[1]}`);
        $.ajax({
            method: 'POST',
            url: '/Admin/Product/UploadImage',
            data: formData,
            dataType: 'text',
            contentType: false,
            processData: false
        }).done(data => showImages(data));
    };

    /*======== DEFINE EVENT ON VIEW ========*/

    function onImgDefaulClick() {
        $('.vt-set-default').on('click', function () {
            let src = $(this).parent().prev().children('img').prop('src').split('/');
            defaultImg.prop('src', `/product/${src[src.length - 1]}`);
        });
    }

    function onImgDelClick() {
        $('.vt-delete-img').on('click', function () {
            let src = $(this).parent().prev().children('img').prop('src').split('/');
            let imgDefault = $('#vt-default-img');
            if (imgDefault.prop('src') === src) {
                imgDefault.prop('src', `/images/img_null.jpg`)
            }
            let parent = $(this).parents()[1];
            $(parent).fadeOut(300);
            parent.remove();
        });
    }

    /*======== ATTACH EVENT ========*/
    $(document).ready(() => getImages());

    $('.vt-gallery img').on("click", function (e) {
        let src = $(this).prop('src');
        $('#vt-photo-show').prop("src", src);
    });

    $('#vt-image-input').on('change', function () {
        uploadImage($(this).prop('files')[0]);
    });

    $('#vt-upload').on('click', () => $('#vt-image-input').click());

    $('#vt-form').on('submit', function (e) {
        let imgs = $('#vt-containter-img img');
        for (let index = 0; index < imgs.length; index++) {
            arImages.push($(imgs[index]).prop('src').split('/').pop())
        }
        console.log(arImages);
        $(`<input type='hidden' name='Images' value='${arImages.toString()}' />`).appendTo($(this));
        $(`<input type='hidden' name='Image' value='${defaultImg.prop('src').split('/').pop()}' />`).appendTo($(this));
        return true;
    });
    /*======== UPDATE VIEW ========*/
    function showImages(images) {
        let containter = $('#vt-containter-img');
        if (typeof (images) !== 'string') {
            arImages = [...images];
            images.forEach(img => containter.append(crtImageElement(img)).fadeIn(300));
        }
        else {
            arImages.push(images);
            containter.append(crtImageElement(images)).fadeIn(300);
        }
        onImgDefaulClick();
        onImgDelClick();
    }

    function crtImageElement(imgName) {
        return `<div class="col-3 p-1 pb-3">
                    <div class="vt-img mx-auto" style="width: 150px; height: 150px">
                        <img src="/product/${imgName}" style="height: 100%">
                    </div>
                    <div class="mx-auto d-flex justify-content-center">
                        <button class="vt-set-default btn btn-primary mdi mdi-tshirt-crew w-50 m-2"></button>
                        <button class="vt-delete-img btn btn-danger mdi mdi-delete w-50 m-2"></button>
                    </div>
                </div> `
    }

    onImgDefaulClick();
    onImgDelClick();
    /*======== END ========*/
    $('#loading').fadeOut(500);
})
