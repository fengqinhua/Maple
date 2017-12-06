using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Maple.Core
{ 
    /// <summary>
    /// 异常信息
    /// </summary>
    [Serializable]
    public class MapleException : Exception
    {
        /// <summary>
        /// 异常信息
        /// </summary>
        public MapleException()
        {
        }

        /// <summary>
        /// 异常信息
        /// </summary>
        /// <param name="message">错误消息文本内容.</param>
        public MapleException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 异常信息
        /// </summary>
		/// <param name="messageFormat">错误消息文本内容模板.</param>
		/// <param name="args">错误消息文本内容参数.</param>
        public MapleException(string messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {
        }

        /// <summary>
        /// 异常信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected MapleException(SerializationInfo
            info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// 异常信息
        /// </summary>
        /// <param name="message">错误消息文本内容.</param>
        /// <param name="innerException">引发当前异常的异常信息.</param>
        public MapleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
