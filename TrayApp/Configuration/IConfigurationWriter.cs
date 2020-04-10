namespace TrayApp.Configuration
{
    public interface IConfigurationWriter
    {
        void WriteConfiguration(TrayConfiguration configuration);
    }
}