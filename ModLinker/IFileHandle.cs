using DokanNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLinker
{
    public interface IFileHandle
    {
        void CloseFile();
        NtStatus Read(byte[] buffer, out int bytesRead, long offset);
        NtStatus Write(byte[] buffer, out int bytesWritten, long offset);
        NtStatus FlushFileBuffers();
        void Cleanup();
        IEntry Entry { get; }
        bool Writable { get; }

        NtStatus SetEndOfFile(long length);
        NtStatus SetAllocationSize(long length);
        NtStatus LockFile(long offset, long length);
        NtStatus UnlockFile(long offset, long length);
    }
}
