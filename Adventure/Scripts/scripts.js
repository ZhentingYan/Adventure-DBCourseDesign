
jQuery(document).ready(function() {
	
    /*
        Fullscreen background
    */
    $.backstretch("assets/img/backgrounds/1.jpg");
    
    $('#top-navbar-1').on('shown.bs.collapse', function(){
    	$.backstretch("resize");
    });
    $('#top-navbar-1').on('hidden.bs.collapse', function(){
    	$.backstretch("resize");
    });
    
    /*
        Form
    */
    $('.registration-form fieldset:first-child').fadeIn('slow');
    
    $('.registration-form input[type="text"], .registration-form input[type="password"], .registration-form textarea').on('focus', function() {
    	$(this).removeClass('input-error');
    });
    
    // next step
    $('.registration-form .btn-next').on('click', function() {
    	var parent_fieldset = $(this).parents('fieldset');
    	var next_step = true;
    	
    	parent_fieldset.find('input[type="text"], input[type="password"], textarea').each(function() {
    		if( $(this).val() == "" ) {
    			$(this).addClass('input-error');
    			next_step = false;
    		}
    		else {
    			$(this).removeClass('input-error');
    		}
    	});
    	
    	if( next_step ) {
    		parent_fieldset.fadeOut(400, function() {
	    		$(this).next().fadeIn();
	    	});
    	}
    	
    });
    
    // previous step
    $('.registration-form .btn-previous').on('click', function() {
    	$(this).parents('fieldset').fadeOut(400, function() {
    		$(this).prev().fadeIn();
    	});
    });
	$('.registration-form .btn-register').on('submit', function(e) {
    	var next_step=true;
    	$(this).find('input[type="text"], input[type="password"], textarea').each(function() {
    		if( $(this).val() == "" ) {
    			e.preventDefault();
				$(this).addClass('input-error');
				next_step=false;
    		}
    		else {
    			$(this).removeClass('input-error');
    		}
		});
    });
    // submit
    $('.registration-form .btn-login').on('submit', function(e) {
    	var next_step=true;
    	$(this).find('input[type="text"], input[type="password"], textarea').each(function() {
    		if( $(this).val() == "" ) {
    			e.preventDefault();
				$(this).addClass('input-error');
				next_step=false;
    		}
    		else {
    			$(this).removeClass('input-error');
    		}
		});
		if(next_step){
			$.ajax({
				type: "post",
				url: "Login/CheckLogin",
				data: { 
					userID: $("#form-username").val(), 
					userpwd: $("#form-password").val(), 
					},
				async: false,
				dataType: 'json',
				success: function (d) {
					if (d.Result == '0') {//登录成功  
						//location.href = 'DataMain.aspx?logper=' + $("#ipt_username").find("option:selected").text();
						alert("登陆成功！");
						//location.href = 'Home/Index';
					}
					else {
						//登录失败                
						alert(d.Message);
					}
				},
				error: function (a) { console.log(a); }
			});
		}
    });
    
    
});
