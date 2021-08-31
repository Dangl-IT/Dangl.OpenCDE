using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Dangl.OpenCDE.Client.Services
{
    public static class SystemBrowserService
    {
        public static bool IsAbsoluteUri(this string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }

        public static void OpenSystemBrowser(string url)
        {
            if (url.IsAbsoluteUri())
            {
                // Take from here: https://stackoverflow.com/a/38604462/4190785
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
            }
        }
    }
}
