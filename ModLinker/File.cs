using System;
using MasterMemory;
using MessagePack;

namespace ModLinker
{
    [MemoryTable("files"), MessagePackObject(true)]
    public class File
    {
        [PrimaryKey]
        public string Path { get; set; }
        public Guid ModId { get; set; }
        public string ModPath { get; set; }
        public string TargetPath { get; set; }
        public string Directory { get; set; }
    }
}