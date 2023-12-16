namespace Service.Deploy.Client.Services;

using System.Text.Json;

using Service.Deploy.Client.Models;

public sealed class ConfigRepository
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

    private DeploySetting Read()
    {
        return File.Exists(file)
            ? JsonSerializer.Deserialize<DeploySetting>(File.ReadAllText(file), Options) ?? new DeploySetting()
            : new DeploySetting();
    }

    private void Write(DeploySetting setting)
    {
        File.WriteAllText(file, JsonSerializer.Serialize(setting, Options));
    }

    public DeployEntry? Find(string name)
    {
        return Read().Entries.FirstOrDefault(x => x.Name == name);
    }

    public void Update(DeployEntry entry)
    {
        var setting = Read();

        var current = setting.Entries.FirstOrDefault(x => x.Name == entry.Name);
        if (current != null)
        {
            current.Url = entry.Url;
            current.Token = entry.Token;
        }
        else
        {
            var newEntries = new DeployEntry[setting.Entries.Length + 1];
            Array.Copy(setting.Entries, 0, newEntries, 0, setting.Entries.Length);
            setting.Entries = newEntries;
            setting.Entries[^1] = entry;
        }

        Write(setting);
    }

    public void Delete(string name)
    {
        var setting = Read();

        for (var i = 0; i < setting.Entries.Length; i++)
        {
            if (setting.Entries[i].Name != name)
            {
                continue;
            }

            var newEntries = new DeployEntry[setting.Entries.Length - 1];

            if (i > 0)
            {
                Array.Copy(setting.Entries, 0, newEntries, 0, i);
            }

            var left = setting.Entries.Length - i - 1;
            if (left > 0)
            {
                Array.Copy(setting.Entries, i + 1, newEntries, i, left);
            }

            setting.Entries = newEntries;

            Write(setting);

            break;
        }
    }
}
