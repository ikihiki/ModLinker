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
    public interface IEntry
    {
        public string Path { get; }
        public string Name { get; }
        public bool IsDirectory { get; }
        public ILayer Layer { get; }
        public DateTime? CreationTime { get; }
        public DateTime? LastAccessTime { get; }
        public DateTime? LastWriteTime { get; }
        public long Length { get; }
        bool Writable { get; }
        //public IEnumerable<string> Children { get; }

        public NtStatus Open(FileAccess access, FileShare share, FileMode mode, FileOptions options,
            FileAttributes attributes, out IFileHandle handle);
        public void Delete();
        NtStatus SetFileAttributes(FileAttributes attributes);
        NtStatus SetFileTime(DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime);
        NtStatus GetFileSecurity(out FileSystemSecurity security, AccessControlSections sections);
        NtStatus SetFileSecurity(FileSystemSecurity security, AccessControlSections sections);
    }
}
