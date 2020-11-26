using System;

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
        public Action? PostExecute { get; }
    }
}
