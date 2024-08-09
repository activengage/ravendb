using System;

namespace Raven35.Abstractions.Util.Streams
{
    public interface IBufferPool : IDisposable
    {
        byte[] TakeBuffer(int size);
        void ReturnBuffer(byte[] buffer);
    }
}
