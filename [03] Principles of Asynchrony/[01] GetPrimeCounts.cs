using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _03__Principles_of_Asynchrony
{
   public class _01__GetPrimeCounts
    {
        public static void Show()
        {
            // sync 
            {
               // DisplayPrimeCounts();
            }
            // task  粗粒度并发
            {
               // Task.Run(() => DisplayPrimeCounts());
            }
            // awaiter  细粒度并发
            {
                DisplayPrimeCountsWithAwaiter();        // 异步 乱序执行 
                DisplayPrimeCountsWithTaskSource();     // 异步 顺序执行
            }
            // sync 细粒度并发
            {
                DisplayPrimeCountsAsync();              // 异步 顺序执行
            }

        }

        public static void DisplayPrimeCounts()
        {
            for (int i = 0; i < 10; i++)
                Console.WriteLine(GetPrimesCount(i * 1000000 + 2, 1000000) +
                 " primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1));
        }

       public static int GetPrimesCount(int start, int count)
        {       
            int primesCount = ParallelEnumerable.Range(start, count).Count(n =>
            {
                return Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i =>
                {
                    return n % i > 0;
                });
            });
            return primesCount;

        }

        public static void DisplayPrimeCountsWithAwaiter()
        {
            // 循环10次迭代 非阻塞 并行执行
            for (int i = 0; i < 10; i++)
            {
               var awaiter =  GetPrimesCountAsync(i * 1000000 + 2, 1000000).GetAwaiter();
                awaiter.OnCompleted(() =>
                   Console.WriteLine(awaiter.GetResult() + " primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1)));
            }
            Console.WriteLine("Done!");  // "Done!" 会提前输出
               
        }

        public static Task<int> GetPrimesCountAsync(int start,int count)
        {
            return Task.Run(() =>
            {
                int primesCount = ParallelEnumerable.Range(start, count).Count(n =>
                {
                    return Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i =>
                    {
                        return n % i > 0;
                    });
                });
                return primesCount;
            });
        }

        public static Task DisplayPrimeCountsWithTaskSource()
        {
            var machine = new PrimesStateMachine();
            machine.DisplayPrimeCountsFrom(0);
            return machine.Task;
        }

        public static async Task DisplayPrimeCountsAsync()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(await GetPrimesCountAsync(i * 1000000 + 2, 1000000) +
                  " primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1));
            }
            Console.WriteLine("Done!");  
        }

       

    }

   public class PrimesStateMachine        // Even more awkward!!
    {
        TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();
        public Task Task { get { return _tcs.Task; } }

        public void DisplayPrimeCountsFrom(int i)
        {
            var awaiter = GetPrimesCountAsync(i * 1000000 + 2, 1000000).GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                Console.WriteLine(awaiter.GetResult()+" primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1));
                if (i++ < 10) DisplayPrimeCountsFrom(i);
                else { Console.WriteLine("Done!"); _tcs.SetResult(null); }
            });
        }

        Task<int> GetPrimesCountAsync(int start, int count)
        {
            return Task.Run(() =>
               ParallelEnumerable.Range(start, count).Count(n =>
                 Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));
        }
    }
}
