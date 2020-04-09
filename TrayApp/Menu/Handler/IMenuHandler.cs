using System.Windows.Forms;

namespace TrayApp.Menu.Handler
{
    public interface IMenuHandler
    {
        int GetSortOrder();

        ToolStripItem[] CreateMenuItems();
    }
}