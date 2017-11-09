using OMTB.Component.Transform;
using OMTB.Dms.Dto.Model;

namespace OMTB.Dms.Dto.Transform
{
    /// <summary>
    /// 通过筛选参数获取慢日志参数转化解组对象
    /// </summary>
    public class DescriptionGetSqlSlowLogByParameterUnmarshaller
    {
        public static DescriptionGetSqlSlowLogByParameterResponse Unmarshall(UnmarshallerContext context)
        {
            DescriptionGetSqlSlowLogByParameterResponse response = new DescriptionGetSqlSlowLogByParameterResponse();
            response.AppName = context.StringValue("GetSqlSlowLogByParameter.AppName");
            response.StarTime = context.StringValue("GetSqlSlowLogByParameter.StartTime");
            response.EndTime = context.StringValue("GetSqlSlowLogByParameter.EndTime");

            return response;
        }
    }
}