using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Data.Models
{
    public class CdeAppFileMimeType
    {
        public Guid Id { get; set; }

        [Required, MaxLength(255)]
        public string MimeType { get; set; }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CdeAppFileMimeType>()
                .HasIndex(e => e.MimeType)
                .IsUnique(true);
        }
    }
}
