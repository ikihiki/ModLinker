using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using DokanNet;

namespace ModLinker
{
    public class LayerService:IDisposable
    {
        private readonly Target target;
        private readonly IEnumerable<Mod> mods;
        private readonly IEnumerable<ILayerProvider> layerProviders;
        private readonly List<ILayer> layerList = new List<ILayer>();
        public event Action Notify;

        public LayerService(Target target, IEnumerable<ILayerProvider> layerProviders)
        {
            this.target = target;
            this.mods = target.Mods;
            this.layerProviders = layerProviders;
            layerList.Add(new  BaseLayer(target.BasePath));
            foreach (var mod in mods)
            {
                var layers = GetLayers(mod);
                if (layers != null)
                {
                    layerList.AddRange(layers);
                }
            }

            layerList.Add(new OverlayLayer(target.OverlayPath));
            foreach (var layer in layerList)
            {
                //layer.CreateNotify+=LayerOnNotify;
            }
        }

        private void LayerOnNotify(IEntry obj)
        {
            Notify?.Invoke();
        }

        public IEnumerable<IEntry> GetEntries(string path)
        {
            var entries = new Dictionary<string, IEntry>();

            foreach (var entry in layerList.SelectMany(layer => layer.GetEntries(path)))
            {
                entries[entry.Path] = entry;
            }
            return entries.Values;
        }

        private IEnumerable<ILayer> GetLayers(Mod mod)
        {
            foreach (var layerProvider in layerProviders)
            {
                if (layerProvider.CanCreateLayer(mod))
                {
                    return layerProvider.CreateLayer(mod);
                }
            }

            return null;
        }


        public void CreateDatabase()
        {

        }

        public void Dispose()
        {
            foreach (var layer in layerList)
            {
                layer.Dispose();
            }
            layerList.Clear();
        }

        public IEntry GetEntry(string fileName)
        {
            throw new NotImplementedException();
        }

        internal IFileHandle UpdateWritable(IFileHandle handle)
        {
            throw new NotImplementedException();
        }

        public NtStatus DeleteFile(string fileName)
        {
            throw new NotImplementedException();
        }

        internal NtStatus DeleteDirectory(string fileName)
        {
            throw new NotImplementedException();
        }

        internal NtStatus MoveFile(string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        public IEntry UpdateWritable(IEntry handle)
        {
            throw new NotImplementedException();
        }
    }
}
