namespace TrayApp.Configuration
{
    public interface IConfigurationWriter
    {
        bool WriteConfiguration(TrayConfiguration configuration);
    }
}