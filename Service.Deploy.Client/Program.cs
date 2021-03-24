namespace Service.Deploy.Client
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:Do not catch general exception types", Justification = "Ignore")]
        public static async Task Main(string[] args)
        {
            try
            {
                // TODO
                if (!await ProcessDeployAsync(args[0], args[1], args[2], args[3]))
                {
                    Environment.Exit(-1);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(-1);
            }
        }

        private static async ValueTask<bool> ProcessDeployAsync(string directory, string address, string name, string token)
        {
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
                    BaseAddress = new Uri(address)
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

                var response = await client.SendAsync(request);

                Console.WriteLine($"Deploy result: status=[{response.StatusCode}], message=[{await response.Content.ReadAsStringAsync()}]");

                return response.IsSuccessStatusCode;
            }
            finally
            {
                File.Delete(archive);
            }
        }
    }
}
