/**
 * Created by yongliang.jia on 2017/6/14.
 */

//格式化CST日期的字串
function formatCSTDate(strDate,format){
 return formatDate(new Date(strDate),format);
}
       
//格式化日期,
function formatDate(date, format) {
 var paddNum = function(num) {
  num += "";
  return num.replace(/^(\d)$/, "0$1");
 };
 //指定格式字符
 var cfg = {
  yyyy: date.getFullYear(), //年 : 4位
  yy: date.getFullYear().toString().substring(2), //年 : 2位
  M: date.getMonth() + 1, //月 : 如果1位的时候不补0
  MM: paddNum(date.getMonth() + 1), //月 : 如果1位的时候补0
  d: date.getDate(), //日 : 如果1位的时候不补0
  dd: paddNum(date.getDate()), //日 : 如果1位的时候补0
  hh: date.getHours(), //时
  mm: date.getMinutes(), //分
  ss: date.getSeconds() //秒
 };
 format || (format = "yyyy-MM-dd hh:mm:ss");
 return format.replace(/([a-z])(\1)*/ig, function(m) { return cfg[m]; });
}

function jsonDateFormat(jsonDate) {//json日期格式转换为正常格式
 try {//出自http://www.cnblogs.com/ahjesus 尊重作者辛苦劳动成果,转载请注明出处,谢谢!
  var date = new Date(parseInt(jsonDate.replace("/Date(", "").replace(")/", ""), 10));
  var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
  var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
  var hours = date.getHours();
  var minutes = date.getMinutes();
  var seconds = date.getSeconds();
  //var milliseconds = date.getMilliseconds();
  return date.getFullYear() + "-" + month + "-" + day + " " + hours + ":" + minutes + ":" + seconds;// + "." + milliseconds;
 } catch (ex) {//出自http://www.cnblogs.com/ahjesus 尊重作者辛苦劳动成果,转载请注明出处,谢谢!
  return "";
 }
}