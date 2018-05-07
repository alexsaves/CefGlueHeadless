using CefGlueHeadless.Delegates;
using Xilium.CefGlue;

namespace CefGlueHeadless.Handlers
{
    /// <summary>
    /// Handles load behavior
    /// </summary>
    public class HeadlessLoadHandler : CefLoadHandler
    {
        /// <summary>
        /// Local copy of the offscreen browser
        /// </summary>
        private readonly HeadlessBrowser offscreenBrowser;

        /// <summary>
        /// Loading state change
        /// </summary>
        public event BrowserLoadingStateChangeDelegate LoadingStateChanged;

        /// <summary>
        /// Fires when loading ended
        /// </summary>
        public event BrowserLoadEndedDelegate LoadingEnded;

        /// <summary>
        /// Fires when there was a loading error
        /// </summary>
        public event BrowserLoadEndedDelegate LoadingError;

        /// <summary>
        /// Sets up a new load handler
        /// </summary>
        /// <param name="offscreenBrowser"></param>
        public HeadlessLoadHandler(HeadlessBrowser offscreenBrowser)
        {
            this.offscreenBrowser = offscreenBrowser;
        }

        /// <summary>
        /// Handles loading state changes
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="isLoading"></param>
        /// <param name="canGoBack"></param>
        /// <param name="canGoForward"></param>
        protected override void OnLoadingStateChange(CefBrowser browser, bool isLoading, bool canGoBack, bool canGoForward)
        {
            LoadingStateChanged?.Invoke(browser, isLoading, canGoBack, canGoForward);
            base.OnLoadingStateChange(browser, isLoading, canGoBack, canGoForward);
        }

        /// <summary>
        /// Handle load ends
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="frame"></param>
        /// <param name="httpStatusCode"></param>
        protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
            LoadingEnded?.Invoke(offscreenBrowser, httpStatusCode);
            base.OnLoadEnd(browser, frame, httpStatusCode);
        }

        /// <summary>
        /// Fires when there was a load issue
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="frame"></param>
        /// <param name="errorCode"></param>
        /// <param name="errorText"></param>
        /// <param name="failedUrl"></param>
        protected override void OnLoadError(CefBrowser browser, CefFrame frame, CefErrorCode errorCode, string errorText, string failedUrl)
        {
            base.OnLoadError(browser, frame, errorCode, errorText, failedUrl);
            LoadingError?.Invoke(offscreenBrowser, 500);
        }
    }
}
