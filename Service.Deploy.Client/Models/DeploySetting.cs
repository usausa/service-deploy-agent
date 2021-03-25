namespace Service.Deploy.Client.Models
{
    using System.Diagnostics.CodeAnalysis;

    public class DeploySetting
    {
        [SuppressMessage("Performance", "CA1819", Justification = "Ignore")]
        [AllowNull]
        public DeployEntry[] Entries { get; set; }
    }
}
