using System;
using System.Collections.Generic;
using MasterMemory;
using MessagePack;

namespace ModLinker
{
    [MemoryTable("mods"), MessagePackObject(true)]
    public class Mod
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        [SecondaryKey(0)]
        public int Order { get; set; }
        public string Name { get; set; }
        public string EntityPath{ get; set; }
        public string RootPath { get; set; }
        public IEnumerable<Link> Links { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
    }
}