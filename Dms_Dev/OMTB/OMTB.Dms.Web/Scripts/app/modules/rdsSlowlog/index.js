/**
 * Created by yongliang.jia on 2017/7/6.
 */

/*界面加载事件*/
$(document).ready(function() {
    getAppInfoList();

    //加载前一天慢日志数据
    var request = {
        AppName: "xiaobao",
        DateRange: $("#select_dateRangeList_rdsSlowLog").val()
    };
    var ajax = {
        url: "/RdsSlowLog/GetRdsSelectResult",
        data: JSON.stringify(request),
        type: "Post",
        dataType: "Json",
        contentType: "application/json;charset=utf-8",
        cache: false,
        success: function(result) {
            if (result.Success == true) {              
                var data = [];
                var columns = [{ data: '' }];
                var resultData = result.Data;
                if (resultData.length > 0) {
                    data = GetTableDataByResult_RdsSolwLog(resultData);
                    columns = GetTableColumnsByResult_RdsSolwLog(resultData);
                }
                SetTableHtml_RdsSolwLog(data,columns);
            }
        }
    };
    $.ajax(ajax);
});

/*点击查询事件*/
$("#btn_select_rdsSlowLog").click(function() {
    GetRdsSelectResult();
});

/*点击导出执行导出慢日志数据的事件*/
$("#btn_Export_rdsSlowLog").click(function(){
    ExportSqlSlowLogResultToExcel();
});

/*获取应用程序信息函数*/
var getAppInfoList = function() {
    var ajax = {
        url: "/RdsSlowLog/GetAppInfoList",
        type: "Get",
        dataType: "Json",
        success: function(result) {
            if (result.Success == true) {
                $.each(result.Data,
                    function(i) {
                        var temp = result.Data[i];
                        $("#select_appNameList_rdsSlowLog").append("<option>" + temp + "</option>");
                    });
            } else {
                var temp = "暂无可用应用程序";
                $("#select_appNameList_rdsSlowLog").append("<option>"+ temp +"</option>");
            }            
        }
    };
    $.ajax(ajax);
};

/*获取Rds慢日志查询结果函数*/
var GetRdsSelectResult = function() {
    var request = {
        AppName: $("#select_appNameList_rdsSlowLog").val(),
        DateRange: $("#select_dateRangeList_rdsSlowLog").val()
    };
    var ajax = {
        url: "/RdsSlowLog/GetRdsSelectResult",
        data: JSON.stringify(request),
        type: "Post",
        dataType: "Json",
        contentType: "application/json;charset=utf-8",
        cache: false,
        success: function(result) {
            if (result.Success == true) {              
                var data = [];
                var columns = [{ data: '' }];
                var resultData = result.Data;
                if (resultData.length > 0) {
                    data = GetTableDataByResult_RdsSolwLog(resultData);
                    columns = GetTableColumnsByResult_RdsSolwLog(resultData);
                }
                SetTableHtml_RdsSolwLog(data,columns);
                showSuccessMessage("查询成功");
            }
            if (result.Success == false) {
                showErrorMessage(result.State);
            }
        }
    };
    $.ajax(ajax);
};

/*通过后端数据获取tableData*/
var GetTableDataByResult_RdsSolwLog = function(resultData) {
    var data = resultData;
    $.each(resultData,
        function (i) {
            var propertyValue = resultData[i].CreateTime;
            var isDate = propertyValue.indexOf('/Date');
            resultData[i].CreateTime = isDate >= 0 
                ? formatDate(new Date(Date.parse(jsonDateFormat(propertyValue))), "yyyy-MM-dd")
                : propertyValue;
        });
    return data;
};

/*通过后端数据获取tableColumns*/
var GetTableColumnsByResult_RdsSolwLog = function(resultData) {
    var columns = [];
    $.each(resultData[0],
        function(i) {
            var temp = { data: i};
            columns.push(temp);
        });
    return columns;
};

/*设置table的Html*/
var SetTableHtml_RdsSolwLog = function(data,columns) {
    $("#table_selectList_rdsSlowLog").html();
    var thHtml = '';
    if (columns.length > 0) {
        $.each(columns,
            function(i) {
                thHtml = thHtml + "<th>" + columns[i].data + "</th>";
            });
        var htmltemp = '<table class="display" id="table_selectListResult_rdsSlowLog_01"><thead><tr>'+thHtml+'</tr></thead></table>';
    
        $("#table_selectList_rdsSlowLog").html(htmltemp);
    }
    BindDataToTable_RdsSolwLog(data, columns);
};

/*绑定数据到Table*/
var BindDataToTable_RdsSolwLog = function(data,columns)
{
    //当列数小于2时水平方向不滚动
    if (columns.length > 2) {
        $("#table_selectListResult_rdsSlowLog_01").dataTable({
            "scrollX": true,
            "scrollY": "600px",
            "scrollCollapse": "true",
            "paging": "false",
            "data": data,
            "columns": columns,
            "aLengthMenu": [[10,20,50,100,200,500,1000,-1],["10","20","50","100","200","500","1000","All"]]
        });
/*    var styleTempValue = $("#table_selectListResult_rdsSlowLog_01").attr("style") + " float: left;";
    $("#table_selectListResult_rdsSlowLog_01").attr("style", styleTempValue);*/
    }else {
        $("#table_selectListResult_rdsSlowLog_01").dataTable({
            "scrollY": "720px",
            "scrollCollapse": "true",
            "paging": "false",
            "data": data,
            "columns": columns,
            "aLengthMenu": [[10,20,50,100,200,500,1000,-1],["10","20","50","100","200","500","1000","All"]]
        });
    }
};

/*导出慢日志查询结果到Excel*/
var ExportSqlSlowLogResultToExcel = function () {
    var request = {
        AppName: $("#select_appNameList_rdsSlowLog").val(),
        DateRange: $("#select_dateRangeList_rdsSlowLog").val()
    };
    $.ajax({
        url: "/RdsSlowLog/ExportSqlSlowLogResultToExcel",
        data: JSON.stringify(request),
        type: "Post",
        dataType: "Json",
        contentType: "application/json;charset=utf-8",
        cache: false,
        success: function (result) {
            if (result.Success == true){
                location.href = result.Data.FileUrl + result.Data.FileName;
            }
            if (result.Success == false){
                showErrorMessage(result.State);
            }
        }
    });
};