using System.Collections.Generic;
using System.Web.Mvc;

using AwesomeTfsMonitor.ViewModels;

namespace AwesomeTfsMonitor.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var viewModel = new LinkListViewModel
            {
                Title = "What's in store?",
                Links = new Dictionary<string, string>
                    {
                        { Url.Action("ProjectNamesView", "Builds"), "Builds" }
                    } 
            };

            return View("LinkListView", viewModel);
        }
    }
}
