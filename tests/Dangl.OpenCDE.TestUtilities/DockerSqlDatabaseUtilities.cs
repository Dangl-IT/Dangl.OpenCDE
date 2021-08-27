using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.TestUtilities
{
    public static class DockerSqlDatabaseUtilities
    {
        public const string SQLSERVER_SA_PASSWORD = "yourStrong(!)Password";
        public const string SQLSERVER_IMAGE = "dangl/mssql-tmpfs";
        public const string SQLSERVER_IMAGE_TAG = "latest";
        public const string SQLSERVER_CONTAINER_NAME_PREFIX = "CdeIntegrationTestsSql";

        public static async Task<(string containerId, string port, string containerName)> EnsureDockerStartedAndGetContainerIdAndPortAsync(string networkName = null)
        {
            await CleanupRunningContainers();
            var dockerClient = GetDockerClient();
            var freePort = GetFreePort();

            // This call ensures that the latest SQL Server Docker image is pulled
            await dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
            {
                FromImage = $"{SQLSERVER_IMAGE}:{SQLSERVER_IMAGE_TAG}"
            }, null, new Progress<JSONMessage>());

            var containerName = (SQLSERVER_CONTAINER_NAME_PREFIX + Guid.NewGuid()).Replace("-", string.Empty);
            var sqlContainerStartParameters = new CreateContainerParameters
            {
                Name = containerName,
                Image = $"{SQLSERVER_IMAGE}:{SQLSERVER_IMAGE_TAG}",
                Env = new List<string>
                    {
                        "ACCEPT_EULA=Y",
                        $"SA_PASSWORD={SQLSERVER_SA_PASSWORD}"
                    },
                HostConfig = new HostConfig
                {
                    Tmpfs = new Dictionary<string, string>
                        {
                            {"/var/opt/mssql/data", "" },
                            {"/var/opt/mssql/log", "" },
                            {"/var/opt/mssql/secrets", "" }
                        },
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                        {
                            {
                                "1433/tcp",
                                new PortBinding[]
                                {
                                    new PortBinding
                                    {
                                        HostPort = freePort
                                    }
                                }
                            }
                        }
                }
            };

            if (!string.IsNullOrWhiteSpace(networkName))
            {
                sqlContainerStartParameters.HostConfig.NetworkMode = networkName;
            }

            var sqlContainer = await dockerClient
                .Containers
                .CreateContainerAsync(sqlContainerStartParameters);

            await dockerClient
                .Containers
                .StartContainerAsync(sqlContainer.ID, new ContainerStartParameters());

            await WaitUntilDatabaseAvailableAsync(freePort);
            return (sqlContainer.ID, freePort, containerName);
        }

        public static async Task CopySeedDatabaseFilesForNewDatabase(string newDatabaseName,
            string dockerContainerId)
        {
            var dockerClient = GetDockerClient();

            var dataFile = await dockerClient
                .Exec
                .ExecCreateContainerAsync(dockerContainerId,
                new ContainerExecCreateParameters
                {
                    Cmd = new List<string>
                    {
                        "cp",
                        $"/var/opt/mssql/data/{SqlServerDockerCollectionFixture.INITIAL_SEED_DATABASE_NAME}.mdf",
                        $"/var/opt/mssql/data/{newDatabaseName}.mdf"
                    }
                });

            await dockerClient.Exec.StartContainerExecAsync(dataFile.ID);

            var logFile = await dockerClient
                    .Exec
                    .ExecCreateContainerAsync(dockerContainerId,
                    new ContainerExecCreateParameters
                    {
                        Cmd = new List<string>
                        {
                                            "cp",
                                            $"/var/opt/mssql/data/{SqlServerDockerCollectionFixture.INITIAL_SEED_DATABASE_NAME}_log.ldf",
                                            $"/var/opt/mssql/data/{newDatabaseName}_log.ldf"
                        }
                    });

            await dockerClient.Exec.StartContainerExecAsync(logFile.ID);
        }

        public static async Task RemoveDatabaseFilesInContainer(string databaseName, string dockerContainerId)
        {
            var dockerClient = GetDockerClient();

            try
            {
                var dataFile = await dockerClient
                    .Exec
                    .ExecCreateContainerAsync(dockerContainerId,
                    new ContainerExecCreateParameters
                    {
                        Cmd = new List<string>
                        {
                        "rm",
                        $"/var/opt/mssql/data/{databaseName}.mdf"
                        }
                    });

                var cts1 = new CancellationTokenSource();
                cts1.CancelAfter(TimeSpan.FromSeconds(10));
                await dockerClient.Exec.StartContainerExecAsync(dataFile.ID, cts1.Token);
            }
            catch
            {
                // Ignoring failures here, the files are eventually deleted once the container is shut down
            }

            try
            {
                var logFile = await dockerClient
                    .Exec
                    .ExecCreateContainerAsync(dockerContainerId,
                    new ContainerExecCreateParameters
                    {
                        Cmd = new List<string>
                        {
                                        "rm",
                                        $"/var/opt/mssql/data/{databaseName}_log.mdf"
                        }
                    });

                var cts2 = new CancellationTokenSource();
                cts2.CancelAfter(TimeSpan.FromSeconds(10));
                await dockerClient.Exec.StartContainerExecAsync(logFile.ID, cts2.Token);
            }
            catch
            {
                // Ignoring failures here, the files are eventually deleted once the container is shut down
            }
        }

        public static async Task EnsureDockerStoppedAndRemovedAsync(string dockerContainerId)
        {
            var dockerClient = GetDockerClient();
            await dockerClient.Containers
                .StopContainerAsync(dockerContainerId, new ContainerStopParameters());
            await dockerClient.Containers
                .RemoveContainerAsync(dockerContainerId, new ContainerRemoveParameters());
        }

        private static DockerClient GetDockerClient()
        {
            var dockerUri = IsRunningOnWindows()
                ? "npipe://./pipe/docker_engine"
                : "unix:///var/run/docker.sock";
            return new DockerClientConfiguration(new Uri(dockerUri))
            {
                // The default is 100ms, which can be a bit too little for highly parallelized tests
                NamedPipeConnectTimeout = TimeSpan.FromSeconds(5)
            }
                .CreateClient();
        }

        private static async Task CleanupRunningContainers()
        {
            var dockerClient = GetDockerClient();

            var runningContainers = await dockerClient.Containers
                .ListContainersAsync(new ContainersListParameters
                {
                    All = true
                });

            foreach (var runningContainer in runningContainers.Where(cont => cont.Names.Any(n => n.Contains(SQLSERVER_CONTAINER_NAME_PREFIX))))
            {
                // Stopping all test containers that are older than one hour, they likely failed to cleanup
                if (runningContainer.Created < DateTime.UtcNow.AddHours(-1))
                {
                    try
                    {
                        await EnsureDockerStoppedAndRemovedAsync(runningContainer.ID);
                    }
                    catch
                    {
                        // Ignoring failures to stop running containers
                    }
                }
            }
        }

        private static async Task WaitUntilDatabaseAvailableAsync(string databasePort)
        {
            var start = DateTime.UtcNow;
            const int maxWaitTimeSeconds = 60;
            var connectionEstablised = false;
            while (!connectionEstablised && start.AddSeconds(maxWaitTimeSeconds) > DateTime.UtcNow)
            {
                try
                {
                    var sqlConnectionString = $"Data Source=localhost,{databasePort};Integrated Security=False;User ID=SA;Password={SQLSERVER_SA_PASSWORD}";
                    await using var sqlConnection = new SqlConnection(sqlConnectionString);
                    await sqlConnection.OpenAsync();
                    connectionEstablised = true;
                }
                catch
                {
                    // If opening the SQL connection fails, SQL Server is not ready yet
                    await Task.Delay(500);
                }
            }

            // Sometimes the database is responding but EF Core still fails to apply the migrations
            await Task.Delay(100);

            if (!connectionEstablised)
            {
                throw new Exception("Connection to the SQL docker database could not be established within 60 seconds.");
            }

            return;
        }

        private static string GetFreePort()
        {
            // Taken from https://stackoverflow.com/a/150974/4190785
            var tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();
            var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();
            return port.ToString();
        }

        private static bool IsRunningOnWindows()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
        }
    }
}
