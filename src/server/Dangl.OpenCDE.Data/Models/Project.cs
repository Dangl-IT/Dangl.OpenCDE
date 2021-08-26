using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Data.Models
{
    public class Project
    {
        public Guid Id { get; set; }

        [MaxLength(400), Required]
        public string Name { get; set; }

        public Guid IdenticonId { get; set; }

        public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

        public string Description { get; set; }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasIndex(p => p.Name)
                .IsUnique();
        }
    }
}
