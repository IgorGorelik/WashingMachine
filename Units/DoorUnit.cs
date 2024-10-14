using System;
using System.Drawing;
using System.Windows.Forms;

namespace WashingMachine
{
    public class DoorUnit : OpenableUnitBase
    {
        #region Properties
        #endregion

        #region Construction
        public DoorUnit(Guid machineID) : base(machineID, MachineUnitType.Door, MachineUnitImageType.DoorOpened, MachineUnitImageType.DoorClosed)
        {
        }
        #endregion

        #region Overrides
        protected override void InitUnitLabel(MachineUnitType unitType)
        {
            base.InitUnitLabel(unitType);
            UnitNameColor = Color.Red;
        }

        protected override void UpdateUnitState(EventArgs e)
        {

        }

        public override bool CanChangeState
        {
            get
            {
                return !InteropMessenger.Instance.FireIsWashingStartedMessage(MachineID);
            }
        }
        #endregion
    }
}
