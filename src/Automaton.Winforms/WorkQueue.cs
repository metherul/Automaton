using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Collections.Concurrent;
using System.Threading;

namespace Automaton.Winforms
{
    public class WorkQueue
    {
        private BlockingCollection<Action> work_queue = new BlockingCollection<Action>();
        public bool Running { get; set; }
        public ProgramLogic ProgramLogic { get; }
        public string[] WorkerStatus { get; private set; }

        private ThreadLocal<int> CpuID = new ThreadLocal<int>();

        public WorkQueue(ProgramLogic logic)
        {
            ProgramLogic = logic;
     
            StartWorkers(Environment.ProcessorCount);

        }

        private void StartWorkers(int processorCount)
        {
            Running = true;
            WorkerStatus = new string[processorCount];
            var threads = Enumerable.Range(0, processorCount)
                                    .Select(i => {
                                        var thread = new Thread(() => WorkerLoop(this, i));
                                        thread.Priority = ThreadPriority.BelowNormal;
                                        thread.Start();
                                        return i;
                                        })
                                    .ToList();
        }
        
        public Task<T> QueueTask<V, T>(Func<V, T> func, V val)
        {
            var p = new TaskCompletionSource<T>();
            work_queue.Add(() =>
            {
                try
                {
                    var result = func(val);
                    p.TrySetResult(result);
                }
                catch (Exception ex)
                {
                    p.TrySetException(ex);
                }
            });
            return p.Task;
        }


        private static void WorkerLoop(WorkQueue queue, int cpu_id)
        {
            queue.CpuID.Value = cpu_id;
            while(queue.Running)
            {
                queue.SetWorkerStatus("Waiting");
                var task = queue.work_queue.Take();
                task();
            }

        }
        public void SetWorkerStatus(string v)
        {
            WorkerStatus[CpuID.Value] = v;
        }
    }
}
