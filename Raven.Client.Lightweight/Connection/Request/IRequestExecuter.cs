using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

using Raven.Abstractions.Replication;
using Raven.Client.Connection.Async;

namespace Raven.Client.Connection.Request
{
	public interface IRequestExecuter
	{
		ReplicationDestination[] FailoverServers { get; set; }

		Task<T> ExecuteOperationAsync<T>(AsyncServerClient serverClient, string method, int currentRequest, Func<OperationMetadata, Task<T>> operation, CancellationToken token);

		Task UpdateReplicationInformationIfNeeded(AsyncServerClient serverClient, bool force = false);

		IDisposable ForceReadFromMaster();

		event EventHandler<FailoverStatusChangedEventArgs> FailoverStatusChanged;

		void AddHeaders(HttpJsonRequest httpJsonRequest, AsyncServerClient serverClient, string currentUrl);
	}
}