using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;

using Greenicicle.TfsTimeline.Models;

namespace Greenicicle.TfsTimeline.Controllers
{
    public class BuildsApiController : Controller
    {
        private readonly IBuildService buildService;

        public BuildsApiController(IBuildService buildService)
        {
            this.buildService = buildService;
        }

        [HttpPost]
        [OutputCache(Duration = 360, Location = OutputCacheLocation.Server)]
        public ActionResult ProjectNames()
        {
            var projectNames = buildService.GetProjects();

            var result = new
            {
                Self = Url.Action("ProjectNames"),
                Projects = projectNames.Select(projectName => new
                {
                    Name = projectName,
                    Url = Url.Action("BuildNames", new { projectName }),
                })
            };

            return Json(result);
        }

        [HttpPost]
        [OutputCache(Duration = 360, Location = OutputCacheLocation.Server)]
        public ActionResult BuildNames(string projectName)
        {
            var buildNames = buildService.GetBuildDefinitions(projectName);

            var result = new
            {
                Self = Url.Action("BuildNames", new { projectName }),
                Projects = buildNames.Select(buildName => new
                {
                    Name = buildName,
                    Url = Url.Action("BuildTimeline", new { projectName, buildName }),
                })
            };

            return Json(result);
        }

        [HttpPost]
        [OutputCache(Duration = 15, Location = OutputCacheLocation.Server)]
        public ActionResult BuildTimeline(string projectName, string buildName, string sinceThisTime)
        {
            DateTime minBuildAge;

            if (!DateTime.TryParse(sinceThisTime, out minBuildAge))
            {
                minBuildAge = DateTime.Today;
            }

            var builds = buildService.GetBuilds(projectName, buildName, minBuildAge)
                .OrderBy(b => b.StartedAt)
                .ToList();

            DateTime? lastFinished = minBuildAge;
            if (builds.Any())
            {
               lastFinished = builds
                  .Where(b => b.Status == BuildStatus.Failed.ToString() || b.Status == BuildStatus.Succeeded.ToString())
                  .Select(b => b.FinishedAt)
                  .OrderByDescending(finished => finished)
                  .FirstOrDefault();
            }

            if (lastFinished == null)
            {
                lastFinished = DateTime.Today;
            }

            var lastFinishedParam = lastFinished.Value.ToString("dd-MM-yyyy HH:mm:ss");

            var buildTimeline = new
            {
                Self = Url.Action("BuildTimeline", new { projectName, buildName }),
                Refresh = Url.Action("BuildTimeline", new { projectName, buildName, sinceThisTime = lastFinishedParam }),
                Builds = builds
            };

            return Json(buildTimeline);
        }
    }
}
