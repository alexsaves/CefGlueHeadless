using Xilium.CefGlue;

namespace CefGlueHeadless.Handlers
{
    /// <summary>
    /// Manages lifespan issues
    /// </summary>
    public class HeadlessLifeSpanHandler : CefLifeSpanHandler
    {
        /// <summary>
        /// A local copy of the headless browser class
        /// </summary>
        private readonly HeadlessBrowser headlessBrowser;

        /// <summary>
        /// Set up a new lifespan handler
        /// </summary>
        /// <param name="headlessBrowser">A copy of the headless browser</param>
        public HeadlessLifeSpanHandler(HeadlessBrowser headlessBrowser)
        {
            this.headlessBrowser = headlessBrowser;
        }

        /// <summary>
        /// Fires after the browser has been created
        /// </summary>
        /// <param name="browser">The browser in question</param>
        protected override void OnAfterCreated(CefBrowser browser)
        {
            base.OnAfterCreated(browser);
            headlessBrowser.BrowserAfterCreated(browser);
        }

        /// <summary>
        /// Fires on the shutdown of the browser
        /// </summary>
        /// <param name="browser">The browser in question</param>
        /// <returns>Whether to allow the close</returns>
        protected override bool DoClose(CefBrowser browser)
        {
            base.DoClose(browser);
            return false;
        }
    }
}
