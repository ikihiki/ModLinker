﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLinker
{
    public interface ILayerProvider
    {
        bool CanCreateLayer(Mod mod);
        IEnumerable<ILayer> CreateLayer(Mod mod);
    }
}
