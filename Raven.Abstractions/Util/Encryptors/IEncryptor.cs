using System;
using System.IO;
using System.Security.Cryptography;

namespace Raven35.Abstractions.Util.Encryptors
{
    public interface IEncryptor
    {
        IHashEncryptor Hash { get; }

        IHashEncryptor CreateHash();

        ISymmetricalEncryptor CreateSymmetrical();

        ISymmetricalEncryptor CreateSymmetrical(int keySize);

        IAsymmetricalEncryptor CreateAsymmetrical();

        IAsymmetricalEncryptor CreateAsymmetrical(byte[] exponent, byte[] modulus);

        IAsymmetricalEncryptor CreateAsymmetrical(int keySize);
    }

    public interface IHashEncryptor : IDisposable
    {
        int StorageHashSize { get; }

        byte[] ComputeForOAuth(byte[] bytes);

        byte[] Compute16(byte[] bytes);

        byte[] Compute16(Stream stream);

        byte[] Compute16(byte[] bytes, int offset, int size);

        byte[] Compute20(byte[] bytes);

        byte[] Compute20(byte[] bytes, int offset, int size);

#if !DNXCORE50
        byte[] TransformFinalBlock();

        void TransformBlock(byte[] bytes, int offset, int length);
#endif
    }

    public interface ISymmetricalEncryptor : IDisposable
    {
        byte[] Key { get; set; }

        byte[] IV { get; set; }

        int KeySize { get; set; }

        void GenerateKey();

        void GenerateIV();

        ICryptoTransform CreateEncryptor();

        ICryptoTransform CreateDecryptor();

        ICryptoTransform CreateDecryptor(byte[] key, byte[] iv);
    }

    public interface IAsymmetricalEncryptor : IDisposable
    {
        int KeySize { get; set; }

        void ImportParameters(byte[] exponent, byte[] modulus);

        byte[] Encrypt(byte[] bytes, bool fOAEP);

        void ImportCspBlob(byte[] keyBlob);

        byte[] ExportCspBlob(bool includePrivateParameters);

        byte[] SignHash(byte[] hash, string str);

        bool VerifyHash(byte[] hash, string str, byte[] signature);

        byte[] Decrypt(byte[] bytes, bool fOAEP);

#if !DNXCORE50
        void FromXmlString(string xml);
#endif

        AsymmetricAlgorithm Algorithm { get; }

        void ImportParameters(RSAParameters parameters);

        RSAParameters ExportParameters(bool includePrivateParameters);
    }

}
