namespace TrayApp.Configuration
{
    public class MachineConfiguration
    {
        public string Uuid { get; internal set; }

        public bool SaveState { get; internal set; }

        public bool AutoStart { get; internal set; }
    }
}