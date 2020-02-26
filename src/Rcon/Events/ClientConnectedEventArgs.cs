using System;
using System.Net.Sockets;

namespace Rcon.Events
{
    /// <summary>
    /// RCON Client connected event args
    /// </summary>
    public class ClientConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// A connected client
        /// </summary>
        public TcpClient Client { get; internal set; }
    }
}
