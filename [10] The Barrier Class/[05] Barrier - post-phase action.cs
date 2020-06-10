using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _10__The_Barrier_Class
{
 public   class _05__Barrier___post_phase_action
    {
		// 后续操作，调用 SignalAndWait n 次之后，所有线程释放之前执行
		static Barrier _barrier = new Barrier(3, barrier => Console.WriteLine());

	 public	static  void Show()
		{
			new Thread(Speak).Start();
			new Thread(Speak).Start();
			new Thread(Speak).Start();
		}

		static void Speak()
		{
			for (int i = 0; i < 5; i++)
			{
				Console.Write(i + " ");
				_barrier.SignalAndWait();
			}
		}
	}
}
