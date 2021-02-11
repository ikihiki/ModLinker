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

        public ILayer CreateLayer(string path, List<Link> links)
        {
            return new TestLayer(links);
        }

        public bool CanCreateLayer(Mod mod)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ILayer> CreateLayer(Mod mod)
        {
            throw new NotImplementedException();
        }
    }
}
