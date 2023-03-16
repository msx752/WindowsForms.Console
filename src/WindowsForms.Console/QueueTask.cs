namespace WindowsForms.Console;

internal class QueueTaskObject
{
    public QueueTaskObject(string message, Color? color, bool showTimeTag)
    {
        this.message = message;
        this.color = color;
        this.showTimeTag = showTimeTag;
    }

    public Color? color { get; set; }
    public string message { get; set; }
    public bool showTimeTag { get; set; }
}

internal class QueueTask : IDisposable
{
    private readonly Task backgroundTask;
    private readonly CancellationTokenSource cancellationTokenSource;
    private readonly FConsole fConsole;
    private bool disposedValue;
    private ConcurrentQueue<QueueTaskObject> tasks;

    public QueueTask(FConsole fConsole)
    {
        this.cancellationTokenSource = new CancellationTokenSource();
        this.tasks = new ConcurrentQueue<QueueTaskObject>();
        this.fConsole = fConsole;
        this.backgroundTask = Task.Factory.StartNew(async () =>
        {
            try
            {
                while (!disposedValue)
                {
                    this.cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (this.tasks.Count > 0)
                    {
                        if (this.tasks.TryPeek(out QueueTaskObject task))
                        {
                            if (this.fConsole.OriginalWrite(task.message, task.color, task.showTimeTag))
                                this.tasks.TryDequeue(out QueueTaskObject r);
                        }
                    }

                    await Task.Delay(2, this.cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception)
            {
            }
        }, cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    internal void Enqueue(QueueTaskObject task)
    {
        this.tasks.Enqueue(task);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                try
                {
                    this.cancellationTokenSource.Cancel(true);
                }
                catch { }
                try
                {
                    this.backgroundTask.Dispose();
                }
                catch { }
                try
                {
#if !NET48
                    this.tasks.Clear();
#endif
                }
                catch { }
            }

            this.disposedValue = true;
            this.tasks = null;
        }
    }
}