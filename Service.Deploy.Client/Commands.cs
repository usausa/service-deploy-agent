// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Service.Deploy.Client;

using System.IO;
using System.IO.Compression;

using Service.Deploy.Client.Models;
using Service.Deploy.Client.Services;

using Smart.CommandLine.Hosting;

public static class CommandBuilderExtensions
{
    public static void AddCommands(this ICommandBuilder commands)
    {
        commands.AddCommand<ConfigCommand>(config =>
        {
            config.AddSubCommand<ConfigUpdateCommand>();
            config.AddSubCommand<ConfigDeleteCommand>();
        });
        commands.AddCommand<DeployCommand>();
    }
}

// Config
[Command("config", Description = "Config settings")]
public sealed class ConfigCommand
{
}

public abstract class ConfigCommandBase
{
    [Option<string>("--config", "-c", Description = "Config file", IsRequired = true)]
    public string Config { get; set; } = default!;
}

// Config Update
[Command("update", Description = "Update config")]
public sealed class ConfigUpdateCommand : ConfigCommandBase, ICommandHandler
{
    [Option<string>("--name", "-n", Description = "Service name", IsRequired = true)]
    public string Name { get; set; } = default!;

    [Option<string>("--url", "-u", Description = "Agent url", IsRequired = true)]
    public string Url { get; set; } = default!;

    [Option<string>("--token", "-t", Description = "Authentication token", IsRequired = true)]
    public string Token { get; set; } = default!;

    public ValueTask ExecuteAsync(CommandContext context)
    {
        var repository = new ConfigRepository(Config);
        repository.Update(new DeployEntry { Name = Name, Url = Url, Token = Token });

        return ValueTask.CompletedTask;
    }
}

// Config Delete
[Command("delete", Description = "Delete config")]
public sealed class ConfigDeleteCommand : ConfigCommandBase, ICommandHandler
{
    [Option<string>("--name", "-n", Description = "Service name", IsRequired = true)]
    public string Name { get; set; } = default!;

    public ValueTask ExecuteAsync(CommandContext context)
    {
        var repository = new ConfigRepository(Config);
        repository.Delete(Name);

        return ValueTask.CompletedTask;
    }
}

// Deploy
[Command("deploy", Description = "Deploy service")]
public sealed class DeployCommand : ConfigCommandBase, ICommandHandler
{
    [Option<string>("--name", "-n", Description = "Service name", IsRequired = true)]
    public string Name { get; set; } = default!;

    [Option<string>("--directory", "-d", Description = "Archive directory", IsRequired = true)]
    public string Directory { get; set; } = default!;

    [Option<string>("--url", "-u", Description = "Agent url")]
    public string? Url { get; set; }

    [Option<string>("--token", "-t", Description = "Authentication token")]
    public string? Token { get; set; }

    public async ValueTask ExecuteAsync(CommandContext context)
    {
        var url = Url;
        var token = Token;

        if (String.IsNullOrEmpty(url) || String.IsNullOrEmpty(token))
        {
            var repository = new ConfigRepository(Config);
            var entry = repository.Find(Name);

            url ??= entry?.Url;
            token ??= entry?.Token;

            if (String.IsNullOrEmpty(url))
            {
                Console.WriteLine("Agent url is required");
                context.ExitCode = -1;
                return;
            }
        }

        var archive = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            // Archive
            await ZipFile.CreateFromDirectoryAsync(Directory, archive, context.CancellationToken);

            // Update
            using var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = static (_, _, _, _) => true;
            using var client = new HttpClient(handler);
            client.Timeout = new TimeSpan(0, 0, 5, 0);
            client.BaseAddress = new Uri(url);

            using var request = new HttpRequestMessage(HttpMethod.Post, $"deploy/update/{Name}");
            using var multipart = new MultipartFormDataContent();

            if (!String.IsNullOrEmpty(token))
            {
                request.Headers.Add("X-Deploy-Token", token);
            }

            var info = new FileInfo(archive);
            using var fileContent = new StreamContent(File.OpenRead(archive));
            multipart.Add(fileContent, "archive", info.Name);

            request.Content = multipart;

            var response = await client.SendAsync(request, context.CancellationToken);

            Console.WriteLine($"Deploy result: status=[{response.StatusCode}], message=[{await response.Content.ReadAsStringAsync(context.CancellationToken)}]");

            context.ExitCode = response.IsSuccessStatusCode ? 0 : -1;
        }
        finally
        {
            File.Delete(archive);
        }
    }
}
