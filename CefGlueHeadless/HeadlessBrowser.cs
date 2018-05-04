using CefGlueHeadless.Delegates;
using System;
using System.Drawing;
using System.Threading.Tasks;
using Xilium.CefGlue;
using System.Timers;

namespace CefGlueHeadless
{
    /// <summary>
    /// Provides a non-windowed interface to CefGlue
    /// </summary>
    public class HeadlessBrowser : IDisposable
    {
        /// <summary>
        /// Was this already disposed?
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// The internal width
        /// </summary>
        private int _width = 0;

        /// <summary>
        /// The internal height
        /// </summary>
        private int _height = 0;

        /// <summary>
        /// The width of the browser
        /// </summary>
        public int Width { get { return _width; } }

        /// <summary>
        /// The height of the browser
        /// </summary>
        public int Height { get { return _height; } }

        /// <summary>
        /// Holds the framerate internally
        /// </summary>
        private int _frameRate = 5;

        /// <summary>
        /// The update rate of the browser internally
        /// </summary>
        public int FrameRate { get { return _frameRate; } }

        /// <summary>
        /// Holds the under-the-hood browser
        /// </summary>
        private CefBrowser browser;

        /// <summary>
        /// Holds the cef client
        /// </summary>
        private HeadlessCefClient client;
        
        /// <summary>
        /// Fires when the loading state changes
        /// </summary>
        public event BrowserLoadingStateChangeDelegate OnLoadingStateChanged;

        /// <summary>
        /// Fires when the browser initializes
        /// </summary>
        public event BrowserInitializedDelegate OnInitialized;

        /// <summary>
        /// Fires when the underlying browser size has changed
        /// </summary>
        public event BrowserSizeChangedDelegate OnSizeChanged;

        /// <summary>
        /// Fires when a bitmap frame is received. Use RequestFrame or RequestFrameAsync to request.
        /// </summary>
        public event BrowserFrameReceivedDelegate OnFrameReceived;

        /// <summary>
        /// Internal browser initialized state
        /// </summary>
        private bool _initialized = false;

        /// <summary>
        /// Is the browser initialized and ready for commands?
        /// </summary>
        public bool Initialized { get { return _initialized; } }

        /// <summary>
        /// The mouse position
        /// </summary>
        private Point _mousePosition = new Point(0, 0);

        /// <summary>
        /// The mouse position
        /// </summary>
        public Point MousePosition { get { return _mousePosition; } }

        /// <summary>
        /// Is the mouse on top of the screen?
        /// </summary>
        private bool _isMouseOver = false;

        /// <summary>
        /// Is the mouse over the screen?
        /// </summary>
        public bool IsMouseOver { get { return _isMouseOver; } }

        /// <summary>
        /// Set up a new browser
        /// </summary>
        /// <param name="width">Initial width of the browser</param>
        /// <param name="height">Initial height of the browser</param>
        /// <param name="url">Starting URL of the browser</param>
        /// <param name="frameRate">The render frame rate of the browser</param>
        public HeadlessBrowser(int width = 500, int height = 500, string url = "about:blank", int frameRate = 5, Color defaultBackground = new Color())
        {
            _width = width;
            _height = height;
            _frameRate = frameRate;
            IntPtr parentHandle = IntPtr.Zero;

            CefWindowInfo windowInfo = CefWindowInfo.Create();

            windowInfo.SetAsChild(parentHandle, new CefRectangle(0, 0, width, height));
            windowInfo.SetAsWindowless(parentHandle, true);

            client = new HeadlessCefClient(this);
            CefBrowserSettings settings = new CefBrowserSettings
            {
                BackgroundColor = new CefColor(defaultBackground.A, defaultBackground.R, defaultBackground.G, defaultBackground.B),
                WebSecurity = CefState.Disabled
            };
            
            CefBrowserHost.CreateBrowser(windowInfo, client, settings, url);

            // Handle load state changes and pass them along
            client.LoadingStateChanged += (browser, loading, back, forward) =>
            {
                OnLoadingStateChanged?.Invoke(browser, loading, back, forward);
            };

            // Handle when an image comes in
            client.OnFrameReceived += (bmImg) =>
            {
                OnFrameReceived?.Invoke(bmImg);
            };
        }

