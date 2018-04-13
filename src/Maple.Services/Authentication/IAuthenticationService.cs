using Maple.Core.Domain.Customers;

namespace Maple.Services.Authentication
{
    /// <summary>
    /// ��֤����ӿ�
    /// </summary>
    public partial interface IAuthenticationService 
    {
        /// <summary>
        /// ��¼
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="isPersistent">Whether the authentication session is persisted across multiple requests</param>
        void SignIn(Customer customer, bool isPersistent);

        /// <summary>
        /// �˳�
        /// </summary>
        void SignOut();

        /// <summary>
        /// �����֤�Ŀͻ�
        /// </summary>
        /// <returns>Customer</returns>
        Customer GetAuthenticatedCustomer();
    }
}