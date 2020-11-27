using System;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Likvido.ApplicationInsights.Telemetry
{
    public abstract class ExecuteBaseOptions
    {
        public ExecuteBaseOptions(string operationName)
        {
            if (string.IsNullOrWhiteSpace(operationName))
            {
                throw new ArgumentNullException(nameof(operationName));
            }

            OperationName = operationName;
        }

        public string OperationName { get; }
        public Action<IOperationHolder<RequestTelemetry>>? Configure { get; set; }
        /// <summary>
        /// looks like for cronjob with intensive dependency tracking 5 sec isn't enough very often
        /// Make sure it's lower for quick cron jobs
        /// can be null only if telemetry client channel is `InMemoryChannel`
        /// </summary>
        public int? FlushWait { get; set; } = 15;
    }
}