        /// <summary>
        /// Request a frame
        /// </summary>
        public void RequestFrame()
        {
            AssertNotInitialized();
            client.RequestFrame();
            this.Invalidate();
        }

        /// <summary>
        /// Request a frame with async
        /// </summary>
        /// <returns>Task</returns>
        public Task<Bitmap> RequestFrameAsync()
        {
            AssertNotInitialized();
            var tcsrc = new TaskCompletionSource<Bitmap>();
            BrowserFrameReceivedDelegate brd = null;
            brd = (Bitmap bmImg) =>
            {
                OnFrameReceived -= brd;
                tcsrc.TrySetResult(bmImg);
            };
            OnFrameReceived += brd;
            RequestFrame();
            return tcsrc.Task;
        }

        /// <summary>
        /// Wait for the browser to initialize
        /// </summary>
        /// <returns>A task that guarantees the browser will initalize before returning</returns>
        public Task WaitForBrowserToInitialize()
        {
            var tcsrc = new TaskCompletionSource<bool>();
            if (_initialized)
            {
                Timer timer = new Timer();
                timer.Elapsed += (obj, args) =>
                {
                    tcsrc.TrySetResult(true);
                };
                timer.Interval = 100;
                timer.AutoReset = false;
                timer.Start();
            }
            else
            {
                BrowserInitializedDelegate intHandler = null;
                intHandler = (br) =>
                {
                    this.OnInitialized -= intHandler;
                    tcsrc.TrySetResult(true);
                };
                this.OnInitialized += intHandler;
            }
            return tcsrc.Task;
        }

        /// <summary>
        /// Change the size of the browser
        /// </summary>
        /// <param name="width">New width in pixels</param>
        /// <param name="height">New height in pixels</param>
        public void Resize(int width, int height)
        {
            AssertNotInitialized();
            if (width != _width || height != _height)
            {
                _width = width;
                _height = height;
                browser.GetHost().WasResized();
            }
        }

        /// <summary>
        /// Change the size of the browser
        /// </summary>
        /// <param name="width">New width in pixels</param>
        /// <param name="height">New height in pixels</param>
        /// <returns>Task</returns>
        public Task ResizeAsync(int width, int height)
        {
            AssertNotInitialized();
            var tcsrc = new TaskCompletionSource<bool>();
            if (width == this.Width && height == this.Height)
            {
                Timer timer = new Timer();
                timer.Elapsed += (obj, args) =>
                {
                    tcsrc.TrySetResult(true);
                };
                timer.Interval = 100;
                timer.AutoReset = false;
                timer.Start();
            } else {
                BrowserSizeChangedDelegate szd = null;
                szd = (HeadlessBrowser browser, int w, int h) =>
                {
                    if (w == width && h == height)
                    {
                        client.OnSizeChanged -= szd;
                        tcsrc.TrySetResult(true);
                    }
    
                };
                client.OnSizeChanged += szd;
                Resize(width, height);
            }
            return tcsrc.Task;
        }
        
        /// <summary>
        /// Set the update rate for the render pass. Recommended not higher than 10.
        /// </summary>
        /// <param name="frameRate">Frame rate from 0-60. Recommended not to be higher than 10.</param>
        public void SetFrameRate(int frameRate)
        {
            if (frameRate < 0 || frameRate > 60)
            {
                throw new Exception("Invalid frame rate. Must be between 0-60.");
            }
            AssertNotInitialized();
            _frameRate = frameRate;
            browser.GetHost().SetWindowlessFrameRate(_frameRate);
        }

