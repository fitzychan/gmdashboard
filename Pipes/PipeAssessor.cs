using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pipes.PreParsedFileProject;
using Pipes.PostParsedProject;

namespace Pipes
{
    public static class PipeAssessor
    {
        public static PostCommandPipe PostPipe
        {
            get
            {
                return new PostCommandPipe();
            }
        }
        public static PreCommandPipe PrePipe
        {
            get
            {
                return new PreCommandPipe();
            }

        }
    }
}
