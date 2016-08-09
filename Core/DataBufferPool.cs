using System.Collections.Concurrent;
using System.Threading;

namespace Greatbone.Core
{
	///
	/// A pool that leases and returns back byte buffers. For json/bson data only.
	///
	/// increased by a 4-times
	///
	public class DataBufferPool
	{
		private readonly Queue[] _queues;

		private int _count;

		public DataBufferPool(int[] specs, int threads)
		{
			int len = specs.Length;



			_queues = new Queue[len];
			for (int i = 0; i < len; i++)
			{

				len / 2 - i

				_queues[i] = new Queue(specs[i], thresholds[i]);
			}
		}

		public byte[] Lease(int group)
		{
			byte[] result;
			if (!_queues[group].TryDequeue(out result))
			{
				result = new byte[_bufsize];
				_queues[group].Enqueue(result);
			}
			return result;
		}

		public void Return(byte[] buf)
		{
			if (Interlocked.Increment(ref _count) < _capacity)
			{
				_queues.Enqueue(buf);
			}
		}

		public byte[] Supercede(byte[] old)
		{
			return null;
		}

		internal class Queue
		{
			private int _size;

			// bytes
			private int _bufspec;


			private readonly ConcurrentQueue<byte[]> _queue;

			internal Queue(int buflen, int size)
			{
				_bufspec = buflen;
				_size = size;
				_queue=new ConcurrentQueue<byte[]>();
			}
		}

		static void sdf()
		{
		}
	}
}