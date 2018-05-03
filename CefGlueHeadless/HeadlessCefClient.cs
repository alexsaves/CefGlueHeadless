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
    public class HeadlessCefClient : CefClient
    {
        private readonly HeadlessLifeSpanHandler lifeSpanHandler;
        private readonly HeadlessRenderHandler renderHandler;
        private readonly HeadlessLoadHandler loadHandler;
        public event BrowserLoadingStateChangeDelegate LoadingStateChanged;

        public HeadlessCefClient(HeadlessBrowser offscreenBrowser)
        {
            lifeSpanHandler = new HeadlessLifeSpanHandler(offscreenBrowser);
            renderHandler = new HeadlessRenderHandler(offscreenBrowser);
            loadHandler = new HeadlessLoadHandler(offscreenBrowser);
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
