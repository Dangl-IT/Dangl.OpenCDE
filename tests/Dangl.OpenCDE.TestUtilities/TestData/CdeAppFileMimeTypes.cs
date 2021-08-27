using Dangl.OpenCDE.Data.Models;
using System;
using System.Collections.Generic;

namespace Dangl.OpenCDE.TestUtilities.TestData
{
    public static class CdeAppFileMimeTypes
    {
        public static List<CdeAppFileMimeType> Values => new List<CdeAppFileMimeType>
        {
            CdeAppFileMimeType01,
            CdeAppFileMimeType02
        };

        public static CdeAppFileMimeType CdeAppFileMimeType01 => new CdeAppFileMimeType
        {
            Id = new Guid("1125adf1-3341-49f6-b29d-c3c73c973dd0"),
            MimeType = "application/octet-stream"
        };

        public static CdeAppFileMimeType CdeAppFileMimeType02 => new CdeAppFileMimeType
        {
            Id = new Guid("3654c51c-388a-43ad-9a1c-06d99e14a841"),
            MimeType = "image/jpeg"
        };
    }
}