        /// <summary>
        /// Fires when the browser is created
        /// </summary>
        /// <param name="browser">The brwoser reference</param>
        internal void BrowserAfterCreated(CefBrowser browser)
        {
            this.browser = browser;
            browser.GetHost().SetWindowlessFrameRate(_frameRate);
            if (!_initialized)
            {
                _initialized = true;
                OnInitialized?.Invoke(this);
            }
        }

        /// <summary>
        /// Get the underlying CefBrowser
        /// </summary>
        /// <returns>The internal CefBrowser instance</returns>
        public CefBrowser GetBrowser()
        {
            AssertNotInitialized();
            return browser;
        }

        /// <summary>
        /// Navigate to a URL
        /// </summary>
        /// <param name="url">A fully qualified URL</param>
        public void Navigate(string url)
        {
            AssertNotInitialized();
            browser.StopLoad();
            browser.GetMainFrame().LoadUrl(url);
        }

        /// <summary>
        /// Navigate to a URL
        /// </summary>
        /// <param name="url">A fully qualified URL</param>
        public void Navigate(Uri url)
        {
            AssertNotInitialized();
            Navigate(url.ToString());
        }

        /// <summary>
        /// Put a mouse move event at the provided position in screen coordinates
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public void SetMouseMove(int x, int y)
        {
            AssertNotInitialized();
            _mousePosition.X = x;
            _mousePosition.Y = y;
            CefBrowserHost hst = browser.GetHost();
            hst.SendMouseMoveEvent(new CefMouseEvent(x, y, CefEventFlags.None), false);
        }

        /// <summary>
        /// Put a mouse move event at the provided position in screen coordinates
        /// </summary>
        public void SetMouseLeave()
        {
            AssertNotInitialized();
            CefBrowserHost hst = browser.GetHost();
            hst.SendMouseMoveEvent(new CefMouseEvent(_mousePosition.X, _mousePosition.Y, CefEventFlags.None), true);
        }

        /// <summary>
        /// Bring the mouse back
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public void SetMouseEnter(int x, int y)
        {
            AssertNotInitialized();
            SetMouseMove(x, y);
        }

        /// <summary>
        /// Force a re-draw
        /// </summary>
        private void Invalidate()
        {
            CefBrowserHost hst = browser.GetHost();
            hst.Invalidate(CefPaintElementType.View);
        }

        /// <summary>
        /// Put a mouse move event at the provided position in screen coordinates
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public void SetMouseClick(int x, int y, bool leftButton = true)
        {
            AssertNotInitialized();
            SetMouseMove(x, y);
            CefBrowserHost hst = browser.GetHost();
            hst.SendMouseClickEvent(new CefMouseEvent(x, y, CefEventFlags.None), leftButton ? CefMouseButtonType.Left : CefMouseButtonType.Right, false, 1);
            Timer timer = new Timer();
            timer.Elapsed += (obj, args) =>
            {
                hst.SendMouseClickEvent(new CefMouseEvent(x, y, CefEventFlags.None), leftButton ? CefMouseButtonType.Left : CefMouseButtonType.Right, true, 1);
            };
            timer.Interval = 100;
            timer.AutoReset = false;
            timer.Start();
        }

        /// <summary>
        /// Throw a message that the browser is not intitialzed if needed
        /// </summary>
        private void AssertNotInitialized()
        {
            if (!_initialized)
            {
                throw new Exception("Browser is not initialized.");
            }
            if (_disposed)
            {
                throw new Exception("Browser is disposed.");
            }
            if (browser == null)
            {
                throw new Exception("Browser is not ready.");
            }
            if (!Headless._didInitialize)
            {
                throw new Exception("You must initialize Headless first.");
            }
        }

        #region Disposal
        /// <summary>
        /// Dispose of this instance
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Handle the heavy lifting of disposal
        /// </summary>
        /// <param name="disposing">Are we disposing?</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                if (_initialized)
                {
                    _initialized = false;
                    var host = this.browser.GetHost();
                    browser.StopLoad();
                    host.CloseBrowser();
                    host.Dispose();
                }
            }

            // Free any unmanaged objects here.
            _disposed = true;
        }
        #endregion
    }
}
