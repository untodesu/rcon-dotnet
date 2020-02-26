using System;
using System.Net.Sockets;

namespace Rcon.Events
{
    /// <summary>
    /// RCON Client authenticated event args
    /// </summary>
    public class ClientAuthenticatedEventArgs : EventArgs
    {
        /// <summary>
        /// Autnenticated client
        /// </summary>
        public TcpClient Client { get; set; }
    }
}
