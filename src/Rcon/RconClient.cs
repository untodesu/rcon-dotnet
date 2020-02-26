using Rcon.Exceptions;
using Rcon.Types;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Rcon
{
    public class RconClient : IRconClient
    {
        private TcpClient m_TcpClient;

        /// <summary>
        /// Connection status
        /// </summary>
        public bool Connected => m_TcpClient.Client.Connected;

        /// <summary>
        /// Authentication status
        /// </summary>
        public bool Authenticated { get; protected set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public RconClient()
        {
            m_TcpClient = new TcpClient();
            Authenticated = false;
        }

        /// <summary>
        /// Kind of destructor
        /// </summary>
        public void Dispose()
        {
            m_TcpClient.Close();
            m_TcpClient = null;
        }

        /// <summary>
        /// Reads byte buffer from the server
        /// </summary>
        /// <returns>Result buffer</returns>
        private async Task<byte[]> ReadServerAsync()
        {
            NetworkStream stream = m_TcpClient.GetStream();

            byte[] buffer = new byte[4096]; // max rcon packet size
            int bytes;
            do {
                bytes = await stream.ReadAsync(buffer, 0, buffer.Length);
            } while(bytes == 0);

            byte[] result = new byte[bytes];
            Array.Copy(buffer, 0, result, 0, bytes);
            return result;
        }

        /// <summary>
        /// Reads byte buffer from the server
        /// </summary>
        /// <returns>Result buffer</returns>
        private byte[] ReadServer()
        {
            NetworkStream stream = m_TcpClient.GetStream();

            byte[] buffer = new byte[4096]; // max rcon packet size
            int bytes;
            do {
                bytes = stream.Read(buffer, 0, buffer.Length);
            } while(bytes == 0);

            byte[] result = new byte[bytes];
            Array.Copy(buffer, 0, result, 0, bytes);
            return result;
        }

        /// <summary>
        /// Writes packet to the server
        /// </summary>
        /// <param name="packet">RCON Packet</param>
        public async Task SendPacketAsync(Packet packet)
        {
            NetworkStream stream = m_TcpClient.GetStream();
            byte[] buffer = packet.GetBytes();
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Writes packet to the server
        /// </summary>
        /// <param name="packet">RCON Packet</param>
        public void SendPacket(Packet packet)
        {
            NetworkStream stream = m_TcpClient.GetStream();
            byte[] buffer = packet.GetBytes();
            stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Reads packet from the server
        /// </summary>
        /// <returns>A packet data</returns>
        public async Task<Packet> ReceivePacketAsync()
        {
            byte[] buffer = await ReadServerAsync();
            return new Packet(buffer);
        }

        /// <summary>
        /// Reads packet from the server
        /// </summary>
        /// <returns>A packet data</returns>
        public Packet ReceivePacket()
        {
            byte[] buffer = ReadServer();
            return new Packet(buffer);
        }

        /// <summary>
        /// Connects to specified server
        /// </summary>
        /// <param name="host">IP or URL(?)</param>
        /// <param name="port">Port (usually 1..65535)</param>
        /// <returns>Connection status</returns>
        public async Task<bool> ConnectAsync(string host, int port)
        {
            await m_TcpClient.ConnectAsync(host, port);
            return Connected;
        }

        /// <summary>
        /// Connects to specified server
        /// </summary>
        /// <param name="host">IP or URL(?)</param>
        /// <param name="port">Port (usually 1..65535)</param>
        /// <returns>Connection status</returns>
        public bool Connect(string host, int port)
        {
            m_TcpClient.Connect(host, port);
            return Connected;
        }

        /// <summary>
        /// Closes connection.
        /// </summary>
        public void Disconnect()
        {
            m_TcpClient.Close();
            m_TcpClient = new TcpClient();
            Authenticated = false;
        }

        /// <summary>
        /// Authenticates (sends auth packet to the server)
        /// </summary>
        /// <param name="password">RCON password</param>
        /// <returns>Authentication status</returns>
        public async Task<bool> AuthenticateAsync(string password)
        {
            if(!Connected) {
                throw new NotConnectedException();
            }

            Packet authPacket = new Packet(PacketType.SERVERDATA_AUTH, password, 0);
            await SendPacketAsync(authPacket);

            Packet responsePacket = await ReceivePacketAsync();
            Authenticated = (responsePacket.Id != -1);
            return Authenticated;
        }

        /// <summary>
        /// Authenticates (sends auth packet to the server)
        /// </summary>
        /// <param name="password">RCON password</param>
        /// <returns>Authentication status</returns>
        public bool Authenticate(string password)
        {
            if(!Connected) {
                throw new NotConnectedException();
            }

            Packet authPacket = new Packet(PacketType.SERVERDATA_AUTH, password, 0);
            SendPacket(authPacket);

            Packet responsePacket = ReceivePacket();
            Authenticated = (responsePacket.Id != -1);
            return Authenticated;
        }

        /// <summary>
        /// Sends command packet to the server
        /// </summary>
        /// <param name="command">Command packet body</param>
        /// <returns>Server response packet body</returns>
        public async Task<string> SendCommandAsync(string command)
        {
            if(!Connected) {
                throw new NotConnectedException();
            }
            if(!Authenticated) {
                throw new NotAuthenticatedException();
            }

            Packet commandPacket = new Packet(PacketType.SERVERDATA_EXECCOMMAND, command);
            await SendPacketAsync(commandPacket);
            Packet responsePacket = await ReceivePacketAsync();
            return responsePacket.Body;
        }

        /// <summary>
        /// Sends command packet to the server
        /// </summary>
        /// <param name="command">Command packet body</param>
        /// <returns>Server response packet body</returns>
        public string SendCommand(string command)
        {
            if(!Connected) {
                throw new NotConnectedException();
            }
            if(!Authenticated) {
                throw new NotAuthenticatedException();
            }

            Packet commandPacket = new Packet(PacketType.SERVERDATA_EXECCOMMAND, command);
            SendPacket(commandPacket);
            Packet responsePacket = ReceivePacket();
            return responsePacket.Body;
        }
    }
}
