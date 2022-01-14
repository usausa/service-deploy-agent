using System.CommandLine;
using System.CommandLine.IO;
using System.CommandLine.NamingConventionBinder;
using System.IO.Compression;

using Service.Deploy.Client.Models;
using Service.Deploy.Client.Services;

#pragma warning disable CA1812

var rootCommand = new RootCommand("Service deploy tool");

// Config
var configCommand = new Command("config", "Config settings");
configCommand.AddOption(new Option<string>(new[] { "--config", "-c" }, "Config file"));
rootCommand.Add(configCommand);

// Config Update
var configUpdateCommand = new Command("update", "Update config");
configUpdateCommand.AddOption(new Option<string>(new[] { "--name", "-n" }, "Service name") { IsRequired = true });
configUpdateCommand.AddOption(new Option<string>(new[] { "--url", "-u" }, "Agent url"));
configUpdateCommand.AddOption(new Option<string>(new[] { "--token", "-t" }, "Authentication token"));
configUpdateCommand.Handler = CommandHandler.Create((string config, string name, string? url, string? token) =>
{
    var repository = new ConfigRepository(config);
    repository.Update(new DeployEntry { Name = name, Url = url, Token = token });
});
configCommand.Add(configUpdateCommand);

// Config Delete
var configDeleteCommand = new Command("delete", "Delete config");
configDeleteCommand.AddOption(new Option<string>(new[] { "--name", "-n" }, "Service name") { IsRequired = true });
configDeleteCommand.Handler = CommandHandler.Create((string config, string name) =>
{
    var repository = new ConfigRepository(config);
    repository.Delete(name);
});
configDeleteCommand.Add(configUpdateCommand);

// Update
var deployCommand = new Command("deploy", "Deploy service");
deployCommand.AddOption(new Option<string>(new[] { "--name", "-n" }, "Service name") { IsRequired = true });
deployCommand.AddOption(new Option<string>(new[] { "--directory", "-d" }, "Archive directory") { IsRequired = true });
deployCommand.AddOption(new Option<string>(new[] { "--config", "-c" }, "Config file"));
deployCommand.AddOption(new Option<string>(new[] { "--url", "-u" }, "Agent url"));
deployCommand.AddOption(new Option<string>(new[] { "--token", "-t" }, "Authentication token"));
deployCommand.Handler = CommandHandler.Create(async (IConsole console, string name, string directory, string? config, string? url, string? token, CancellationToken cancel) =>
{
    if (String.IsNullOrEmpty(url) || String.IsNullOrEmpty(token))
    {
        var repository = new ConfigRepository(config);
        var entry = repository.Find(name);

        url ??= entry?.Url;
        token ??= entry?.Token;

        if (String.IsNullOrEmpty(url))
        {
            console.Out.WriteLine("Agent url is required");
            return -1;
        }
    }

    var archive = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    try
    {
        // Archive
        ZipFile.CreateFromDirectory(directory, archive);

        // Update
        using var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        using var client = new HttpClient(handler)
        {
            Timeout = new TimeSpan(0, 0, 5, 0),
            BaseAddress = new Uri(url)
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, $"deploy/update/{name}");
        using var multipart = new MultipartFormDataContent();

        if (!String.IsNullOrEmpty(token))
        {
            request.Headers.Add("X-Deploy-Token", token);
        }

        var info = new FileInfo(archive);
        using var fileContent = new StreamContent(File.OpenRead(archive));
        multipart.Add(fileContent, "archive", info.Name);

        request.Content = multipart;

        var response = await client.SendAsync(request, cancel);

        console.Out.WriteLine($"Deploy result: status=[{response.StatusCode}], message=[{await response.Content.ReadAsStringAsync(cancel)}]");

        return response.IsSuccessStatusCode ? 0 : -1;
    }
    finally
    {
        File.Delete(archive);
    }
});
deployCommand.Add(configUpdateCommand);

return await rootCommand.InvokeAsync(args);
