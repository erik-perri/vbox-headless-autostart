using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using TrayApp.Configuration;
using TrayApp.VirtualMachine;

namespace TrayApp.Forms
{
    public partial class ConfigureForm : Form
    {
        private readonly VisualStyleRenderer disabledCheckBoxRendererUnchecked = new VisualStyleRenderer(
            VisualStyleElement.Button.CheckBox.UncheckedDisabled
        );

        private readonly VisualStyleRenderer disabledCheckBoxRendererChecked = new VisualStyleRenderer(
            VisualStyleElement.Button.CheckBox.CheckedDisabled
        );

        private List<MachineRow> currentRows;

        public AppConfiguration UpdatedConfiguration { get; private set; }

        public ConfigureForm(AppConfiguration configuration, IMachineMetadata[] machines)
        {
            InitializeComponent();
            SetLocationBottomRight();

            SetupConfiguration(configuration ?? throw new ArgumentNullException(nameof(configuration)));
            SetupDataGrid(machines ?? Array.Empty<IMachineMetadata>(), configuration.Machines.ToArray());
        }

        private void SetLocationBottomRight()
        {
            const int offset = 10;
            var workingArea = Screen.GetWorkingArea(this);
            StartPosition = FormStartPosition.Manual;
            Location = new Point(workingArea.Right - Size.Width - offset, workingArea.Bottom - Size.Height - offset);
        }

        private void SetupConfiguration(AppConfiguration configuration)
        {
            var index = comboBoxLogLevel.FindStringExact(configuration.LogLevel.ToString());
            if (index != -1)
            {
                comboBoxLogLevel.SelectedIndex = index;
            }

            checkBoxKeepAwakeMenu.Checked = configuration.ShowKeepAwakeMenu;
            checkBoxStartWithWindows.Checked = configuration.StartWithWindows;
        }

        private void SetupDataGrid(IMachineMetadata[] machines, MachineConfiguration[] machineConfigurations)
        {
            currentRows = new List<MachineRow>();

            foreach (var machine in machines)
            {
                var configuration = Array.Find(machineConfigurations, c => c.Uuid == machine.Uuid);
                var monitored = configuration != null;

                if (configuration == null)
                {
                    configuration = ConfigurationFactory.GetDefaultMachineConfiguration(machine.Uuid);
                }

                currentRows.Add(new MachineRow()
                {
                    Monitored = monitored,
                    Uuid = machine.Uuid,
                    Name = machine.Name,
                    AutoStart = configuration.AutoStart,
                    SaveState = configuration.SaveState,
                });
            }

            foreach (var configuration in machineConfigurations)
            {
                var machine = Array.Find(machines, m => m.Uuid == configuration.Uuid);
                if (machine == null)
                {
                    currentRows.Add(new MachineRow()
                    {
                        Monitored = true,
                        Uuid = configuration.Uuid,
                        Name = "Not found in VirtualBox",
                        AutoStart = configuration.AutoStart,
                        SaveState = configuration.SaveState,
                        Disabled = true,
                    });
                }
            }

            var source = new BindingSource() { DataSource = currentRows };

            dataGridMachines.AutoGenerateColumns = false;
            dataGridMachines.DataSource = source;
        }

        private void OnCancel(object sender, EventArgs e)
        {
            UpdatedConfiguration = null;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void OnSave(object sender, EventArgs e)
        {
            if (currentRows == null)
            {
                throw new InvalidOperationException("Missing current rows");
            }

            // Build the new configuration machine list
            var machines = new List<MachineConfiguration>();
            foreach (var machineRow in currentRows)
            {
                if (!machineRow.Monitored)
                {
                    continue;
                }

                machines.Add(new MachineConfiguration(machineRow.Uuid, machineRow.SaveState, machineRow.AutoStart));
            }

            if (!Enum.TryParse(comboBoxLogLevel.SelectedItem?.ToString(), out LogLevel logLevel))
            {
                throw new InvalidOperationException("Unknwon log level specified");
            }

            // Save the new configuration for the caller
            UpdatedConfiguration = new AppConfiguration(
                logLevel,
                checkBoxKeepAwakeMenu.Checked,
                checkBoxStartWithWindows.Checked,
                new ReadOnlyCollection<MachineConfiguration>(machines.ToArray())
            );

            DialogResult = DialogResult.OK;
            Close();
        }

        private void Machines_SelectionChanged(object sender, EventArgs e)
        {
            // Prevent rows from being selected, we don't allow the user to edit anything where selection makes sense
            dataGridMachines.ClearSelection();
        }

        private void Machines_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }

