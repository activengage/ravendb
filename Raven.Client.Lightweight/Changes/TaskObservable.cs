using System;
using System.Threading.Tasks;

namespace Raven35.Client.Changes
{
    public interface IObservableWithTask<T> : IObservable<T>
    {
        Task<IObservable<T>> Task { get; }
    }
}
