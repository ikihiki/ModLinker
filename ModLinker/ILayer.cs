using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLinker
{
    public interface ILayer : IDisposable
    {
        IEnumerable<string> GetAllDirectories();
        IEnumerable<string> GetAllFiles();
        Stream GetFile(string path);
        IEnumerable<IEntry> GetEntries(string path);
        event Action<IEntry> Notify;
    }
}
