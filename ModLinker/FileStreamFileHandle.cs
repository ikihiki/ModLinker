using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DokanNet;

namespace ModLinker
{
    class FileStreamFileHandle:IFileHandle
    {
        public IEntry Entry { get; set; }
        public bool Writable => fileStream.CanWrite;

        private FileStream fileStream;

        public FileStreamFileHandle(IEntry entry, string path, System.IO.FileAccess access, FileShare share, FileMode mode, FileOptions options)
        {
            this.Entry = entry;
            fileStream = new FileStream(path, mode, access, share, 4096, options);
        }

        public void CloseFile()
        {
            fileStream.Close();
        }

        public NtStatus Read(byte[] buffer, out int bytesRead, long offset)
        {
            try
            {
                lock (fileStream) //Protect from overlapped read
                {
                    fileStream.Position = offset;
                    bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                }
                return DokanResult.Success;
            }
            catch
            {
                bytesRead = 0;
                return DokanResult.AccessDenied;
            }
        }

        public NtStatus Write(byte[] buffer, out int bytesWritten, long offset)
        {
            var append = offset == -1;
            lock (fileStream) //Protect from overlapped write
            {
                if (append)
                {
                    if (fileStream.CanSeek)
                    {
                        fileStream.Seek(0, SeekOrigin.End);
                    }
                    else
                    {
                        bytesWritten = 0;
                        return DokanResult.Error;
                    }
                }
                else
                {
                    fileStream.Position = offset;
                }
                fileStream.Write(buffer, 0, buffer.Length);
            }
            bytesWritten = buffer.Length;
            return DokanResult.Success;
        }

        public NtStatus FlushFileBuffers()
        {
            try
            {
                fileStream.Flush();
                return  DokanResult.Success;
            }
            catch (IOException)
            {
                return DokanResult.DiskFull;
            }
        }

        public void Cleanup()
        {
            CloseFile();
        }
        public NtStatus SetEndOfFile(long length)
        {
            try
            {
                fileStream.SetLength(length);
                return DokanResult.Success;
            }
            catch (IOException)
            {
                return DokanResult.DiskFull;
            }
        }

        public NtStatus SetAllocationSize(long length)
        {
            try
            {
               fileStream.SetLength(length);
               return DokanResult.Success;
            }
            catch (IOException)
            {
                return DokanResult.DiskFull;
            }
        }

        public NtStatus LockFile(long offset, long length)
        {
            try
            {
                fileStream.Lock(offset, length);
                return DokanResult.Success;
            }
            catch (IOException)
            {
                return DokanResult.AccessDenied;
            }
        }

        public NtStatus UnlockFile(long offset, long length)
        {
            try
            {
                fileStream.Unlock(offset, length);
                return DokanResult.Success;
            }
            catch (IOException)
            {
                return DokanResult.AccessDenied;
            }
        }
    }
}
