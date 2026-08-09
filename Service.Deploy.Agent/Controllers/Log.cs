namespace Service.Deploy.Agent.Controllers;

#pragma warning disable SYSLIB1006
public static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Deploy update request. name=[{name}]")]
    public static partial void InfoDeployUpdateRequest(this ILogger logger, string name);

    [LoggerMessage(Level = LogLevel.Information, Message = "Deploy update success. name=[{name}]")]
    public static partial void InfoDeployUpdateSuccess(this ILogger logger, string name);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Deploy entry not found. name=[{name}]")]
    public static partial void WarnDeployEntryNotFound(this ILogger logger, string name);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Deploy token is invalid. name=[{name}]")]
    public static partial void WarnDeployTokenIsInvalid(this ILogger logger, string name);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Delete directory failed. name=[{name}], directory=[{directory}]")]
    public static partial void WarnDeleteDirectoryFailed(this ILogger logger, string name, string directory);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Deploy entry is invalid. name=[{name}]")]
    public static partial void WarnDeployEntryIsInvalid(this ILogger logger, string name);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Stop service failed. name=[{name}]")]
    public static partial void WarnStopServiceFailed(this ILogger logger, string name);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Start service failed. name=[{name}]")]
    public static partial void WarnStartServiceFailed(this ILogger logger, string name);
}
#pragma warning restore SYSLIB1006
