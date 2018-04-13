//using System;
//using System.Collections.Generic;
//using System.Security.Claims;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Http;
//using Maple.Core.Domain.Customers;
//using Maple.Services.Customers;

//namespace Maple.Services.Authentication
//{
//    /// <summary>
//    /// Represents service using cookie middleware for the authentication
//    /// </summary>
//    public partial class CookieAuthenticationService : IAuthenticationService
//    {
//        #region �ֶ�

//        private readonly CustomerSettings _customerSettings;
//        private readonly ICustomerService _customerService;
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        private Customer _cachedCustomer;

//        #endregion

//        #region ���캯��

//        /// <summary>
//        /// Ctor
//        /// </summary>
//        /// <param name="customerSettings">Customer settings</param>
//        /// <param name="customerService">Customer service</param>
//        /// <param name="httpContextAccessor">HTTP context accessor</param>
//        public CookieAuthenticationService(CustomerSettings customerSettings,
//            ICustomerService customerService,
//            IHttpContextAccessor httpContextAccessor)
//        {
//            this._customerSettings = customerSettings;
//            this._customerService = customerService;
//            this._httpContextAccessor = httpContextAccessor;
//        }

//        #endregion

//        #region ��������

//        /// <summary>
//        /// Sign in
//        /// </summary>
//        /// <param name="customer">Customer</param>
//        /// <param name="isPersistent">Whether the authentication session is persisted across multiple requests</param>
//        public virtual async void SignIn(Customer customer, bool isPersistent)
//        {
//            if (customer == null)
//                throw new ArgumentNullException(nameof(customer));

//            //create claims for customer's username and email
//            var claims = new List<Claim>();

//            if (!string.IsNullOrEmpty(customer.Username))
//                claims.Add(new Claim(ClaimTypes.Name, customer.Username, ClaimValueTypes.String, MapleCookieAuthenticationDefaults.ClaimsIssuer));

//            if (!string.IsNullOrEmpty(customer.Email))
//                claims.Add(new Claim(ClaimTypes.Email, customer.Email, ClaimValueTypes.Email, MapleCookieAuthenticationDefaults.ClaimsIssuer));

//            //create principal for the current authentication scheme
//            var userIdentity = new ClaimsIdentity(claims, MapleCookieAuthenticationDefaults.AuthenticationScheme);
//            var userPrincipal = new ClaimsPrincipal(userIdentity);

//            //set value indicating whether session is persisted and the time at which the authentication was issued
//            var authenticationProperties = new AuthenticationProperties
//            {
//                IsPersistent = isPersistent,
//                IssuedUtc = DateTime.UtcNow
//            };

//            //sign in
//            await _httpContextAccessor.HttpContext.SignInAsync(MapleCookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, authenticationProperties);

//            //cache authenticated customer
//            _cachedCustomer = customer;
//        }

//        /// <summary>
//        /// �˳�
//        /// </summary>
//        public virtual async void SignOut()
//        {
//            //reset cached customer
//            _cachedCustomer = null;

//            //and sign out from the current authentication scheme
//            await _httpContextAccessor.HttpContext.SignOutAsync(MapleCookieAuthenticationDefaults.AuthenticationScheme);
//        }

//        /// <summary>
//        /// ��ȡ��ǰ��Ȩ�û�����Ϣ
//        /// </summary>
//        /// <returns>Customer</returns>
//        public virtual Customer GetAuthenticatedCustomer()
//        {
//            //����ѻ����û���Ϣ��ֱ�ӷ���
//            if (_cachedCustomer != null)
//                return _cachedCustomer;

//            //���Ի�þ��������֤���û���ʶ
//            var authenticateResult = _httpContextAccessor.HttpContext.AuthenticateAsync(MapleCookieAuthenticationDefaults.AuthenticationScheme).Result;
//            if (!authenticateResult.Succeeded)
//                return null;
//            Customer customer = null;
//            if (_customerSettings.UsernamesEnabled)
//            {
//                //����ͨ�����ƻ�ȡ�û���Ϣ
//                var usernameClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name
//                    && claim.Issuer.Equals(MapleCookieAuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
//                if (usernameClaim != null)
//                    customer = _customerService.GetCustomerByUsername(usernameClaim.Value);
//            }
//            else
//            {
//                //����ͨ�������ȡ�û���Ϣ
//                var emailClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email 
//                    && claim.Issuer.Equals(MapleCookieAuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
//                if (emailClaim != null)
//                    customer = _customerService.GetCustomerByEmail(emailClaim.Value);
//            }

//            //У���û���Ϣ�Ƿ�Ϲ�
//            if (customer == null || !customer.Active || customer.RequireReLogin || customer.Deleted || !customer.IsRegistered())
//                return null;

//            //���浱ǰ��Ȩ���û���Ϣ
//            _cachedCustomer = customer;

//            return _cachedCustomer;
//        }

//        #endregion
//    }
//}