using CoreLibrary.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Interfaces
{
    public interface ICommunicator
    {
        void StartListening();  // Start listening for incoming messages
        void SendMessage(Message message);  // Send a message
        Message ReceiveMessage();  // Receive a message
    }
}
