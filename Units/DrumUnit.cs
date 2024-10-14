using System;
using System.Drawing;
using System.Threading.Tasks;

namespace WashingMachine
{
    public class DrumUnit : SwitchableUnitBase
    {
        #region Definitions
        public class DrumStateChangedEventArgs : EventArgs
        {
            #region Properties
            public bool HasWater { get; set; }
            #endregion

            #region Construction
            public DrumStateChangedEventArgs(bool hasWarter) { HasWater = hasWarter; }
            #endregion
        }
        #endregion

        #region Delegates
        public delegate void DrumStateChangedEventHandler(object sender, EventArgs e);
        #endregion

        #region Events
        public event DrumStateChangedEventHandler DrumStateChanged;
        #endregion

        #region Properties
        protected bool hasWater = false;
        public bool HasWater
        {
            get { return hasWater; }
            set
            {
                hasWater = value;
                SwitchWaterState();
                OnWaterStateChanged(hasWater);
            }
        }
        #endregion

        #region Construction
        public DrumUnit(Guid machineID) : base(machineID, MachineUnitType.Drum, MachineUnitImageType.DrumWithWater, MachineUnitImageType.DrumWithoutWater)
        {
            AllowUserClick = false;

            InteropMessenger.Instance.ActionExecutionFinished += DrumUnit_ActionExecutionFinished;
            InteropMessenger.Instance.WaterTemperatureChange += DrumUnit_WaterTemperatureChange;
            InteropMessenger.Instance.CancelProcess += DrumUnit_CancelProcess;
            InteropMessenger.Instance.Drain += DrumUnit_Drain;
        }
        #endregion

        #region Methods
        protected virtual string GetWaterLabelText()
        {
            return hasWater ? "is filled with water" : "is empty";
        }
        protected virtual void SwitchWaterState()
        {
            try
            {
                Invoke(new Action(() =>
                {
                    UnitImageType = hasWater ? ActivatedUnitType : DeactivatedUnitType;
                    UnitImage = ImageLibrary.Instance[UnitImageType];
                    UnitNameColor = hasWater ? Color.Green : Color.Red;
                    UnitNameText = $"{UnitType} {GetWaterLabelText()}";
                }));
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }
        #endregion

        #region Overrides
        protected override void InitUnitLabel(MachineUnitType unitType)
        {
            base.InitUnitLabel(unitType);
            UnitNameColor = hasWater ? Color.Green : Color.Red;
            UnitNameText = $"{UnitType} {GetWaterLabelText()}";
        }

        protected override void SwitchState()
        {
            try
            {
                // Hiding original switch state based on IsOn property
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }
        protected override void UpdateUnitState(EventArgs e)
        {
        }
        #endregion

        #region Handlers
        private void DrumUnit_CancelProcess(Guid machineID)
        {
            try
            {
                if (machineID == MachineID)
                {
                    TokenSource.Cancel();
                    HasWater = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }
        protected virtual void OnWaterStateChanged(bool hasWater)
        {
            DrumStateChanged?.Invoke(this, new DrumStateChangedEventArgs(hasWater));
        }
        private void DrumUnit_WaterTemperatureChange(Guid machineID, int temperature)
        {
            try
            {
                Invoke(new Action(() =>
                {
                    UnitNameText = $"{UnitType} {GetWaterLabelText()} ({temperature}°)";
                }));
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }
        private void DrumUnit_Drain(Guid machineID)
        {
            try
            {
                Invoke(new Action(() =>
                {
                    UnitNameText = $"{UnitType} draining water";
                }));
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }
        private void DrumUnit_ActionExecutionFinished(Guid machineID, WashingModes.WashingActions action)
        {
            if (machineID == MachineID)
            {
                switch (action)
                {
                    case WashingModes.WashingActions.PumpWater:
                        HasWater = true;
                        break;
                    case WashingModes.WashingActions.Drain:
                        HasWater = false;
                        break;
                }
            }
        }
        #endregion
    }
}
