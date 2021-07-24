using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLinker
{
    public class DirectoryLayerProvider : ILayerProvider
    {
        public bool CanCreateLayer(Mod mod)
        {
            return System.IO.Directory.Exists(mod.ModPath);
        }

        public IEnumerable<ILayer> CreateLayer(Mod mod)
        {
            return mod.Links.Select(link => new DirectoryLayer(Guid.NewGuid().ToString(), this, mod, link, 0));
        }
    }

    public class DirectoryLayer : ILayer
    {
        private readonly Mod mod;
        private readonly DirectoryInfo rootDirectoryInfo;
        private readonly PathMapper pathMapper;
        private bool writable;
        public string Id { get; }
        public ILayerProvider Provider { get; }
        public Link Link { get; }
        private uint priority;

        public DirectoryLayer(string id, ILayerProvider provider, Mod mod, Link link, uint priority, bool writable=false)
        {
            Id = id;
            Provider = provider;
            this.mod = mod;
            this.Link = link;
            this.priority = priority;
            this.writable = writable;
            pathMapper = new PathMapper(Path.Combine(mod.ModPath, link.ModPath), Path.Combine(mod.RootPath, link.TargetPath));
            rootDirectoryInfo = new DirectoryInfo(pathMapper.SourcePath);
        }


        public void Dispose()
        {

        }

        private IEntry CreateEntryFromFile(string file)
        {
            var info = new FileInfo(file);
            return new FileEntry(
                pathMapper.GetMappedPath(file),
                file,
                this,
                writable
            );
        }
        private IEntry CreateEntryFromDirectory(string dir)
        {
            return new DirectoryEntry(
                pathMapper.GetMappedPath(dir),
                dir,
                this,
                writable
            );
        }




        public IEnumerable<EntryData> GetPreloadEntries()
        {
            return rootDirectoryInfo.EnumerateFileSystemInfos()
                    .Select(info => new EntryData(
                        info.Attributes.HasFlag(FileAttributes.Directory),
                        info.Name,
                        pathMapper.GetMappedPath(info.FullName),
                        pathMapper.GetMappedPath(Path.GetDirectoryName(info.FullName)),
                        info.Attributes.HasFlag(FileAttributes.Directory),
                        this,
                        info.CreationTime,
                        info.LastAccessTime,
                        info.LastWriteTime,
                        (info as FileInfo)?.Length ?? 0,
                        writable,
                        priority
                    ))
                ;
        }

        public IEnumerable<EntryData> GetEntries(string path)
        {
            var sourcePath = pathMapper.GetSourcePath(path);
            if (Directory.Exists(sourcePath))
            {
                return new DirectoryInfo(sourcePath)
                        .EnumerateFileSystemInfos()
                        .Select(info => new EntryData(
                            info.Attributes.HasFlag(FileAttributes.Directory),
                            info.Name,
                            pathMapper.GetMappedPath(info.FullName),
                            pathMapper.GetMappedPath(Path.GetDirectoryName(info.FullName)),
                            info.Attributes.HasFlag(FileAttributes.Directory),
                            this,
                            info.CreationTime,
                            info.LastAccessTime,
                            info.LastWriteTime,
                            (info as FileInfo)?.Length ?? 0,
                            writable,
                            priority
                        ))
                    ;
            }

            return Enumerable.Empty<EntryData>();
        }

        public IEntry GetEntry(string path)
        {
            var sourcePath = pathMapper.GetSourcePath(path);
            if (Directory.Exists(sourcePath))
            {
                return CreateEntryFromDirectory(sourcePath);
            }

            if (File.Exists(sourcePath))
            {
                return CreateEntryFromFile(sourcePath);
            }

            return null;
        }
    }
}
