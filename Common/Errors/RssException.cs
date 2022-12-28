using System;
using System.Runtime.Serialization;

namespace Common.Errors
{
    [Serializable]
    public class RssException : Exception
    {
        public RssException()
        {
        }

        public RssException(string message) : base(message)
        {
        }

        public RssException(string message, Exception inner) : base(message, inner)
        {
        }

        public RssException(int code, Exception inner) : base(ErrorCodes.GetMessage(code), inner)
        {
        }

        protected RssException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}