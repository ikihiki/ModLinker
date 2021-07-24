using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ModLinker
{
    public class ModManager
    {
        private Target target;
        private LayerService layerService;
        public ModLinkerOperation Operation { get; private set; }

        public async Task Load(string path, IEnumerable<ILayerProvider> layerProviders)
        {
            await Load(new FileStream(path, FileMode.Open), layerProviders);
        }


        public async Task Load(Stream stream, IEnumerable<ILayerProvider> layerProviders)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml 
                .Build();
            using (var reader = new StreamReader(stream))
            {
                 target= deserializer.Deserialize<Target>(reader);
            }
            Dictionary<int, List<Mod>> orders = new Dictionary<int, List<Mod>>(){{int.MaxValue,new List<Mod>()}};

            foreach (var mod in target.Mods)
            {
                if (!Path.IsPathRooted(mod.ModPath))
                {
                    mod.ModPath = Path.GetFullPath(mod.ModPath, target.ModDirectory);
                }

                if (!Path.IsPathRooted(mod.RootPath))
                {
                    mod.RootPath = Path.GetFullPath(mod.RootPath, target.RootPath);
                }

                if (mod.Order > 0)
                {
                    if (!orders.ContainsKey(mod.Order))
                    {
                        orders.Add(mod.Order, new List<Mod>());
                    }
                    orders[mod.Order].Add(mod);
                }
                else
                {
                    orders[int.MaxValue].Add(mod);
                }
            }

            var mods = orders
                    .OrderBy(kvp => kvp.Key)
                    .SelectMany(kvp => kvp.Value)
                ;

            target.Mods = mods;


            var layers = new List<ILayer>();
            foreach (var mod in target.Mods)
            {
                foreach (var layerProvider in layerProviders)
                {
                    if (layerProvider.CanCreateLayer(mod))
                    {
                        layers.AddRange(layerProvider.CreateLayer(mod));
                        break;
                    }
                }
            }

            layerService = new LayerService(layers, target.RootPath);
            await layerService.PreLoad();
            Operation = new ModLinkerOperation(layerService);
        }
    }
}
