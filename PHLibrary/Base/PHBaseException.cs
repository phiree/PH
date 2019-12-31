using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibrary.Base
{
    public class PHBaseException : Exception
    {
        public PHBaseException()
        {
        }
        public PHBaseException(Exception baseException)
        {
            BaseException = baseException;
        }

        public Exception BaseException { get; set; }
    }
}
