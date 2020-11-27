# Likvido.ApplicationInsights.Telemetry
Small util library to work with app insights library
# Utils
## ServiceNameInitializer
Adjust all telemetry with role name. Helps to search in metrics

![image](https://user-images.githubusercontent.com/3293183/100420543-3919b080-30b9-11eb-8b4d-eadeaaa55a1b.png)

To register it just add the following code the configure services
```
services.AddSingleton<ITelemetryInitializer>(new ServiceNameInitializer(<RoleName>));
```

## AvoidSamplingTelemetryInitializer
Prevents metrics which match pecified condition to be sample on the client side
To register it just add the following code the configure services
```
services.AddSingleton<ITelemetryInitializer>(
    new AvoidSamplingTelemetryInitializer(
        t => t is RequestTelemetry telemetry && telemetry.Name == operationName))
```

## AvoidRequestSamplingTelemetryInitializer
Just a small convenient wrapper around `AvoidSamplingTelemetryInitializer`

```
services.AddSingleton<ITelemetryInitializer>(
    new AvoidRequestSamplingTelemetryInitializer(operationName))
```

## TelemetryClientExtensions.ExecuteAsRequest/TelemetryClientExtensions.ExecuteAsRequestAsync
Exectes operations as `RequestTelemetry`
Usage
```
await telemetryClient.ExecuteAsRequestAsync(new ExecuteAsRequestAsyncOptions(operationName, func));
```
### Optinal fields
* `Action<IOperationHolder<RequestTelemetry>>? Configure` - if more telemetry details need to be added. Is called right before operations itself
* `int? FlushWait` - time for telemetry to be pushed. Usefull for console apps. In secondss - default 15
* `Action? PostExecute` - is called after an operation just before operation.Dispose(). That means logs will be still attached to the RequestTelemetry and method exectuion time will be included