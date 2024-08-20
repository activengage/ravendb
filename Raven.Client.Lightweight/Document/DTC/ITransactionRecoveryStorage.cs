#if !DNXCORE50
// -----------------------------------------------------------------------
//  <copyright file="ITransactionRecoveryStorage.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Raven35.Client.Document.DTC
{
    public interface ITransactionRecoveryStorage
    {
        ITransactionRecoveryStorageContext Create();
    }
}
#endif