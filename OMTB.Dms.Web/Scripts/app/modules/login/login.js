/**
 * Created by yongliang.jia on 2017/6/9.
 */
var allowedGetSms = true;

/*点击获取验证码*/
$("#smsButton").click(function() {
    getSms();
});

/*点击登录按钮*/
$("#SignIn").click(function() {
    SignIn();
});

/*回车事件*/
document.onkeydown = function(e) {
    var ev = document.all ? window.event : e;
    if (ev.keyCode == 13) {
        SignIn();
    }
};

/*获取验证码*/
var getSms = function() {
    if (allowedGetSms == false) {
        return;
    }
    $("#smsCodeWarning").html('&nbsp;');
    $("#smsCodeWarning").css("display", "none"); 
    /*向后端post发送验证码短请求*/
    $.post("/Account/SendCode",
        {
            phone: $("#user_phone").val()
        },
        function() {
            againGetSms();
        });
};

/*重新获取*/
var againGetSms = function() {
    allowedGetSms = false;
    $("#smsButton").addClass("disabled");

    var remainingTime = 60;
    var interval = setInterval(function() {
        remainingTime--;
        $("#smsButton").text(remainingTime + 'S');
        if (remainingTime <= 0) {
            $("#smsButton").removeClass("disabled");
            $("#smsButton").text("重新获取");
            allowedGetSms = true;
            clearInterval(interval);
        }
    }, 1000);
};

/*登录验证*/
var SignIn = function() {
    var loginViewModel = {
        PhoneNumber: $("#user_phone").val(),
        Code: $("#user_validateCode").val()
    };
    var ajax = {
        url: "/Account/SignIn",
        data: JSON.stringify(loginViewModel),
        type: 'Post',
        dataType: 'json',
        contentType: "application/json;charset=utf-8",
        cache: false,
        success: function(result) {
            if (result == 1) {
                window.location.href = '/SqlOperation/Index';
            } else {
                $("#smsCodeWarning").show();
                $("#smsCodeWarning").html(getErrorMessage(result)); 
            }
        }
    };
    $.ajax(ajax);
};