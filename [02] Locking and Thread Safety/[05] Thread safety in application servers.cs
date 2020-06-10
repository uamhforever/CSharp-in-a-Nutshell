using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _02__Locking_and_Thread_Safety
{
    /// <summary>
    /// Web应用服务器的安全性
    /// </summary>
    public class _05__Thread_safety_in_application_servers
    {
        /*
         应用服务器为每个客户端的每一个请求创建一个独立的对象实例，
        减少线程间交互，而交互通常发生在静态字段上
             
             
             
             */

        public void Show()
        {
            new Thread(() => UserCache.GetUser(1).Dump()).Start();
            new Thread(() => UserCache.GetUser(1).Dump()).Start();
            new Thread(() => UserCache.GetUser(1).Dump()).Start();
        }

        static class UserCache
        {
            static Dictionary<int, User> _users = new Dictionary<int, User>();

            internal static User GetUser(int id)
            {
                User u = null;

                lock (_users)
                    if (_users.TryGetValue(id, out u))  // 首先尝试去缓存字典中取数据
                        return u;

                u = RetrieveUser(id);           // Method to retrieve from database;
                lock (_users) _users[id] = u;
                return u;
            }

            static User RetrieveUser(int id)
            {
                return new User { ID = id };
            }
        }

        class User { public int ID; }
    }
}
