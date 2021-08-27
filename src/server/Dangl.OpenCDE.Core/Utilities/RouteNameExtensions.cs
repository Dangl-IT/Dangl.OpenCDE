using System;

namespace Dangl.OpenCDE.Core.Utilities
{
    public static class RouteNameExtensions
    {
        public static string WithoutAsyncSuffix(this string input)
        {
            if (input.EndsWith("Async", StringComparison.InvariantCultureIgnoreCase))
            {
                return input[0..^5];
            }

            return input;
        }

        public static string WithoutControllerSuffix(this string input)
        {
            if (input.EndsWith("Controller", StringComparison.InvariantCultureIgnoreCase))
            {
                return input[0..^10];
            }

            return input;
        }
    }
}
