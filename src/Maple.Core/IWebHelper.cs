using Microsoft.AspNetCore.Http;

namespace Maple.Core
{
    /// <summary>
    /// �ӿ� web helper
    /// </summary>
    public partial interface IWebHelper
    {
        /// <summary>
        /// ��ȡǰһ������ĵ�ַ��referrer URL�����������
        /// </summary>
        /// <returns>URL referrer</returns>
        string GetUrlReferrer();

        /// <summary>
        /// ��ȡ����Http�����û���IP��ַ
        /// </summary>
        /// <returns>String of IP address</returns>
        string GetCurrentIpAddress();

        /// <summary>
        /// ��ȡ��ǰҳ��URL
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <param name="useSsl">Value indicating whether to get SSL secured page URL. Pass null to determine automatically</param>
        /// <param name="lowercaseUrl">Value indicating whether to lowercase URL</param>
        /// <returns>Page URL</returns>
        string GetThisPageUrl(bool includeQueryString, bool? useSsl = null, bool lowercaseUrl = false);

        /// <summary>
        /// ��鵱ǰHttp�����Ƿ�Ϊ�ܱ�����
        /// </summary>
        /// <returns></returns>
        bool IsCurrentConnectionSecured();

        /// <summary>
        /// ��ȡ��ǰվ��Host��ַ
        /// </summary>
        /// <param name="useSsl"></param>
        /// <returns></returns>
        string GetTenantHost(bool useSsl);

        /// <summary>
        /// ��ȡ��ҳ��ַ
        /// </summary>
        /// <param name="useSsl"></param>
        /// <returns></returns>
        string GetTenantLocation(bool? useSsl = null);

        /// <summary>
        /// �Ƿ�Ϊ��̬��Դ
        /// </summary>
        /// <returns></returns>
        bool IsStaticResource();

        /// <summary>
        /// �޸�URL��ѯ�ַ���
        /// </summary>
        /// <param name="url">URL to modify</param>
        /// <param name="queryStringModification">Query string modification</param>
        /// <param name="anchor">Anchor</param>
        /// <returns>New URL</returns>
        string ModifyQueryString(string url, string queryStringModification, string anchor);

        /// <summary>
        /// �Ƴ�URL�еĲ�ѯ�ַ���
        /// </summary>
        /// <param name="url">URL to modify</param>
        /// <param name="queryString">Query string to remove</param>
        /// <returns>New URL without passed query string</returns>
        string RemoveQueryString(string url, string queryString);

        /// <summary>
        /// ��ѯURL�в�ѯ�ַ���ֵ
        /// </summary>
        /// <typeparam name="T">Returned value type</typeparam>
        /// <param name="name">Query parameter name</param>
        /// <returns>Query string value</returns>
        T QueryString<T>(string name);

        /// <summary>
        /// ��������Ӧ�ó���
        /// </summary>
        /// <param name="makeRedirect">A value indicating whether we should made redirection after restart</param>
        void RestartAppDomain(bool makeRedirect = false);

        /// <summary>
        /// �ж������Ƿ��ض���
        /// </summary>
        bool IsRequestBeingRedirected { get; }

        /// <summary>
        /// ȷ���ͻ����Ƿ�ʹ��POST�ض�����λ��
        /// </summary>
        bool IsPostBeingDone { get; set; }

        /// <summary>
        /// ȷ�������Ƿ�Ϊ��������
        /// </summary>
        /// <param name="req">HTTP request</param>
        /// <returns>True, if HTTP request URI references to the local host</returns>
        bool IsLocalRequest(HttpRequest req);

        /// <summary>
        /// ��ȡ�����ԭʼ·����������ѯ
        /// </summary>
        /// <param name="request">HTTP request</param>
        /// <returns>Raw URL</returns>
        string GetRawUrl(HttpRequest request);
    }
}
