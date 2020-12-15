using System;
using System.Collections.Generic;
using MasterMemory;
using MessagePack;

namespace ModLinker
{
    [MemoryTable("target"), MessagePackObject(true)]
    public class Target
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string RootPath { get; set; }

        public IEnumerable<Mod> Mods { get; set; }
    }
}