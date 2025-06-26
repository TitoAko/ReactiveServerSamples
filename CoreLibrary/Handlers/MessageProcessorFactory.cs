using CoreLibrary.Interfaces;
using CoreLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Handlers
{
    public static class MessageProcessorFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="IMessageProcessor"/> configured for either client or server usage.
        /// </summary>
        /// <remarks>The method determines the mode of operation based on whether <paramref
        /// name="client"/> is provided. If <paramref name="client"/> is <see langword="null"/>, the processor is
        /// initialized for server-side operations, utilizing <paramref name="broadcastMessage"/> if provided.
        /// Otherwise, the processor is initialized for client-side operations.</remarks>
        /// <param name="loggingService">The logging service used to record events and diagnostics.</param>
        /// <param name="client">An optional client instance. If provided, the processor will be configured for client-side operations. If
        /// <paramref name="client"/> is <see langword="null"/>, the processor will be configured for server-side
        /// operations.</param>
        /// <param name="broadcastMessage">An optional broadcast message instance used for server-side operations. Ignored if <paramref name="client"/>
        /// is not <see langword="null"/>.</param>
        /// <returns>A new instance of <see cref="IMessageProcessor"/> configured for the specified mode of operation.</returns>
        public static IMessageProcessor CreateProcessor(LoggingService loggingService, IClient? client = null, IBroadcastMessage? broadcastMessage = null)
        {
            // Ensure only one of client or broadcastMessage is provided (not both)
            if (client == null && broadcastMessage != null)
            {
                return new MessageProcessor(loggingService, broadcastMessage);
            }
            else if (client != null && broadcastMessage == null)
            {
                return new MessageProcessor(loggingService, client!);
            }
            else
            {
                throw new ArgumentException("Either client or broadcastMessage must be provided, but not both.");
            }
        }
    }
}
