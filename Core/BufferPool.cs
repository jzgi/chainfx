using System.Collections.Concurrent;
using System.Threading;

namespace Greatbone.Core
{
	///
	/// A pool that leases and returns back byte buffers.
	///
	public class BufferPool
	{
		private readonly ConcurrentQueue<byte[]> _queue;

		private readonly int _bufsize;

		private readonly int _capacity;

		private int _count;

		public BufferPool(int bufsize, int capacity)
		{
			_bufsize = bufsize;
			_queue = new ConcurrentQueue<byte[]>();
		}

		public byte[] Lease()
		{
			byte[] result;
			if (!_queue.TryDequeue(out result))
			{
				result = new byte[_bufsize];
				_queue.Enqueue(result);
			}
			return result;
		}

		public void Return(byte[] buf)
		{
			if (Interlocked.Increment(ref _count) < _capacity)
			{
				_queue.Enqueue(buf);
			}
		}
	}
}