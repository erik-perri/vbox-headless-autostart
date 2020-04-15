using System;
using System.Collections.ObjectModel;
using TrayApp.Menu.Handler;

namespace TrayApp.Menu
{
    public class TrayHandlerCollection
    {
        public ReadOnlyCollection<IMenuHandler> Handlers { get; }

        public TrayHandlerCollection(IMenuHandler[] handlers)
        {
            if (handlers == null)
            {
                throw new ArgumentNullException(nameof(handlers));
            }

            Handlers = new ReadOnlyCollection<IMenuHandler>(handlers);
        }
    }
}