using CefGlueHeadless.Delegates;
using CefGlueHeadless.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace CefGlueHeadless
{
    /// <summary>
    /// Client for Cef
    /// </summary>
    public class HeadlessCefClient : CefClient
    {
        private readonly HeadlessLifeSpanHandler lifeSpanHandler;
        private readonly HeadlessRenderHandler renderHandler;
        private readonly HeadlessLoadHandler loadHandler;

        /// <summary>
        /// Fires when the loading state changes
        /// </summary>
        public event BrowserLoadingStateChangeDelegate LoadingStateChanged;

        /// <summary>
        /// Fires when the underlying browser size has changed
        /// </summary>
        public event BrowserSizeChangedDelegate OnSizeChanged;

        public HeadlessCefClient(HeadlessBrowser offscreenBrowser)
        {
            lifeSpanHandler = new HeadlessLifeSpanHandler(offscreenBrowser);
            renderHandler = new HeadlessRenderHandler(offscreenBrowser);
            loadHandler = new HeadlessLoadHandler(offscreenBrowser);
            renderHandler.OnSizeChanged += (browser, width, height) => OnSizeChanged?.Invoke(browser, width, height);
            loadHandler.LoadingStateChanged += (browser, loading, back, forward) => LoadingStateChanged?.Invoke(browser, loading, back, forward);
        }

        protected override CefLifeSpanHandler GetLifeSpanHandler()
        {
            return lifeSpanHandler;
        }

        protected override CefRenderHandler GetRenderHandler()
        {
            return renderHandler;
        }

        protected override CefLoadHandler GetLoadHandler()
        {
            return loadHandler;
        }
    }
}
