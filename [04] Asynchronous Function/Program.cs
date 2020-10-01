using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _04__Asynchronous_Function
{
    class Program
    {
        static  void Main(string[] args)
        {
            AsyncBasic asyncBasic = new AsyncBasic();
            asyncBasic.Show();

           

            Console.WriteLine("Main Thread End.");
            Console.ReadKey();
        }
    }

    // 2.异步编程基础
    public class AsyncBasic
    {
        public  async void Show()
        {
            Action action = () => Console.WriteLine("Action result");
            Action result = await DelayResult<Action>(action, TimeSpan.FromSeconds(5));
            result.BeginInvoke(null, null);


            //await ProcessTasksAsync();
            Console.WriteLine("Main" + Thread.CurrentThread.ManagedThreadId);
            await ResumeOnContextAsync();

            await ResumeWithoutContextAsync();

        }
        // 2.1 异步暂停一段时间 场景：简单的超时
        public async  Task<T> DelayResult<T>(T result, TimeSpan delay)
        {
            await Task.Delay(delay);
            return result;
        }
        public async  Task<string> DownloadStringWithRetries(string uri)
        {
            using (var client = new HttpClient())
            {
                TimeSpan nextDelay = TimeSpan.FromSeconds(1);
                // 重试策略: 重试的延迟时间会逐次增加 防止服务器被太多的重试阻塞
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        return await client.GetStringAsync(uri);
                    }
                    catch
                    {

                    }
                    await Task.Delay(nextDelay);
                    nextDelay += TimeSpan.FromSeconds(i);
                }
                // 最后重试一次 以便让调用知道出错信息
                return await client.GetStringAsync(uri);
            }
        }
        // 2.4 等待一组任务完成 场景：执行几个任务 等待它们全部完成
        public async Task AwaitAllTask()
        {
            Task task1 = Task.FromResult(3);
            Task task2 = Task.FromResult(5);
            Task task3 = Task.FromResult(7);
            await Task.WhenAll(task1, task2, task3);
        }
        public async Task<string> DownAllAsync(IEnumerable<string> urls)
        {
            var httpClient = new HttpClient();
            // 创建任务组 此时任务均未启动
            var downloads = urls.Select(url => httpClient.GetStringAsync(url));

            // 所有任务同步开始
            Task<string>[] downloadTasks = downloads.ToArray();

            // 所有任务已经开始执行  

            // 异步等待所有任务完成 并返回一个 结果任务 completedTask 捕获的异常存储在 completedTask 中
            Task<string[]> completedTask =  Task.WhenAll(downloadTasks);
            string[] htmlPages = null;
            try
            {
                // Task 对象被Await调用 异常再次被 引发
                htmlPages =  await completedTask;
            }
            catch
            {
                // 捕获任务组异常
                AggregateException ae = completedTask.Exception;
                throw ae;
            }
         
            return string.Concat(htmlPages);
        }
        // 2.5 等待任意一个任务完成 场景：同时向多个Web服务器请求股票价格 只获取第一个响应的
        public async Task<int> FirstRespondingUrlAsync(string urlA,string urlB)
        {
            var httpClient = new HttpClient();

            // 并发地开始两个下载任务
            Task<byte[]> downloadTaskA = httpClient.GetByteArrayAsync(urlA);
            Task<byte[]> downloadTaskB = httpClient.GetByteArrayAsync(urlB);

            // 等待任意一个任务完成
            Task<byte[]> completedTask = await Task.WhenAny(downloadTaskA, downloadTaskB);

            // 返回从URL得到的长度
            byte[] data = await completedTask;
            return data.Length;
        }
        // 2.6 任务完成时的处理 场景：任务一完成就进行处理 不需要等待其它任务 而不是等待所有任务完成才进行处理
        private async Task<int> DelayAndReturnAsync(int val)
        {
            await Task.Delay(TimeSpan.FromSeconds(val));
            return val;
        }
        public async Task ProcessTasksAsync()
        {
            // 创建任务队列 
            Task<int> taskA = DelayAndReturnAsync(2);
            Task<int> taskB = DelayAndReturnAsync(15);
            Task<int> taskC = DelayAndReturnAsync(1);
            var tasks = new[] { taskA, taskB, taskC };

            // 执行任务队列 期望输出 1 2 15

            // 实际输出 2 15 1
            //foreach (var task in tasks)
            //{
            //    var result = await task;
            //    Console.WriteLine(result);
            //}

            // 实际输出 2 15 1
            //int[] results = await Task.WhenAll(tasks);
            //results.ToList().ForEach(s => Console.WriteLine(s));

            // 实际输出 1 2 15
           var processingTasks = tasks.Select(async t => {
                var result = await t;
                Console.WriteLine(result);  // 相当于任务t 的延续
            });
            await Task.WhenAll(processingTasks.ToArray());
            
        }
        // 2.7 避免上下文切换
        // 场景：UI线程，拥有大量的Async方法，这些Async方法会在被Await调用后恢复运行时，
        //      切换到UI上下文中，引起性能上的问题
        //      Async方法需要上下文：处理UI元素或ASP.NET 请求/响应
        //      Async方法摆脱上下文：执行后台指令
        //      Async方法部分需要上下，部分摆脱上下文：拆分更多的Async方法，组织不同层次代码
        public async Task ResumeOnContextAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
         
            // Async 方法在同一个上下文中恢复运行
            //Console.WriteLine("ResumeOnContextAsync" + Thread.CurrentThread.ManagedThreadId);
        }
        public async Task ResumeWithoutContextAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
         
            // Async 方法恢复运行时，会丢弃上下文
            //Console.WriteLine("ResumeWithoutContextAsync" + Thread.CurrentThread.ManagedThreadId);
        }
    }
}