            // Change the row background on hover
            foreach (DataGridViewCell cell in dataGridMachines.Rows[e.RowIndex].Cells)
            {
                cell.Style.BackColor = Color.FromArgb(0xEE, 0xEE, 0xEE);
            }
        }

        private void Machines_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }

            // Change the row background back from the hover
            foreach (DataGridViewCell cell in dataGridMachines.Rows[e.RowIndex].Cells)
            {
                cell.Style.BackColor = Color.White;
            }
        }

        private void Machines_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Invalidate the auto-start and save state columns when the monitored column is changed to cause the
            // checkbox re-render
            if (e.ColumnIndex == columnMonitored.Index && e.RowIndex != -1)
            {
                dataGridMachines.InvalidateCell(columnAutoStart.Index, e.RowIndex);
                dataGridMachines.InvalidateCell(columnSaveState.Index, e.RowIndex);
            }
        }

        private void Machines_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            // The checkbox cell does not report the value changed until the cell selection changes, this causes it to
            // report the change immediately
            if (e.ColumnIndex == columnMonitored.Index && e.RowIndex != -1)
            {
                dataGridMachines.EndEdit();
            }
        }

        private void Machines_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // Render the name and UUID columns as gray when the row is disabled (found in the configuration but not
            // in the VirtualBox machine list)
            foreach (var index in new int[] { columnName.Index, columnUuid.Index })
            {
                if (dataGridMachines.Rows[e.RowIndex].Cells[index] is DataGridViewTextBoxCell textBoxCell)
                {
                    textBoxCell.Style.ForeColor = currentRows[e.RowIndex].Disabled ? Color.DimGray : Color.Black;
                }
            }

            // Draw a disabled checkbox over the auto-start and save state columns when the row is disabled or not
            // monitored
            foreach (var index in new int[] { columnAutoStart.Index, columnSaveState.Index })
            {
                var cell = dataGridMachines.Rows[e.RowIndex].Cells[index] as DataGridViewCheckBoxCell;

                cell.ReadOnly = !currentRows[e.RowIndex].Monitored;

                if (cell.ReadOnly || currentRows[e.RowIndex].Disabled)
                {
                    var bounds = dataGridMachines.GetCellDisplayRectangle(index, e.RowIndex, false);
                    const int checkBoxSize = 16;

                    bounds.X += (bounds.Width - checkBoxSize) / 2;
                    bounds.Y += (bounds.Height - checkBoxSize) / 2;
                    bounds.Width = checkBoxSize;
                    bounds.Height = checkBoxSize;

                    var drawChecked = false;

                    if (index == columnAutoStart.Index)
                    {
                        drawChecked = currentRows[e.RowIndex].AutoStart;
                    }
                    else if (index == columnSaveState.Index)
                    {
                        drawChecked = currentRows[e.RowIndex].SaveState;
                    }

                    if (VisualStyleRenderer.IsSupported)
                    {
                        if (drawChecked)
                        {
                            disabledCheckBoxRendererChecked.DrawBackground(e.Graphics, bounds);
                        }
                        else
                        {
                            disabledCheckBoxRendererUnchecked.DrawBackground(e.Graphics, bounds);
                        }
                    }
                    else
                    {
                        if (drawChecked)
                        {
                            ControlPaint.DrawCheckBox(e.Graphics, bounds, ButtonState.Checked | ButtonState.Inactive);
                        }
                        else
                        {
                            ControlPaint.DrawCheckBox(e.Graphics, bounds, ButtonState.Inactive);
                        }
                    }
                }
            }
        }

        internal class MachineRow
        {
            public bool Monitored { get; set; }

            public string Uuid { get; internal set; }

            public string Name { get; internal set; }

            public bool AutoStart { get; set; }

            public bool SaveState { get; set; }

            public bool Disabled { get; set; }
        }

        // Prevent some of the flickering that occurs when hovering or changing states
        internal class DoubleBufferedDataGridView : DataGridView
        {
            public new bool DoubleBuffered
            {
                get { return base.DoubleBuffered; }
                set { base.DoubleBuffered = value; }
            }

            public DoubleBufferedDataGridView()
            {
                DoubleBuffered = true;
            }
        }
    }
}