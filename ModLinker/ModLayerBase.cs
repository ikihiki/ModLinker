using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLinker
{
    public abstract class ModLayerBase : ILayer
    {
        public IEnumerable<Link> Links { get; }

        protected ModLayerBase(IEnumerable<Link> links)
        {
            Links = links;
        }

        protected void RegisterDirectories(IEnumerable<string> paths)
        {

        }

        protected void RegisterFiles(IEnumerable<string> paths)
        {

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

        public event Action<IEntry> Notify;

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
