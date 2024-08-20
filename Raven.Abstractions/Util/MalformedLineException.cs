using System;

namespace Raven35.Abstractions.Util
{
    [Serializable]
    public class MalformedLineException : Exception
    {
        public MalformedLineException()
        {
        }

        public MalformedLineException(string message) : base(message)
        {
        }

        public MalformedLineException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

