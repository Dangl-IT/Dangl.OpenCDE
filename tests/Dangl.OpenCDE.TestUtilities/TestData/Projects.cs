using Dangl.OpenCDE.Data.Models;
using System;
using System.Collections.Generic;

namespace Dangl.OpenCDE.TestUtilities.TestData
{
    public static class Projects
    {
        public static List<Project> Values => new List<Project>
        {
            Project01,
            Project02,
            Project03
        };

        public static Project Project01 => new Project
        {
            Id = new Guid("57f79803-c6f7-4fa1-b1e3-c7fc23552e7d"),
            Name = "Hospital Expansion",
            CreatedAtUtc = new DateTimeOffset(2020, 7, 2, 17, 39, 0, TimeSpan.Zero),
            IdenticonId = new Guid("16ed6ee9-8d3b-4b8a-a8db-f18994d2e240"),
            Description = "Constructing a new east wing for the existing building"
        };

        public static Project Project02 => new Project
        {
            Id = new Guid("c84b199c-7933-484e-bff4-dacf1b3bff6b"),
            Name = "New Middle School",
            CreatedAtUtc = new DateTimeOffset(2020, 7, 3, 17, 39, 0, TimeSpan.Zero),
            IdenticonId = new Guid("48b50440-7c39-4394-8a64-31ccdb76cf8c"),
            Description = "Rosenheim's new school will house up to 1.000 students"
        };

        public static Project Project03 => new Project
        {
            Id = new Guid("48fb5140-f444-446f-b50c-f674514ebe1d"),
            Name = "Rental Modernization",
            CreatedAtUtc = new DateTimeOffset(2020, 7, 4, 17, 39, 0, TimeSpan.Zero),
            IdenticonId = new Guid("daaac8e0-98be-42c9-a897-6ca2797a96b7"),
            Description = "Bringing existing rental units up to date with the newest regulations"
        };
    }
}
