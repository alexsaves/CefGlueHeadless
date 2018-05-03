using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Xilium.CefGlue;

namespace CefGlueHeadless.Handlers
{
    public class HeadlessRenderHandler : CefRenderHandler
    {
        private readonly HeadlessBrowser headlessBrowser;

        public HeadlessRenderHandler(HeadlessBrowser headlessBrowser)
        {
            this.headlessBrowser = headlessBrowser;
        }

        protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
        {
            var rect = new Rectangle(0, 0, headlessBrowser.Width, headlessBrowser.Height);

            try
            {
                if (width != headlessBrowser.WindowBitmap.Width || height != headlessBrowser.WindowBitmap.Height)
                {
                    headlessBrowser.WindowBitmap.Dispose();
                    Console.WriteLine("** resizing bitmap to " + width.ToString() + ", " + height.ToString());
                    headlessBrowser.WindowBitmap = new Bitmap(width, height);
                }
                var screenShot = headlessBrowser.WindowBitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                var totalSize = screenShot.Stride * screenShot.Height;
                var temporaryBuffer = new byte[totalSize];

                var screenShotPtr = screenShot.Scan0;

                Marshal.Copy(buffer, temporaryBuffer, 0, totalSize);
                Marshal.Copy(temporaryBuffer, 0, screenShotPtr, totalSize);

                headlessBrowser.WindowBitmap.UnlockBits(screenShot);
                Console.WriteLine("BM Width: " + width.ToString() + ", vs: " + headlessBrowser.Width.ToString());

            }
            catch (Exception ex)
            {
                //throw;
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
