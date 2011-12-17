using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections;

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

        public string deleteRequest(Uri uri)
        {
            WebRequest request = WebRequest.Create(uri);
            request.Method = "DELETE";
            request.ContentType = "application/x-www-form-urlencoded";

            WebResponse response = request.GetResponse();
            response.Close();

            return response.Headers != null ? response.Headers.ToString() : "Results are null";
        }

        private static void PushData(Stream input, Stream output, byte[] imageBuffer)
        {
            byte[] buffer = imageBuffer;
            int bytesRead;

            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) != 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
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
