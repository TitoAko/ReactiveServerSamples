using CoreLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Messaging.MessageTypes
{
    public class TextMessage : IMessageType
    {
        public void ProcessMessage(string sender, string content)
        {
            // Process the text message
            Console.WriteLine($"Processing text message from {sender}: {content}");
        }
    }
}
