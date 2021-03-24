namespace Service.Deploy.Client.Services
{
    using Service.Deploy.Client.Models;

    public class ConfigRepository
    {
        public ConfigRepository(string config)
        {
        }

        public DeployEntry? Find(string name)
        {
            return null;
        }

        public void Update(DeployEntry entry)
        {
            // TODO
        }

        public void Delete(string name)
        {
            // TODO
        }
    }
}
