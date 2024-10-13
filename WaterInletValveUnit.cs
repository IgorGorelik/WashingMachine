using System;
using System.Drawing;

namespace WashingMachine
{
    public class WaterInletValveUnit : SwitchableUnitBase
    {
        #region Construction
        public WaterInletValveUnit() : base(MachineUnitType.WaterInletValve, MachineUnitImageType.WaterInletValveOn, MachineUnitImageType.WaterInletValveOff)
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
