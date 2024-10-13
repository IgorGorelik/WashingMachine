using System;
using System.Drawing;
using System.Windows.Forms;

namespace WashingMachine
{
    public class DisplayPanelUnit : BaseUnit
    {
        #region Construction
        public DisplayPanelUnit() : base(MachineUnitType.DisplayPanel, MachineUnitImageType.DisplayPanel)
        {
            UnitContextMenu = new ContextMenu();

            MenuItem menuItem1 = new MenuItem("Rotate 90°");
            menuItem1.Click += (sender, e) =>
            {
                // Implement your logic for rotating the image by 90 degrees
                // (e.g., change the image displayed in the PictureBox)
            };
            UnitContextMenu.MenuItems.Add(menuItem1);
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

        #endregion
    }
}
