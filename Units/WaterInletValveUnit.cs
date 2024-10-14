using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace WashingMachine
{
    public class WaterInletValveUnit : SwitchableUnitBase
    {
        #region Construction
        public WaterInletValveUnit(Guid machineID) : base(machineID, MachineUnitType.WaterInletValve, MachineUnitImageType.WaterInletValveOn, MachineUnitImageType.WaterInletValveOff)
        {
            InteropMessenger.Instance.OpenWaterValve += WaterInletValveUnit_OpenWaterValve;
            InteropMessenger.Instance.Drain += WaterInletValveUnit_Drain;
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
        private void WaterInletValveUnit_Drain(Guid machineID)
        {
            try
            {
                IsOn = true;
                Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    InteropMessenger.Instance.FireActionExecutionFinishedMessage(MachineID, WashingModes.WashingActions.Drain);
                    IsOn = false;
                });
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }
        private void WaterInletValveUnit_OpenWaterValve(Guid MachineID)
        {
            IsOn = true;

            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                InteropMessenger.Instance.FireActionExecutionFinishedMessage(MachineID, WashingModes.WashingActions.OpenWaterValve);
                IsOn = false;
            });
        }
        #endregion    
    }
}
