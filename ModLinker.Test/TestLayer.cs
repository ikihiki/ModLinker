using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLinker.Test
{
    public class TestLayer:Layer
    {
        public TestLayer(IEnumerable<Link> links) : base(links)
        {
        }
    }
}
