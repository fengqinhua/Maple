using Microsoft.AspNetCore.Http;

namespace Maple.Services.Authentication
{
    /// <summary>
    /// 身份认证方案的缺省值
    /// </summary>
    public static class MapleCookieAuthenticationDefaults
    {
        /// <summary>
        /// 认证方案
        /// </summary>
        public const string AuthenticationScheme = "Authentication";

        /// <summary>
        /// 外部认证的方案
        /// </summary>
        public const string ExternalAuthenticationScheme = "ExternalAuthentication";

        /// <summary>
        /// 用于认证的Cookie前缀
        /// </summary>
        public static readonly string CookiePrefix = ".Maple.";

        /// <summary>
        /// The issuer that should be used for any claims that are created
        /// </summary>
        public static readonly string ClaimsIssuer = "Maple";

        /// <summary>
        /// 登录的页面地址
        /// </summary>
        public static readonly PathString LoginPath = new PathString("/login");

        /// <summary>
        /// 退出的页面地址
        /// </summary>
        public static readonly PathString LogoutPath = new PathString("/logout");

        /// <summary>
        /// 访问未授权页面后跳转到页面地址
        /// </summary>
        public static readonly PathString AccessDeniedPath = new PathString("/page-not-found");

        /// <summary>
        /// 返回URL的确认参数
        /// </summary>
        public static readonly string ReturnUrlParameter = "";
    }
}
