namespace Hw7.Exercise2
{
    /// <summary>
    /// Recurrent job.
    /// </summary>
    public sealed class RecurrentJob : IDisposable
    {
        private RecurrentJob(TimeSpan dueTime, TimeSpan interval, object? context, int times, Action<int, object?> job)
        {
            IsRunning = true;
            _counter = default;
            _times = times;
            _callback = job;
            _timer = new Timer(SyncCallback!, context, dueTime, interval);
        }

        private readonly Timer? _timer;
        private readonly Action<int, object?>? _callback;
        private int _counter;
        private readonly int _times;
        private readonly object _sync = new();

        /// <summary> Running flag. </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Stops job and disposes all resources.
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
            IsRunning = false;
        }

        /// <summary>
        /// Starts new recurrent job.
        /// </summary>
        /// <param name="dueTime">Delay before first job execution.</param>
        /// <param name="interval">Delay between job executions.</param>
        /// <param name="times">Job executions count.</param>
        /// <param name="job">Job.</param>
        /// <param name="context">Optional context.</param>
        /// <returns>Retruns new started job.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when one of the required arguments 
        /// (<paramref name="dueTime"/>, <paramref name="interval"/>, <paramref name="times"/>)
        /// is negative
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Throws when job is <c>null</c>.
        /// </exception>
        public static RecurrentJob Run(TimeSpan dueTime, TimeSpan interval, int times, Action<int, object?> job, object? context)
        {
            if (dueTime.ToString().Contains('-'))
                throw new ArgumentOutOfRangeException(nameof(dueTime));
            else if (interval.ToString().Contains('-'))
                throw new ArgumentOutOfRangeException(nameof(interval));
            else if (times < 0)
                throw new ArgumentOutOfRangeException(nameof(times));
            else if (job is null)
                throw new ArgumentNullException(nameof(job));

            return new RecurrentJob(dueTime, interval, context, times, job);

        }

        private void SyncCallback(object state)
        {
            if (Monitor.TryEnter(_sync))
            {
                try
                {

                    if (_counter >= _times)
                        Dispose();
                    DoSomeWork(state);
                }
                finally
                {
                    if (Monitor.IsEntered(_sync))
                        Monitor.Exit(_sync);
                }
            }

        }

        private void DoSomeWork(object context)
        {
            _callback?.Invoke(_counter, context ?? new object());
            _ = Interlocked.Increment(ref _counter);
        }

    }
}
