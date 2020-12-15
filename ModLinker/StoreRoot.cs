using System;
using System.Collections.Generic;

namespace ModLinker
{
    public class StoreRoot
    {
        Dictionary<Guid, Target> Targets{ get; set; }
        Dictionary<Guid, Mod> Mods{ get; set; }
    }
}