using System;
using System.Collections.Generic;
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
            using (HeadlessBrowser browser = new HeadlessBrowser(500, 300, "http://www.cnn.com"))
            {
                WaitForBrowserInitialization(browser).GetAwaiter().GetResult();

                //TakeScreenshotAsync("http://www.google.com", 530, 530, args).GetAwaiter().GetResult();

                Console.WriteLine("Press any key.");
                Console.ReadKey();
                //TakeAnotherScreenshotAsync(1000, 530).GetAwaiter().GetResult();
                Console.WriteLine("Press any key.");
                Console.ReadKey();
            }
            Console.WriteLine("Destroying..");
            Headless.Destroy();
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

        public static async Task<int> TakeScreenshotAsync(HeadlessBrowser browser, string url, int width, int height, string[] args)
        {
            await Task.Delay(5000).ConfigureAwait(false);
            browser.WindowBitmap.Save("output.png");
            return 0;
        }

        public static async Task<int> TakeAnotherScreenshotAsync(HeadlessBrowser browser, int width, int height)
        {
            browser.Resize(width, height);
            // wait for the site to be loaded
            await Task.Delay(10000).ConfigureAwait(false);
            Console.WriteLine("Saving 2 at " + browser.WindowBitmap.Width.ToString() + ", " + browser.WindowBitmap.Height.ToString());
            browser.WindowBitmap.Save("output2.png");
            // dispose everything

            return 0;
        }
    }
}
