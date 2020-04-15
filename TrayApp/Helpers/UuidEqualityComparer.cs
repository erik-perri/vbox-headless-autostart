using System;
using System.Collections.Generic;

namespace TrayApp.Helpers
{
    public class UuidEqualityComparer : IEqualityComparer<IUuidContainer>
    {
        public bool Equals(IUuidContainer a, IUuidContainer b)
        {
            return a?.Uuid == b?.Uuid;
        }

        public int GetHashCode(IUuidContainer machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            return machine.Uuid.GetHashCode(StringComparison.Ordinal);
        }
    }
}