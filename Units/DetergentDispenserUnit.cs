using System;
using System.Drawing;
using System.Threading.Tasks;

namespace WashingMachine
{
    public class DetergentDispenserUnit : SwitchableUnitBase
    {
        #region Construction
        public DetergentDispenserUnit(Guid machineID) : base(machineID, MachineUnitType.DetergentDispenser, MachineUnitImageType.DetergentDispenserOn, MachineUnitImageType.DetergentDispenserOff)
        {
            InteropMessenger.Instance.DispenseDetegent += DispenserUnit_DispenseDetegent;
        }
        #endregion

        #region Overrides

        protected override void UpdateUnitState(EventArgs e)
        {

        }
        #endregion

        #region Handlers
        private void DispenserUnit_DispenseDetegent(Guid machineID)
        {
            IsOn = true;
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                InteropMessenger.Instance.FireActionExecutionFinishedMessage(MachineID, WashingModes.WashingActions.DispenseDetegent);
                IsOn = false;
            });
        }
        #endregion
    }
}
