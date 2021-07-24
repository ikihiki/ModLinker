using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using DokanNet;

namespace ModLinker.Test
{
    public class TestFileInfo:IDokanFileInfo
    {
        public WindowsIdentity GetRequestor()
        {
            throw new NotImplementedException();
        }

        public bool TryResetTimeout(int milliseconds)
        {
            throw new NotImplementedException();
        }

        public object Context { get; set; }
        public bool DeleteOnClose { get; set; }
        public bool IsDirectory { get; set; }
        public bool NoCache { get; }
        public bool PagingIo { get; }
        public int ProcessId { get; }
        public bool SynchronousIo { get; }
        public bool WriteToEndOfFile { get; }
    }
}
