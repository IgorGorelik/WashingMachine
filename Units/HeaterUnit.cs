using System;
using System.Drawing;

namespace WashingMachine
{
    public class HeaterUnit : SwitchableUnitBase
    {
        #region Construction
        public HeaterUnit(Guid machineID) : base(machineID, MachineUnitType.Heater, MachineUnitImageType.HeaterOn, MachineUnitImageType.HeaterOff)
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
