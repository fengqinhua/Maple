using Microsoft.AspNetCore.Http;

namespace Maple.Core
{
    /// <summary>
    /// 接口 web helper
    /// </summary>
    public partial interface IWebHelper
    {
        /// <summary>
        /// 获取referrer URL，如果存在
        /// </summary>
        /// <returns>URL referrer</returns>
        string GetUrlReferrer();

        /// <summary>
        /// 获取当前客户端用户所在IP地址
        /// </summary>
        /// <returns>String of IP address</returns>
        string GetCurrentIpAddress();

        /// <summary>
        /// 获取当前页面URL
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <param name="useSsl">Value indicating whether to get SSL secured page URL. Pass null to determine automatically</param>
        /// <param name="lowercaseUrl">Value indicating whether to lowercase URL</param>
        /// <returns>Page URL</returns>
        string GetThisPageUrl(bool includeQueryString, bool? useSsl = null, bool lowercaseUrl = false);

        /// <summary>
        /// 检查当前Http连接是否为受保护的
        /// </summary>
        /// <returns></returns>
        bool IsCurrentConnectionSecured();

        /// <summary>
        /// 获取主机地址
        /// </summary>
        /// <param name="useSsl">Whether to get SSL secured URL</param>
        /// <returns>Store host location</returns>
        string GetStoreHost(bool useSsl);

        /// <summary>
        /// 获取主机地址
        /// </summary>
        /// <param name="useSsl">Whether to get SSL secured URL; pass null to determine automatically</param>
        /// <returns>Store location</returns>
        string GetStoreLocation(bool? useSsl = null);

        /// <summary>
        /// 是否为静态资源
        /// </summary>
        /// <returns></returns>
        bool IsStaticResource();

        /// <summary>
        /// 修改查询字符串
        /// </summary>
        /// <param name="url">URL to modify</param>
        /// <param name="queryStringModification">Query string modification</param>
        /// <param name="anchor">Anchor</param>
        /// <returns>New URL</returns>
        string ModifyQueryString(string url, string queryStringModification, string anchor);

        /// <summary>
        /// 移除URL中的查询字符串
        /// </summary>
        /// <param name="url">URL to modify</param>
        /// <param name="queryString">Query string to remove</param>
        /// <returns>New URL without passed query string</returns>
        string RemoveQueryString(string url, string queryString);

        /// <summary>
        /// 查询URL中查询字符的值
        /// </summary>
        /// <typeparam name="T">Returned value type</typeparam>
        /// <param name="name">Query parameter name</param>
        /// <returns>Query string value</returns>
        T QueryString<T>(string name);

        /// <summary>
        /// 重新启动应用程序
        /// </summary>
        /// <param name="makeRedirect">A value indicating whether we should made redirection after restart</param>
        void RestartAppDomain(bool makeRedirect = false);
        
        /// <summary>
        /// 获取请求是否被重定向
        /// </summary>
        bool IsRequestBeingRedirected { get; }

        /// <summary>
        /// 确定客户端是否使用POST重定向到新位置
        /// </summary>
        bool IsPostBeingDone { get; set; }

        /// <summary>
        /// 确定访问是否为本地请求
        /// </summary>
        /// <param name="req">HTTP request</param>
        /// <returns>True, if HTTP request URI references to the local host</returns>
        bool IsLocalRequest(HttpRequest req);

        /// <summary>
        /// 获取请求的原始路径和完整查询
        /// </summary>
        /// <param name="request">HTTP request</param>
        /// <returns>Raw URL</returns>
        string GetRawUrl(HttpRequest request);
    }
}
