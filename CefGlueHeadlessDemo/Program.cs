using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefGlueHeadless;

namespace CefGlueHeadlessDemo
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Setting up..");
            Console.WriteLine("****");
            Headless.Initialize();
            using (HeadlessBrowser browser = new HeadlessBrowser(500, 300, "http://www.cnn.com", 10))
            {
                WaitForBrowserInitialization(browser).GetAwaiter().GetResult();
                Console.WriteLine();
                Console.WriteLine("******************************************************************************");
                Console.WriteLine("***** Initialized!!");
                Console.WriteLine("******************************************************************************");
                Console.ReadKey();
                ResizeAndScreenshot(browser, 750, 300).GetAwaiter().GetResult();
                Console.WriteLine("Got Screenshot");
                ResizeAndScreenshot(browser, 1000, 450).GetAwaiter().GetResult();
                Console.WriteLine("Got Screenshot");
                ResizeAndScreenshot(browser, 405, 600).GetAwaiter().GetResult();
                Console.WriteLine("Got Screenshot");
            }
            Console.WriteLine("Destroying..");
            Headless.Destroy();
            Environment.Exit(0);
        }
        
        /// <summary>
        /// Wait for a browser initialization
        /// </summary>
        /// <param name="browser">Browser instance</param>
        /// <returns></returns>
        public static async Task<int> WaitForBrowserInitialization(HeadlessBrowser browser)
        {
            await browser.WaitForBrowserToInitialize().ConfigureAwait(false);
            return 0; 
        }

        /// <summary>
        /// Change the browser size, wait, then screenshot
        /// </summary>
        /// <param name="browser">Browser instance</param>
        /// <returns></returns>
        public static async Task<int> ResizeAndScreenshot(HeadlessBrowser browser, int w, int h)
        {
            await browser.ResizeAsync(w, h).ConfigureAwait(false);
            Bitmap result = await browser.RequestFrameAsync().ConfigureAwait(false);
            result.Save("b_" + w.ToString() + "x" + h.ToString() + ".png");
            return 0;
        }
        
    }
}
