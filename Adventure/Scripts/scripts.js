
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
		if(next_step){
			var originPasswd=$("#form-password").val();
			var repeatPasswd=$("#form-repeat-password").val();

			if(originPasswd!=repeatPasswd)
				alert("两次密码输入不一致！");
			else if($("#form-username").val().length<5)
				alert("用户名输入太短！");
			else{
				alert("注册提交了");

			$.ajax({
				type: "post",
				url: "Register/registerCheck",
				data: { 
					userID: $("#form-username").val(), 
					userpwd: $("#form-password").val(), 
					userIntroduction:$("#form-about-yourself").val(), 
					userFirstName:$("#form-first-name").val(),
					userLastName:$("#form-last-name").val(),  
					userMail:$("#form-email").val(),  
					userPhoneNum:$("#form-phoneNumber").val(),
					userCountry:$("#form-country").val(),
					userGender:$("#form-gender").val()
					},
				async: false,
				dataType: 'json',
				success: function (d) {
					if (d.Result == '0') {//登录成功  
						//location.href = 'DataMain.aspx?logper=' + $("#ipt_username").find("option:selected").text();
						alert("注册成功！");
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
		}
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
