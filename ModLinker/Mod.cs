using System.Collections.Generic;
using MasterMemory;
using MessagePack;

namespace ModLinker
{
    [MemoryTable("mods"), MessagePackObject(true)]
    public class Mod
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }
        public IEnumerable<Link> Links { get; set; }
        public string Url{ get; set; }
        public string Description { get; set; }
    }
}