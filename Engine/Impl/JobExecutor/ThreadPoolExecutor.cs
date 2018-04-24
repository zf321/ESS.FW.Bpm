using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    public class ThreadPoolExecutor
    {
        private static readonly JobExecutorLogger Log = ProcessEngineLogger.JobExecutorLogger;
        /// <summary>
        /// 并发运行的任务数
        /// </summary>
        private int corePoolSize;
        /// <summary>
        /// 最大任务数
        /// </summary>
        private int maxPoolSize;
        private long keepAliveTime;
        /// <summary>
        /// 队列限制长度，null为不限制
        /// </summary>
        private int? queueSize;
        /// <summary>
        /// 添加后是否自动Start
        /// </summary>
        private bool isAutoRun;
        private static bool isActivity;
        /// <summary>
        /// 缓冲任务队列
        /// </summary>
        private Queue<System.Threading.Tasks.Task> workQueue;
        public System.Threading.CancellationTokenSource cancelToken = new System.Threading.CancellationTokenSource();
        private TaskFactory taskFactory;
        //private List<System.Threading.Tasks.Task> tasks = new List<System.Threading.Tasks.Task>();
        private SortedList<DateTime, System.Threading.Tasks.Task> taskList = new SortedList<DateTime, System.Threading.Tasks.Task>();
        protected static readonly object taskList_LOCK = new object();
        private System.Threading.Tasks.Task obTask;
        //public ThreadPoolExecutor(int corePoolSize, int maxPoolSize, long keepAliveTime, int? queueSize, bool isAutoRun)
        //{
        //    //this.corePoolSize = corePoolSize;
        //    //this.maxPoolSize = maxPoolSize;
        //    //this.keepAliveTime = keepAliveTime;
        //    //this.queueSize = queueSize;
        //    //workQueue = new Queue<System.Threading.Tasks.Task>();
        //    taskFactory = new TaskFactory(cancelToken.Token);
        //}
        public ThreadPoolExecutor()
        {
            obTask = ObTaskInit();
        }
        /// <summary>
        /// 监控线程初始化
        /// </summary>
        private System.Threading.Tasks.Task ObTaskInit()
        {
            System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(() =>
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(50);
                    if (isActivity && taskList.Count > 0)
                    {
                        lock (taskList_LOCK)
                        {
                            if (taskList.First().Key <= DateTime.Now)
                            {
                                var toRun = taskList.First();
                                taskList.Remove(toRun.Key);
                                toRun.Value.Start();
                            }
                        }
                    }
                }
            });
            Log.LogInfo("监控线程", "初始化完成...");
            task.Start();
            return task;
        }
        public void Start()
        {
            isActivity = true;
            if (obTask.Status != TaskStatus.Running)
            {
                ObTaskInit();
            }
            Log.LogInfo("监控线程", "已启动...");
        }
        /// <summary>
        /// 不会停止监控线程
        /// </summary>
        public void Stop()
        {
            isActivity = false;
            Log.LogInfo("监控线程", "已停止...");
        }
        public SortedList<DateTime, System.Threading.Tasks.Task> TaskList
        {
            get { return taskList; }
        }
        protected void AddTask(System.Threading.Tasks.Task task, DateTime? runTime = null)
        {
            if (runTime == null)
                runTime = DateTime.Now;
            lock (taskList_LOCK)
            {
                while (taskList.IndexOfKey((DateTime)runTime) > -1)
                {
                    runTime = ((DateTime)runTime).AddMilliseconds(1);
                }
                taskList.Add((DateTime)runTime, task);
                Log.LogInfo("当前剩余任务总数：", taskList.Count.ToString());
            }
        }
        public void AddTask(Action act, DateTime? runTime = null)
        {
            AddTask(new System.Threading.Tasks.Task(act, cancelToken.Token), runTime);
        }
        public void AddTask(IThreadStart runnable, DateTime? runTime = null)
        {
            AddTask(runnable.Run, runTime);
        }
        public void AddWorker(Action act)
        {

            //if (act == null)
            //{
            //    throw new System.Exception("action is Null");
            //}
            //taskFactory.StartNew(act);
            //System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(act, cancelToken.Token);
            //if (isAutoRun && !isActivity)
            //{
            //    tasks.Add(t);
            //    foreach (var item in tasks)
            //    {
            //        item.Start();
            //        item.ContinueWith(m => tasks.Remove(m));
            //    }
            //}
            //workQueue.Enqueue(t);
        }
    }
}
