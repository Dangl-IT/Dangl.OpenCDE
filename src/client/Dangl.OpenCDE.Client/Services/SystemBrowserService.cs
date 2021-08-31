using System;
using System.Diagnostics;

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
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }
    }
}
