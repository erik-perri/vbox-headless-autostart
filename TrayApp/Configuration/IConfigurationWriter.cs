namespace TrayApp.Configuration
{
    public interface IConfigurationWriter
    {
        void WriteConfiguration(AppConfiguration configuration);
    }
}