using System;
using System.Threading.Tasks;

namespace Likvido.ApplicationInsights.Telemetry
{
    public class ExecuteAsRequestAsyncOptions : ExecuteBaseOptions
    {
        public ExecuteAsRequestAsyncOptions(string operationName, Func<Task> func) : base(operationName)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            Func = func;
        }

        public Func<Task> Func { get; }
        public Func<Task>? PostExecute { get; }
    }
}
