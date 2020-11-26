using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Likvido.ApplicationInsights.Telemetry
{
    public class AvoidRequestSamplingTelemetryInitializer : ITelemetryInitializer
    {
        private readonly AvoidSamplingTelemetryInitializer _innerInitializer;

        public AvoidRequestSamplingTelemetryInitializer(string operationName)
        {
            _innerInitializer = new AvoidSamplingTelemetryInitializer(
                t => t is RequestTelemetry telemetry && telemetry.Name == operationName);
        }

        public void Initialize(ITelemetry telemetry)
        {
            _innerInitializer.Initialize(telemetry);
        }
    }
}
