using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Errors
{
    [System.Serializable]
    public class RssException : Exception
    {
        public RssException() { }

        public RssException(string message) : base(message) { }

        public RssException(string message, Exception inner) : base(message, inner) { }

        public RssException(int code, Exception inner) : base(ErrorCodes.GetMessage(code), inner) { }

        protected RssException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
