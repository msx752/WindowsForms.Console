using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsForms.Console
{
    internal class QueueTaskObject
    {
        public QueueTaskObject(string message, Color? color, bool showTimeTag)
        {
            this.message = message;
            this.color = color;
            this.showTimeTag = showTimeTag;
        }

        public string message { get; set; }
        public Color? color { get; set; }
        public bool showTimeTag { get; set; }
    }

    internal class QueueTask : IDisposable
    {
        private bool disposedValue;
        private ConcurrentQueue<QueueTaskObject> tasks;
        private readonly FConsole fConsole;
        private readonly CancellationTokenSource cancellationTokenSource;

        public QueueTask(FConsole fConsole)
        {
            cancellationTokenSource = new CancellationTokenSource();
            tasks = new ConcurrentQueue<QueueTaskObject>();
            this.fConsole = fConsole;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    while (!disposedValue)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (tasks.Count > 0)
                        {
                            if (tasks.TryPeek(out QueueTaskObject task))
                            {
                                if (this.fConsole.OriginalWrite(task.message, task.color, task.showTimeTag))
                                    tasks.TryDequeue(out QueueTaskObject r);
                            }
                        }
                        Task.Delay(2).Wait();
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
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal void Enqueue(QueueTaskObject task)
        {
            tasks.Enqueue(task);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cancellationTokenSource.Cancel(true);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
                tasks = null;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~QueueTask()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
    }
}