using System;
using System.Text;

namespace Rcon.Types
{
    /// <summary>
    /// Both requests and responses are sent as TCP packets.
    /// Their payload follows the following basic structure.
    /// </summary>
    public class Packet
    {
        /// <summary>
        /// The packet id field is a 32-bit little endian integer chosen by the client for each request.
        /// It may be set to any positive integer.
        /// When the server responds to the request, the response packet will have the same packet id as the original request (unless it is a failed SERVERDATA_AUTH_RESPONSE packet - see below.) It need not be unique, but if a unique packet id is assigned, it can be used to match incoming responses to their corresponding requests.
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// The packet type field is a 32-bit little endian integer, which indicates the purpose of the packet.
        /// Its value will always be either 0, 2, or 3
        /// </summary>
        public PacketType Type { get; set; }

        /// <summary>
        /// The packet body field is a null-terminated string encoded in ASCII (i.e. ASCIIZ) (C# specific - writes zero manually)
        /// Depending on the packet type, it may contain either the RCON password for the server, the command to be executed, or the server's response to a request.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The packet size field is a 32-bit little endian integer, representing the length of the request in bytes.
        /// Note that the packet size field itself is not included when determining the size of the packet, so the value of this field is always 4 less than the packet's actual length.
        /// The minimum possible value for packet size is 10.
        /// </summary>
        public Int32 Size { get { return Body.Length + 10; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The type of packet</param>
        /// <param name="body">Packet body (payload)</param>
        /// <param name="id">Packet ID</param>
        public Packet(PacketType type, string body, Int32 id)
        {
            Id = id;
            Type = type;
            Body = body ?? String.Empty;
        }

        /// <summary>
        /// Constructor.
        /// Creates new Packet class with auto-generated ID
        /// </summary>
        /// <param name="type">The type of packet</param>
        /// <param name="body">Packet body (payload)</param>
        public Packet(PacketType type, string body)
        {
            Id = Environment.TickCount;
            Type = type;
            Body = body ?? String.Empty;
        }

        /// <summary>
        /// Constructor.
        /// Creates new Packet class instance from byte buffer.
        /// </summary>
        /// <remarks>Buffer length have to be greater or equal to 10</remarks>
        /// <param name="buffer"></param>
        public Packet(byte[] buffer)
        {
            if(buffer.Length < 10) {
                throw new ArgumentException(String.Format("Buffer is too short ({0} < 10)", buffer.Length));
            }

            Int32 length = buffer.ConvertToInt32(0);
            Id = buffer.ConvertToInt32(4);
            Type = (PacketType)buffer.ConvertToInt32(8);
            Body = Encoding.ASCII.GetString(buffer, 12, (length - 10));

            // TODO: Probably add assertion (Size == length)
        }

        /// <summary>
        /// Converts class data to byte buffer
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            byte[] buffer = new byte[Size + 4];

            // Write data
            Size.ConvertToLEBuffer().CopyTo(buffer, 0);
            Id.ConvertToLEBuffer().CopyTo(buffer, 4);
            ((Int32)Type).ConvertToLEBuffer().CopyTo(buffer, 8);
            Encoding.ASCII.GetBytes(Body).CopyTo(buffer, 12);

            return buffer;
        }
    }
}
