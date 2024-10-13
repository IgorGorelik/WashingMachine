using System;
using System.Drawing;

namespace WashingMachine
{
    public class HeaterUnit : SwitchableUnitBase
    {
        #region Construction
        public HeaterUnit() : base(MachineUnitType.Heater, MachineUnitImageType.HeaterOn, MachineUnitImageType.HeaterOff)
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
