using Rcon.Types;
using System;
using System.Net.Sockets;

namespace Rcon.Events
{
    /// <summary>
    /// RCON client sent a packet event args
    /// </summary>
    public class ClientSentPacketEventArgs : EventArgs
    {
        /// <summary>
        /// Packet sender
        /// </summary>
        public TcpClient Client { get; internal set; }

        /// <summary>
        /// Received data packet
        /// </summary>
        public Packet Packet { get; internal set; }
    }
}
