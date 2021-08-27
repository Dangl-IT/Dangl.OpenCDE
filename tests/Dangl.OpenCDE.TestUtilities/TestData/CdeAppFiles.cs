using Dangl.OpenCDE.Data.IO;
using System;
using System.Collections.Generic;

namespace Dangl.OpenCDE.TestUtilities.TestData
{
    public static class CdeAppFiles
    {
        public static List<IntegrationTestsCdeAppFile> Values => new List<IntegrationTestsCdeAppFile>
        {
            Project01_File01_Picture,
            Project01_File02_Ifc,
            Project01_File03_GAEB,
            Project01_File04_Excel,
            Project02_File01_Picture,
            Project02_File02_Ifc,
            Project02_File03_GAEB,
            Project02_File04_Excel,
            Project03_File01_Picture,
            Project03_File02_Ifc,
            Project03_File03_GAEB,
            Project03_File04_Excel
        };

        public static IntegrationTestsCdeAppFile Project01_File01_Picture => new IntegrationTestsCdeAppFile
        {
            Id = new Guid("e85f5ad2-7576-4f55-be49-8bdd82c8d4bd"),
            ContainerName = FileContainerNames.PROJECT_DOCUMENTS,
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            FileName = "Construction Site.jpg",
            MimeTypeId = CdeAppFileMimeTypes.CdeAppFileMimeType02.Id,
            SizeInBytes = 88_706,
            TestFile = TestFile.construction_site_picture
        };

        public static IntegrationTestsCdeAppFile Project01_File02_Ifc => new IntegrationTestsCdeAppFile
        {
            Id = new Guid("4fcae3c1-e307-485b-ae6d-9e99a534bad7"),
            ContainerName = FileContainerNames.PROJECT_DOCUMENTS,
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            FileName = "Duplex House.ifc",
            MimeTypeId = CdeAppFileMimeTypes.CdeAppFileMimeType02.Id,
            SizeInBytes = 2_366_050,
            TestFile = TestFile.DuplexHouse_Architecture
        };

        public static IntegrationTestsCdeAppFile Project01_File03_GAEB => new IntegrationTestsCdeAppFile
        {
            Id = new Guid("f922eae3-78dd-464d-8b55-21658b215dd2"),
            ContainerName = FileContainerNames.PROJECT_DOCUMENTS,
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            FileName = "BoQ.X86",
            MimeTypeId = CdeAppFileMimeTypes.CdeAppFileMimeType02.Id,
            SizeInBytes = 10_174,
            TestFile = TestFile.GAEBXML_EN_Minimal_x86
        };

        public static IntegrationTestsCdeAppFile Project01_File04_Excel => new IntegrationTestsCdeAppFile
        {
            Id = new Guid("5af98d72-b9a4-4055-8246-72c2f5754ba7"),
            ContainerName = FileContainerNames.PROJECT_DOCUMENTS,
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            FileName = "BoQ.xlsx",
            MimeTypeId = CdeAppFileMimeTypes.CdeAppFileMimeType02.Id,
            SizeInBytes = 17_863,
            TestFile = TestFile.GAEBXML_EN_Minimal_xlsx
        };

        public static IntegrationTestsCdeAppFile Project02_File01_Picture => new IntegrationTestsCdeAppFile
        {
            Id = new Guid("9ee61847-723d-48ce-a80b-724c77196376"),
            ContainerName = FileContainerNames.PROJECT_DOCUMENTS,
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            FileName = "Construction Site.jpg",
            MimeTypeId = CdeAppFileMimeTypes.CdeAppFileMimeType02.Id,
            SizeInBytes = 88_706,
            TestFile = TestFile.construction_site_picture
        };

        public static IntegrationTestsCdeAppFile Project02_File02_Ifc => new IntegrationTestsCdeAppFile
        {
            Id = new Guid("0d370942-281a-40bb-a0d9-0f98944441c1"),
            ContainerName = FileContainerNames.PROJECT_DOCUMENTS,
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            FileName = "Duplex House.ifc",
            MimeTypeId = CdeAppFileMimeTypes.CdeAppFileMimeType02.Id,
            SizeInBytes = 2_366_050,
            TestFile = TestFile.DuplexHouse_Architecture
        };

        public static IntegrationTestsCdeAppFile Project02_File03_GAEB => new IntegrationTestsCdeAppFile
        {
            Id = new Guid("211450ac-1c9f-401a-8a96-c64dafec4f27"),
            ContainerName = FileContainerNames.PROJECT_DOCUMENTS,
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            FileName = "BoQ.X86",
            MimeTypeId = CdeAppFileMimeTypes.CdeAppFileMimeType02.Id,
            SizeInBytes = 10_174,
            TestFile = TestFile.GAEBXML_EN_Minimal_x86
        };

        public static IntegrationTestsCdeAppFile Project02_File04_Excel => new IntegrationTestsCdeAppFile
        {
            Id = new Guid("44bd1359-11eb-46ef-9190-7d2c5c225fcf"),
            ContainerName = FileContainerNames.PROJECT_DOCUMENTS,
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            FileName = "BoQ.xlsx",
            MimeTypeId = CdeAppFileMimeTypes.CdeAppFileMimeType02.Id,
            SizeInBytes = 17_863,
            TestFile = TestFile.GAEBXML_EN_Minimal_xlsx
        };

        public static IntegrationTestsCdeAppFile Project03_File01_Picture => new IntegrationTestsCdeAppFile
        {
            Id = new Guid("41692ca9-8718-4d33-ba05-356a793e8b61"),
            ContainerName = FileContainerNames.PROJECT_DOCUMENTS,
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            FileName = "Construction Site.jpg",
            MimeTypeId = CdeAppFileMimeTypes.CdeAppFileMimeType02.Id,
            SizeInBytes = 88_706,
            TestFile = TestFile.construction_site_picture
        };

        public static IntegrationTestsCdeAppFile Project03_File02_Ifc => new IntegrationTestsCdeAppFile
        {
            Id = new Guid("922b0470-df47-41c8-8c95-b94a30fbcea2"),
            ContainerName = FileContainerNames.PROJECT_DOCUMENTS,
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            FileName = "Duplex House.ifc",
            MimeTypeId = CdeAppFileMimeTypes.CdeAppFileMimeType02.Id,
            SizeInBytes = 2_366_050,
            TestFile = TestFile.DuplexHouse_Architecture
        };

        public static IntegrationTestsCdeAppFile Project03_File03_GAEB => new IntegrationTestsCdeAppFile
        {
            Id = new Guid("2671f670-3e64-45c2-89b3-2cd703f330cc"),
            ContainerName = FileContainerNames.PROJECT_DOCUMENTS,
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            FileName = "BoQ.X86",
            MimeTypeId = CdeAppFileMimeTypes.CdeAppFileMimeType02.Id,
            SizeInBytes = 10_174,
            TestFile = TestFile.GAEBXML_EN_Minimal_x86
        };

        public static IntegrationTestsCdeAppFile Project03_File04_Excel => new IntegrationTestsCdeAppFile
        {
            Id = new Guid("ab2f22ef-e8b1-46c0-a863-a00120478b6f"),
            ContainerName = FileContainerNames.PROJECT_DOCUMENTS,
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            FileName = "BoQ.xlsx",
            MimeTypeId = CdeAppFileMimeTypes.CdeAppFileMimeType02.Id,
            SizeInBytes = 17_863,
            TestFile = TestFile.GAEBXML_EN_Minimal_xlsx
        };
    }
}
