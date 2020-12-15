using System.Collections.Generic;
using MasterMemory;
using MessagePack;

namespace ModLinker
{
    [MemoryTable("directories"), MessagePackObject(true)]
    public class Directory
    {
        [PrimaryKey]
        public string Path { get; set; }

        public IEnumerable<string> ChildrenDirectory { get; set; }
        public IEnumerable<string> Files { get; set; }
    }
}