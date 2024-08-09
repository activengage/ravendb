using System.Security.Cryptography;
using Raven35.Abstractions.Util.Encryptors;

namespace Raven35.Database.FileSystem.Util
{
    public class HashKey
    {
        public HashKey(byte[] buffer, int size)
        {
            Strong = Encryptor.Current.Hash.Compute20(buffer, 0, size);
            Weak = new RabinKarpHasher(size).Init(buffer, 0, size);
        }

        public HashKey()
        {

        }

        public byte[] Strong { get; set; }
        public int Weak { get; set; }
    }
}
