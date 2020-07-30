$(document).ready(function () {
	var indexNav = $("#sidebar-menu").data("nav");
	$(`#sidebar-menu li:eq(${indexNav})`).addClass("active");
	$("#loading").fadeOut(500);
});

/*======== TREAT REQUEST ========*/
$(document).ajaxError((...param) => {
	console.log("***Error: " + param[3] + " => " + param[2].url);
	console.log(param[2]);
});

$(document).ajaxStart(() => {
	// $("#loading").fadeIn(500);
})

$(document).ajaxComplete((...param) => {
	console.log("Request: " + param[2].url);
})

$(document).ajaxStop(() => {
	// $("#loading").fadeOut(500);
})

var dateFormter = 'DD/MM/YYYY';

/*======== PAGINATION ========*/
function calPages(length, pageItems) {
	if (length % pageItems != 0) return parseInt(length / pageItems) + 1;
	return length / pageItems;
}

function onChangePage(handler) {
	$('.page-link').on('click', function () {
		$("html,body").animate({ scrollTop: "0" }, 500);
		$('#vt-pagination').children('.active').removeClass('active');
		$(this).parent().addClass('active');
		handler($(this).text());
	});
}

var showPages = function (length, pageItems, handler) {
	let containter = $('#vt-pagination');
	containter.empty();
	let pages = calPages(length, pageItems);
	for (let index = 1; index <= pages; index++) {
		containter.append(`<li class="page-item"><a class="page-link" href="#">${index}</a></li>`);
	}
	//Attach when click page
	onChangePage(handler);
}

var firstPageOnlyView = () => {
	$('#vt-pagination').children('.active').removeClass('active');
	$(`#vt-pagination .page-item:first-child`).addClass('active');
}

var startLoader = () => {
	let el = $('#vt-loader');
	el.show();
	el.parent().addClass('vt-event-none');
}

var stopLoader = () => {
	let el = $('#vt-loader');
	el.hide();
	el.parent().removeClass('vt-event-none');
}


/*======== 5. TOASTER ========*/
	// function callToaster(positionClass) {
	//   if (document.getElementById("toaster")) {
	// 	toastr.options = {
	// 	  closeButton: true,
	// 	  debug: false,
	// 	  newestOnTop: false,
	// 	  progressBar: true,
	// 	  positionClass: positionClass,
	// 	  preventDuplicates: false,
	// 	  onclick: null,
	// 	  showDuration: "300",
	// 	  hideDuration: "1000",
	// 	  timeOut: "5000",
	// 	  extendedTimeOut: "1000",
	// 	  showEasing: "swing",
	// 	  hideEasing: "linear",
	// 	  showMethod: "fadeIn",
	// 	  hideMethod: "fadeOut"
	// 	};
	// 	toastr.success("Welcome to sleek", "Howdy!");
	//   }
	// }

	// if (document.dir != "rtl" ){
	//   callToaster("toast-top-right");
	// }else {
	//   callToaster("toast-top-left");
	// }




