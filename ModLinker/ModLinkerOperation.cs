using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using DokanNet;
using Microsoft.VisualBasic;
using FileAccess = DokanNet.FileAccess;

namespace ModLinker
{
    public class ModLinkerOperation : IDokanOperations
    {
        private readonly LayerService layerService;

        public ModLinkerOperation(LayerService layerService)
        {
            this.layerService = layerService;
        }


        public NtStatus CreateFile(string fileName, FileAccess access, FileShare share, FileMode mode, FileOptions options,
            FileAttributes attributes, IDokanFileInfo info)
        {
            var entry = layerService.GetEntry(fileName);
            if (info.IsDirectory)
            {

            }
            else
            {
                if (entry.IsDirectory)
                {
                    return 
                }

                IFileHandle handle;
                var result = entry.Open(access, share, mode, options, attributes, out handle);
                if (result == DokanResult.Success)
                {
                    info.Context = handle;
                }
                return result;
            }
        }

        public void Cleanup(string fileName, IDokanFileInfo info)
        {
            if (info.Context is IFileHandle handle)
            {
                handle.Cleanup();
                if (info.DeleteOnClose)
                {
                    var entry = handle.Entry;
                    entry.Delete();
                }
            }
        }

        public void CloseFile(string fileName, IDokanFileInfo info)
        {
            (info.Context as IFileHandle)?.CloseFile();
            info.Context = null;
        }

        public NtStatus ReadFile(string fileName, byte[] buffer, out int bytesRead, long offset, IDokanFileInfo info)
        {
            if (info.Context is IFileHandle handle)
            {
                return handle.Read(buffer, out bytesRead, offset);
            }

            bytesRead = 0;
            return DokanResult.InternalError;
        }

        public NtStatus WriteFile(string fileName, byte[] buffer, out int bytesWritten, long offset, IDokanFileInfo info)
        {
            if (GetWritableFileHandle(info, out var handle))
            {
                return handle.Write(buffer, out bytesWritten, offset);
            }

            bytesWritten = 0;
            return DokanResult.InternalError;
        }

        public NtStatus FlushFileBuffers(string fileName, IDokanFileInfo info)
        {
            if (GetWritableFileHandle(info, out var handle))
            {
                return handle.FlushFileBuffers();
            }

            return DokanResult.InternalError;
        }
        
        public NtStatus SetEndOfFile(string fileName, long length, IDokanFileInfo info)
        {
            if (GetWritableFileHandle(info, out var handle))
            {
                return handle.SetEndOfFile(length);
            }

            return DokanResult.InternalError;
        }

        public NtStatus SetAllocationSize(string fileName, long length, IDokanFileInfo info)
        {
            if (GetWritableFileHandle(info, out var handle))
            {
                return handle.SetAllocationSize(length);
            }

            return DokanResult.InternalError;
        }

        public NtStatus LockFile(string fileName, long offset, long length, IDokanFileInfo info)
        {
            if (info.Context is IFileHandle handle)
            {
                return handle.LockFile(offset,length);
            }

            return DokanResult.InternalError;
        }

        public NtStatus UnlockFile(string fileName, long offset, long length, IDokanFileInfo info)
        {
            if (info.Context is IFileHandle handle)
            {
                return handle.UnlockFile(offset, length);
            }

            return DokanResult.InternalError;
        }
        public NtStatus GetFileInformation(string fileName, out FileInformation fileInfo, IDokanFileInfo info)
        {
            var entry = layerService.GetEntry(fileName);
            fileInfo = new FileInformation
            {
                CreationTime = entry.CreationTime,
                FileName = entry.Name,
                LastAccessTime = entry.LastAccessTime,
                LastWriteTime = entry.LastWriteTime,
                Length = entry.Length
            };
            return DokanResult.Success;
        }

        public NtStatus FindFiles(string fileName, out IList<FileInformation> files, IDokanFileInfo info)
        {
            files = new List<FileInformation>(layerService.GetEntries(fileName).Select(entry =>
            {
                return new FileInformation
                {
                    FileName = entry.Path,
                    CreationTime = entry.CreationTime,
                    LastAccessTime = entry.LastAccessTime,
                    LastWriteTime = entry.LastWriteTime,
                    Attributes = entry.IsDirectory ? FileAttributes.Directory : FileAttributes.Normal
                };
            }));

            return DokanResult.Success;
        }

