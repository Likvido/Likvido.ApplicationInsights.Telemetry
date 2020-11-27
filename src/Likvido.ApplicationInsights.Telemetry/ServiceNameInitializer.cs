using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Likvido.ApplicationInsights.Telemetry
{
    public class ServiceNameInitializer : ITelemetryInitializer
    {
        private readonly string _roleName;

        public ServiceNameInitializer(string roleName)
        {
            _roleName = roleName;
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry?.Context?.Cloud != null)
            {
                telemetry.Context.Cloud.RoleName = _roleName;
            }
        }
    }
}
