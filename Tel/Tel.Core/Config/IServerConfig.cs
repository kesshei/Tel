namespace Tel.Core.Config
{
    public interface IServerConfig
    {
        string WebDomain { get; set; }

        string[] WebAllowAccessIps { get; set; }

        bool EnableForward { get; set; }
    }
}
