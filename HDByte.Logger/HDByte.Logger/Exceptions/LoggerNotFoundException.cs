using System;
using System.Collections.Generic;
using System.Text;

namespace HDByte.Logger.Exceptions
{
    public class LoggerNotFoundException : Exception
    {
        public LoggerNotFoundException(string message) : base(message) { }
    }
}
