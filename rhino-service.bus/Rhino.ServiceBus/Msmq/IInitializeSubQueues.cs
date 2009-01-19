namespace Rhino.ServiceBus.Msmq
{
	using System;

	/// <summary>
	/// Provides an interface for the registering creating sub queues
	/// </summary>
	/// <remarks>
	/// Since <see cref="FlatQueueStrategy"/> uses sibling queues in place
	/// of MSMQ 4.0 subqueues we need a way to create these when the app
	/// starts up.
	/// </remarks>
	public interface IInitializeSubQueues
	{
		/// <summary>
		/// Creates subqueues for an endpoint (#timeouts, #errors, #discarded)
		/// </summary>
		/// <param name="endpoint">Used to figure out the path of the subqueue.
		/// The subqueue will append #something to your queue path</param>
		/// <param name="transactional">If the queues should be created transactional
		/// or not.
		/// </param>
		void InitializeSubQueues(Uri endpoint, bool transactional);
	}

	public class FlatQueueSubQueueInitializer : IInitializeSubQueues
	{
		public void InitializeSubQueues(Uri endpoint, bool transactional)
		{
			FlatQueueStrategy.InitializeSubQueues(endpoint,transactional);
		}
	}
}