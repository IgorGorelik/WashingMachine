using System;

namespace WashingMachine
{
    public class DetergentDispenserUnit : BaseUnit
    {
        #region Construction
        public DetergentDispenserUnit() : base(MachineUnitType.DetergentDispenser, MachineUnitImageType.DetergentDispenser)
        {
        }
        #endregion

        #region Overrides

        protected override void UpdateUnitState(EventArgs e)
        {

        }
        #endregion

        #region Handlers

        #endregion
    }
}
