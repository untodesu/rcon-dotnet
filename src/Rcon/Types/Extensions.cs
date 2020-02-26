using System;

namespace Rcon.Types
{
    /// <summary>
    /// An extension class for buffers and networking streams.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Converts 32-bit value to low-endian byte buffer
        /// </summary>
        /// <param name="value">Source value</param>
        /// <returns>Result buffer</returns>
        public static byte[] ConvertToLEBuffer(this Int32 value)
        {
            byte[] data = BitConverter.GetBytes(value);
            if(!BitConverter.IsLittleEndian) {
                Array.Reverse(data);
            }

            return data;
        }

        /// <summary>
        /// Converts low-endian byte buffer to 32-bit value
        /// </summary>
        /// <param name="buffer">Source buffer, this</param>
        /// <param name="pos">Offset inside the buffer</param>
        /// <returns>Result value</returns>
        public static Int32 ConvertToInt32(this byte[] buffer, int pos)
        {
            // Create new copy of buffer itself
            // We cannot just reverse source array
            byte[] copy = new byte[buffer.Length];
            buffer.CopyTo(copy, 0);
            if(!BitConverter.IsLittleEndian) {
                Array.Reverse(copy);
            }

            return BitConverter.ToInt32(copy, pos);
        }
    }
}
