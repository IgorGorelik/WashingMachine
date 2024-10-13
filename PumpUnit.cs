using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WashingMachine
{
    public class PumpUnit : SwitchableUnitBase
    {
        #region Construction
        public PumpUnit() : base(MachineUnitType.Pump, MachineUnitImageType.PumpOn, MachineUnitImageType.PumpOff)
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
