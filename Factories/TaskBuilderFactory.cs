using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Data.Notify;
using log4net;

namespace Infrastructure.Data.Factories
{

    public static class TaskBuilderFactory
    {

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static class Message
        {
            public static string Success = "SUCCESS";
            public static string Fail = "FAIL";
            public static string Slow = "SLOW";
        }

        public delegate void Func();

        public static void AddTasks(List<Task> tasks, List<Func> taskList)
        {

            foreach (var func in taskList)
            {
                NewTask(tasks, func);
            }

        }

        public static void NewTask(List<Task> tasks, Func f, Notifications messages, string message)
        {

            var factory = new TaskFactory();

            var task = factory.StartNew(
                () => {
                    messages.Add(new Notification
                    {
                        Id = "Start Task",
                        Type = NotificationType.Information,
                        UserMessage = message,
                        Source = "TaskBuilderFactory"
                    });

                    f();

                    messages.Add(new Notification
                    {
                        Id = "End Task",
                        Type = NotificationType.Information,
                        UserMessage = message,
                        Source = "TaskBuilderFactory"
                    });
                });

            tasks.Add(task);

        }

        public static void NewTask(List<Task> tasks, Func f)
        {

            var factory = new TaskFactory();
            var task = factory.StartNew(() => { f(); });

            tasks.Add(task);

        }

        public static string NewTask(Func f, double timeoutSec)
        {

            string msg = Message.Success;

            #region with threading cancellation
            var isTimeOut = false;
            var wait = new ManualResetEvent(false);

            var work = new Thread(() =>
            {
                //set the function
                f();

                wait.Set();
            });

            try
            {
                //start to run
                work.Start();

                //setup 
                var signal = wait.WaitOne((int)timeoutSec * 1000);

                //after x secs if didn't receive the done signal then should stop the work
                if (!signal)
                {
                    work.Abort();
                    isTimeOut = true;
                }

                if (isTimeOut)
                    msg = Message.Slow;
            }
            catch (Exception ex)
            {
                work.Abort();
                msg = Message.Fail;
            }

            return msg;
            #endregion

        }

    }

}