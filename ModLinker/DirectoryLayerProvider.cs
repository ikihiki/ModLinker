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
            return mod.Links.Select(link => new DirectoryLayer(mod, link));
        }
    }

    public class DirectoryLayer : ILayer
    {
        private readonly Mod mod;
        private readonly Link link;
        private readonly string[] rootParts;
        private readonly string modLinkRootPath;
        private readonly DirectoryInfo rootDirectoryInfo;

        public DirectoryLayer(Mod mod, Link link)
        {
            this.mod = mod;
            this.link = link;
            rootParts = link.TargetPath.Split('\\');
            modLinkRootPath = Path.Combine(mod.ModPath, mod.RootPath, link.ModPath);
            rootDirectoryInfo = new DirectoryInfo(modLinkRootPath);
        }


        public void Dispose()
        {

        }

        public IEnumerable<string> GetAllDirectories()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAllFiles()
        {
            throw new NotImplementedException();
        }

        public Stream GetFile(string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEntry> GetEntries(string path)
        {
            var parts = path.Split('\\', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= rootParts.Length)
            {
                if (path.StartsWith("\\"+link.TargetPath))
                {
                    return GetSubEntries(parts);
                }
                else
                {
                    return Enumerable.Empty<IEntry>();
                }

            }
            else
            {
                return GetEntriesPartToRoot(parts);
            }
        }

        public event Action<IEntry> Notify;


        private IEnumerable<IEntry> GetEntriesPartToRoot(string[] parts)
        {
            for (int i = 0; i < rootParts.Length; i++)
            {
                if (parts.Length <= i)
                {
                    return new[]
                    {
                        new Entry
                        {
                            Path = "\\" + Path.Combine(rootParts.Take(i+1).ToArray()), 
                            CreationTime = rootDirectoryInfo.CreationTime,
                            LastAccessTime = rootDirectoryInfo.LastAccessTime,
                            LastWriteTime = rootDirectoryInfo.LastWriteTime,
                            Name = rootParts[i],
                            IsDirectory = true, 
                            Layer = this
                        }
                    };
                }

                if (parts[i] != rootParts[i])
                {
                    return Enumerable.Empty<IEntry>();
                }
            }
            return Enumerable.Empty<IEntry>();
        }

        private IEnumerable<IEntry> GetSubEntries(string[] parts)
        {


            var path = Path.Combine(Enumerable
                .Concat(new[] { modLinkRootPath }, parts.Skip(rootParts.Length)).ToArray());

            if (!System.IO.Directory.Exists(path))
            {
                return Enumerable.Empty<IEntry>();
            }

            var dirs = System.IO.Directory.GetDirectories(path)
                .Select(CreateEntryFromDirectory);
            var files = System.IO.Directory.GetFiles(path)
                .Select(CreateEntryFromFile);
            return Enumerable.Concat(dirs, files);


        }

        private IEntry CreateEntryFromFile(string file)
        {
            var info = new FileInfo(file);
            return new FileEntry(
                "\\" + Path.Combine(link.TargetPath, Path.GetRelativePath(modLinkRootPath, file)),
                file,
                this,
                false
            );
        }
        private IEntry CreateEntryFromDirectory(string dir)
        {
            return new DirectoryEntry(
                "\\" + Path.Combine(link.TargetPath, Path.GetRelativePath(modLinkRootPath, dir)),
                dir,
                this,
                false
            );
        }



    }
}
