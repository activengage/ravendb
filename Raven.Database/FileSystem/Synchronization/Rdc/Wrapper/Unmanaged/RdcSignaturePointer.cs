using System.Runtime.InteropServices;

namespace Raven35.Database.FileSystem.Synchronization.Rdc.Wrapper.Unmanaged
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct RdcSignaturePointer
    {
        public uint Size;
        public uint Used;
        [MarshalAs(UnmanagedType.Struct)]
        public RdcSignature Data;
    }
}
