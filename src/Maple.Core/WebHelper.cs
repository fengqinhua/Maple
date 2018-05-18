using Maple.Core.Configuration;
using Maple.Core.Data.DataSettings;
using Maple.Core.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Maple.Core
{
    /// <summary>
    /// 接口 web helper 实现类
    /// </summary>
    public partial class WebHelper : IWebHelper
    {
        #region Const

        private const string NullIpAddress = "::1";

        #endregion

        #region Fields 
        /// <summary>
        /// HttpContext 访问器
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;
        /// <summary>
        /// 定义宿主程序运行环境所需的配置参数
        /// </summary>
        private readonly HostingConfig _hostingConfig;

        #endregion

        #region Ctor

        /// <summary>
        /// 接口 web helper 实现类
        /// </summary>
        /// <param name="hostingConfig">定义宿主程序运行环境所需的配置参数</param>
        /// <param name="httpContextAccessor">HttpContext 访问器</param>
        public WebHelper(HostingConfig hostingConfig, IHttpContextAccessor httpContextAccessor)
        {
            this._hostingConfig = hostingConfig;
            this._httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region 方法

        /// <summary>
        /// 检查当前HTTP请求是否可用
        /// </summary>
        /// <returns>如果可用 返回True; 否则 false</returns>
        protected virtual bool IsRequestAvailable()
        {
            if (_httpContextAccessor == null || _httpContextAccessor.HttpContext == null)
                return false;

            try
            {
                if (_httpContextAccessor.HttpContext.Request == null)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 请求是否为本地
        /// </summary>
        /// <param name="address">IP address</param>
        /// <returns>Result</returns>
        protected virtual bool IsIpAddressSet(IPAddress address)
        {
            return address != null && address.ToString() != NullIpAddress;
        }

        /// <summary>
        /// 修改web.config文件的最后修改时间
        /// </summary>
        /// <returns></returns>
        protected virtual bool TryWriteWebConfig()
        {
            try
            {
                File.SetLastWriteTimeUtc(CommonHelper.MapPath("~/web.config"), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 获取前一次请求的地址（referrer URL），如果存在
        /// </summary>
        /// <returns>URL referrer</returns>
        public virtual string GetUrlReferrer()
        {
            if (!IsRequestAvailable())
                return string.Empty;

            //某些情况下是无用的链接 ，例如IE8
            return _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Referer];
        }

        /// <summary>
        /// 获取发起Http请求用户的IP地址
        /// </summary>
        /// <returns>发起Http请求用户的IP地址</returns>
        public virtual string GetCurrentIpAddress()
        {
            if (!IsRequestAvailable())
                return string.Empty;

            var result = string.Empty;
            try
            {
                //首先尝试从转发报头获取IP地址
                if (_httpContextAccessor.HttpContext.Request.Headers != null)
                {
                    //the X-Forwarded-For (XFF) HTTP header field is a de facto standard for identifying the originating IP address of a client
                    //connecting to a web server through an HTTP proxy or load balancer
                    var forwardedHttpHeaderKey = "X-FORWARDED-FOR";
                    if (!string.IsNullOrEmpty(_hostingConfig.ForwardedHttpHeader))
                    {
                        //but in some cases server use other HTTP header
                        //in these cases an administrator can specify a custom Forwarded HTTP header (e.g. CF-Connecting-IP, X-FORWARDED-PROTO, etc)
                        forwardedHttpHeaderKey = _hostingConfig.ForwardedHttpHeader;
                    }

                    var forwardedHeader = _httpContextAccessor.HttpContext.Request.Headers[forwardedHttpHeaderKey];
                    if (!StringValues.IsNullOrEmpty(forwardedHeader))
                        result = forwardedHeader.FirstOrDefault();
                }

                //if this header not exists try get connection remote IP address
                if (string.IsNullOrEmpty(result) && _httpContextAccessor.HttpContext.Connection.RemoteIpAddress != null)
                    result = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            catch
            {
                return string.Empty;
            }

            //some of the validation
            if (result != null && result.Equals("::1", StringComparison.InvariantCultureIgnoreCase))
                result = "127.0.0.1";

            //移除端口号
            if (!string.IsNullOrEmpty(result))
                result = result.Split(':').FirstOrDefault();

            return result;
        }

        /// <summary>
        /// 获取当前页面URL
        /// </summary>
        /// <param name="includeQueryString">标识是否包含查询参数</param>
        /// <param name="useSsl">标识是否为SSL，如果输入Null系统将会自动判断</param>
        /// <param name="lowercaseUrl">标识是否将URL结果以小写形式输出</param>
        /// <returns>当前页面URL</returns>
        public virtual string GetThisPageUrl(bool includeQueryString, bool? useSsl = null, bool lowercaseUrl = false)
        {
            if (!IsRequestAvailable())
                return string.Empty;

            if (!useSsl.HasValue)
                useSsl = IsCurrentConnectionSecured();

            //get the host considering using SSL
            var url = GetTenantHost(useSsl.Value).TrimEnd('/');

            //get full URL with or without query string
            url += includeQueryString ? GetRawUrl(_httpContextAccessor.HttpContext.Request)
                : $"{_httpContextAccessor.HttpContext.Request.PathBase}{_httpContextAccessor.HttpContext.Request.Path}";

            if (lowercaseUrl)
                url = url.ToLowerInvariant();

            return url;
        }

        /// <summary>
        /// 判断是否SSL（当前连接是否已被保护）
        /// </summary>
        /// <returns>如果当前连接已被保护SSL，那么则返回True</returns>
        public virtual bool IsCurrentConnectionSecured()
        {
            if (!IsRequestAvailable())
                return false;


            //use HTTP_CLUSTER_HTTPS?
            if (_hostingConfig.UseHttpClusterHttps)
                return _httpContextAccessor.HttpContext.Request.Headers["HTTP_CLUSTER_HTTPS"].ToString().Equals("on", StringComparison.OrdinalIgnoreCase);

            //检测服务端是否使用了负载均衡器
            //use HTTP_X_FORWARDED_PROTO?
            if (_hostingConfig.UseHttpXForwardedProto)
                return _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-Proto"].ToString().Equals("https", StringComparison.OrdinalIgnoreCase);

            return _httpContextAccessor.HttpContext.Request.IsHttps;
        }

        /// <summary>
        /// 获取当前站点Host地址
        /// </summary>
        /// <param name="useSsl">是否为HTTPS</param>
        /// <returns></returns>
        public virtual string GetTenantHost(bool useSsl)
        {
            var result = string.Empty;

            //尝试从请求头中获取Host信息
            var hostHeader = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Host];
            if (!StringValues.IsNullOrEmpty(hostHeader))
                result = "http://" + hostHeader.FirstOrDefault();
            else
            {
                //判断数据库是否已安装
                if (MainDataSettingsHelper.DatabaseIsInstalled())
                {
                    //获取当前站点上下文信息
                    var currentTenant = EngineContext.Current.Resolve<ITenantContext>().CurrentTenant;
                    if (currentTenant == null)
                        throw new Exception("Current tenant cannot be loaded");
                    result = currentTenant.Url;
                }
            }

            if (!result.IsNullOrEmpty() && useSsl)
                result = result.Replace("http://", "https://");

            if (!result.EndsWith("/"))
                result += "/";

            return result;
        }

        /// <summary>
        /// 获取网页地址
        /// </summary>
        /// <param name="useSsl">是否为HTTPS</param>
        /// <returns></returns>
        public virtual string GetTenantLocation(bool? useSsl = null)
        {
            //whether connection is secured
            if (!useSsl.HasValue)
                useSsl = IsCurrentConnectionSecured();

            //get store host
            var host = GetTenantHost(useSsl.Value).TrimEnd('/');

            //add application path base if exists
            if (IsRequestAvailable())
                host += _httpContextAccessor.HttpContext.Request.PathBase;


            if (!host.EndsWith("/"))
                host += "/";

            return host;
        }

        /// <summary>
        /// 判断请求的资源是否为静态资源
        /// </summary>
        /// <returns>True if the request targets a static resource file.</returns>
        public virtual bool IsStaticResource()
        {
            if (!IsRequestAvailable())
                return false;

            string path = _httpContextAccessor.HttpContext.Request.Path;

            //a little workaround. FileExtensionContentTypeProvider contains most of static file extensions. So we can use it
            //source: https://github.com/aspnet/StaticFiles/blob/dev/src/Microsoft.AspNetCore.StaticFiles/FileExtensionContentTypeProvider.cs
            //if it can return content type, then it's a static file
            var contentTypeProvider = new FileExtensionContentTypeProvider();
            return contentTypeProvider.TryGetContentType(path, out string _);
        }

        /// <summary>
        /// 修改URL中的查询参数
        /// </summary>
        /// <param name="url">URL to modify</param>
        /// <param name="queryStringModification">Query string modification</param>
        /// <param name="anchor">Anchor</param>
        /// <returns>New URL</returns>
        public virtual string ModifyQueryString(string url, string queryStringModification, string anchor)
        {
            if (url == null)
                url = string.Empty;

            if (queryStringModification == null)
                queryStringModification = string.Empty;

            if (anchor == null)
                anchor = string.Empty;

            var str = string.Empty;
            var str2 = string.Empty;
            if (url.Contains("#"))
            {
                str2 = url.Substring(url.IndexOf("#") + 1);
                url = url.Substring(0, url.IndexOf("#"));
            }
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(queryStringModification))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    var dictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                    foreach (var str3 in str.Split(new[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            var strArray = str3.Split(new[] { '=' });
                            if (strArray.Length == 2)
                            {
                                if (!dictionary.ContainsKey(strArray[0]))
                                {
                                    //do not add value if it already exists
                                    //two the same query parameters? theoretically it's not possible.
                                    //but MVC has some ugly implementation for checkboxes and we can have two values
                                    //find more info here: http://www.mindstorminteractive.com/topics/jquery-fix-asp-net-mvc-checkbox-truefalse-value/
                                    //we do this validation just to ensure that the first one is not overridden
                                    dictionary[strArray[0]] = strArray[1];
                                }
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    foreach (var str4 in queryStringModification.Split(new[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str4))
                        {
                            var strArray2 = str4.Split(new[] { '=' });
                            if (strArray2.Length == 2)
                            {
                                dictionary[strArray2[0]] = strArray2[1];
                            }
                            else
                            {
                                dictionary[str4] = null;
                            }
                        }
                    }
                    var builder = new StringBuilder();
                    foreach (var str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
                else
                {
                    str = queryStringModification;
                }
            }
            if (!string.IsNullOrEmpty(anchor))
            {
                str2 = anchor;
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)) + (string.IsNullOrEmpty(str2) ? "" : ("#" + str2)));
        }

        /// <summary>
        /// 移除URL中的查询参数
        /// </summary>
        /// <param name="url">URL to modify</param>
        /// <param name="queryString">Query string to remove</param>
        /// <returns>New URL without passed query string</returns>
        public virtual string RemoveQueryString(string url, string queryString)
        {
            if (url == null)
                url = string.Empty;

            if (queryString == null)
                queryString = string.Empty;

            var str = string.Empty;
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(queryString))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    var dictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                    foreach (var str3 in str.Split(new[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            var strArray = str3.Split(new[] { '=' });
                            if (strArray.Length == 2)
                            {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    dictionary.Remove(queryString);

                    var builder = new StringBuilder();
                    foreach (var str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)));
        }

        /// <summary>
        /// 获取URL中的查询参数
        /// </summary>
        /// <typeparam name="T">Returned value type</typeparam>
        /// <param name="name">Query parameter name</param>
        /// <returns>Query string value</returns>
        public virtual T QueryString<T>(string name)
        {
            if (!IsRequestAvailable())
                return default(T);

            if (StringValues.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Query[name]))
                return default(T);

            return CommonHelper.To<T>(_httpContextAccessor.HttpContext.Request.Query[name].ToString());
        }

        /// <summary>
        /// 重启应用程序
        /// </summary>
        /// <param name="makeRedirect">A value indicating whether we should made redirection after restart</param>
        public virtual void RestartAppDomain(bool makeRedirect = false)
        {
            //the site will be restarted during the next request automatically
            //_applicationLifetime.StopApplication();

            //"touch" web.config to force restart
            var success = TryWriteWebConfig();
            if (!success)
            {
                throw new MapleException("maple needs to be restarted due to a configuration change, but was unable to do so." + Environment.NewLine +
                    "To prevent this issue in the future, a change to the web server configuration is required:" + Environment.NewLine +
                    "- run the application in a full trust environment, or" + Environment.NewLine +
                    "- give the application write access to the 'web.config' file.");
            }
        }

        /// <summary>
        /// 判断请求是否被重定向
        /// </summary>
        public virtual bool IsRequestBeingRedirected
        {
            get
            {
                var response = _httpContextAccessor.HttpContext.Response;
                //ASP.NET 4 style - return response.IsRequestBeingRedirected;
                int[] redirectionStatusCodes = { StatusCodes.Status301MovedPermanently, StatusCodes.Status302Found };
                return redirectionStatusCodes.Contains(response.StatusCode);
            }
        }

        /// <summary>
        /// 确定客户端是否使用POST重定向到新位置
        /// </summary>
        public virtual bool IsPostBeingDone
        {
            get
            {
                if (_httpContextAccessor.HttpContext.Items["maple.IsPOSTBeingDone"] == null)
                    return false;

                return Convert.ToBoolean(_httpContextAccessor.HttpContext.Items["maple.IsPOSTBeingDone"]);
            }
            set
            {
                _httpContextAccessor.HttpContext.Items["maple.IsPOSTBeingDone"] = value;
            }
        }

        /// <summary>
        /// 确定访问是否为本地请求
        /// </summary>
        /// <param name="req">HTTP request</param>
        /// <returns>True, if HTTP request URI references to the local host</returns>
        public virtual bool IsLocalRequest(HttpRequest req)
        {
            //source: https://stackoverflow.com/a/41242493/7860424
            var connection = req.HttpContext.Connection;
            if (IsIpAddressSet(connection.RemoteIpAddress))
            {
                //We have a remote address set up
                return IsIpAddressSet(connection.LocalIpAddress)
                    //Is local is same as remote, then we are local
                    ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress)
                    //else we are remote if the remote IP address is not a loopback address
                    : IPAddress.IsLoopback(connection.RemoteIpAddress);
            }

            return true;
        }

        /// <summary>
        ///  获取请求的原始路径和完整查询
        /// </summary>
        /// <param name="request">HTTP request</param>
        /// <returns>Raw URL</returns>
        public virtual string GetRawUrl(HttpRequest request)
        {
            //first try to get the raw target from request feature
            //note: value has not been UrlDecoded
            var rawUrl = request.HttpContext.Features.Get<IHttpRequestFeature>()?.RawTarget;

            //or compose raw URL manually
            if (string.IsNullOrEmpty(rawUrl))
                rawUrl = $"{request.PathBase}{request.Path}{request.QueryString}";

            return rawUrl;
        }

        #endregion
    }
}
