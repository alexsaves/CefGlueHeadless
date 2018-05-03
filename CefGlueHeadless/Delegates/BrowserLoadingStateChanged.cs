using Xilium.CefGlue;

namespace CefGlueHeadless.Delegates
{
    public delegate void BrowserLoadingStateChangeDelegate(CefBrowser browser, bool isLoading, bool canGoBack, bool canGoForward);
}
