# CefGlueHeadless
A fully headless demonstration client for CefGlue/3. You can find CefGlue at (https://bitbucket.org/xilium/xilium.cefglue/wiki/Home). This is an example of how you could use Cef and CefGlue to create a simplified headless browser for testing, screenshotting, or video capture. This project borrows ideas and sample code from CefGlue, and [others](https://www.joelverhagen.com/blog/2013/12/headless-chromium-in-c-with-cefglue/).

The goals of the project are:

1. Create a simple interface to a headless Cef browser implementation.
1. Minimize CPU impact and not do any more work than necessary.
1. Simplify common tasks like resizing, image capture, issuing mouse events.
1. Embrace asynchronous programming and provide convenient interfaces to controlling asynchronous tasks in C#.
