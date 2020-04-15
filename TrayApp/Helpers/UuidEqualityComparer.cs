using CommonLib.Configuration;
using CommonLib.VirtualMachine;
using System;
using System.Collections.Generic;

namespace TrayApp.Helpers
{
    public class UuidEqualityComparer : IEqualityComparer<IMachineMetadata>, IEqualityComparer<MachineConfiguration>
    {
        public bool Equals(IMachineMetadata a, IMachineMetadata b)
        {
            return a?.Uuid == b?.Uuid;
        }

        public bool Equals(MachineConfiguration a, MachineConfiguration b)
        {
            return a?.Uuid == b?.Uuid;
        }

        public int GetHashCode(IMachineMetadata machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            return machine.Uuid.GetHashCode(StringComparison.Ordinal);
        }

        public int GetHashCode(MachineConfiguration machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            return machine.Uuid.GetHashCode(StringComparison.Ordinal);
        }
    }
}