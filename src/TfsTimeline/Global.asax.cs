using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

using Ninject;
using Ninject.Web.Common;

namespace AwesomeTfsMonitor
{
    public class MvcApplication : NinjectHttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               null,
               "",
               new { controller = "Home", action = "Index" });

            routes.MapRoute(
               null,
               "builds",
               new { controller = "Builds", action = "ProjectNamesView" });
            
            routes.MapRoute(
                null,
                "builds/{projectName}",
                new { controller = "Builds", action = "BuildNamesView" });

            routes.MapRoute(
                null,
                "builds/{projectName}/{buildName}",
                new { controller = "Builds", action = "BuildTimelineView" });

            routes.MapRoute(
                null,
                "api/builds",
                new { controller = "BuildsApi", action = "ProjectNames" });
            
            routes.MapRoute(
                null,
                "api/builds/{projectName}",
                new { controller = "BuildsApi", action = "BuildNames" });

            routes.MapRoute(
                null,
                "api/builds/{projectName}/{buildName}",
                new { controller = "BuildsApi", action = "BuildTimeline" });
        }

        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();

            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected override IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            return kernel;
        }
    }
}