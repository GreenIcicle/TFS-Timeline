using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace AwesomeTfsMonitor.Models
{
    public class Tfs2010BuildService : IBuildService
    {
        private readonly IBuildServer buildServer;
        private readonly VersionControlServer versionControlServer;
        private readonly ITestManagementService testManagementService;

        public Tfs2010BuildService(
            IBuildServer buildServer,
            VersionControlServer versionControlServer,
            ITestManagementService testManagementService)
        {
            this.buildServer = buildServer;
            this.versionControlServer = versionControlServer;
            this.testManagementService = testManagementService;
        }

        public IEnumerable<string> GetProjects()
        {
            var teamProjects = versionControlServer.GetAllTeamProjects(true);
            return teamProjects.Select(p => p.Name).ToList();
        }

        public IEnumerable<string> GetBuildDefinitions(string projectName)
        {
            var buildDefinitions = buildServer.QueryBuildDefinitions(projectName);
            return buildDefinitions.Select(b => b.Name).ToList();
        }

        public IEnumerable<BuildInformation> GetBuilds(string projectName, string buildDefinition, DateTime minFinishedTime)
        {
            IBuildDetailSpec buildDetailSpec = buildServer.CreateBuildDetailSpec(projectName, buildDefinition);

            buildDetailSpec.MaxBuildsPerDefinition = 5;
            buildDetailSpec.QueryOrder = BuildQueryOrder.StartTimeDescending;
            buildDetailSpec.MinFinishTime = minFinishedTime.AddSeconds(1);

            var builds = buildServer.QueryBuilds(buildDetailSpec);
            var tests = testManagementService.GetTeamProject(projectName);

            return builds.Builds
                .Select(b => new
                    {
                        Build = b,
                        TestRuns = tests.TestRuns.ByBuild(b.Uri).ToList(),
                        StaticAnalysis = b.GetStaticAnalysisInfo() ?? new StaticAnalysisInfo(),
                        CheckinComments = b.GetCheckinComments(),
                        WorkItemTitles = b.GetWorkItemTitles()
                    })
                .Select(b => new BuildInformation
                {
                    
                    Uri = b.Build.Uri.ToString(), 
                    
                    StartedAt = b.Build.StartTime,
                    FinishedAt = b.Build.FinishTime,
                    TriggeredBy = RemoveDomainFromUsername(b.Build.RequestedFor),
                    Status = TranslateBuildStatus(b.Build.Status).ToString(),
                    TestsFailed = b.TestRuns.Select(run => run.Statistics.FailedTests).Sum(),
                    TestsPassed = b.TestRuns.Select(run => run.Statistics.PassedTests).Sum(),
                    TestsTotal = b.TestRuns.Select(run => run.Statistics.TotalTests).Sum(),
                    CodeAnalysisErrors = b.StaticAnalysis.StaticAnalysisErrors,
                    CodeAnalysisWarnings = b.StaticAnalysis.StaticAnalysisWarnings,
                    CompilerWarnings = b.StaticAnalysis.CompilationWarnings,
                    CheckinComments = string.Join(" +++ ", b.CheckinComments),
                    WorkItemTitles = string.Join(", ", b.WorkItemTitles),
                }).ToList();
        }

        public BuildStatus TranslateBuildStatus(Microsoft.TeamFoundation.Build.Client.BuildStatus tfsStatus)
        {
            switch (tfsStatus)
            {
                case Microsoft.TeamFoundation.Build.Client.BuildStatus.Succeeded:
                    return BuildStatus.Succeeded;

                case Microsoft.TeamFoundation.Build.Client.BuildStatus.PartiallySucceeded:
                case Microsoft.TeamFoundation.Build.Client.BuildStatus.Failed:
                case Microsoft.TeamFoundation.Build.Client.BuildStatus.Stopped:
                    return BuildStatus.Failed;

                case Microsoft.TeamFoundation.Build.Client.BuildStatus.InProgress:
                    return BuildStatus.Running;

                case Microsoft.TeamFoundation.Build.Client.BuildStatus.NotStarted:
                    return BuildStatus.Queued;

                default:
                    return BuildStatus.Unknown;
            }

        }

        public string RemoveDomainFromUsername(string userName)
        {
            var parts = userName.Split('\\');
            if (parts.Length == 2)
            {
                return parts[1];
            }

            return userName;
        }
    }
}
