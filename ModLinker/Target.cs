using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MasterMemory;
using MessagePack;

namespace ModLinker
{
    public class Target
    {
        public string Name { get; set; }
        public string RootPath { get; set; }
        public string ModDirectory { get; set; }
        public IEnumerable<Mod> Mods { get; set; }

        private MemoryDatabase database;

        public void Construct()
        {
            var builder = new DatabaseBuilder();
            builder.Append(Mods);
            Dictionary<string, Directory> directories = new Dictionary<string, Directory>();
            Dictionary<string, File> files = new Dictionary<string, File>();
            foreach (var mod in Mods)
            {
                foreach (var dir in GetDirectories(mod))
                {
                    if (directories.ContainsKey(dir.Path))
                    {
                        directories[dir.Path].ChildrenDirectory =
                            directories[dir.Path].ChildrenDirectory.Concat(dir.ChildrenDirectory)
                            .Distinct()
                            ;
                        directories[dir.Path].Files =
                            directories[dir.Path].Files.Concat(dir.Files)
                            .Distinct()
                            ;
                    }
                    else
                    {
                        directories.Add(dir.Path, dir);
                    }
                }

                foreach(var file in GetFiles(mod))
                {
                    files[file.Path] = file;
                }
            }
            builder.Append(directories.Values);
            builder.Append(files.Values);
            database = new MemoryDatabase(builder.Build());
        }

        private IEnumerable<Directory> GetDirectories(Mod mod)
        {
            return mod switch
            {
                _ when System.IO.Directory.Exists(mod.EntityPath)
                    => GetDirectoriesFromDrectory(mod),
                _ when System.IO.File.Exists(mod.EntityPath) && Path.GetExtension(mod.EntityPath) == "zip"
                    => GetDirectoriesFromZip(mod),
                _ => Enumerable.Empty<Directory>()
            };
        }
        private IEnumerable<Directory> GetDirectoriesFromDrectory(Mod mod)
        {
            var rootDir = new DirectoryInfo(Path.Combine(mod.EntityPath, mod.RootPath));
            if (!rootDir.Exists)
            {
                return Enumerable.Empty<Directory>();
            }

            return mod.Links
                .SelectMany(link =>
                {
                    var linkRoot = new DirectoryInfo(Path.Combine(rootDir.FullName, link.ModPath));
                    if (!linkRoot.Exists)
                    {
                        return Enumerable.Empty<Directory>();
                    }
                    return linkRoot.EnumerateDirectories("*", new EnumerationOptions { RecurseSubdirectories = true })
                    .Select(dir => new Directory
                    {
                        ChildrenDirectory = dir.EnumerateDirectories().Select(child => Path.Combine(link.TargetPath, Path.GetRelativePath(child.FullName, linkRoot.FullName))),
                        Files = dir.EnumerateFiles().Select(file => Path.Combine(link.TargetPath, Path.GetRelativePath(file.FullName, rootDir.FullName))),
                        Path = Path.Combine(link.TargetPath, Path.GetRelativePath(dir.FullName, rootDir.FullName))
                    });
                });
        }

        private IEnumerable<Directory> GetDirectoriesFromZip(Mod mod)
        {

        }

        private IEnumerable<File> GetFiles(Mod mod)
        {
            return mod switch
            {
                _ when System.IO.Directory.Exists(mod.EntityPath)
                    => GetFilesFromDrectory(mod),
                _ when System.IO.File.Exists(mod.EntityPath) && Path.GetExtension(mod.EntityPath) == "zip"
                    => GetFilesFromZip(mod),
                _ => Enumerable.Empty<File>()
            };
        }

        private IEnumerable<File> GetFilesFromZip(Mod mod)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<File> GetFilesFromDrectory(Mod mod)
        {
            var rootDir = new DirectoryInfo(Path.Combine(mod.EntityPath, mod.RootPath));
            if (!rootDir.Exists)
            {
                return Enumerable.Empty<File>();
            }

            return mod.Links
                .SelectMany(link =>
                {
                    var linkRoot = new DirectoryInfo(Path.Combine(rootDir.FullName, link.ModPath));
                    if (!linkRoot.Exists)
                    {
                        return Enumerable.Empty<File>();
                    }
                    return linkRoot.EnumerateFiles("*", new EnumerationOptions { RecurseSubdirectories = true })
                    .Select(file => new File
                    {
                        Directory = Path.Combine(link.TargetPath, Path.GetRelativePath(file.DirectoryName, linkRoot.FullName)),
                        ModId = mod.Id,
                        ModPath = Path.GetRelativePath(mod.EntityPath, file.FullName),
                        Path = Path.Combine(link.TargetPath, Path.GetRelativePath(file.FullName, linkRoot.FullName))
                    });
                });
        }
    }
}