using System;
using OMTB.Component.Transform;
using OMTB.Dms.Common;
using OMTB.Dms.Dto.Model;

namespace OMTB.Dms.Dto.Transform
{
    /// <summary>
    /// 通过应用名称获取慢日志参数转化解组对象
    /// </summary>
    public class DescriptionGetSqlSlowLogByAppNameUmarshaller
    {
        public static DescriptionGetSqlSlowLogByAppNameResponse Unmarshall(UnmarshallerContext context)
        {
            DescriptionGetSqlSlowLogByAppNameResponse response = new DescriptionGetSqlSlowLogByAppNameResponse();
            response.RequestId = new Guid().ToString();
            response.AppName = context.StringValue("GetSqlSlowLogByAppName.AppName");
            response.DateRange = context.EnumValue<DateRangeEnum>("GetSqlSlowLogByAppName.DateRange");

            return response;
        }
    }
}