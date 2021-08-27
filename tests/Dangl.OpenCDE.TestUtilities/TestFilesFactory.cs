using System;
using System.IO;
using System.Linq;

namespace Dangl.OpenCDE.TestUtilities
{
    public class TestFilesFactory
    {
        public static string GetTestFileContentAsString(TestFile testFile)
        {
            using var resourceStream = GetTestfileAsStream(testFile);
            using var streamReader = new StreamReader(resourceStream);
            return streamReader.ReadToEnd();
        }

        public static Stream GetTestfileAsStream(TestFile testFile)
        {
            var resourceName = typeof(TestFilesFactory).Assembly
                .GetManifestResourceNames()
                .FirstOrDefault(r => r.Contains(testFile.ToString()));

            if (resourceName == null)
            {
                resourceName = typeof(TestFilesFactory).Assembly
                    .GetManifestResourceNames()
                    .SingleOrDefault(r => r
                    .Replace(".", "_").EndsWith(testFile.ToString()));
            }
            if (resourceName == null)
            {
                throw new Exception("No resource found for the test file");
            }

            return typeof(TestFilesFactory)
                .Assembly
                .GetManifestResourceStream(resourceName);
        }

        public static Stream GetTestfileAsStream(string testFile)
        {
            var resourceName = typeof(TestFilesFactory).Assembly
                .GetManifestResourceNames()
                .Single(r => r.Contains(testFile));
            return typeof(TestFilesFactory)
                .Assembly
                .GetManifestResourceStream(resourceName);
        }

        public static string GetTestFileAsBase64String(TestFile testFile)
        {
            using var testFileStream = GetTestfileAsStream(testFile);
            return ConvertStreamToBase64String(testFileStream);
        }

        public static string ConvertStreamToBase64String(Stream stream)
        {
            using var memStream = new MemoryStream();
            stream.CopyTo(memStream);
            var bytes = memStream.ToArray();
            return Convert.ToBase64String(bytes);
        }
    }
}
