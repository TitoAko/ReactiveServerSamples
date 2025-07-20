using CoreLibrary.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Interfaces
{
    /// <summary>
    /// Defines an asynchronous handler for processing messages.
    /// </summary>
    public interface IMessageProcessor
    {
        /// <summary>
        /// Asynchronously processes a given message.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ProcessAsync(Message message);
    }
}
