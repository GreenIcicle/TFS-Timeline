using System;

namespace AwesomeTfsMonitor.Models
{
    public class BuildInformation
    {
        public string Uri { get; set; }

        public string TriggeredBy { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        public string Status { get; set; }

        public int TestsTotal { get; set; }

        public int TestsFailed { get; set; }

        public int TestsPassed { get; set; }

        public int CodeAnalysisErrors { get; set; }

        public int CodeAnalysisWarnings { get; set; }

        public int StyleAnalysisErrors { get; set; }

        public int StyleAnalysisWarnings { get; set; }

        public int CompilerWarnings { get; set; }

        public string CheckinComments { get; set; }

        public string WorkItemTitles { get; set; }
    }
}
