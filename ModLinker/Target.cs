using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ModLinker
{
    public class Target
    {
        public string Name { get; set; }
        public string RootPath { get; set; }
        public string ModDirectory { get; set; }
        public IEnumerable<Mod> Mods { get; set; }
        public string OverlayPath { get; set; }
        public string BasePath { get; set; }

        //private MemoryDatabase database;

        //public void Construct()
        //{
        //    var builder = new DatabaseBuilder();
        //    builder.Append(Mods);
        //    Dictionary<string, Directory> directoryList = new Dictionary<string, Directory>();
        //    Dictionary<string, File> fileList = new Dictionary<string, File>();
        //    foreach (var mod in Mods.OrderBy(mod=>mod.Order))
        //    {
        //        var (dirs, files) = GetDirectoriesAndFiles(mod);
        //        foreach (var dir in dirs)
        //        {
        //            if (directoryList.ContainsKey(dir.Path))
        //            {
        //                directoryList[dir.Path].ChildrenDirectory =
        //                    directoryList[dir.Path].ChildrenDirectory.Concat(dir.ChildrenDirectory)
        //                    .Distinct()
        //                    ;
        //                directoryList[dir.Path].Files =
        //                    directoryList[dir.Path].Files.Concat(dir.Files)
        //                    .Distinct()
        //                    ;
        //            }
        //            else
        //            {
        //                directoryList.Add(dir.Path, dir);
        //            }
        //        }

        //        foreach (var file in files)
        //        {
        //            fileList[file.Path] = file;
        //        }
        //    }
        //    builder.Append(directoryList.Values);
        //    //builder.Append(fileList.Values);
        //    database = new MemoryDatabase(builder.Build());
        //}

        //private (IEnumerable<Directory> directory, IEnumerable<File> file) GetDirectoriesAndFiles(Mod mod)
        //{
        //    return mod switch
        //    {
        //        _ when System.IO.Directory.Exists(mod.EntityPath)
        //            => GetDirectoriesAndFilesFromDrectory(mod),
        //        _ when System.IO.File.Exists(mod.EntityPath) && Path.GetExtension(mod.EntityPath) == "zip"
        //            => GetDirectoriesAndFilesFromZip(mod),
        //        _ => (Enumerable.Empty<Directory>(),Enumerable.Empty<File>())
        //    };
        //}
        //private (IEnumerable<Directory> directory, IEnumerable<File> file) GetDirectoriesAndFilesFromDrectory(Mod mod)
        //{
        //    var rootDir = new DirectoryInfo(Path.Combine(mod.EntityPath, mod.RootPath));
        //    if (!rootDir.Exists)
        //    {
        //        return (Enumerable.Empty<Directory>(), Enumerable.Empty<File>());
        //    }

        //    return (
        //        mod.Links
        //            .SelectMany(link =>
        //            {
        //                var linkRoot = new DirectoryInfo(Path.Combine(rootDir.FullName, link.ModPath));
        //                if (!linkRoot.Exists)
        //                {
        //                    return Enumerable.Empty<Directory>();
        //                }
        //                return linkRoot.EnumerateDirectories("*", new EnumerationOptions { RecurseSubdirectories = true })
        //                .Select(dir => new Directory
        //                {
        //                    ChildrenDirectory = dir.EnumerateDirectories().Select(child => Path.Combine(link.TargetPath, Path.GetRelativePath(child.FullName, linkRoot.FullName))),
        //                    Files = dir.EnumerateFiles().Select(file => Path.Combine(link.TargetPath, Path.GetRelativePath(file.FullName, rootDir.FullName))),
        //                    Path = Path.Combine(link.TargetPath, Path.GetRelativePath(dir.FullName, rootDir.FullName))
        //                });
        //            }),
        //        mod.Links
        //            .SelectMany(link =>
        //            {
        //                var linkRoot = new DirectoryInfo(Path.Combine(rootDir.FullName, link.ModPath));
        //                if (!linkRoot.Exists)
        //                {
        //                    return Enumerable.Empty<File>();
        //                }
        //                return linkRoot.EnumerateFiles("*", new EnumerationOptions { RecurseSubdirectories = true })
        //                .Select(file => new File
        //                {
        //                    Directory = Path.Combine(link.TargetPath, Path.GetRelativePath(file.DirectoryName, linkRoot.FullName)),
        //                    ModId = mod.Id,
        //                    ModPath = Path.GetRelativePath(mod.EntityPath, file.FullName),
        //                    Path = Path.Combine(link.TargetPath, Path.GetRelativePath(file.FullName, linkRoot.FullName))
        //                });
        //            })
        //        );
        //}

        //private static IEnumerable<string> GetAllDirs(IEnumerable<string> other, string path)
        //{
        //    var parent = Path.GetDirectoryName(path);
        //    if(string.IsNullOrEmpty(parent))
        //    {
        //        return other;
        //    }else{
        //        return GetAllDirs(other, parent).Concat(new[] { parent });
        //    }
        //}


        //private (IEnumerable<Directory> directory, IEnumerable<File> file) GetDirectoriesAndFilesFromZip(Mod mod)
        //{
        //    try
        //    {
        //        using ZipArchive archive = ZipFile.OpenRead(mod.EntityPath);
        //        var files = archive.Entries.ToDictionary(entory => entory.FullName);
        //        var dirs = files.Keys.SelectMany(file => GetAllDirs(Enumerable.Empty<string>(), file))
        //            .Distinct()
        //            .ToDictionary(dir => dir, dir => new { Path = dir, Children = new List<string>(), Files = new List<string>() });
        //        foreach(var dir in dirs.Values)
        //        {
        //            var parent = Path.GetDirectoryName(dir.Path);
        //            if(!string.IsNullOrEmpty(parent) && dirs.ContainsKey(parent))
        //            {
        //                dirs[parent].Children.Add(dir.Path);
        //            }
        //        }
        //        foreach(var file in files.Keys)
        //        {
        //            var parent = Path.GetDirectoryName(file);
        //            if (!string.IsNullOrEmpty(parent) && dirs.ContainsKey(parent))
        //            {
        //                dirs[parent].Files.Add(file);
        //            }  
        //        }

        //        return (
        //            mod.Links
        //                .SelectMany(link =>
        //                {
        //                    return dirs.Values.Where(dir => dir.Path.StartsWith(link.ModPath))
        //                    .Select(dir => new Directory
        //                    {
        //                        ChildrenDirectory = dir.Children.Select(child => Path.Combine(link.TargetPath, child.Replace(link.ModPath, ""))),
        //                        Files = dir.Files.Select(file => Path.Combine(link.TargetPath, file.Replace(link.ModPath, ""))),
        //                        Path = Path.Combine(link.TargetPath, dir.Path.Replace(link.ModPath, ""))
        //                    });
        //                }),
        //            mod.Links
        //                .SelectMany(link =>
        //                {
        //                    return files.Values.Where(file => file.FullName.StartsWith(link.ModPath))
        //                        .Select(file => new File
        //                        {
        //                            Directory = Path.GetDirectoryName(file.FullName),
        //                            ModId = mod.Id,
        //                            ModPath = file.FullName,
        //                            Path = Path.Combine(link.TargetPath, file.FullName.Replace(link.ModPath, "")),
        //                        });
        //                })
        //        );
        //    }
        //    catch
        //    {
        //        return (Enumerable.Empty<Directory>(), Enumerable.Empty<File>());
        //    }
        //}
    }
}