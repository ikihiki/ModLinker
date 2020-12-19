using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLinker
{
    public class BaseLayer:ILayer
    {
        private readonly string rootPath;

        public BaseLayer(string rootPath)
        {
            this.rootPath = rootPath;
            throw new NotImplementedException();
        }

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
