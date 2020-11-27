using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Likvido.ApplicationInsights.Telemetry
{
    public static class TelemetryClientExtensions
    {
        /// <summary>
        /// Tracks operation as a request metric
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestOptions"></param>
        public static void ExecuteAsRequest(
            this TelemetryClient client,
            ExecuteAsRequestOptions requestOptions)
        {
            client.ExecuteAsRequestAsync(requestOptions?.ToAsyncOptions()!).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Tracks operation as a request metric
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestOptions"></param>
        public static async Task ExecuteAsRequestAsync(
            this TelemetryClient client,
            ExecuteAsRequestAsyncOptions requestOptions)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (requestOptions == null)
            {
                throw new ArgumentNullException(nameof(requestOptions));
            }

            try
            {
                await client.DoExecuteAsRequestAsync(requestOptions).ConfigureAwait(false);
            }
            catch
            {
                //check this https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/try-finally to get
                //why this catch is here
                throw;
            }
        }

        private static async Task DoExecuteAsRequestAsync(
            this TelemetryClient client,
            ExecuteAsRequestAsyncOptions options)
        {
            var operationName = options.OperationName;
            var flushWait = options.FlushWait;
            var operation = client.StartOperation<RequestTelemetry>(operationName);
            try
            {
                options.Configure?.Invoke(operation);

                client.TrackEvent($"{operationName} started");
                await options.Func().ConfigureAwait(false);
                client.TrackEvent($"{operationName} finished");

                operation.Telemetry.Success = true;
                operation.Telemetry.ResponseCode = "0";
            }
            catch (Exception e)
            {
                //to make sure we will have exception info related to a metric
                client.TrackException(e, new Dictionary<string, string> { ["OperationName"] = operationName });
                operation.Telemetry.Success = false;
                operation.Telemetry.ResponseCode = "500";

                throw;
            }
            finally
            {
                if (options.PostExecute != null)
                {
                    try
                    {
                        await options.PostExecute().ConfigureAwait(false);
                    }
                    catch(Exception e)
                    {
                        client.TrackException(
                            e,
                            new Dictionary<string, string> { ["OperationName"] = operationName, ["PostExecute"] = "true" });
                    }
                }
                operation.Dispose();
                client.Flush();

                if (flushWait.HasValue)
                {
                    //https://docs.microsoft.com/en-us/azure/azure-monitor/app/console#full-example
                    //https://github.com/microsoft/ApplicationInsights-dotnet/issues/407
                    await Task.Delay(TimeSpan.FromSeconds(flushWait.Value)).ConfigureAwait(false);
                }
            }
        }
    }
}
