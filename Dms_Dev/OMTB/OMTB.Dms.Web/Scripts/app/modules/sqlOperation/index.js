/**
 * Created by yongliang.jia on 2017/6/12.
 */


/*界面加载事件*/
$(document).ready(function() {
    getDbconnectInfoList();
});

/*点击执行事件*/
$("#btn_sqlExcute_sqlOperation").click(function() {
    GetSqlExcuteResult();
});

/*按下F8执行的事件*/
document.onkeydown = function(e) {
    var ev = document.all ? window.event : e;
    if (ev.keyCode == 119) {
        GetSqlExcuteResult();
    }
};

/*点击导出执行的事件*/
$("#btn_Export_sqlOperation").click(function() {
    ExportSqlExcuteResultToExcel();
});

/*获取数据库连接信息函数*/
var getDbconnectInfoList = function() {
    var ajax = {
        url: "/SqlOperation/GetDbconnectInfoList",
        type: "Get",
        dataType: "Json",
        success: function(result) {
            if (result.Success == true) {
                $.each(result.Data,
                    function(i) {
                        var temp = result.Data[i].DbConnectName;
                        $("#select_dbConnectList_sqlOperation").append("<option>" + temp + "</option>");
                    });
            } else {
                var temp = "暂无可用数据库";
                $("#select_dbConnectList_sqlOperation").append("<option>"+ temp +"</option>");
            }            
        }
    };
    $.ajax(ajax);
};

/*获取sql执行结果函数*/
var GetSqlExcuteResult = function() {
    var getSqlExcuteResultViewModel = {
        SqlString: $("#textarea_sqlString_sqlOperation").val(),
        DbConnectName: $("#select_dbConnectList_sqlOperation").val()
    };
    var ajax = {
        url: "/SqlOperation/GetSqlExcuteResult",
        data: JSON.stringify(getSqlExcuteResultViewModel),
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
                    data = GetTableDataByResult_sqlExcute(resultData);
                    columns = GetTableColumnsByResult_sqlExcute(resultData);
                }
                SetTableHtml_sqlExcute(data,columns);
                showSuccessMessage("执行成功");
            }
            if (result.Success == false) {
                showErrorMessage(result.State);
            }
        }
    };
    $.ajax(ajax);
};

/*通过后端数据获取tableData*/
var GetTableDataByResult_sqlExcute = function(resultData) {
    var data = [];
    $.each(resultData,
        function(i) {
            var temp = {};
            $.each(resultData[i],              
                function(j) {
                    var propertyValue = resultData[i][j].Value + "";
                    var isDate = propertyValue.indexOf('/Date');
                    temp[resultData[i][j].Key] = isDate >= 0 ? jsonDateFormat(propertyValue) : propertyValue;                    
                });
            data.push(temp);
        });
    return data;
};

/*通过后端数据获取tableColumns*/
var GetTableColumnsByResult_sqlExcute = function(resultData) {
    var columns = [];
    $.each(resultData[0],
        function(i) {
            var temp = { data: resultData[0][i].Key};
            columns.push(temp);
        });
    return columns;
};

/*设置table的Html*/
var SetTableHtml_sqlExcute = function(data,columns) {
    $("#table_sqlExcuteResult_sqlOpration").html();
    var thHtml = '';
    if (columns.length > 0) {
        $.each(columns,
            function(i) {
                thHtml = thHtml + "<th>" + columns[i].data + "</th>";
            });
        var htmltemp = '<table class="display" id="table_sqlExcuteResult_sqlOpration_01"><thead><tr>'+thHtml+'</tr></thead></table>';
    
        $("#table_sqlExcuteResult_sqlOpration").html(htmltemp);
    }
    BindDataToTable_sqlExcute(data, columns);
};

/*绑定数据到Table*/
var BindDataToTable_sqlExcute = function(data,columns)
{
    //当列数小于2时水平方向不滚动
    if (columns.length > 2) {
        $("#table_sqlExcuteResult_sqlOpration_01").dataTable({
            "scrollX": true,
            "scrollY": "370px",
            "scrollCollapse": "true",
            "paging": "false",
            "data": data,
            "columns": columns
        });
        var styleTempValue = $("#table_sqlExcuteResult_sqlOpration_01").attr("style") + " float: left;";
        $("#table_sqlExcuteResult_sqlOpration_01").attr("style", styleTempValue);
    } else {
        $("#table_sqlExcuteResult_sqlOpration_01").dataTable({
            "scrollY": "370px",
            "scrollCollapse": "true",
            "paging": "false",
            "data": data,
            "columns": columns
        });
    }
    
};

/*导出Sql执行返回结果到Excel*/
var ExportSqlExcuteResultToExcel = function() {
    var model = {
        SqlString: $("#textarea_sqlString_sqlOperation").val(),
        DbConnectName: $("#select_dbConnectList_sqlOperation").val()
    };
    var ajax = {
        url: "/SqlOperation/ExportSqlExcuteResultToExcel",
        data: JSON.stringify(model),
        type: "Post",
        dataType: "Json",
        contentType: "application/json;charset=utf-8",
        cache: false,
        success: function(result) {
            if (result.Success == true) {
                location.href = result.Data.FileUrl + result.Data.FileName;
            }
            if (result.Success == false) {
                showErrorMessage(result.State);
            }
        }
    };
    $.ajax(ajax);
};