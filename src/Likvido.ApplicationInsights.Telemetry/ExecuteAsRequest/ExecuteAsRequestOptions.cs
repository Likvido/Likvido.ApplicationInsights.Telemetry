using System;
using System.Threading.Tasks;

namespace Likvido.ApplicationInsights.Telemetry
{
    public class ExecuteAsRequestOptions : ExecuteBaseOptions
    {
        public ExecuteAsRequestOptions(string operationName, Action func) : base(operationName)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            Func = func;
        }

        public Action Func { get; }
        public Action? PostExecute { get; set; }

        public ExecuteAsRequestAsyncOptions ToAsyncOptions()
        {
            return new ExecuteAsRequestAsyncOptions(OperationName, ActionToAsyncFunc(Func)!)
            {
                FlushWait = FlushWait,
                Configure = Configure,
                PostExecute = ActionToAsyncFunc(PostExecute)
            };
        }

        private static Func<Task>? ActionToAsyncFunc(Action? func)
        {
            if (func == null)
            {
                return null;
            }
            return () =>
            {
                func();
                return Task.CompletedTask;
            };
        }
    }
}
