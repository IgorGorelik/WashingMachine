using System;
using System.Drawing;
using System.Threading.Tasks;

namespace WashingMachine
{
    public class HeaterUnit : SwitchableUnitBase
    {
        #region Propeties
        public int Temperature { get; set; }
        #endregion
        #region Construction
        public HeaterUnit(Guid machineID) : base(machineID, MachineUnitType.Heater, MachineUnitImageType.HeaterOn, MachineUnitImageType.HeaterOff)
        {
            InteropMessenger.Instance.HeatWater += HeaterUnit_HeatWater;
            InteropMessenger.Instance.CancelProcess += HeaterUnit_CancelProcess;
        }
        #endregion

        #region Methods
        protected void ExecuteHeater()
        {
            Task.Run(async () =>
            {
                int currentTemperature = 0;
                while (!TokenSource.IsCancellationRequested && (currentTemperature < Temperature))
                {
                    currentTemperature += 5;
                    InteropMessenger.Instance.FireWaterTemperatureChangeMessage(MachineID, currentTemperature);

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                await Task.Delay(TimeSpan.FromSeconds(2));
                InteropMessenger.Instance.FireActionExecutionFinishedMessage(MachineID, WashingModes.WashingActions.Heat);
                IsOn = false;
            });
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
        private void HeaterUnit_CancelProcess(Guid machineID)
        {
            try
            {
                if (machineID == MachineID)
                {
                    TokenSource.Cancel();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }
        private void HeaterUnit_HeatWater(Guid machineID)
        {
            IsOn = true;
            ExecuteHeater();
        }
        #endregion    
    }
}
