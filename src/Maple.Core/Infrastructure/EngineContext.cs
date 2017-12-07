using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Maple.Core.Infrastructure
{
    /// <summary>
    ///  提供 Maple 引擎 的单例实例
    /// </summary>
    public class EngineContext
    {
        #region Methods

        /// <summary>
        /// 创建 Maple 引擎.
        /// 任何时候仅只能有一个线程访问该方法
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Create()
        {
            if (Singleton<IEngine>.Instance == null)
                Singleton<IEngine>.Instance = new MapleEngine();

            return Singleton<IEngine>.Instance;
        }

        /// <summary>
        /// 通过自定义的Maple引擎替换默认的Maple引擎实例
        /// </summary>
        /// <param name="engine"></param>
        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 获取或访问 Maple引擎 单例
        /// </summary>
        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                {
                    Create();
                }

                return Singleton<IEngine>.Instance;
            }
        }

        #endregion
    }
}
