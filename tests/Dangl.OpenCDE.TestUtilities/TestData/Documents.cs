﻿using Dangl.OpenCDE.Data.Models;
using System;
using System.Collections.Generic;

namespace Dangl.OpenCDE.TestUtilities.TestData
{
    public static class Documents
    {
        public static List<Document> Values => new List<Document>
        {
            Project01_Document01,
            Project01_Document02,
            Project01_Document03,
            Project01_Document04,
            Project01_Document05,
            Project02_Document01,
            Project02_Document02,
            Project02_Document03,
            Project02_Document04,
            Project02_Document05,
            Project03_Document01,
            Project03_Document02,
            Project03_Document03,
            Project03_Document04,
            Project03_Document05,
        };

        public static Document Project01_Document01 => new Document
        {
            Id = new Guid("b4471132-3722-4d33-9e1c-779c3e79e4b0"),
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            Description = "This is an example document generated by the database seed",
            Name = "Picture",
            FileId = CdeAppFiles.Project01_File01_Picture.Id,
            ProjectId = Projects.Project01.Id
        };

        public static Document Project01_Document02 => new Document
        {
            Id = new Guid("2b96cc61-82b7-4a8d-b874-b5299d23c040"),
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            Description = "This is an example document generated by the database seed",
            Name = "IFC Model",
            FileId = CdeAppFiles.Project01_File02_Ifc.Id,
            ProjectId = Projects.Project01.Id
        };

        public static Document Project01_Document03 => new Document
        {
            Id = new Guid("ed8e3540-a0c9-4a66-aad0-1b90f4c4f3ce"),
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            Description = "This is an example document generated by the database seed",
            Name = "GAEB BoQ",
            FileId = CdeAppFiles.Project01_File03_GAEB.Id,
            ProjectId = Projects.Project01.Id
        };

        public static Document Project01_Document04 => new Document
        {
            Id = new Guid("16bb2c1e-4641-4780-a1de-04a5745d2db3"),
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            Description = "This is an example document generated by the database seed",
            Name = "Excel BoQ",
            FileId = CdeAppFiles.Project01_File04_Excel.Id,
            ProjectId = Projects.Project01.Id
        };

        public static Document Project01_Document05 => new Document
        {
            Id = new Guid("903b9c05-5767-4c99-b5d3-3d72c6df3a08"),
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            Description = "This is an example document generated by the database seed. It has no content available, just metadata",
            Name = "Missing Document",
            ProjectId = Projects.Project01.Id
        };

        public static Document Project02_Document01 => new Document
        {
            Id = new Guid("3a06cd91-ae0f-4684-afe6-7e1e115ff614"),
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            Description = "This is an example document generated by the database seed",
            Name = "Picture",
            FileId = CdeAppFiles.Project02_File01_Picture.Id,
            ProjectId = Projects.Project02.Id
        };

        public static Document Project02_Document02 => new Document
        {
            Id = new Guid("fec4bfd7-d81b-46b9-966a-ff785be2ac65"),
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            Description = "This is an example document generated by the database seed",
            Name = "IFC Model",
            FileId = CdeAppFiles.Project02_File02_Ifc.Id,
            ProjectId = Projects.Project02.Id
        };

        public static Document Project02_Document03 => new Document
        {
            Id = new Guid("4737fedf-2e17-4bbd-9368-d23bcb59ea6d"),
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            Description = "This is an example document generated by the database seed",
            Name = "GAEB BoQ",
            FileId = CdeAppFiles.Project02_File03_GAEB.Id,
            ProjectId = Projects.Project02.Id
        };

        public static Document Project02_Document04 => new Document
        {
            Id = new Guid("2388c9fb-1589-4245-b67f-a353a78da25a"),
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            Description = "This is an example document generated by the database seed",
            Name = "Excel BoQ",
            FileId = CdeAppFiles.Project02_File04_Excel.Id,
            ProjectId = Projects.Project02.Id
        };

        public static Document Project02_Document05 => new Document
        {
            Id = new Guid("8935ac18-243c-4c86-b2b4-ba5e5b3620cb"),
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            Description = "This is an example document generated by the database seed. It has no content available, just metadata",
            Name = "Missing Document",
            ProjectId = Projects.Project02.Id
        };

        public static Document Project03_Document01 => new Document
        {
            Id = new Guid("d2cd5099-bd63-4cd3-87e8-4d5762785327"),
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            Description = "This is an example document generated by the database seed",
            Name = "Picture",
            FileId = CdeAppFiles.Project03_File01_Picture.Id,
            ProjectId = Projects.Project03.Id
        };

        public static Document Project03_Document02 => new Document
        {
            Id = new Guid("2a93cf3b-3fa4-4e5e-a5e8-ce667acfcfa2"),
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            Description = "This is an example document generated by the database seed",
            Name = "IFC Model",
            FileId = CdeAppFiles.Project03_File02_Ifc.Id,
            ProjectId = Projects.Project03.Id
        };

        public static Document Project03_Document03 => new Document
        {
            Id = new Guid("5bfb135a-f373-4598-b2ba-b8ee77dcab34"),
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            Description = "This is an example document generated by the database seed",
            Name = "GAEB BoQ",
            FileId = CdeAppFiles.Project03_File03_GAEB.Id,
            ProjectId = Projects.Project03.Id
        };

        public static Document Project03_Document04 => new Document
        {
            Id = new Guid("23dd6bc3-0ea5-46a2-80bd-6a775f655674"),
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            Description = "This is an example document generated by the database seed",
            Name = "Excel BoQ",
            FileId = CdeAppFiles.Project03_File04_Excel.Id,
            ProjectId = Projects.Project03.Id
        };

        public static Document Project03_Document05 => new Document
        {
            Id = new Guid("3203d251-6ed1-4da9-9642-20609d054fd0"),
            CreatedAtUtc = new DateTimeOffset(2021, 8, 26, 12, 0, 0, TimeSpan.Zero),
            Description = "This is an example document generated by the database seed. It has no content available, just metadata",
            Name = "Missing Document",
            ProjectId = Projects.Project03.Id
        };
    }
}