using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _02__Locking_and_Thread_Safety
{
    /// <summary>
    /// 信号量(非排它锁)
    /// </summary>
    public class _10__Semaphore
    {
        /*
         *  信号量可用于限制并发数，防止太多的线程同时执行特定的代码
            如同俱乐部，有特定的容量，还有门卫保护，一旦满员之后，
         不允许他人进入，人们只能在外面排队，当有人离开时，才准许
         另外一个人进入
         */

        static SemaphoreSlim _sem = new SemaphoreSlim(3);    // Capacity of 3

       public static void Show()
        {
            // 5个线程试图进入聚乐部，但最多只允许3个线程同时进入
            for (int i = 1; i <= 5; i++) new Thread(Enter).Start(i);
        }

        static void Enter(object id)
        {
            Console.WriteLine(id + " wants to enter");
            _sem.Wait();
            Console.WriteLine(id + " is in!");           // Only three threads
            Thread.Sleep(1000 * (int)id);               // can be here at
            Console.WriteLine(id + " is leaving");       // a time.
            _sem.Release();
        }
    }
}
