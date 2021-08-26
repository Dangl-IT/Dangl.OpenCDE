using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Data.Models
{
    public class Document
    {
        public Guid Id { get; set; }

        public Guid ProjectId { get; set; }

        public Project Project { get; set; }

        [MaxLength(400)]
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid? FileId { get; set; }

        public CdeAppFile File { get; set; }

        public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>()
                .HasIndex(e => e.Name);

            modelBuilder.Entity<Document>()
                .HasIndex(e => e.ProjectId);
        }
    }
}
