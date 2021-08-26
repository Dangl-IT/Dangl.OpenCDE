using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Dangl.OpenCDE.Core.Tests
{
    public class ControllerRouteTests
    {
        [Fact]
        public void FindsAnyRoutes()
        {
            Assert.NotEmpty(DefinedRoutes);
        }

        [Theory]
        [MemberData(nameof(DefinedRoutes))]
        public void AllRoutesBeginWithApi(string route)
        {
            Assert.StartsWith("api", route);
        }

        public static List<object[]> DefinedRoutes => GetAllRoutes().Select(r => new object[] { r }).ToList();

        private static List<string> GetAllRoutes()
        {
            var routeAttributes = typeof(Controllers.StatusController)
                .Assembly
                .DefinedTypes
                .Where(t => typeof(ControllerBase).IsAssignableFrom(t))
                .Where(t => t.GetCustomAttributes(true).Any(at => at is RouteAttribute))
                .Select(t => t.GetCustomAttributes(true).OfType<RouteAttribute>().First().Template)
                .ToList();
            return routeAttributes;
        }
    }
}
