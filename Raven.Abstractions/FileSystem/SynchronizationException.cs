using System;
using System.Runtime.Serialization;
using Raven35.Imports.Newtonsoft.Json;

namespace Raven35.Abstractions.FileSystem
{
    [Serializable]
    public class SynchronizationException : Exception
    {
        public SynchronizationException()
        {
        }

        public SynchronizationException(string message)
            : base(message)
        { }

        public SynchronizationException(string message, Exception inner)
            : base(message, inner)
        { }

#if !DNXCORE50
        protected SynchronizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
#endif
    }
}
