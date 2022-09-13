using Dangl.OpenCDE.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Dangl.OpenCDE.Data
{
    public class CdeDbContext : IdentityDbContext<CdeUser, CdeRole, Guid>
    {
        public CdeDbContext(DbContextOptions<CdeDbContext> options) : base(options)
        {
        }

        public DbSet<Document> Documents { get; set; }
        public DbSet<CdeAppFile> Files { get; set; }
        public DbSet<CdeAppFileMimeType> FileMimeTypes { get; set; }
        public DbSet<OpenCdeDocumentSelection> OpenCdeDocumentSelections { get; set; }
        public DbSet<OpenCdeDocumentUploadSession> OpenCdeDocumentUploadSessions { get; set; }
        public DbSet<OpenCdeDocumentDownloadSession> OpenCdeDocumentDownloadSessions { get; set; }
        public DbSet<PendingOpenCdeUploadFile> PendingOpenCdeUploadFiles { get; set; }
        public DbSet<Project> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            Document.OnModelCreating(builder);
            CdeAppFile.OnModelCreating(builder);
            CdeAppFileMimeType.OnModelCreating(builder);
            Project.OnModelCreating(builder);

            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(entity => entity.GetProperties())
                .Where(property => property.ClrType == typeof(decimal)
                    || property.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(28,6)");
            }

            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(entity => entity.GetProperties())
                .Where(property => property.ClrType == typeof(Guid)
                    && property.Name == "Id"))
            {
                if (string.IsNullOrWhiteSpace(property.GetDefaultValueSql()))
                {
                    property.SetDefaultValueSql("newsequentialid()");
                }
            }

            base.OnModelCreating(builder);
        }
    }
}
