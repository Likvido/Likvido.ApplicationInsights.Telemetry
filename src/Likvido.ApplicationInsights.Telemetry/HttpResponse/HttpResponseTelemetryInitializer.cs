using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Likvido.ApplicationInsights.Telemetry
{
    /// <summary>
    /// Enrich telemetry with response content.
    /// </summary>
    public class HttpResponseTelemetryInitializer : ITelemetryInitializer
    {
        private readonly Func<HttpResponseTelemetryConditionInfo, bool> _condition;

        public List<string> ContentMediaTypes { get; } = new List<string>
        {
            "application/json",
            "text/plain",
            "text/html"
        };

        public HttpResponseTelemetryInitializer(
            Func<HttpResponseTelemetryConditionInfo, bool> condition)
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        public HttpResponseTelemetryInitializer(
            Func<HttpResponseTelemetryConditionInfo, bool> condition,
            List<string> contentMediaTypes) : this(condition)
        {
            ContentMediaTypes = contentMediaTypes;
        }

        public void Initialize(ITelemetry telemetry)
        {
            try
            {
                if (telemetry is DependencyTelemetry dependencyTelemetry)
                {
                    Initialize(dependencyTelemetry);
                }
            }
            catch
            {
                // Do not fail initializer.
            }
        }

        private void Initialize(DependencyTelemetry telemetry)
        {
            // Check if telemetry is for http response.
            if (telemetry.Type == "Http" &&
                telemetry.TryGetOperationDetail("HttpResponse", out var httpResponseObj) &&
                httpResponseObj is HttpResponseMessage httpResponse)
            {
                var properties = new List<(string Key, string Value)>();

                if (httpResponse.RequestMessage.Content != null &&
                    IsValidMediaType(httpResponse.RequestMessage.Content.Headers) &&
                    _condition(new HttpResponseTelemetryConditionInfo(
                        telemetry,
                        HttpResponseCustomProperty.RequestContent,
                        httpResponse.StatusCode,
                        httpResponse.RequestMessage.Method,
                        httpResponse.RequestMessage.Content.Headers)) &&
                    TryReadContentString(httpResponse.RequestMessage.Content, out var requestContentString))
                {
                    properties.Add((HttpResponseCustomProperty.RequestContent.ToString(), requestContentString!));
                }

                if (httpResponse.Content != null &&
                    IsValidMediaType(httpResponse.Content.Headers) &&
                    _condition(new HttpResponseTelemetryConditionInfo(
                        telemetry,
                        HttpResponseCustomProperty.ResponseContent,
                        httpResponse.StatusCode,
                        httpResponse.RequestMessage.Method,
                        httpResponse.Content.Headers)) &&
                    TryReadContentString(httpResponse.Content, out var responseContentString))
                {
                    properties.Add((HttpResponseCustomProperty.ResponseContent.ToString(), responseContentString!));
                }

                telemetry.AddCustomProperties(properties);
            }
        }

        private static bool TryReadContentString(HttpContent content, out string? contentString)
        {
            try
            {
                contentString = content.ReadAsStringAsync().GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                contentString = $"-- Failed to read content: {ex.Message}";
                return false;
            }
        }

        private bool IsValidMediaType(HttpContentHeaders contentHeaders)
        {
            return contentHeaders?.ContentType?.MediaType != null &&
                ContentMediaTypes.Contains(contentHeaders.ContentType.MediaType);
        }
    }
}
