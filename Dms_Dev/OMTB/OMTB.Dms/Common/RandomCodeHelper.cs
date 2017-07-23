using System;

namespace OMTB.Dms.Common
{
    /// <summary>
    /// 验证码帮助类
    /// </summary>
    public static class RandomCodeHelper
    {
        static readonly Random Random = new Random();
        /// <summary>
        /// 生成默认六位随机码
        /// </summary>
        /// <returns></returns>
        public static string NewRandomCode()
        {
            return NewRandomCode(6);
        }
        /// <summary>
        /// 按长度生成随机码
        /// </summary>
        /// <param name="length">随机长度</param>
        /// <returns></returns>
        public static string NewRandomCode(int length)
        {
            string result = "";
            for (int i = 0; i < length; i++)
            {
                result += Random.Next(10).ToString();
            }
            return result;
        }
    }
}
