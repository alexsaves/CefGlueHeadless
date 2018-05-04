using Xilium.CefGlue;

namespace CefGlueHeadless.Handlers
{
    public class HeadlessLifeSpanHandler : CefLifeSpanHandler
    {
        private readonly HeadlessBrowser headlessBrowser;

        public HeadlessLifeSpanHandler(HeadlessBrowser headlessBrowser)
        {
            this.headlessBrowser = headlessBrowser;
        }

        protected override void OnAfterCreated(CefBrowser browser)
        {
            base.OnAfterCreated(browser);
            headlessBrowser.BrowserAfterCreated(browser);
        }

        protected override bool DoClose(CefBrowser browser)
        {
            base.DoClose(browser);
            return false;
        }
    }
}
