using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLinker.Test
{
    public class TestLayer:ILayer
    {
        public TestLayer(IEnumerable<Link> links)
        {
        }

        public void Dispose()
        {
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

        public IEnumerable<IEntry> GetEntries(string path)
        {
            throw new NotImplementedException();
        }

        public event Action<string> CreateNotify;
        public event Action<string> UpdateNotify;
        public event Action<string> DeleteNotify;
        public event Action<(string oldPath, string newPath)> RenameNotify;
    }
}
