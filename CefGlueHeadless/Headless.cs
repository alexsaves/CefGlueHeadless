using CefGlueHeadless.Delegates;
using System;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace CefGlueHeadless
{
    /// <summary>
    /// Provides housekeeping interface to Cef for headless use
    /// </summary>
    public static class Headless
    {
        /// <summary>
        /// Did we intitialize yet?
        /// </summary>
        public static bool _didInitialize = false;

        /// <summary>
        /// Set up Cef and initialize dependencies.
        /// MUST be called before using HeadlessBrowser
        /// </summary>
        public static void Initialize()
        {
            // Load CEF. This checks for the correct CEF version.
            CefRuntime.Load();

            // Start the secondary CEF process.
            var cefMainArgs = new CefMainArgs(new string[0]);
            var cefApp = new HeadlessApp();

            // This is where the code path divereges for child processes.
            if (CefRuntime.ExecuteProcess(cefMainArgs, cefApp) != -1)
            {
                Console.Error.WriteLine("CefRuntime could not start the secondary process.");
            }

            // Settings for all of CEF (e.g. process management and control).
            var cefSettings = new CefSettings
            {
                SingleProcess = false,
                MultiThreadedMessageLoop = true,
                BackgroundColor = new CefColor(255, 255, 255, 255),
                IgnoreCertificateErrors = true
            };

            // Start the browser process (a child process).
            CefRuntime.Initialize(cefMainArgs, cefSettings, cefApp);

            // Instruct CEF to not render to a window at all.
            CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
            cefWindowInfo.SetAsWindowless(IntPtr.Zero, false);

            _didInitialize = true;
        }

        /// <summary>
        /// Wait for the browser to load
        /// </summary>
        /// <param name="browser">The browser to wait on</param>
        /// <returns></returns>
        public static Task WaitForBrowserLoadingAsync(HeadlessBrowser browser)
        {
            var tcs = new TaskCompletionSource<bool>();
            BrowserLoadingStateChangeDelegate handler = null;
            handler = (cefBrowser, loading, back, forward) => {
                if (loading) return;
                browser.OnLoadingStateChanged -= handler;
                tcs.TrySetResult(true);
            };
            browser.OnLoadingStateChanged += handler;
            return tcs.Task;
        }

        /// <summary>
        /// Tear down everything needed.
        /// MUST BE CALLED when shutting down your program
        /// </summary>
        public static void Destroy()
        {
            CefRuntime.Shutdown();
        }
    }
}
