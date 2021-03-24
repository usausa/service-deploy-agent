namespace Service.Deploy.Client.Commands
{
    using System.CommandLine;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    using Service.Deploy.Client.Framework;

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

        protected override ValueTask<int> ExecuteAsync(Parameter parameter, IConsole console, CancellationToken cancel)
        {
            // TODO
            return ValueTask.FromResult(0);
        }

        //private static async ValueTask<bool> ProcessDeployAsync(string directory, string address, string name, string token)
        //{
        //    var archive = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        //    try
        //    {
        //        // Archive
        //        ZipFile.CreateFromDirectory(directory, archive);

        //        // Update
        //        using var handler = new HttpClientHandler
        //        {
        //            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        //        };
        //        using var client = new HttpClient(handler)
        //        {
        //            Timeout = new TimeSpan(0, 0, 5, 0),
        //            BaseAddress = new Uri(address)
        //        };

        //        using var request = new HttpRequestMessage(HttpMethod.Post, $"deploy/update/{name}");
        //        using var multipart = new MultipartFormDataContent();

        //        if (!String.IsNullOrEmpty(token))
        //        {
        //            request.Headers.Add("X-Deploy-Token", token);
        //        }

        //        var info = new FileInfo(archive);
        //        using var fileContent = new StreamContent(File.OpenRead(archive));
        //        multipart.Add(fileContent, "archive", info.Name);

        //        request.Content = multipart;

        //        var response = await client.SendAsync(request);

        //        Console.WriteLine($"Deploy result: status=[{response.StatusCode}], message=[{await response.Content.ReadAsStringAsync()}]");

        //        return response.IsSuccessStatusCode;
        //    }
        //    finally
        //    {
        //        File.Delete(archive);
        //    }
        //}
    }
}
