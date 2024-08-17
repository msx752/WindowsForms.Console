namespace WindowsForms.Console
{
    internal readonly struct QueueTaskObject
    {
        public QueueTaskObject(string message, Color? color, bool showTimeTag)
        {
            Message = message;
            Color = color;
            ShowTimeTag = showTimeTag;
        }

        public Color? Color { get; }
        public string Message { get; }
        public bool ShowTimeTag { get; }
    }

    internal class QueueTask : IDisposable
    {
        private readonly Task _backgroundTask;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly FConsole _fConsole;
        private bool _disposed;
        private readonly ConcurrentQueue<QueueTaskObject> _tasks;

        public QueueTask(FConsole fConsole)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _tasks = new ConcurrentQueue<QueueTaskObject>();
            _fConsole = fConsole;

            _backgroundTask = Task.Factory.StartNew(ProcessQueue, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void Enqueue(QueueTaskObject task)
        {
            _tasks.Enqueue(task);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _cancellationTokenSource.Cancel();
                    _backgroundTask.Dispose();
#if !NET48
                    _tasks.Clear();
#endif
                }

                _disposed = true;
            }
        }

        private async Task ProcessQueue()
        {
            var spinWait = new SpinWait();

            try
            {
                while (!_disposed)
                {
                    if (_tasks.TryDequeue(out QueueTaskObject task))
                    {
                        if (_fConsole.OriginalWrite(task.Message, task.Color, task.ShowTimeTag))
                        {
                            // Successfully processed the task
                        }
                    }
                    else
                    {
                        // If queue is empty, yield the CPU but avoid context switches when possible
                        spinWait.SpinOnce();
                        await Task.Yield();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Task was cancelled
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                // Consider logging ex.Message or using a logging framework
            }
        }
    }
}
