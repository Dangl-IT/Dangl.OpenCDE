using Dangl.OpenCDE.TestUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Dangl.OpenCDE.Tests.Integration
{
    public class TestFilesFactoryTests
    {
        public static List<object[]> TestFiles => Enum.GetValues(typeof(TestFile))
                .Cast<TestFile>()
                .Select(tf => new object[] { tf })
                .ToList();

        [Fact]
        public void FindsAnyTestFiles()
        {
            Assert.Equal(4, TestFiles.Count);
        }

        [Theory]
        [MemberData(nameof(TestFiles))]
        public void CanReadTestFileAsStream(TestFile testFile)
        {
            var result = TestFilesFactory.GetTestfileAsStream(testFile);
            Assert.NotNull(result);
        }
    }
}
