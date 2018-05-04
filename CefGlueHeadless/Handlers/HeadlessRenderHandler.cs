using CefGlueHeadless.Delegates;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Xilium.CefGlue;

namespace CefGlueHeadless.Handlers
{
    /// <summary>
    /// Handles rendering heavy lifting
    /// </summary>
    public class HeadlessRenderHandler : CefRenderHandler
    {
        /// <summary>
        /// Holds a copy of the underlying browser for convenience
        /// </summary>
        private readonly HeadlessBrowser headlessBrowser;

        /// <summary>
        /// Reports the actual discovered width of the CefBrowser
        /// </summary>
        public int actualWidth = 0;

        /// <summary>
        /// Reports the actual discovered height of the CefBrowser
        /// </summary>
        public int actualHeight = 0;
        
        /// <summary>
        /// Fires when the underlying browser size has changed
        /// </summary>
        public event BrowserSizeChangedDelegate OnSizeChanged;

        /// <summary>
        /// Set up a new instance of what will be the render handler
        /// </summary>
        /// <param name="headlessBrowser"></param>
        public HeadlessRenderHandler(HeadlessBrowser headlessBrowser)
        {
            this.headlessBrowser = headlessBrowser;
            this.actualWidth = headlessBrowser.Width;
            this.actualHeight = headlessBrowser.Height;
        }

        /// <summary>
        /// Handle paints
        /// </summary>
        /// <param name="browser">Underlying CefBrowser</param>
        /// <param name="type">Paint element type</param>
        /// <param name="dirtyRects">Parts of the display that are dirty</param>
        /// <param name="buffer">Pointer to video memory</param>
        /// <param name="width">Actual underlying browser width</param>
        /// <param name="height">Actual underlying browser height</param>
        protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
        {
            var rect = new Rectangle(0, 0, headlessBrowser.Width, headlessBrowser.Height);
            bool signalSizeChanged = false;
            try
            {
                if (width != headlessBrowser.WindowBitmap.Width || height != headlessBrowser.WindowBitmap.Height)
                {
                    headlessBrowser.WindowBitmap.Dispose();
                    headlessBrowser.WindowBitmap = new Bitmap(width, height);
                    signalSizeChanged = true;
                }
                BitmapData screenShot = headlessBrowser.WindowBitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                int totalSize = screenShot.Stride * screenShot.Height;
                var temporaryBuffer = new byte[totalSize];

                IntPtr screenShotPtr = screenShot.Scan0;

                Marshal.Copy(buffer, temporaryBuffer, 0, totalSize);
                Marshal.Copy(temporaryBuffer, 0, screenShotPtr, totalSize);

                headlessBrowser.WindowBitmap.UnlockBits(screenShot);
            }
            catch (Exception ex)
            {
                // Don't bother until we have an actual image
                signalSizeChanged = false;
            }

            // Signal size changes if necessary
            if (signalSizeChanged)
            {
                actualWidth = width;
                actualHeight = height;
                OnSizeChanged?.Invoke(headlessBrowser, width, height);
            }
        }

        protected override bool GetScreenInfo(CefBrowser browser, CefScreenInfo screenInfo)
        {
            return false;
        }

        protected override bool GetRootScreenRect(CefBrowser browser, ref CefRectangle rect)
        {
            rect.X = 0;
            rect.Y = 0;
            rect.Width = headlessBrowser.Width;
            rect.Height = headlessBrowser.Height;
            return true;
        }

        protected override bool GetScreenPoint(CefBrowser browser, int viewX, int viewY, ref int screenX, ref int screenY)
        {
            screenX = viewX;
            screenY = viewY;
            return true;
        }

        protected override bool GetViewRect(CefBrowser browser, ref CefRectangle rect)
        {
            rect = new CefRectangle(0, 0, headlessBrowser.Width, headlessBrowser.Height);
            return true;
        }

        protected override void OnPopupSize(CefBrowser browser, CefRectangle rect) { }

        protected override void OnCursorChange(CefBrowser browser, IntPtr cursorHandle, CefCursorType type, CefCursorInfo customCursorInfo) { }

        protected override void OnScrollOffsetChanged(CefBrowser browser, double x, double y) { }

        protected override CefAccessibilityHandler GetAccessibilityHandler()
        {
            //throw new NotImplementedException();
            return null;
        }

        protected override void OnImeCompositionRangeChanged(CefBrowser browser, CefRange selectedRange, CefRectangle[] characterBounds)
        {
            //throw new NotImplementedException();
        }
    }
}
