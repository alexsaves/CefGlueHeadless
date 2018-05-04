# CefGlueHeadless
A fully headless demonstration client for CefGlue/3. You can find CefGlue at (https://bitbucket.org/xilium/xilium.cefglue/wiki/Home). This is an example of how you could use Cef and CefGlue to create a simplified headless browser for testing, screenshotting, or video capture. This project borrows ideas and sample code from CefGlue, and [others](https://www.joelverhagen.com/blog/2013/12/headless-chromium-in-c-with-cefglue/).

The goals of the project are:

1. Create a simple interface to a headless Cef browser implementation.
1. Minimize CPU impact and not do any more work than necessary.
1. Simplify common tasks like resizing, image capture, issuing mouse events.
1. Embrace asynchronous programming and provide convenient interfaces to controlling asynchronous tasks in C#.

# What you will need

You will need CefGlue, and a fully compiled binary for Cef for your operating system, which you can find here (http://opensource.spotify.com/cefbuilds/index.html). Note: you must get the exact right version of CEF for your version of CefGlue. Place these files in the Dependencies/ folder and use the text file describing what files need to be where.

# Build notes

When the sample app in this demo builds, it XCOPY's the files from the Dependencies/ folder to the location of your EXE. 

# Troubleshooting

If you get something about 'Unexpected Image' or 'Bad Image' its either because you are using the X64 version of CEF for a X86 version of your app or vice versa.

If you get something about a missing image, then it's probably because you haven't got the CEF binaries in the same folder as your EXE. Take a look at the TXT file in the Dependencies/ folder for information. I can't include these files for licensing reasons.