        public NtStatus FindFilesWithPattern(string fileName, string searchPattern, out IList<FileInformation> files, IDokanFileInfo info)
        {
            var entry = layerService.GetEntry(fileName);
        }

        public NtStatus SetFileAttributes(string fileName, FileAttributes attributes, IDokanFileInfo info)
        {
            var entry = layerService.GetEntry(fileName);
            if (entry == null)
            {
                return DokanResult.FileNotFound;
            }

            if (GetWritableEntry(entry, out var writableEntry))
            {
                 return writableEntry.SetFileAttributes(attributes);
            }

            return DokanResult.AccessDenied;
        }

        public NtStatus SetFileTime(string fileName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime,
            IDokanFileInfo info)
        {
            var entry = layerService.GetEntry(fileName);
            if (entry == null)
            {
                return DokanResult.FileNotFound;
            }

            if (GetWritableEntry(entry, out var writableEntry))
            {
                return writableEntry.SetFileTime(creationTime, lastAccessTime, lastWriteTime);
            }

            return DokanResult.AccessDenied;
        }
        
        public NtStatus GetFileSecurity(string fileName, out FileSystemSecurity security, AccessControlSections sections,
            IDokanFileInfo info)
        {
            var entry = layerService.GetEntry(fileName);
            if (entry == null)
            {
                security = null;
                return DokanResult.FileNotFound;
            }
            return entry.GetFileSecurity(out security, sections);
        }

        public NtStatus SetFileSecurity(string fileName, FileSystemSecurity security, AccessControlSections sections,
            IDokanFileInfo info)
        {
            var entry = layerService.GetEntry(fileName);
            if (entry == null)
            {
                return DokanResult.FileNotFound;
            }

            if (GetWritableEntry(entry, out var writableEntry))
            {
                return writableEntry.SetFileSecurity(security, sections);
            }

            return DokanResult.AccessDenied;
        }

        public NtStatus DeleteFile(string fileName, IDokanFileInfo info)
        {
            return layerService.DeleteFile(fileName);
        }

        public NtStatus DeleteDirectory(string fileName, IDokanFileInfo info)
        {
            return layerService.DeleteDirectory(fileName);
        }

        public NtStatus MoveFile(string oldName, string newName, bool replace, IDokanFileInfo info)
        {
            return layerService.MoveFile(oldName,newName);
        }

        public NtStatus GetDiskFreeSpace(out long freeBytesAvailable, out long totalNumberOfBytes, out long totalNumberOfFreeBytes,
            IDokanFileInfo info)
        {
            freeBytesAvailable = 0;
            totalNumberOfBytes = 0;
            totalNumberOfFreeBytes = 0;
            return DokanResult.NotImplemented;
        }

        public NtStatus GetVolumeInformation(out string volumeLabel, out FileSystemFeatures features, out string fileSystemName,
            out uint maximumComponentLength, IDokanFileInfo info)
        {
            volumeLabel = "";
            features = FileSystemFeatures.None;
            fileSystemName = "";
            maximumComponentLength = 0;
            return DokanResult.NotImplemented;
        }



        public NtStatus Mounted(IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus Unmounted(IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus FindStreams(string fileName, out IList<FileInformation> streams, IDokanFileInfo info)
        {
            throw new NotImplementedException();
        }

        private bool GetWritableFileHandle(IDokanFileInfo info, out  IFileHandle fileHandle)
        {
            if (info.Context is IFileHandle handle)
            {
                if (handle.Writable)
                {
                    fileHandle = handle;
                    return true;
                }

                fileHandle = layerService.UpdateWritable(handle);
                return fileHandle != null;
            }

            fileHandle = null;
            return false;
        }

        private bool GetWritableEntry(IEntry entry, out IEntry writableEntry)
        {
            if (entry.Writable)
            {
                writableEntry = entry;
                return true;
            }
            else
            {
                writableEntry = layerService.UpdateWritable(entry);
                return writableEntry != null;
            }
        }
    }
}
