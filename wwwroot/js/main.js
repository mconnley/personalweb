jQuery(document).ready(function($) {


	var mastheadheight = $('.ds-header').outerHeight();
	console.log(mastheadheight);
	if (mastheadheight >= 180) {
		$(".ds-banner,.ds-main-section").css("margin-top" , mastheadheight);
	}
	else {
		$(".ds-banner,.ds-main-section").css("margin-top" , 180);
	}

	$(window).scroll(function(){
	    if ($(window).scrollTop() >= 10) {
	        $('.ds-header').addClass('ds-fixed-header');
	    }
	    else {
	        $('.ds-header').removeClass('ds-fixed-header');
	    }
	}).scroll();


});