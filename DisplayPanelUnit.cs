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
        public DisplayPanelUnit() : base(MachineUnitType.DisplayPanel, MachineUnitImageType.DisplayPanel)
        {
            InitContentMenu();
        }
        #endregion

        #region Methods
        private void InitContentMenu()
        {
            UnitContextMenu = new ContextMenuStrip();
            UnitContextMenu.Opening += UnitContextMenu_Opening;
        }

        private void LoadContextMenu()
        {
            try
            {
                UnitContextMenu.Items.Clear();
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
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }

        protected void RefreshLabelText()
        {
            UnitNameText = "Click right mouse button for menu";
            if(!string.IsNullOrEmpty(SelectedWashMode))
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
        private void UnitContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LoadContextMenu();
        }

        private void MenuItemStart_Click(object sender, EventArgs e)
        {

        }
        #endregion
    }
}
