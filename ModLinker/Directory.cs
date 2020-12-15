using MasterMemory;
using MessagePack;

namespace ModLinker
{
    [MemoryTable("directories"), MessagePackObject(true)]
    public class Directory
    {
        [PrimaryKey]
        public string Path { get; set; }
    }
}