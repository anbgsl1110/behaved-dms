using System.ComponentModel;

namespace OMTB.Dms.Common
{
    /// <summary>
    /// 服务请求状态枚举
    /// </summary>
    public enum ServiceStateEnum
    {
        /// <summary>
        /// 请求成功
        /// </summary>
        [Description("成功")]
        Success = 1,

        /// <summary>
        /// 请求失败
        /// </summary>
        [Description("失败")]
        Failed = 0,

        #region 通用

        /// <summary>
        /// 请求结果为空
        /// </summary>
        [Description("请求结果为空")]
        IsNull = 10001,

        /// <summary>
        /// 参数不能为空
        /// </summary>
        [Description("参数不能为空")]
        ParameterNotNull = 10002,
        
        /// <summary>
        /// 参数错误
        /// </summary>
        ParameterError = 10003,
        
        #endregion

        #region 登录

        /// <summary>
        /// 验证码错误
        /// </summary>
        [Description("验证码错误")]
        ValidateCodeError = 20001,

        /// <summary>
        /// 验证码不能为空
        /// </summary>
        [Description("验证码不能为空")]
        ValidateCodeNotNull = 20002,
        #endregion

        #region SQL操作

        /// <summary>
        /// Sql字符串不能为空或者空白
        /// </summary>
        [Description("Sql字符串不能为空或者空白")]
        SqlStringNotNullOrWhite = 30001,

        /// <summary>
        /// Sql语法错误
        /// </summary>
        [Description("Sql语法错误")]
        SqlSyntaxError = 30002,
        
        /// <summary>
        /// 包含限制命令
        /// </summary>
        [Description("包含限制命令")]
        ContainLimitCommand = 30003,
        
        /// <summary>
        /// 没有可导出数据
        /// </summary>
        [Description("没有可导出数据")]
        NoDataExported = 30004,
        
        /// <summary>
        /// 查询数据数量超过限制
        /// </summary>
        [Description("查询数据数量超过限制")]
        OverSelcetAmountLimit = 30005
        
        #endregion
    }
}