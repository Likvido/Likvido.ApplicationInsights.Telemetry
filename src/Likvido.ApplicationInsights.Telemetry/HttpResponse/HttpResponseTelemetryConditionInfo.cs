using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.ApplicationInsights.DataContracts;

namespace Likvido.ApplicationInsights.Telemetry
{
    public class HttpResponseTelemetryConditionInfo
    {
        public DependencyTelemetry Telemetry { get; set; }
        public HttpResponseCustomProperty Property { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public HttpMethod Method { get; set; }
        public HttpContentHeaders ContentHeaders { get; set; }

        public HttpResponseTelemetryConditionInfo(
            DependencyTelemetry telemetry,
            HttpResponseCustomProperty property,
            HttpStatusCode statusCode,
            HttpMethod method,
            HttpContentHeaders contentHeaders)
        {
            Telemetry = telemetry;
            Property = property;
            StatusCode = statusCode;
            Method = method;
            ContentHeaders = contentHeaders;
        }
    }
}
