using Maple.Core.Domain.Customers;

namespace Maple.Services.Authentication
{
    /// <summary>
    /// 认证服务接口
    /// </summary>
    public partial interface IAuthenticationService 
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="isPersistent">Whether the authentication session is persisted across multiple requests</param>
        void SignIn(Customer customer, bool isPersistent);

        /// <summary>
        /// 退出
        /// </summary>
        void SignOut();

        /// <summary>
        /// 获得认证的客户
        /// </summary>
        /// <returns>Customer</returns>
        Customer GetAuthenticatedCustomer();
    }
}