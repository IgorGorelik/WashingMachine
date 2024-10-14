using System;
using System.Drawing;
using System.Windows.Forms;

namespace WashingMachine
{
    public class DisplayPanelUnit : BaseUnit
    {
        #region Properties
        private string SelectedWashMode = string.Empty;
        #endregion

        #region Construction
        public DisplayPanelUnit(Guid machineID) : base(machineID, MachineUnitType.DisplayPanel, MachineUnitImageType.DisplayPanel)
        {
            InitContentMenu();

            InteropMessenger.Instance.ActionExecuted += DisplayPanelUnit_ActionExecuted;
            InteropMessenger.Instance.ActionExecutionFinished += DisplayPanelUnit_ActionExecutionFinished;
            InteropMessenger.Instance.Finish += DisplayPanelUnit_Finish;
            InteropMessenger.Instance.CancelProcess += DsiplayPanelUnit_CancelProcess;
        }
        #endregion

        #region Methods
        private void InitContentMenu()
        {
            UnitContextMenu = new ContextMenuStrip();
            UnitContextMenu.Opening += UnitContextMenu_Opening;
        }

        private void LoadContextMenu(bool started)
        {
            try
            {
                UnitContextMenu.Items.Clear();
                if (!started)
                {
                    ToolStripMenuItem menuItemWashMode = new ToolStripMenuItem("Wash Modes");

                    foreach (var washMode in WashingModes.Instance.WashModes)
                    {
                        ToolStripMenuItem subMenuItem = new ToolStripMenuItem(washMode);
                        menuItemWashMode.DropDownItems.Add(subMenuItem);

                        if (washMode.Equals(SelectedWashMode))
                        {
                            subMenuItem.Font = new Font(subMenuItem.Font, FontStyle.Bold);
                            subMenuItem.ForeColor = Color.Red;
                        }

                        subMenuItem.Click += (s, args) =>
                        {
                            SelectedWashMode = washMode;
                            RefreshLabelText();
                        };
                    }

                    UnitContextMenu.Items.Add(menuItemWashMode);

                    if (!string.IsNullOrEmpty(SelectedWashMode))
                    {
                        ToolStripMenuItem menuItemStart = new ToolStripMenuItem("Start");
                        menuItemStart.Click += MenuItemStart_Click;
                        UnitContextMenu.Items.Add(menuItemStart);
                    }
                }
                else
                {
                    UnitContextMenu = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }

        protected void RefreshLabelText()
        {
            UnitNameText = "Click right mouse button for menu";
            if (!string.IsNullOrEmpty(SelectedWashMode))
            {
                UnitNameText = $"Mode: {SelectedWashMode}\n{UnitNameText}";
            }
        }
        #endregion

        #region Overrides
        protected override void UpdateUnitState(EventArgs e)
        {
        }

        protected override void InitUnitLabel(MachineUnitType unitType)
        {
            base.InitUnitLabel(unitType);
            UnitNameText = "Click right mouse button for menu";
        }
        #endregion

        #region Handlers
        private void DsiplayPanelUnit_CancelProcess(Guid machineID)
        {
            if (machineID == MachineID)
            {
                Invoke(new Action(() =>
                {
                    UnitNameText = $"Process cancelled.";
                    LoadContextMenu(false);
                }));                
            }

        }

        private void DisplayPanelUnit_Finish(Guid machineID)
        {
            if (machineID == MachineID)
            {
                Invoke(new Action(() =>
                {
                    UnitNameText = $"Process finished.";
                    LoadContextMenu(false);
                }));
            }
        }
        private void DisplayPanelUnit_ActionExecuted(Guid machineID, WashingModes.WashingActions action)
        {
            if (machineID == MachineID)
            {
                Invoke(new Action(() =>
                {
                    UnitNameText = $"Executing {action}";
                }));
            }
        }

        private void DisplayPanelUnit_ActionExecutionFinished(Guid machineID, WashingModes.WashingActions action)
        {
            if (machineID == MachineID)
            {
                Invoke(new Action(() =>
                {
                    UnitNameText = $"{action} finished.";
                }));
            }
        }

        private void UnitContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LoadContextMenu(InteropMessenger.Instance.FireIsWashingStartedMessage(MachineID));
        }

        private void MenuItemStart_Click(object sender, EventArgs e)
        {
            InteropMessenger.Instance.FireStartWashingMessage(MachineID, SelectedWashMode);
        }
        #endregion
    }
}
