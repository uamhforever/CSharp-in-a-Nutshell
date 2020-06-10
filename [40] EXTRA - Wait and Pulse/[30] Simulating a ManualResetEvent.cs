using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _40__EXTRA___Wait_and_Pulse
{
  public  class _30__Simulating_a_ManualResetEvent
    {
		 void Show()
		{
			new Thread(() => { Thread.Sleep(2000); Set(); }).Start();
			Console.WriteLine("Waiting...");
			WaitOne();
			Console.WriteLine("Signaled");
		}

		readonly object _locker = new object();
		bool _signal;

		void WaitOne()
		{
			lock (_locker)
			{
				while (!_signal) Monitor.Wait(_locker);
			}
		}

		void Set()
		{
			lock (_locker) { _signal = true; Monitor.PulseAll(_locker); }
		}

		void Reset() { lock (_locker) _signal = false; }
	}
}
