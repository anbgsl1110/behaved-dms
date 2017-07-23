using System.ComponentModel;

namespace OMTB.Dms.Common
{
    /// <summary>
    /// 日期范围枚举
    /// </summary>
    public enum DateRangeEnum
    {
        [Description("最近一天")]
        OneDaY = 1,
        
        [Description("最近三天")]
        ThreeDay = 3,
        
        [Description("最近七天")]
        SevenDay = 7
    }
}