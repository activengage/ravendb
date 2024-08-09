#if !DNXCORE50
namespace Raven35.Client.Document.DTC
{
    public class IsolatedStorageTransactionRecoveryStorage : ITransactionRecoveryStorage
    {
        public ITransactionRecoveryStorageContext Create()
        {
            return new IsolatedStorageTransactionRecoveryContext();
        }
    }
}
#endif