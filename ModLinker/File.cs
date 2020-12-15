using MasterMemory;
using MessagePack;

namespace ModLinker
{
    [MemoryTable("files"), MessagePackObject(true)]
    public class File
    {
        [PrimaryKey]
        public string Path { get; set; }
        public Mod Mod { get; set; }
        public string ModPath { get; set; }
    }
}