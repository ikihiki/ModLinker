using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLinker
{
    public interface ILayerProvider
    {
        bool CanCreateLayer(string path);
        ILayer CreateLayer(string path, IEnumerable<Link> links);
    }
}
