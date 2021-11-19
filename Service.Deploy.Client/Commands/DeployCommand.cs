namespace Service.Deploy.Client.Commands;

using System;
using System.CommandLine;
using System.CommandLine.IO;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Service.Deploy.Client.Framework;
using Service.Deploy.Client.Services;

public sealed class DeployCommand : ExecuteCommandBase<DeployCommand.Parameter>
{
    public class Parameter
    {
        [AllowNull]
        public string Name { get; set; }

        [AllowNull]
        public string Directory { get; set; }

        public string? Config { get; set; }

        public string? Url { get; set; }

        public string? Token { get; set; }
    }

    protected override Command CreateCommand() => new("deploy", "Deploy service")
    {
        new Option<string>(new[] { "--name", "-n" }, "Service name") { IsRequired = true },
        new Option<string>(new[] { "--directory", "-d" }, "Archive directory") { IsRequired = true },
        new Option<string>(new[] { "--config", "-c" }, "Config file"),
        new Option<string>(new[] { "--url", "-u" }, "Agent url"),
        new Option<string>(new[] { "--token", "-t" }, "Authentication token")
    };

    protected override async ValueTask<int> ExecuteAsync(Parameter parameter, IConsole console, CancellationToken cancel)
    {
        if (String.IsNullOrEmpty(parameter.Url) || String.IsNullOrEmpty(parameter.Token))
        {
            var config = new ConfigRepository(parameter.Config);
            var entry = config.Find(parameter.Name);

            parameter.Url ??= entry?.Url;
            parameter.Token ??= entry?.Token;

            if (String.IsNullOrEmpty(parameter.Url))
            {
                console.Out.WriteLine("Agent url is required");
                return -1;
            }
        }

        var archive = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            // Archive
            ZipFile.CreateFromDirectory(parameter.Directory, archive);

            // Update
            using var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };
            using var client = new HttpClient(handler)
            {
                Timeout = new TimeSpan(0, 0, 5, 0),
                BaseAddress = new Uri(parameter.Url)
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, $"deploy/update/{parameter.Name}");
            using var multipart = new MultipartFormDataContent();

            if (!String.IsNullOrEmpty(parameter.Token))
            {
                request.Headers.Add("X-Deploy-Token", parameter.Token);
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
    }
}
