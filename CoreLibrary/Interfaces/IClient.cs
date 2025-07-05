using CoreLibrary.Messaging;

namespace CoreLibrary.Interfaces
{
    public interface IClient
    {
        /// <summary>
        /// send message to the server
        /// </summary>
        /// <param name="message">Message to send to the server</param>
        void SendMessage(Message message);
        /// <summary>
        /// Receive message from the server
        /// </summary>
        /// <returns>A message received from the server</returns>
        Message ReceiveMessage();
        void Connect();  // Establish a connection (e.g., connect to the server)
        void Disconnect();  // Disconnect from the server

    }
}