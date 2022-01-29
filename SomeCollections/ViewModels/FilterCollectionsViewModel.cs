using Microsoft.AspNetCore.Mvc.Rendering;
using SomeCollections.Models;
using System.Collections.Generic;

namespace SomeCollections.ViewModels
{
    public class FilterCollectionsViewModel
    {
        public IEnumerable<Collection> Collections { get; set; }
        public SelectList Tags { get; set; }
        public string Name { get; set; }
    }
}
