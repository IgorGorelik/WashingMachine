using System;
using System.Drawing;
using System.Windows.Forms;

namespace WashingMachine
{
    public class PowerSupplyUnit : SwitchableUnitBase
    {

        #region Construction
        public PowerSupplyUnit(Guid machineID) : base(machineID, MachineUnitType.PowerSupply, MachineUnitImageType.PowerSupplyOn, MachineUnitImageType.PowerSupplyOff)
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

        #endregion

        #region Handlers
        #endregion    
    }
}
