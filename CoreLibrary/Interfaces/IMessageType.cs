namespace CoreLibrary.Interfaces
{
    /// <summary>
    /// Represents the base interface for different types of messages.
    /// </summary>
    public interface IMessageType
    {
        /// <summary>
        /// Processes the message logic for the given sender and content.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="content">The content of the message.</param>
        void ProcessMessage(string sender, string content);
    }
}
