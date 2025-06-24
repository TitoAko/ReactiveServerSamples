using CoreLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Utilities
{
    public class Logger : ILogger
    {
        public void Log(string message)
        {
            // Log the message to the console
            Console.WriteLine($"[Log]: {message}");
        }
    }
}
