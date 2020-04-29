using System;
using System.Collections.Generic;
using System.Text;

namespace HDByte.Logger.Exceptions
{
    public class LoggerAlreadyExistsException : Exception
    {
        public LoggerAlreadyExistsException(string message) : base(message) { }
    }
}
