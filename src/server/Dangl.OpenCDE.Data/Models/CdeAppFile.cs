using Dangl.AspNetCore.FileHandling;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Data.Models
{
    public class CdeAppFile
    {
        public Guid Id { get; set; }

        public Guid MimeTypeId { get; set; }

        public CdeAppFileMimeType MimeType { get; set; }

        [Required]
        public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

        public Guid? CreatedByUserId { get; set; }

        public CdeUser CreatedByUser { get; set; }

        [Required, MaxLength(FileHandlerDefaults.FILE_CONTAINER_NAME_MAX_LENGTH)]
        public string ContainerName { get; set; }

        [Required, MaxLength(255)]
        public string FileName { get; set; }

        [Required]
        public long SizeInBytes { get; set; }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CdeAppFile>()
                .HasOne(e => e.MimeType)
                .WithMany()
                .HasForeignKey(e => e.MimeTypeId)
                .IsRequired(true);

            modelBuilder.Entity<CdeAppFile>()
                .HasIndex(e => e.FileName);

            modelBuilder.Entity<CdeAppFile>()
                .HasIndex(e => e.CreatedAtUtc);

            modelBuilder.Entity<CdeAppFile>()
                .HasIndex(e => e.ContainerName);
        }
    }
}
