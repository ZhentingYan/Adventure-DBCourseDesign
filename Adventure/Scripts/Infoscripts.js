
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
    $('.registration-form').on('submit', function(e) {
    	
    	$(this).find('input[type="text"], input[type="password"], textarea').each(function() {
    		if( $(this).val() == "" ) {
    			e.preventDefault();
    			$(this).addClass('input-error');
    		}
    		else {
    			$(this).removeClass('input-error');
			}
		});
				//用户名验证：(数字字母或下划线6到20位)
				var reUser = /^\w{6,20}$/;

				//邮箱验证：        
				var reMail = /^[a-z0-9][\w\.\-]*@[a-z0-9\-]+(\.[a-z]{2,5}){1,2}$/i;
		
				//密码验证：
				var rePass = /^[\w!@#$%^&*]{6,20}$/;
		
				//手机号码验证：
				var rePhone = /^1[34578]\d{9}$/;
	
			var originPasswd=$("#form-password").val();
			var repeatPasswd=$("#form-repeat-password").val();
			if(originPasswd!=repeatPasswd){
				alert("两次密码输入不一致！");
				$("#form-repeat-password").addClass('input-error');
				$("#form-password").addClass('input-error');
    			e.preventDefault();
			}
			else if(!reUser.test($("#form-username").val())){
				$("#form-username").addClass('input-error');
				alert("用户名输入有误！");
    			e.preventDefault();
			}else if(!rePhone.test($("#form-phoneNumber").val())){
				$("#form-phoneNumber").addClass('input-error');
				alert("手机号输入有误！");
    			e.preventDefault();
			}else if(!rePass.test($("#form-password").val())){
				alert("密码过短或过长！");
				$("#form-repeat-password").addClass('input-error');
				$("#form-password").addClass('input-error');
    			e.preventDefault();
			}else if(!reMail.test($("#form-email").val())){
				alert("邮箱不符合规则！");
				$("#form-email").addClass('input-error');
    			e.preventDefault();
			}
    });
    
	// submit

});
