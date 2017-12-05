using System;

namespace DbDocGenerate.Common
{
    /// <summary>
    /// 自定义异常类。
    /// </summary>
    public class ExceptionHelper : Exception
    {
        public ExceptionHelper()
        {
            //
        }


        public ExceptionHelper(string msg) : base(msg)
        {
            //
        }
    }
}