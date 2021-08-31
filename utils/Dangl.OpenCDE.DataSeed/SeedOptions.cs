using CommandLine;

namespace Dangl.OpenCDE.DataSeed
{
    public class SeedOptions
    {
        [Option('s', "sql-connection-string", Required = true, HelpText = "The SQL Server connection string")]
        public string SqlConnectionString { get; set; }

        [Option('a', "azure-blob-file-manager-connection-string", Required = false, HelpText = "The connection string for Azure Blob file storage")]
        public string AzureBlobFileManagerConnectionString { get; set; }
    }
}
