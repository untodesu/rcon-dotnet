using System;
using System.Runtime.Serialization;

namespace Rcon.Exceptions
{
    /// <summary>
    /// An exception being thrown if client is trying to send a packet unauthenticated
    /// </summary>
    class NotAuthenticatedException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public NotAuthenticatedException()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception string</param>
        public NotAuthenticatedException(string message) : base(message)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception string</param>
        /// <param name="innerException">Exception happened inside</param>
        public NotAuthenticatedException(string message, Exception innerException) : base(message, innerException)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected NotAuthenticatedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
