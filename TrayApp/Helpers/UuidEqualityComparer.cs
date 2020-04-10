using System;
using System.Collections.Generic;
using TrayApp.Configuration;
using TrayApp.VirtualMachine;

namespace TrayApp.Helpers
{
    public class UuidEqualityComparer : IEqualityComparer<IMachine>, IEqualityComparer<MachineConfiguration>
    {
        public bool Equals(IMachine a, IMachine b)
        {
            return a?.Uuid == b?.Uuid;
        }

        public bool Equals(MachineConfiguration a, MachineConfiguration b)
        {
            return a?.Uuid == b?.Uuid;
        }

        public int GetHashCode(IMachine machine)
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