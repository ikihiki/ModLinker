using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLinker.Test
{
    public class TestLayerProvider:ILayerProvider
    {
        public bool CanCreateLayer(string path)
        {
            return true;
        }

        public Layer CreateLayer(string path, List<Link> links)
        {
            return new TestLayer(links);
        }
    }
}
