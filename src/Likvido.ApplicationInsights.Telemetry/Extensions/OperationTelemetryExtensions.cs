using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace Likvido.ApplicationInsights.Telemetry
{
    internal static class OperationTelemetryExtensions
    {
        // Application insights limits:
        // https://github.com/MicrosoftDocs/azure-docs/blob/master/includes/application-insights-limits.md
        private const int PropertyValueMaxLength = 8192;
        private const int CustomPropertiesTotalMaxLength = 25000; // Try to not exeed the total message length (32768).

        public static void AddCustomProperties(
            this OperationTelemetry telemetry,
            IEnumerable<(string Key, string Value)> properties)
        {
            var chunkedProperties = properties
                .Where(prop => !string.IsNullOrWhiteSpace(prop.Value))
                .OrderBy(prop => prop.Value.Length) // Add smaller property values first.
                .SelectMany(ChunkProperty);

            var lengthLeft = CustomPropertiesTotalMaxLength;
            foreach (var prop in chunkedProperties)
            {
                // Trim value if total length exeeded.
                var propValue = lengthLeft < prop.Value.Length
                    ? $"{prop.Value.Substring(0, Math.Max(lengthLeft, 0))}\n--trimmed"
                    : prop.Value;

                telemetry.Properties.Add(prop.Key, propValue);
                lengthLeft -= prop.Value.Length;
            }
        }

        private static IEnumerable<(string Key, string Value)> ChunkProperty((string Key, string Value) prop)
        {
            if (prop.Value.Length < PropertyValueMaxLength)
            {
                yield return prop;
                yield break;
            }

            for (
                int chunkStart = 0, i = 1;
                chunkStart < prop.Value.Length;
                chunkStart += PropertyValueMaxLength, i++)
            {
                yield return ($"{prop.Key}_{i}", prop.Value.Substring(chunkStart, Math.Min(PropertyValueMaxLength, prop.Value.Length - chunkStart)));
            }
        }
    }
}
