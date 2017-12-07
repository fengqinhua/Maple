
namespace Maple.Core.Plugins
{
    /// <summary>
    /// 接口：插件的描述信息
    /// </summary>
    public interface IDescriptor
    {
        /// <summary>
        /// 插件名称
        /// </summary>
        string SystemName { get; set; }

        /// <summary>
        /// 插件别名
        /// </summary>
         string FriendlyName { get; set; }
    }
}