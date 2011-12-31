using System;
using System.Net;

namespace MinusEngine
{
    public class CookieAwareWebClient : WebClient
    {

        [System.Security.SecuritySafeCritical]
        public CookieAwareWebClient()
            : base()
        {
        }

        private CookieContainer m_container = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = m_container;
            }
            return request;
        }

        public HttpStatusCode DeleteRequest(Uri uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "DELETE";
            request.ContentType = "application/x-www-form-urlencoded";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Close();
            return response.StatusCode;
        }

        public void clearCookies() {
            m_container = new CookieContainer();
        }

        public string getCookieHeader(Uri uri)
        {
            return m_container.GetCookieHeader(uri);
        }

        public void setCookieHeader(Uri uri, string header)
        {
            m_container.SetCookies(uri, header);
        }
    }
}
