
using Microsoft.TeamFoundation.Build.Client;

using Moq;

using Ploeh.AutoFixture;

namespace Greenicicle.TfsTimeline.Models
{
    public class GetStaticAnalysisInfoTests
    {
        private readonly Fixture fixture = new Fixture();

        public void When_there_are_no_nodes_it_should_return_null()
        {
            var buildDetail = fixture.Build<IBuildDetail>();
            
        }
    }
}