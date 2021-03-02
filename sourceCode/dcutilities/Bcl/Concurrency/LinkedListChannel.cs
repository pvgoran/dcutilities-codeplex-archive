using System.Threading;


namespace DigitallyCreated.Utilities.Bcl.Concurrency
{
	/// <summary>
	/// A Channel, otherwise known as a concurrent queue, is a concurrency utility that allows the
	/// safe enqueuing (putting) and dequeuing (taking) of data by multiple threads. 
	/// Channels are often used as a mechanism to safely exchange data between threads. This 
	/// channel is implemented using an internal linked list and uses lock-splitting to ensure
	/// putters and takers can operate independently.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class supports the safe use of interrupts. An interrupt that occurs within a method of
	/// this class results in the action performed by the method not occurring.
	/// </para>
	/// <para>
	/// This class does not support the safe use of <see cref="Thread.Abort()"/>. Its use may leave
	/// this class in an undefined state.
	/// </para>
	/// </remarks>
	/// <typeparam name="T">The type of object the channel will contain</typeparam>
	/// <seealso cref="IChannel{T}"/>
	public class LinkedListChannel<T> : IChannel<T>
	{
		private readonly object _PutLock;
		private readonly object _TakeLock;
		private Node _BlankFirst;
		private Node _Last;


		/// <summary>
		/// Constructor, creates an empty <see cref="LinkedListChannel{T}"/>
		/// </summary>
		public LinkedListChannel()
		{
			_PutLock = new object();
			_TakeLock = new object();
			_BlankFirst = _Last = new Node { Data = default(T) };
		}


		/// <summary>
		/// Put (enqueue) data on the channel
		/// </summary>
		/// <param name="data">The data to put on the channel</param>
		/// <exception cref="ThreadInterruptedException">
		/// If the calling thread was interrupted while trying to acquire one of the internal 
		/// locks.
		/// </exception>
		public void Put(T data)
		{
			Node node = new Node { Data = data };

			lock (_PutLock)
			{
				//Lock the node we're operating on, in case we're trying to operate on
				//the same node as a taker
				lock (_Last)
				{
					//If we're about to put into an empty channel, we should pulse
					//any waiting threads to let them know they can take new data
					//---
					//It's okay to touch _BlankFirst here because if _Last and
					//_BlankFirst are equal, then we've locked _BlankFirst, and if
					//they're not, we are only reading stale data (reference read is atomic)
					//which is fine, since if the current Taker is about to switch out
					//_BlankFirst for _Last, the next Taker won't be able to read our
					//changes to _Last (which will now be _BlankFirst) until we release 
					//the lock on it
					if (_Last == _BlankFirst)
						Monitor.Pulse(_Last);

					_Last.Next = node;
					_Last = node;
				}
			}
		}


		/// <summary>
		/// Take (dequeue) data from the channel
		/// </summary>
		/// <returns>The data taken from the channel</returns>
		/// <exception cref="ThreadInterruptedException">
		/// If the calling thread was interrupted while trying to acquire one of the internal 
		/// locks.
		/// </exception>
		public T Take()
		{
			lock (_TakeLock)
			{
				//Lock the node we're operating on, in case we're trying to operate on
				//the same node as a putter
				lock (_BlankFirst)
				{
					//We're on the blank first node, so if there is no data on the channel
					//(no next node) wait until we're told there is (we are pulsed)
					//Don't worry about needing to pulse if interrupted, since we can be 
					//the only one waiting since we hold _TakeLock as we wait
					while (_BlankFirst.Next == null)
						Monitor.Wait(_BlankFirst);

					Node first = _BlankFirst.Next;
					T data = first.Data;

					//Release the data for GC, so it's not being held around when this node becomes the blank one
					first.Data = default(T); 

					//Shift this node into the blank node's position
					_BlankFirst = first;

					return data;
				}
			}
		}


		/// <summary>
		/// Internal node class for the linked list
		/// </summary>
		private class Node
		{
			public T Data;
			public Node Next;
		}
	}
}