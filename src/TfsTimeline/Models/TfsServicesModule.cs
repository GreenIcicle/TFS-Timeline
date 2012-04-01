using System.Configuration;

using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

using Ninject.Modules;

namespace AwesomeTfsMonitor.Models
{
    public class TfsServicesModule : NinjectModule
    {
        public override void Load()
        {
            var teamFoundationServerUrl = ConfigurationManager.AppSettings["TeamFoundationServerUrl"];
            var tfs = new TfsTeamProjectCollection(TfsTeamProjectCollection.GetFullyQualifiedUriForName(teamFoundationServerUrl));

            Bind<IBuildServer>().ToMethod(ctx => tfs.GetService<IBuildServer>());
            Bind<VersionControlServer>().ToMethod(ctx => tfs.GetService<VersionControlServer>());
            Bind<ITestManagementService>().ToMethod(ctx => tfs.GetService<ITestManagementService>());

            Bind<IBuildService>().To<Tfs2010BuildService>().InSingletonScope();
        }
    }
}
