using System;
using System.Collections.Generic;

namespace ModLinker
{
    public class Mod
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public string ModPath{ get; set; }
        public string RootPath { get; set; }
        public IEnumerable<Link> Links { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
    }
}