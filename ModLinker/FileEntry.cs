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
    public class FileEntry : IEntry
    {
        public string Path { get; set; }
        public string RealPath => fileInfo.FullName;
        public string Name => fileInfo.Name;
        public bool IsDirectory => false;
        public bool IsExists => fileInfo.Exists;
        public ILayer Layer { get; set; }
        public DateTime? CreationTime => fileInfo.CreationTime;
        public DateTime? LastAccessTime => fileInfo.LastAccessTime;
        public DateTime? LastWriteTime => fileInfo.LastWriteTime;
        public long Length => fileInfo.Length;
        public bool Writable { get; }

        private FileInfo fileInfo;
        private const FileAccess DataWriteAccess = FileAccess.WriteData | FileAccess.AppendData |
                                                   FileAccess.Delete |
                                                   FileAccess.GenericWrite;


        public FileEntry(string path, string realPath, ILayer layer, bool writable)
        {
            Path = path;
            fileInfo = new FileInfo(realPath);
            Layer = layer;
            Writable = writable;
        }


        public NtStatus Open(FileAccess access, FileShare share, FileMode mode, FileOptions options,
            FileAttributes attributes,
            out IFileHandle handle)
        {
            var readAccess = (access & DataWriteAccess) == 0;
            if (!Writable && !readAccess)
            {
                handle = null;
                return DokanResult.AccessDenied;
            }
            
            
            if (Writable)
            {
                handle = new FileStreamFileHandle(this, RealPath, readAccess? System.IO.FileAccess.Read:System.IO.FileAccess.Write, share, mode, options);
                return DokanResult.Success;
            }
            else
            {
                if (mode == FileMode.OpenOrCreate && !IsExists || mode != FileMode.Open)
                {
                    handle = null;
                    return DokanResult.AccessDenied;
                }
                handle = new FileStreamFileHandle(this, RealPath, System.IO.FileAccess.Read, share, mode, options);
                return DokanResult.Success;
            }
        }

        public void Delete()
        {
            if (!Writable)
            {
                return;
            }
            fileInfo.Delete();
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
                fileInfo.CreationTime = creationTime.Value;
            }
            if (lastAccessTime != null)
            {
                fileInfo.LastAccessTime = lastAccessTime.Value;
            }
            if (lastWriteTime != null)
            {
                fileInfo.LastWriteTime = lastWriteTime.Value;
            }

            return DokanResult.Success;
        }

        public NtStatus GetFileSecurity(out FileSystemSecurity security, AccessControlSections sections)
        {
            try
            {
                security = fileInfo.GetAccessControl(sections);
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
                fileInfo.SetAccessControl((FileSecurity)security);
                return DokanResult.Success;
            }
            catch (UnauthorizedAccessException)
            {
                return DokanResult.AccessDenied;
            }
        }
    }
}
