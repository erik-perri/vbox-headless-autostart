using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;
using TrayApp.Menu.Handler;

namespace TrayApp.Menu
{
    public class TrayContextMenuStrip : ContextMenuStrip
    {
        private readonly ILogger<TrayContextMenuStrip> logger;
        private readonly IMenuHandler[] handlers;

        public TrayContextMenuStrip(ILogger<TrayContextMenuStrip> logger, IMenuHandler[] handlers)
        {
            logger.LogTrace(".ctor");

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
        }

        protected override void Dispose(bool disposing)
        {
            logger.LogTrace($"Dispose({disposing})");

            base.Dispose(disposing);
        }

        public void CreateContextMenu()
        {
            Items.Clear();

            foreach (var handler in handlers)
            {
                var items = handler.CreateMenuItems();
                if (items.Length > 0)
                {
                    Items.AddRange(items);
                }
            }
        }

        public void UpdateContextMenu()
        {
            foreach (var handler in handlers)
            {
                if (handler is IMenuHandlerUpdateAware updateHandler)
                {
                    updateHandler.UpdateMenuItems();
                }
            }
        }
    }
}