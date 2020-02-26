using System;

namespace Rcon.Types
{
    /// <summary>
    /// The type of RCON Packet.
    /// The packet type field is a 32-bit little endian integer, which indicates the purpose of the packet.
    /// Its value will always be either 0, 2, or 3.
    /// </summary>
    public enum PacketType : Int32
    {
        /// <summary>
        /// Typically, the first packet sent by the client will be a SERVERDATA_AUTH packet,
        /// which is used to authenticate the connection with the server.
        /// </summary>
        SERVERDATA_AUTH = 3,

        /// <summary>
        /// This packet type represents a command issued to the server by a client.
        /// This can be a ConCommand such as mp_switchteams or changelevel, a command to set a cvar such as sv_cheats 1, or a command to fetch the value of a cvar, such as sv_cheats.
        /// The response will vary depending on the command issued.
        /// </summary>
        SERVERDATA_EXECCOMMAND = 2,

        /// <summary>
        /// This packet is a notification of the connection's current auth status.
        /// When the server receives an auth request, it will respond with an empty SERVERDATA_RESPONSE_VALUE, followed immediately by a SERVERDATA_AUTH_RESPONSE indicating whether authentication succeeded or failed.
        /// Note that the status code is returned in the packet id field, so when pairing the response with the original auth request, you may need to look at the packet id of the preceeding SERVERDATA_RESPONSE_VALUE.
        /// </summary>
        SERVERDATA_AUTH_RESPONSE = 2,

        /// <summary>
        /// A SERVERDATA_RESPONSE_VALUE packet is the response to a SERVERDATA_EXECCOMMAND request.
        /// Note that particularly long responses may be sent in multiple SERVERDATA_RESPONSE_VALUE packets.
        /// </summary>
        SERVERDATA_RESPONSE_VALUE = 0,

    }
}
