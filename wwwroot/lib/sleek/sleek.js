/************ Event Of Sleek ***************/

/*======== MOBILE OVERLAY ========*/
if ($(window).width() < 768) {
	$(".sidebar-toggle").on("click", function () {
	$("body").css("overflow", "hidden");
	$('body').prepend('<div class="mobile-sticky-body-overlay"></div>');
	});

	$(document).on("click", '.mobile-sticky-body-overlay', function (e) {
	$(this).remove();
	$("#body").removeClass("sidebar-mobile-in").addClass("sidebar-mobile-out");
	$("body").css("overflow", "auto");
	});
}
/*======== SIDEBAR TOGGLE FOR MOBILE ========*/
if ($(window).width() < 768) {
	$(document).on("click", ".sidebar-toggle", function (e) {
	e.preventDefault();
	var min = "sidebar-mobile-in",
		min_out = "sidebar-mobile-out",
		body = "#body";
	$(body).hasClass(min)
		? $(body)
		.removeClass(min)
		.addClass(min_out)
		: $(body)
		.addClass(min)
		.removeClass(min_out);
	});
}
/*======== SIDEBAR TOGGLE FOR VARIOUS SIDEBAR LAYOUT ========*/
var body = $("#body");
if ($(window).width() >= 768) {

	if (typeof window.isMinified === "undefined") {
	window.isMinified = false;
	}
	if (typeof window.isCollapsed === "undefined") {
	window.isCollapsed = false;
	}

	$("#sidebar-toggler").on("click", function () {
	if (
		body.hasClass("sidebar-fixed-offcanvas") ||
		body.hasClass("sidebar-static-offcanvas")
	) {
		$(this)
		.addClass("sidebar-offcanvas-toggle")
		.removeClass("sidebar-toggle");
		if (window.isCollapsed === false) {
		body.addClass("sidebar-collapse");
		window.isCollapsed = true;
		window.isMinified = false;
		} else {
		body.removeClass("sidebar-collapse");
		body.addClass("sidebar-collapse-out");
		setTimeout(function () {
			body.removeClass("sidebar-collapse-out");
		}, 300);
		window.isCollapsed = false;
		}
	}

	if (
		body.hasClass("sidebar-fixed") ||
		body.hasClass("sidebar-static")
	) {
		$(this)
		.addClass("sidebar-toggle")
		.removeClass("sidebar-offcanvas-toggle");
		if (window.isMinified === false) {
		body
			.removeClass("sidebar-collapse sidebar-minified-out")
			.addClass("sidebar-minified");
		window.isMinified = true;
		window.isCollapsed = false;
		} else {
		body.removeClass("sidebar-minified");
		body.addClass("sidebar-minified-out");
		window.isMinified = false;
		}
	}
	});
}

if ($(window).width() >= 768 && $(window).width() < 992) {
	if (
	body.hasClass("sidebar-fixed") ||
	body.hasClass("sidebar-static")
	) {
	body
		.removeClass("sidebar-collapse sidebar-minified-out")
		.addClass("sidebar-minified");
	window.isMinified = true;
	}
}
