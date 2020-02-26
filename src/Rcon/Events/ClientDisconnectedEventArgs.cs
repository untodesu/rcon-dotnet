using System;
using System.Net;

namespace Rcon.Events
{
    /// <summary>
    /// RCON client disconnected event args
    /// </summary>
    public class ClientDisconnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Disconnected client address
        /// </summary>
        public EndPoint EndPoint { get; internal set; }
    }
}
