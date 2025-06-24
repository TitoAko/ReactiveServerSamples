using CoreLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Messaging.MessageTypes
{
    internal class TypingMessage : IMessageType
    {
        public void ProcessMessage(string sender, string content)
        {
            // Process typing indicator
            Console.WriteLine($"{sender} is typing...");
        }
    }
}
