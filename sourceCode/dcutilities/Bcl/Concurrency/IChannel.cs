namespace DigitallyCreated.Utilities.Bcl.Concurrency
{
	/// <summary>
	/// A Channel, otherwise known as a concurrent queue, is a concurrency utility that allows the
	/// safe enqueuing (putting) and dequeuing (taking) of data by multiple threads. 
	/// Channels are often used as a mechanism to safely exchange data between threads.
	/// </summary>
	/// <typeparam name="T">The type of object the channel will contain</typeparam>
	public interface IChannel<T>
	{
		/// <summary>
		/// Put (enqueue) data on the channel
		/// </summary>
		/// <param name="data">The data to put on the channel</param>
		void Put(T data);

		/// <summary>
		/// Take (dequeue) data from the channel
		/// </summary>
		/// <returns>The data taken from the channel</returns>
		T Take();
	}
}