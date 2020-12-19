using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLinker
{
    class LayerService
    {
        private readonly Target target;
        private readonly IEnumerable<Mod> mods;
        private readonly IEnumerable<ILayerProvider> layerProviders;

        private readonly List<ILayer> layers = new List<ILayer>();

        public LayerService(Target target, IEnumerable<Mod> mods, IEnumerable<ILayerProvider> layerProviders)
        {
            this.target = target;
            this.mods = mods;
            this.layerProviders = layerProviders;
            layers.Add(new  BaseLayer(target.RootPath));
            foreach (var mod in mods)
            {
                var layer = GetLayer(mod);
                if (layer != null)
                {
                    layers.Add(layer);
                }
            }

            layers.Add(new OverlayLayer());
        }

        private ILayer GetLayer(Mod mod)
        {
            foreach (var layerProvider in layerProviders)
            {
                if (layerProvider.CanCreateLayer(mod.EntityPath))
                {
                    return layerProvider.CreateLayer(mod.EntityPath, mod.Links);
                }
            }

            return null;
        }

        public void CreateDatabase()
        {

        }
    }
}
