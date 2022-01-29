using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SomeCollections.Models
{
    public class FilterCollections
    {
        public IEnumerable<Collection> Collections { get; set; }
        public SelectList Tags { get; set; }
        public string Name { get; set; }
    }
}
