using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WashingMachine
{
    public class PumpUnit : SwitchableUnitBase
    {
        #region Construction
        public PumpUnit(Guid machineID) : base(machineID, MachineUnitType.Pump, MachineUnitImageType.PumpOn, MachineUnitImageType.PumpOff)
        {
            InteropMessenger.Instance.PumpWater += PumpUnit_PumpWater;
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
        private void PumpUnit_PumpWater(Guid MachineID)
        {
            IsOn = true;
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                InteropMessenger.Instance.FireActionExecutionFinishedMessage(MachineID, WashingModes.WashingActions.PumpWater);
                IsOn = false;
            });
        }

        #endregion
    }
}
