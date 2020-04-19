using System;
using System.Collections.ObjectModel;
using TrayApp.Configuration;

namespace TrayApp.State
{
    public class MachineConfigurationChangeEventArgs : EventArgs
    {
        public ReadOnlyCollection<MachineConfiguration> PreviousMachines { get; }

        public ReadOnlyCollection<MachineConfiguration> NewMachines { get; }

        public MachineConfigurationChangeEventArgs(
            ReadOnlyCollection<MachineConfiguration> previousMachines,
            ReadOnlyCollection<MachineConfiguration> newMachines
        )
        {
            PreviousMachines = previousMachines ?? throw new ArgumentNullException(nameof(previousMachines));
            NewMachines = newMachines ?? throw new ArgumentNullException(nameof(newMachines));
        }
    }
}