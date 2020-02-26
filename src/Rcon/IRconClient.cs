using Rcon.Types;
using System;
using System.Threading.Tasks;

namespace Rcon
{
    /// <summary>
    /// RCON Client interface.
    /// </summary>
    public interface IRconClient : IDisposable
    {
        /// <summary>
        /// Connection status
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Authentication status
        /// </summary>
        bool Authenticated { get; }

        /// <summary>
        /// Writes packet to the server
        /// </summary>
        /// <param name="packet">RCON Packet</param>
        Task SendPacketAsync(Packet packet);

        /// <summary>
        /// Writes packet to the server
        /// </summary>
        /// <param name="packet">RCON Packet</param>
        void SendPacket(Packet packet);

        /// <summary>
        /// Reads packet from the server
        /// </summary>
        /// <returns>A packet data</returns>
        Task<Packet> ReceivePacketAsync();

        /// <summary>
        /// Reads packet from the server
        /// </summary>
        /// <returns>A packet data</returns>
        Packet ReceivePacket();

        /// <summary>
        /// Connects to specified server
        /// </summary>
        /// <param name="host">IP or URL(?)</param>
        /// <param name="port">Port (usually 1..65535)</param>
        /// <returns>Connection status</returns>
        Task<bool> ConnectAsync(string host, int port);

        /// <summary>
        /// Connects to specified server
        /// </summary>
        /// <param name="host">IP or URL(?)</param>
        /// <param name="port">Port (usually 1..65535)</param>
        /// <returns>Connection status</returns>
        bool Connect(string host, int port);

        /// <summary>
        /// Closes connection.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Authenticates (sends auth packet to the server)
        /// </summary>
        /// <param name="password">RCON password</param>
        /// <returns>Authentication status</returns>
        Task<bool> AuthenticateAsync(string password);

        /// <summary>
        /// Authenticates (sends auth packet to the server)
        /// </summary>
        /// <param name="password">RCON password</param>
        /// <returns>Authentication status</returns>
        bool Authenticate(string password);

        /// <summary>
        /// Sends command packet to the server
        /// </summary>
        /// <param name="command">Command packet body</param>
        /// <returns>Server response packet body</returns>
        Task<string> SendCommandAsync(string command);

        /// <summary>
        /// Sends command packet to the server
        /// </summary>
        /// <param name="command">Command packet body</param>
        /// <returns>Server response packet body</returns>
        string SendCommand(string command);
    }
}
