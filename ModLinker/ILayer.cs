using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLinker
{

    public class EntryData
    {
        public bool IsLazy { get; }
        public string Name { get; }
        public string ParentDirectory { get; }
        public string Path { get; }
        public bool IsDirectory { get; }
        public ILayer Layer { get; }
        public DateTime? CreationTime { get; }
        public DateTime? LastAccessTime { get; }
        public DateTime? LastWriteTime { get; }
        public long Length { get; }
        public bool Writable { get; }
        public uint Priority { get; }


        public EntryData(bool isLazy,string name, string path, string parentDirectory, bool isDirectory, ILayer layer, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime, long length, bool writable, uint priority)
        {
            IsLazy = isLazy;
            Path = path;
            IsDirectory = isDirectory;
            Layer = layer;
            CreationTime = creationTime;
            LastAccessTime = lastAccessTime;
            LastWriteTime = lastWriteTime;
            Length = length;
            Writable = writable;
            Priority = priority;
            Name = name;
            ParentDirectory = parentDirectory;
        }
    }

    public interface ILayer : IDisposable
    {
        public string Id { get; }
        public ILayerProvider Provider { get; }
        public Link Link { get; }

        IEnumerable<EntryData> GetPreloadEntries();
        IEnumerable<EntryData> GetEntries(string path);
        IEntry GetEntry(string path);
    }
}
