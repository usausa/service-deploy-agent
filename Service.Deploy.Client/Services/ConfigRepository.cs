namespace Service.Deploy.Client.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    using Service.Deploy.Client.Models;

    public class ConfigRepository
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true
        };

        private readonly string file;

        public ConfigRepository(string? config)
        {
            file = String.IsNullOrEmpty(config)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".svcdeploy")
                : config;
        }

        private DeployEntry[] Read()
        {
            if (!File.Exists(file))
            {
                return Array.Empty<DeployEntry>();
            }

            return JsonSerializer.Deserialize<DeployEntry[]>(File.ReadAllText(file), Options) ?? Array.Empty<DeployEntry>();
        }

        private void Write(DeployEntry[] entries)
        {
            File.WriteAllText(file, JsonSerializer.Serialize(entries, Options));
        }

        public DeployEntry? Find(string name)
        {
            return Read().FirstOrDefault(x => x.Name == name);
        }

        public void Update(DeployEntry entry)
        {
            var entries = Read();

            var current = entries.FirstOrDefault(x => x.Name == entry.Name);
            if (current != null)
            {
                current.Url = entry.Url;
                current.Token = entry.Token;
            }
            else
            {
                var newEntries = new DeployEntry[entries.Length + 1];
                Array.Copy(entries, 0, newEntries, 0, entries.Length);
                entries = newEntries;
                entries[^1] = entry;
            }

            Write(entries);
        }

        public void Delete(string name)
        {
            var entries = Read();

            for (var i = 0; i < entries.Length; i++)
            {
                if (entries[i].Name != name)
                {
                    continue;
                }

                var newEntries = new DeployEntry[entries.Length - 1];

                if (i > 0)
                {
                    Array.Copy(entries, 0, newEntries, 0, i);
                }

                var left = entries.Length - i - 1;
                if (left > 0)
                {
                    Array.Copy(entries, i + 1, newEntries, i, left);
                }

                Write(newEntries);

                break;
            }
        }
    }
}
