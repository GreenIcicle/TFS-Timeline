using System.Linq;
using System.Web.Mvc;

using Greenicicle.TfsTimeline.Models;
using Greenicicle.TfsTimeline.ViewModels;

namespace Greenicicle.TfsTimeline.Controllers
{
    public class BuildsController : Controller
    {
        private readonly IBuildService buildService;

        public BuildsController(IBuildService buildService)
        {
            this.buildService = buildService;
        }

        public ActionResult ProjectNamesView()
        {
            var projectNames = buildService.GetProjects();

            var viewModel = new LinkListViewModel
            {
                Title = "Projects on TFS",
                Links = projectNames.ToDictionary(
                    projectName => Url.Action("BuildNamesView", new { projectName }),
                    projectName => projectName)
            };

            return View("LinkListView", viewModel);
        }
        
        public ActionResult BuildNamesView(string projectName)
        {
            var buildNames = buildService.GetBuildDefinitions(projectName);

            var viewModel = new LinkListViewModel
            {
                Title = string.Format("Builds for project {0}", projectName),
                Links = buildNames.ToDictionary(
                    buildName => Url.Action("BuildTimelineView", new { projectName, buildName }),
                    buildName => buildName)
            };

            return View("LinkListView", viewModel);
        }

        public ActionResult BuildTimelineView(string projectName, string buildName)
        {
            return View(new LatestBuildsViewModel { ProjectName = projectName, BuildName = buildName });
        }
    }
}
