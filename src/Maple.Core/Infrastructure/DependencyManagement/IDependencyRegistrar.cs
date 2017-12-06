using Autofac;
using Maple.Core.Configuration;


namespace Maple.Core.Infrastructure.DependencyManagement
{
    /// <summary>
    /// 依赖注册接口
    /// </summary>
    public interface IDependencyRegistrar
    {
        /// <summary>
        /// 执行类型注册
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        void Register(ContainerBuilder builder, ITypeFinder typeFinder, MapleConfig config);
        /// <summary>
        /// 获取注册顺序
        /// </summary>
        int Order { get; }
    }
}
