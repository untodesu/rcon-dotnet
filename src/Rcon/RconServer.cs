using Rcon.Events;
using Rcon.Types;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Rcon
{
    public class RconServer : IRconServer
    {
        /// <summary>
        /// TCP Server
        /// </summary>
        private TcpListener m_TcpListener;

        /// <summary>
        /// Disposed status
        /// </summary>
        private bool m_Disposed;

        /// <summary>
        /// RCON Password
        /// </summary>
        public string Password { get; protected set; }

        /// <summary>
        /// Triggered when someone connects to the server
        /// </summary>
        public event EventHandler<ClientConnectedEventArgs> OnClientConnected;

        /// <summary>
        /// Triggered when connected client disconnects from the server
        /// </summary>
        public event EventHandler<ClientDisconnectedEventArgs> OnClientDisconnected;

        /// <summary>
        /// Triggered when connected client authenticates successfully
        /// </summary>
        public event EventHandler<ClientAuthenticatedEventArgs> OnClientAuthenticated;

        /// <summary>
        /// Triggered when client sent a packet to the server
        /// </summary>
        public event EventHandler<ClientSentPacketEventArgs> OnClientPacketReceived;

        /// <summary>
        /// Triggered when received packet from the client is SERVERDATA_EXECCOMMAND.
        /// This should return response packet body (SERVERDATA_RESPONSE_VALUE)
        /// </summary>
        public event ReturnEventHandler<string, ClientSentCommandEventArgs> OnClientCommandReceived;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="port">Port to listen</param>
        /// <param name="address">Local IP</param>
        public RconServer(string password, int port, IPAddress address)
        {
            m_Disposed = false;
            m_TcpListener = new TcpListener(address, port);
            Password = password;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="port">Port to listen</param>
        public RconServer(string password, int port)
        {
            m_Disposed = false;
            m_TcpListener = new TcpListener(IPAddress.Loopback, port);
            Password = password;
        }

        /// <summary>
        /// Kinda destructor.
        /// </summary>
        public void Dispose()
        {
            m_TcpListener.Stop();
            m_TcpListener = null;
            m_Disposed = true;
        }

        /// <summary>
        /// Reads a packet from client
        /// </summary>
        /// <param name="client">Client to receive from</param>
        /// <returns>Result packet</returns>
        public async Task<Packet> ReceivePacketAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[4096]; // max rcon packet size
            int bytes;
            do {
                bytes = await stream.ReadAsync(buffer, 0, buffer.Length);
            } while(bytes == 0);

            byte[] result = new byte[bytes];
            Array.Copy(buffer, 0, result, 0, bytes);

            return new Packet(result);
        }

        /// <summary>
        /// Reads a packet from client
        /// </summary>
        /// <param name="client">Client to receive from</param>
        /// <returns>Result packet</returns>
        public Packet ReceivePacket(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[4096]; // max rcon packet size
            int bytes;
            do {
                bytes = stream.Read(buffer, 0, buffer.Length);
            } while(bytes == 0);

            byte[] result = new byte[bytes];
            Array.Copy(buffer, 0, result, 0, bytes);

            return new Packet(result);
        }

        /// <summary>
        /// Sends packet to client
        /// </summary>
        /// <param name="client">Client to send to</param>
        /// <param name="packet">Packet to send</param>
        public async Task SendPacketAsync(TcpClient client, Packet packet)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = packet.GetBytes();
            await stream.WriteAsync(buffer, 0, buffer.Length);
            await stream.FlushAsync();
        }

        /// <summary>
        /// Sends packet to client
        /// </summary>
        /// <param name="client">Client to send to</param>
        /// <param name="packet">Packet to send</param>
        public void SendPacket(TcpClient client, Packet packet)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = packet.GetBytes();
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }

        /// <summary>
        /// Process connected client while it's connected
        /// </summary>
        /// <param name="client"></param>
        private async void ProcessClient(TcpClient client)
        {
            while(client.Client.Connected && !m_Disposed) {
                Packet packet;
                try {
                    packet = await ReceivePacketAsync(client);
                }
                catch {
                    // we got forced disconnect
                    if(!client.Client.Connected) {
                        OnClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs { EndPoint = client.Client.LocalEndPoint });
                        return;
                    }
                    else {
                        throw;
                    }
                }

                // Trigger the first event
                OnClientPacketReceived?.Invoke(this, new ClientSentPacketEventArgs { Client = client, Packet = packet });

                // Trigger the second event and make response
                if(packet.Type == PacketType.SERVERDATA_EXECCOMMAND) {
                    string responseString = String.Empty;
                    if(OnClientCommandReceived != null) {
                        responseString = OnClientCommandReceived(this, new ClientSentCommandEventArgs { Client = client, Command = packet.Body });
                    }

                    Packet response = new Packet(PacketType.SERVERDATA_RESPONSE_VALUE, responseString, packet.Id);
                    await SendPacketAsync(client, response);
                    continue;
                }
            }
        }

        /// <summary>
        /// Start server asynchronously
        /// </summary>
        public async void StartAsync()
        {
            m_TcpListener.Start();
            while(!m_Disposed) {
                TcpClient client = await m_TcpListener.AcceptTcpClientAsync();
                OnClientConnected?.Invoke(this, new ClientConnectedEventArgs { Client = client });

                // Authenticate
                Packet authPacket = await ReceivePacketAsync(client);
                if((authPacket.Type == PacketType.SERVERDATA_AUTH) && authPacket.Body == Password) {
                    OnClientAuthenticated?.Invoke(this, new ClientAuthenticatedEventArgs { Client = client });

                    // send response
                    Packet authResponse = new Packet(PacketType.SERVERDATA_AUTH_RESPONSE, String.Empty, authPacket.Id);
                    await SendPacketAsync(client, authResponse);

                    // start processing
                    ProcessClient(client);
                }
                else {
                    Packet failResponse = new Packet(PacketType.SERVERDATA_AUTH_RESPONSE, String.Empty, -1);
                    await SendPacketAsync(client, failResponse);
                }
            }
        }

        /// <summary>
        /// Start server synchronously (Like StartAsync, but will lock current execution state)
        /// </summary>
        public void Start()
        {
            m_TcpListener.Start();
            while(!m_Disposed) {
                TcpClient client = m_TcpListener.AcceptTcpClient();
                OnClientConnected?.Invoke(this, new ClientConnectedEventArgs { Client = client });

                // Authenticate
                Packet authPacket = ReceivePacket(client);
                if((authPacket.Type == PacketType.SERVERDATA_AUTH) && authPacket.Body == Password) {
                    OnClientAuthenticated?.Invoke(this, new ClientAuthenticatedEventArgs { Client = client });

                    // send response
                    Packet authResponse = new Packet(PacketType.SERVERDATA_AUTH_RESPONSE, String.Empty, authPacket.Id);
                    SendPacket(client, authResponse);

                    // start processing
                    ProcessClient(client);
                }
                else {
                    Packet failResponse = new Packet(PacketType.SERVERDATA_AUTH_RESPONSE, String.Empty, -1);
                    SendPacket(client, failResponse);
                }
            }
        }
    }
}
