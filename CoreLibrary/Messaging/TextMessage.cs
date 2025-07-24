using CoreLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Messaging
{
    /// <summary>Marker type for plain chat payloads.</summary>
    public readonly record struct TextMessage : IMessageType
    {
        public void ProcessMessage(string sender, string content)
        {
            Console.WriteLine($"Text message from {sender}: {content}");
        }
    }
}
