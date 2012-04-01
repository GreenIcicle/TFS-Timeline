using System.Collections.Generic;
using System.Linq;

using Microsoft.TeamFoundation.Build.Client;

namespace AwesomeTfsMonitor.Models
{
    public static class BuildDetailExtensions
    {
        public static StaticAnalysisInfo GetStaticAnalysisInfo(this IBuildDetail buildDetail)
        {
            var configurationSummary = buildDetail.Information.GetNodesByType("ConfigurationSummary").FirstOrDefault();

            if (configurationSummary == null)
            {
                return null;
            }

            return new StaticAnalysisInfo
                {
                    StaticAnalysisErrors = int.Parse(configurationSummary.Fields["TotalStaticAnalysisErrors"]),
                    StaticAnalysisWarnings = int.Parse(configurationSummary.Fields["TotalStaticAnalysisWarnings"]),
                    CompilationWarnings = int.Parse(configurationSummary.Fields["TotalCompilationWarnings"])
                };
        }
        
        public static IEnumerable<string> GetCheckinComments(this IBuildDetail buildDetail)
        {
            return buildDetail.Information.GetNodesByType("AssociatedChangeset")
                .Select(node => node.Fields["Comment"])
                .ToList();
        }

        public static IEnumerable<string> GetWorkItemTitles(this IBuildDetail buildDetail)
        {
            return buildDetail.Information.GetNodesByType("AssociatedWorkItem")
                .Select(node => node.Fields["Title"])
                .ToList();
        }
    }
}