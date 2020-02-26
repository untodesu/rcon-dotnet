using System;
using System.Runtime.Serialization;

namespace Rcon.Exceptions
{
    /// <summary>
    /// An exception being thrown if client is trying to authenticate disconnected
    /// </summary>
    class NotConnectedException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public NotConnectedException()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception string</param>
        public NotConnectedException(string message) : base(message)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception string</param>
        /// <param name="innerException">Exception happened inside</param>
        public NotConnectedException(string message, Exception innerException) : base(message, innerException)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected NotConnectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
