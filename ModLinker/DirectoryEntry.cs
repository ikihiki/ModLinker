using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using DokanNet;
using FileAccess = DokanNet.FileAccess;

namespace ModLinker
{
    class DirectoryEntry:IEntry
    {
        public string Path { get; }
        public string RealPath => directoryInfo.FullName;
        public string Name => directoryInfo.Name;
        public bool IsDirectory => true;
        public ILayer Layer { get; }
        public DateTime? CreationTime => directoryInfo.CreationTime;
        public DateTime? LastAccessTime => directoryInfo.LastAccessTime;
        public DateTime? LastWriteTime => directoryInfo.LastWriteTime;
        public long Length => 0;
        public bool Writable { get; }

        private readonly DirectoryInfo directoryInfo;

        public DirectoryEntry(string path, string realPath, ILayer layer, bool writable)
        {
            Path = path;
            directoryInfo = new DirectoryInfo(realPath);
            Layer = layer;
            Writable = writable;
        }

        public NtStatus Open(FileAccess access, FileShare share, FileMode mode, FileOptions options, FileAttributes attributes,
            out IFileHandle handle)
        {
            handle = null;
            switch (mode)
            {
                case FileMode.Open:
                    return directoryInfo.Exists ? DokanResult.Success : DokanResult.FileNotFound;
                case FileMode.CreateNew:
                    if (directoryInfo.Exists)
                    {
                        return DokanResult.FileExists;
                    }
                    directoryInfo.Create();
                    return DokanResult.Success;
            }

            return DokanResult.Success;
        }

        public void Delete()
        {
            directoryInfo.Delete();
        }

        public NtStatus SetFileAttributes(FileAttributes attributes)
        {
            if (!Writable)
            {
                return DokanResult.AccessDenied;
            }
            File.SetAttributes(RealPath, attributes);
            return DokanResult.Success;
        }

        public NtStatus SetFileTime(DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime)
        {
            if (!Writable)
            {
                return DokanResult.AccessDenied;
            }

            if (creationTime != null)
            {
                directoryInfo.CreationTime = creationTime.Value;
            }
            if (lastAccessTime != null)
            {
                directoryInfo.LastAccessTime = lastAccessTime.Value;
            }
            if (lastWriteTime != null)
            {
                directoryInfo.LastWriteTime = lastWriteTime.Value;
            }

            return DokanResult.Success;
        }

        public NtStatus GetFileSecurity(out FileSystemSecurity security, AccessControlSections sections)
        {
            try
            {
                security = directoryInfo.GetAccessControl(sections);
                return DokanResult.Success;
            }
            catch (UnauthorizedAccessException)
            {
                security = null;
                return DokanResult.AccessDenied;
            }
        }

        public NtStatus SetFileSecurity(FileSystemSecurity security, AccessControlSections sections)
        {
            if (!Writable)
            {
                return DokanResult.AccessDenied;
            }

            try
            {
                directoryInfo.SetAccessControl((DirectorySecurity)security);
                return DokanResult.Success;
            }
            catch (UnauthorizedAccessException)
            {
                return DokanResult.AccessDenied;
            }
        }
    }
}
