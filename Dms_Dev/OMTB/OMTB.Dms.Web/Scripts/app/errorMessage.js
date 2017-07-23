/**
 * Created by yongliang.jia on 2017/6/14.
 */

var errorMessage = {
    1: "请求成功",
    0: "请求失败",
    
    //通用 10001开头
    10001: "请求结果为空",
    10002: "执行的SQL语句不能为空",
    10003: "参数错误",
    
    //登录 20001开头
    20001: "验证码错误",
    20002: "验证码不能为空",
    
    //Sql操作 30001开头
    30001: "Sql字符串不能为空或者空白",
    30002: "Sql语法错误",
    30003: "包含限制命令",
    30004: "有相同名称的列或者没有可导出数据",
    30005: "查询数据数量超过限制"
};

/*根据错误枚举获取错误信息*/
var getErrorMessage = function(errorCode) {
    return errorMessage[errorCode];
};

/*显示错误消息提示窗体*/
var showErrorMessage = function(errorcode) {
    if ($('#errorMessageModal').length > 0) {
        $('#errorMessageModal').remove();
    }
    var errorMessageModalHtml = '<div class="modal fade" id="errorMessageModal" tabindex="-1" role="dialog">'+
        '<div class="modal-dialog" role="document">'+
        '<div class="modal-content">'+
        '<div class="modal-header">'+
        '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>'+
        '<h4 class="modal-title">错误提示消息</h4>'+
        '</div>'+
        '<div class="modal-body">'+
        '<p style="color: red">'+getErrorMessage(errorcode)+'&hellip;</p>'+
        '</div>'+
        '</div><!-- /.modal-content -->'+
        '</div><!-- /.modal-dialog -->'+
        '</div><!-- /.modal -->';
    $("body").append(errorMessageModalHtml);    
    $('#errorMessageModal').modal('show');
};

/*显示错误消息提示窗体*/
var showSuccessMessage = function(message) {
    if ($('#successMessageModal').length > 0) {
        $('#successMessageModal').remove();
    }
    var successMessageModalHtml = '<div class="modal fade" id="successMessageModal" tabindex="-1" role="dialog">'+
        '<div class="modal-dialog" role="document">'+
        '<div class="modal-content">'+
        '<div class="modal-header">'+
        '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>'+
        '<h4 class="modal-title">操作结果</h4>'+
        '</div>'+
        '<div class="modal-body">'+
        '<p style="color: green">'+message+'&hellip;</p>'+
        '</div>'+
        '</div><!-- /.modal-content -->'+
        '</div><!-- /.modal-dialog -->'+
        '</div><!-- /.modal -->';
    $("body").append(successMessageModalHtml);    
    $('#successMessageModal').modal('show');
};