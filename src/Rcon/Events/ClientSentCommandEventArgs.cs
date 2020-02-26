using System;
using System.Net.Sockets;

namespace Rcon.Events
{
    /// <summary>
    /// RCON client sent a command packet (SERVERDATA_EXECCOMMAND) event args
    /// </summary>
    public class ClientSentCommandEventArgs : EventArgs
    {
        /// <summary>
        /// Packet sender
        /// </summary>
        public TcpClient Client { get; internal set; }

        /// <summary>
        /// Packet body (command)
        /// </summary>
        public string Command { get; internal set; }
    }
}
