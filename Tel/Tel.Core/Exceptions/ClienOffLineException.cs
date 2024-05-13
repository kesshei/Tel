using System;

namespace Tel.Core.Exceptions
{
    public class ClienOffLineException : Exception
    {
        public ClienOffLineException(string message)
            : base(message)
        {
        }
    }
}
