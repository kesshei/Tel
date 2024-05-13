using System;
using System.Collections.Generic;
using System.Text;

namespace Tel.Core.Exceptions
{
    public class APIErrorException : Exception
    {
        public APIErrorException(string message)
            : base(message)
        {
        }
    }
}
