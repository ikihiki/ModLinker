﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLinker
{
    public class BaseLayer:ILayer
    {
        private readonly string rootPath;
        private readonly FileSystemWatcher fileSystemWatcher;

        public event Action<IEntry> Notify;
        public event Action<string> CreateNotify;
        public event Action<string> UpdateNotify;
        public event Action<string> DeleteNotify;
        public event Action<(string oldPath, string newPath)> RenameNotify;

        public BaseLayer(string rootPath)
        {
            this.rootPath = rootPath;
            fileSystemWatcher = new FileSystemWatcher
            {
                Path = rootPath,
                IncludeSubdirectories = true
            };
            fileSystemWatcher.Changed+=FileSystemWatcherOnChanged;
            fileSystemWatcher.Created+=FileSystemWatcherOnCreated;
            fileSystemWatcher.Deleted+=FileSystemWatcherOnDeleted;
            fileSystemWatcher.Renamed+=FileSystemWatcherOnRenamed;
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void FileSystemWatcherOnRenamed(object sender, RenamedEventArgs e)
        {
            throw new NotImplementedException();
            if (Notify is not null)
            {
                //Notify.Invoke(new IEntry { Path = e.FullPath});
            }
            
        }

        private void FileSystemWatcherOnDeleted(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
            if (Notify is not null)
            {
                //Notify.Invoke(new IEntry { Path = e.FullPath });
            }

        }

        private void FileSystemWatcherOnCreated(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
            if (Notify is not null)
            {
                //Notify.Invoke(new IEntry { Path = e.FullPath });
            }
        }

        private void FileSystemWatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
            if (Notify is not null)
            {
                //Notify.Invoke(new IEntry { Path = e.FullPath });
            }
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
            if (path.StartsWith("\\"))
            {
                path = "." + path.Substring(1);
            }

            if (!System.IO.Directory.Exists(path))
            {
                return Enumerable.Empty<IEntry>();
            }
            
            var fullPath = Path.Combine(rootPath, path);
            var dirs = System.IO.Directory.GetDirectories(fullPath)
                .Select(CreateEntryFromDirectory);
            var files = System.IO.Directory.GetFiles(fullPath)
                .Select(CreateEntryFromFile);
            return Enumerable.Concat(dirs, files);
        }

        private IEntry CreateEntryFromFile(string file)
        {
            throw new NotImplementedException();
            var info = new FileInfo(file);
            //return new Entry
            //{
            //    Path = "\\" + Path.GetRelativePath(rootPath, file),
            //    Name = info.Name,
            //    CreationTime = info.CreationTime,
            //    LastAccessTime = info.LastAccessTime,
            //    LastWriteTime = info.LastWriteTime,
            //    Length = info.Length,
            //    IsDirectory = false,
            //    Layer = this
            //};
        }

        private IEntry CreateEntryFromDirectory(string dir)
        {
            throw new NotImplementedException();
            var info = new DirectoryInfo(dir);
            //return new Entry
            //{
            //    Path = "\\" + Path.GetRelativePath(rootPath, dir),
            //    Name = info.Name,
            //    CreationTime = info.CreationTime,
            //    LastAccessTime =info.LastAccessTime,
            //    LastWriteTime =  info.LastWriteTime,
            //    IsDirectory = true,
            //    Layer = this
            //};
        }

        public void Dispose()
        {
            fileSystemWatcher.Dispose();
        }

        public override string ToString()
        {
            return base.ToString() + "(" + rootPath + ")";
        }
    }
}
