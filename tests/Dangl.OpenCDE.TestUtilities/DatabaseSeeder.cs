using Dangl.AspNetCore.FileHandling;
using Dangl.AspNetCore.FileHandling.Azure;
using Dangl.OpenCDE.Data;
using Dangl.OpenCDE.Data.IO;
using Dangl.OpenCDE.TestUtilities.TestData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.TestUtilities
{
    public class DatabaseSeeder
    {
        private readonly IServiceProvider _serviceProvider;

        public DatabaseSeeder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task SeedDatabaseAsync()
        {
            using (var serviceScope = _serviceProvider.CreateScope())
            {
                await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<CdeDbContext>();
                var existingUserIds = await dbContext.Users.Select(u => u.Id).ToListAsync();
                dbContext.Users.AddRange(Users.SeedValues
                    .Where(u => !existingUserIds.Contains(u.UserId.Value))
                    .Select(u => new Data.Models.CdeUser
                    {
                        Id = u.UserId.Value,
                        Email = u.Email,
                        IdenticonId = u.IdenticonId,
                        EmailConfirmed = true,
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        NormalizedEmail = u.Email.ToUpperInvariant(),
                        NormalizedUserName = u.Username.ToUpperInvariant(),
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = u.Username
                    }));
                await dbContext.SaveChangesAsync();
            }

            using (var serviceScope = _serviceProvider.CreateScope())
            {
                await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<CdeDbContext>();
                dbContext.Projects.AddRange(Projects.Values);
                await dbContext.SaveChangesAsync();
            }

            using (var serviceScope = _serviceProvider.CreateScope())
            {
                await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<CdeDbContext>();
                dbContext.FileMimeTypes.AddRange(CdeAppFileMimeTypes.Values);
                await dbContext.SaveChangesAsync();
            }

            using (var serviceScope = _serviceProvider.CreateScope())
            {
                await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<CdeDbContext>();
                dbContext.Files.AddRange(CdeAppFiles.Values);
                await dbContext.SaveChangesAsync();
            }

            using (var serviceScope = _serviceProvider.CreateScope())
            {
                await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<CdeDbContext>();
                dbContext.Documents.AddRange(Documents.Values);
                await dbContext.SaveChangesAsync();
            }

            await GenerateSeedFilesAsync(_serviceProvider);
        }

        public static async Task GenerateSeedFilesAsync(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var fileHandler = serviceScope.ServiceProvider.GetRequiredService<IFileManager>();
                if (fileHandler is AzureBlobFileManager azureBlobFileManager)
                {
                    var storageInitializer = serviceScope.ServiceProvider.GetRequiredService<AzureBlobStorageInitializer>();

                    await storageInitializer.EnsureAzureBlobContainersPresentAsync();
                    await storageInitializer.EnsureAzureBlobStorageHasCorsEnabledAsync();
                }

                foreach (var cdeAppTestFile in CdeAppFiles.Values)
                {
                    await using var testFileStream = TestFilesFactory.GetTestfileAsStream(cdeAppTestFile.TestFile);
                    await fileHandler.SaveFileAsync(cdeAppTestFile.Id,
                        cdeAppTestFile.ContainerName,
                        cdeAppTestFile.FileName,
                        testFileStream);
                }
            }
        }
    }
}
