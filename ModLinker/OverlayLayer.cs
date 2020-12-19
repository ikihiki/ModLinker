using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLinker
{
    class OverlayLayer:ILayer
    {
        public IEnumerable<string> GetAllDirectories()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAllFiles()
        {
            throw new NotImplementedException();
        }

        public Stream GetFile(string path)
        {
            throw new NotImplementedException();
        }
    }
}
