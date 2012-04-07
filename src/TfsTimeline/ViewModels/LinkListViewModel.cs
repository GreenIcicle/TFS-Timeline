using System.Collections.Generic;

namespace Greenicicle.TfsTimeline.ViewModels
{
    public class LinkListViewModel
    {
        public string Title { get; set; }

        public IDictionary<string, string> Links { get; set; }
    }
}