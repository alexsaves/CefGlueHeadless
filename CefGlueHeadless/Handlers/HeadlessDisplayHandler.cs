using CefGlueHeadless.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace CefGlueHeadless.Handlers
{
    /// <summary>
    /// Subclasses display handler
    /// </summary>
    public class HeadlessDisplayHandler : CefDisplayHandler
    {
        /// <summary>
        /// Private internal reference to the browser
        /// </summary>
        private readonly HeadlessBrowser offscreenBrowser;

        /// <summary>
        /// Fires when a console message is received
        /// </summary>
        public event BrowserConsoleMessageReceivedDelegate OnConsoleMessageReceived;

        /// <summary>
        /// Set up a new display handler
        /// </summary>
        /// <param name="offscreenBrowser"></param>
        public HeadlessDisplayHandler(HeadlessBrowser offscreenBrowser)
        {
            this.offscreenBrowser = offscreenBrowser;
        }

        /// <summary>
        /// Fires when console messages happen
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="source"></param>
        /// <param name="line"></param>
        /// <returns>Whether or not to allow the message</returns>
        protected override bool OnConsoleMessage(CefBrowser browser, CefLogSeverity level, string message, string source, int line)
        {
            OnConsoleMessageReceived?.Invoke(offscreenBrowser, message, source, line);
            return false;
        }
    }
}
