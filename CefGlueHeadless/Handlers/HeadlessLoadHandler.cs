using CefGlueHeadless.Delegates;
using Xilium.CefGlue;

namespace CefGlueHeadless.Handlers
{
    public class HeadlessLoadHandler : CefLoadHandler
    {
        private readonly HeadlessBrowser offscreenBrowser;
        public event BrowserLoadingStateChangeDelegate LoadingStateChanged;

        public HeadlessLoadHandler(HeadlessBrowser offscreenBrowser)
        {
            this.offscreenBrowser = offscreenBrowser;
        }

        protected override void OnLoadingStateChange(CefBrowser browser, bool isLoading, bool canGoBack, bool canGoForward)
        {
            LoadingStateChanged?.Invoke(browser, isLoading, canGoBack, canGoForward);
            base.OnLoadingStateChange(browser, isLoading, canGoBack, canGoForward);
        }
    }
}
