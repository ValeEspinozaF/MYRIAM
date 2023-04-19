using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MYRIAM
{
    public class InputErrorException : Exception
    {
        public InputErrorException()
        {
        }

        public InputErrorException(string message)
            : base("\n\n" + message + "\n")
        {
        }

        public InputErrorException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
