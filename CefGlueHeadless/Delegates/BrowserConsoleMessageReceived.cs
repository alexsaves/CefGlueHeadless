using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefGlueHeadless.Delegates
{
    public delegate void BrowserConsoleMessageReceivedDelegate(HeadlessBrowser browser, string message, string source, int line);
}
