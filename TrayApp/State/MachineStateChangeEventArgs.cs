using System;
using System.Collections.ObjectModel;
using TrayApp.VirtualMachine;

namespace TrayApp.State
{
    public class MachineStateChangeEventArgs : EventArgs
    {
        public ReadOnlyCollection<IMachineMetadata> PreviousMachines { get; }

        public ReadOnlyCollection<IMachineMetadata> NewMachines { get; }

        public MachineStateChangeEventArgs(
            ReadOnlyCollection<IMachineMetadata> previousMachines,
            ReadOnlyCollection<IMachineMetadata> newMachines
        )
        {
            PreviousMachines = previousMachines ?? throw new ArgumentNullException(nameof(previousMachines));
            NewMachines = newMachines ?? throw new ArgumentNullException(nameof(newMachines));
        }

        public MachineStateChangeEventArgs(IMachineMetadata[] previousMachines, IMachineMetadata[] newMachines)
        {
            PreviousMachines = new ReadOnlyCollection<IMachineMetadata>(previousMachines);
            NewMachines = new ReadOnlyCollection<IMachineMetadata>(newMachines);
        }
    }
}