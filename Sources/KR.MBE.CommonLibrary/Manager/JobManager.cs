using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading;
using System.Threading.Tasks;
using KR.MBE.CommonLibrary.Struct;

namespace KR.MBE.CommonLibrary.Manager
{
    public class JobManager
    {
        private static readonly Lazy<JobManager> _instance = new Lazy<JobManager>(() => new JobManager());
        public static JobManager Instance => _instance.Value;

        private Channel<(JobOrderManager jobOrder, CancellationTokenSource cts)> _jobChannel;
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _runningJobs = new();
        private CancellationTokenSource _managerCts;
        private Task _worker;

        public bool IsRunning { get; set; } = false;

        private JobManager() { }
        
        public void Start()
        {
            if (IsRunning) return;

            IsRunning = true;
            _managerCts = new CancellationTokenSource();
            _jobChannel = Channel.CreateUnbounded<(JobOrderManager, CancellationTokenSource)>();

            _worker = Task.Run(async () =>
            {
                try
                {
                    await foreach (var (jobOrder, cts) in _jobChannel.Reader.ReadAllAsync(_managerCts.Token))
                    {
                        _runningJobs.TryAdd(jobOrder.m_sJobOrderID, cts);

                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await jobOrder.RunJob(cts.Token);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[{jobOrder.m_sJobOrderID}] Error: {ex.Message}");

                                jobOrder.SetError(ex.Message, ex.StackTrace);
                            }
                            finally
                            {
                                _runningJobs.TryRemove(jobOrder.m_sJobOrderID, out _);
                            }
                        });
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("JobManager stopped.");
                }
            });
        }

        public void Stop()
        {
            if(!IsRunning)
            {
                LogManager.Instance.Debug("JobManager is not running.");
                return;
            }

            IsRunning = false;


            // Stop accepting jobs
            _jobChannel.Writer.Complete();

            // Cancel worker loop
            _managerCts.Cancel();

            // Cancel all running jobs
            foreach (var cts in _runningJobs.Values)
            {
                cts.Cancel();
            }

            Console.WriteLine("JobManager.Stop() called.");

        }

        public void Enqueue(JobOrderManager jobOrder)
        {
            var cts = new CancellationTokenSource();
            _jobChannel.Writer.TryWrite((jobOrder, cts));
        }

        public void Cancel(string jobId)
        {
            if (_runningJobs.TryGetValue(jobId, out var cts))
            {
                cts.Cancel();
                LogManager.Instance.Debug($"Job [{jobId}] cancel requested.");
            }
        }
    }
}
