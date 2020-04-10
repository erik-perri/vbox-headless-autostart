using System;
using System.Collections.Generic;

namespace TrayApp.VirtualMachine
{
    public class UuidEqualityComparer : IEqualityComparer<IMachine>
    {
        public bool Equals(IMachine a, IMachine b)
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
    }
}