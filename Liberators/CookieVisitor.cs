using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

namespace Liberators
{
    class CookieVisitor : CefSharp.ICookieVisitor
    {
        public event Action<CefSharp.Cookie> SendCookie;
        public bool Visit(Cookie cookie, int count, int total, ref bool deleteCookie)
        {
            deleteCookie = false;
            if (SendCookie != null) {
                SendCookie(cookie);
            }

            return true;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }        
    }
}
