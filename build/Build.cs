using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.DocFX.DocFXTasks;
using static Nuke.Common.IO.TextTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;
using static Nuke.Common.Tools.Docker.DockerTasks;
using static Nuke.Common.Tools.Npm.NpmTasks;
using static Nuke.Common.IO.HttpTasks;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Tools.DocFX;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Tools.Coverlet;
using System.IO;
using Nuke.Common.Tools.ReportGenerator;
using System.Xml.Linq;
using static Nuke.Common.Tools.NSwag.NSwagTasks;
using Nuke.Common.Tools.NSwag;
using Nuke.Common.Tools.Teams;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nuke.Common.Tools.Slack;
using Nuke.Common.Tools.AzureKeyVault.Attributes;
using Nuke.Common.Tools.AzureKeyVault;
using static Nuke.GitHub.GitHubTasks;
using static Nuke.GitHub.ChangeLogExtensions;
using static Nuke.Common.ChangeLog.ChangelogTasks;
using Nuke.GitHub;
using System.IO.Compression;
using static Nuke.WebDocu.WebDocuTasks;
using Nuke.WebDocu;

[CheckBuildProjectConfigurations]
class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Compile);

    [KeyVaultSettings(
        BaseUrlParameterName = nameof(KeyVaultBaseUrl),
        ClientIdParameterName = nameof(KeyVaultClientId),
        ClientSecretParameterName = nameof(KeyVaultClientSecret))]
    readonly KeyVaultSettings KeyVaultSettings;

    [Parameter] string KeyVaultBaseUrl;
    [Parameter] string KeyVaultClientId;
    [Parameter] string KeyVaultClientSecret;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter] readonly string DockerImageName = "danglopencde";
    [KeyVaultSecret] string DanglCiCdSlackWebhookUrl;
    [KeyVaultSecret] string DanglCiCdTeamsWebhookUrl;
    [KeyVaultSecret] string DockerRegistryUrl;
    [KeyVaultSecret] string DockerRegistryUsername;
    [KeyVaultSecret] string DockerRegistryPassword;
    [KeyVaultSecret] string GitHubAuthenticationToken;
    [KeyVaultSecret] string DocuBaseUrl;
    [KeyVaultSecret("DanglOpenCDE-DocuApiKey")] string DocuApiKey;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion(NoFetch = true)] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath DocsDirectory => RootDirectory / "docs";
    AbsolutePath DocFxFile => RootDirectory / "docs" / "docfx.json";
    AbsolutePath ChangelogFile => RootDirectory / "CHANGELOG.md";
    AbsolutePath PublishDirectory => OutputDirectory / "publish";

    public Build()
    {
        // Otherwise, Angular compiler output is logged to stderr instead of stdout.
        NpmTasks.NpmLogger = (outputType, message) => Logger.Normal(message);
    }

    protected override void OnTargetFailed(string target)
    {
        if (IsServerBuild)
        {
            SendTeamsMessage("Build Failed", $"Target {target} failed for Dangl.OpenCDE, " +
                        $"Branch: {GitRepository.Branch}", true);
        }
    }

    void SendTeamsMessage(string title, string message, bool isError)
    {
        if (!string.IsNullOrWhiteSpace(DanglCiCdTeamsWebhookUrl))
        {
            var themeColor = isError ? "f44336" : "00acc1";
            try
            {
                TeamsTasks.SendTeamsMessage(m => m
                        .SetTitle(title)
                        .SetText(message)
                        .SetThemeColor(themeColor),
                        DanglCiCdTeamsWebhookUrl);
            }
            catch { /* Ignoring failures here */}
        }
    }

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .DependsOn(GenerateVersion)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    Target GenerateVersion => _ => _
    .Executes(() =>
    {
        var buildDate = DateTime.UtcNow;

        GenerateBackendVersion(buildDate);
        GenerateFrontendVersion(buildDate);
    });

    private void GenerateBackendVersion(DateTime buildDate)
    {
        var filePath = SourceDirectory / "server" / "Dangl.OpenCDE.Shared" / "VersionsService.cs";

        var currentDateUtc = $"new DateTime({buildDate.Year}, {buildDate.Month}, {buildDate.Day}, {buildDate.Hour}, {buildDate.Minute}, {buildDate.Second}, DateTimeKind.Utc)";

        var content = $@"using System;

namespace Dangl.OpenCDE.Shared
{{
    // This file is automatically generated
    [System.CodeDom.Compiler.GeneratedCode(""GitVersionBuild"", """")]
    public static class VersionsService
    {{
        public static string Version => ""{GitVersion.NuGetVersionV2}"";
        public static string CommitInfo => ""{GitVersion.FullBuildMetaData}"";
        public static string CommitDate => ""{GitVersion.CommitDate}"";
        public static string CommitHash => ""{GitVersion.Sha}"";
        public static string InformationalVersion => ""{GitVersion.InformationalVersion}"";
        public static DateTime BuildDateUtc {{ get; }} = {currentDateUtc};
    }}
}}";
        WriteAllText(filePath, content);
    }

    void GenerateFrontendVersion(DateTime buildDate)
    {
        var filePath = SourceDirectory / "server" / "dangl-opencde-ui" / "src" / "app" / "version.ts";

        var currentDateUtc = $"new Date(Date.UTC({buildDate.Year}, {buildDate.Month - 1}, {buildDate.Day}, {buildDate.Hour}, {buildDate.Minute}, {buildDate.Second}))";

        var content = $@"// This file is automatically generated as part of the build process

export const version = {{
    version: ""{GitVersion.NuGetVersionV2}"",
    commitInfo: ""{GitVersion.FullBuildMetaData}"",
    commitDate: ""{GitVersion.CommitDate}"",
    commitHash: ""{GitVersion.Sha}"",
    informationalVersion: ""{GitVersion.InformationalVersion}"",
    buildDateUtc: {currentDateUtc}
}}";
        WriteAllText(filePath, content);
    }

    Target BuildDocFxMetadata => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DocFXMetadata(x => x
                .SetProcessEnvironmentVariable("DOCFX_SOURCE_BRANCH_NAME", GitVersion.BranchName)
                .SetProjects(DocFxFile));
        });

    Target BuildDocumentation => _ => _
        .DependsOn(Clean)
        .DependsOn(BuildDocFxMetadata)
        .Executes(() =>
        {
            CopyFile(ChangelogFile, DocsDirectory / "CHANGELOG.md");
            DocFXBuild(x => x
                .SetProcessEnvironmentVariable("DOCFX_SOURCE_BRANCH_NAME", GitVersion.BranchName)
                .SetConfigFile(DocFxFile));
            DeleteFile(DocsDirectory / "CHANGELOG.md");
        });

    Target Coverage => _ => _
        .DependsOn(Compile)
        .Requires(() => Configuration == "Debug")
        .Executes(() =>
        {
            Logger.Normal("Ensuring that latest SQL Docker image is present");
            DockerPull(c => c.SetName("dangl/mssql-tmpfs:latest"));

            var testProjects = GlobFiles(TestsDirectory, "**/*.csproj")
                // The test utilities are excluded as they don't contain any tests
                .Where(t => !t.EndsWith("Dangl.OpenCDE.TestUtilities.csproj"));
            try
            {
                DotNetTest(c => c
                    .EnableCollectCoverage()
                    .SetCoverletOutputFormat(CoverletOutputFormat.cobertura)
                    .EnableNoBuild()
                    .SetTestAdapterPath(".")
                    .AddProcessEnvironmentVariable("DANGL_OPENCDE_IGNORE_SQLSERVER_PARALLEL_LIMIT", "true")
                    .SetProcessArgumentConfigurator(a => a
                        .Add($"/p:Include=[Dangl.OpenCDE.*]*")
                        .Add($"/p:ExcludeByFile=\"{SourceDirectory / "Dangl.OpenCDE" / "Migrations" }*.cs\"")
                        .Add($"/p:Exclude=[Dangl.OpenCDE.TestUtilities]*"))
                    .CombineWith(cc => testProjects
                        .Select(testProject =>
                        {
                            var projectDirectory = Path.GetDirectoryName(testProject);
                            var projectName = Path.GetFileNameWithoutExtension(testProject);
                            return cc
                             .SetProjectFile(testProject)
                             .SetLoggers($"xunit;LogFilePath={OutputDirectory / projectName}_testresults.xml")
                             .SetCoverletOutput($"{OutputDirectory / projectName}_coverage.xml");
                        })),
                            degreeOfParallelism: Environment.ProcessorCount,
                            completeOnFailure: true);
            }
            finally
            {
                EnsureTestFilesHaveUniqueTimestamp();

                // Merge coverage reports, otherwise they might not be completely
                // picked up by Jenkins
                ReportGenerator(c => c
                    .SetFramework("net5.0")
                    .SetReports(OutputDirectory / "*_coverage.xml")
                    .SetTargetDirectory(OutputDirectory)
                    .SetReportTypes(ReportTypes.Cobertura));
            }
        });

    private void EnsureTestFilesHaveUniqueTimestamp()
    {
        var testResults = GlobFiles(OutputDirectory, "*_testresults.xml").ToList();
        var runtime = DateTime.Now;

        foreach (var testResultFile in testResults)
        {
            // The "run-time" attributes of the assemblies is ensured to be unique for each single assembly by this test,
            // since in Jenkins, the format is internally converted to JUnit. Aterwards, results with the same timestamps are
            // ignored. See here for how the code is translated to JUnit format by the Jenkins plugin:
            // https://github.com/jenkinsci/xunit-plugin/blob/d970c50a0501f59b303cffbfb9230ba977ce2d5a/src/main/resources/org/jenkinsci/plugins/xunit/types/xunitdotnet-2.0-to-junit.xsl#L75-L79
            var xDoc = XDocument.Load(testResultFile);
            var assemblyNodes = xDoc.Root.Elements().Where(e => e.Name.LocalName == "assembly");
            foreach (var assemblyNode in assemblyNodes)
            {
                assemblyNode.SetAttributeValue("run-time", $"{runtime:HH:mm:ss}");
                runtime = runtime.AddSeconds(1);
            }

            xDoc.Save(testResultFile);
        }
    }

    Target BuildFrontendSwaggerClient => _ => _
    .Executes(() =>
    {
        var nSwagConfigPath = SourceDirectory / "server" / "Dangl.OpenCDE" / "nswag.json";

        NSwagExecuteDocument(x => x
                .SetProcessWorkingDirectory(SourceDirectory / "server" / "Dangl.OpenCDE")
                .SetProcessEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development")
                .SetNSwagRuntime("NetCore31")
                .SetInput(nSwagConfigPath));
    });

    Target FrontEndRestore => _ => _
        .After(Clean)
        .Executes(() =>
        {
            EnsureCleanDirectory(SourceDirectory / "server" / "dangl-opencde-ui" / "node_modules");
            DeleteDirectory(SourceDirectory / "server" / "dangl-opencde-ui" / "node_modules");
            Npm("ci", SourceDirectory / "server" / "dangl-opencde-ui");
        });

    Target BuildFrontend => _ => _
        .DependsOn(GenerateVersion)
        .DependsOn(FrontEndRestore)
        .Executes(() =>
        {
            EnsureCleanDirectory(SourceDirectory / "server" / "Dangl.OpenCDE" / "wwwroot" / "dist");
                NpmRun(x => x
                    .SetProcessWorkingDirectory(SourceDirectory / "server" / "dangl-opencde-ui")
                    .SetCommand("build:production"));

            var assetsSrc = SourceDirectory / "server" / "dangl-opencde-ui" / "src" / "assets";
            var assetsDest = SourceDirectory / "server" / "Dangl.OpenCDE" / "wwwroot" / "assets";
            EnsureCleanDirectory(assetsDest);
            CopyDirectoryRecursively(assetsSrc, assetsDest, DirectoryExistsPolicy.Merge);
        });

    Target Publish => _ => _
        .DependsOn(Restore)
        .DependsOn(GenerateVersion)
        .DependsOn(BuildFrontend)
        .Requires(() => Configuration == Configuration.Release)
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetProject(SourceDirectory / "server" / "Dangl.OpenCDE")
                .SetOutput(PublishDirectory)
                .SetConfiguration(Configuration));
        });

    Target BuildDocker => _ => _
        .DependsOn(Publish)
        .Executes(() =>
        {
            DockerPull(c => c.SetName("mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic"));

            CopyDirectoryRecursively(PublishDirectory, OutputDirectory / "Docker");

            foreach (var configFileToDelete in GlobFiles(OutputDirectory / "Docker", "web*.config"))
            {
                File.Delete(configFileToDelete);
            }

            CopyFile(SourceDirectory / "server" / "Dangl.OpenCDE" / "Dockerfile", OutputDirectory / "Docker" / "Dockerfile", Nuke.Common.IO.FileExistsPolicy.Overwrite);

            DockerBuild(c => c
                .SetFile(OutputDirectory / "Docker" / "Dockerfile")
                .SetTag(DockerImageName + ":dev")
                .SetPath(".")
                .SetProcessWorkingDirectory(OutputDirectory / "Docker"));

            EnsureCleanDirectory(OutputDirectory / "Docker");
        });

    Target PushDocker => _ => _
        .DependsOn(BuildDocker)
        .Requires(() => DockerRegistryUrl)
        .Requires(() => DockerRegistryUsername)
        .Requires(() => DockerRegistryPassword)
        .OnlyWhenDynamic(() => IsOnBranch("main") || IsOnBranch("develop"))
        .Executes(async () =>
        {
            DockerLogin(x => x
                .SetUsername(DockerRegistryUsername)
                .SetServer(DockerRegistryUrl)
                .SetPassword(DockerRegistryPassword)
                .DisableProcessLogOutput());

            await PushDockerWithTag("dev");
            await EnsureAppIsAtLatestVersionAsync("https://opencde-dev.dangl.dev/api/status");

            if (IsOnBranch("main"))
            {
                await PushDockerWithTag("latest");
                await PushDockerWithTag(GitVersion.SemVer);
                await EnsureAppIsAtLatestVersionAsync("https://opencde.dangl.dev/api/status");
            }
        });

    async Task EnsureAppIsAtLatestVersionAsync(string appStatusUrl)
    {
        var timeoutInSeconds = 180;
        var statusIsAtLatestVersion = false;
        var start = DateTime.UtcNow;
        while (!statusIsAtLatestVersion && DateTime.UtcNow < start.AddSeconds(timeoutInSeconds))
        {
            try
            {
                var stagingVersion = JObject.Parse(await HttpDownloadStringAsync(appStatusUrl))["version"].ToString();
                statusIsAtLatestVersion = stagingVersion == GitVersion.NuGetVersionV2;
            }
            catch
            {
                await Task.Delay(1_000);
            }
        }

        ControlFlow.Assert(statusIsAtLatestVersion, $"Status at {appStatusUrl} does not indicate latest version.");
        Logger.Normal($"App at {appStatusUrl} is at latest version {GitVersion.NuGetVersionV2}");
    }

    private async Task PushDockerWithTag(string tag)
    {
        DockerTag(c => c
            .SetSourceImage(DockerImageName + ":" + "dev")
            .SetTargetImage($"{DockerRegistryUrl}/{DockerImageName}:{tag}"));
        DockerPush(c => c
            .SetName($"{DockerRegistryUrl}/{DockerImageName}:{tag}"));

        var message = $"A new container version was pushed for Dangl.OpenCDE, Version {GitVersion.NuGetVersionV2}, Tag {DockerRegistryUrl}/{DockerImageName}:{tag}";
        await SlackTasks.SendSlackMessageAsync(c => c
            .SetUsername("Dangl CI Build")
            .SetAttachments(new SlackMessageAttachment()
                .SetText(message)
                .SetColor("good")
                .SetFields(new[]
                {
                            new SlackMessageField
                            ()
                            .SetTitle("Tag")
                            .SetValue($"{DockerRegistryUrl}/{DockerImageName}:{tag}")
                })),
                DanglCiCdSlackWebhookUrl);
        SendTeamsMessage("Docker Push", message, false);
    }

    public bool IsOnBranch(string branchName)
    {
        return GitVersion.BranchName.Equals(branchName) || GitVersion.BranchName.Equals($"origin/{branchName}");
    }

    Target UploadDocumentation => _ => _
        .DependsOn(BuildDocumentation)
        .Requires(() => DocuApiKey)
        .Requires(() => DocuBaseUrl)
        .Executes(() =>
        {
            var changeLog = GetCompleteChangeLog(ChangelogFile);

            WebDocu(s => s
                .SetDocuBaseUrl(DocuBaseUrl)
                .SetDocuApiKey(DocuApiKey)
                .SetMarkdownChangelog(changeLog)
                .SetSourceDirectory(OutputDirectory / "docs")
                .SetVersion(GitVersion.NuGetVersion)
            );
        });

    Target PublishGitHubRelease => _ => _
         .Requires(() => GitHubAuthenticationToken)
         .OnlyWhenDynamic(() => IsOnBranch("main"))
         .Executes(async () =>
         {
             var releaseTag = $"v{GitVersion.MajorMinorPatch}";

             var changeLogSectionEntries = ExtractChangelogSectionNotes(ChangelogFile);
             var latestChangeLog = changeLogSectionEntries
                 .Aggregate((c, n) => c + Environment.NewLine + n);
             var completeChangeLog = $"## {releaseTag}" + Environment.NewLine + latestChangeLog;

             var repositoryInfo = GetGitHubRepositoryInfo(GitRepository);

             await PublishRelease(x => x
                     .SetCommitSha(GitVersion.Sha)
                     .SetReleaseNotes(completeChangeLog)
                     .SetRepositoryName(repositoryInfo.repositoryName)
                     .SetRepositoryOwner(repositoryInfo.gitHubOwner)
                     .SetTag(releaseTag)
                     .SetToken(GitHubAuthenticationToken));
         });
}
