using System;
using System.Drawing;

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
        public DrumUnit() : base(MachineUnitType.Drum, MachineUnitImageType.DrumWithWater, MachineUnitImageType.DrumWithoutWater)
        {
            AllowUserClick = false;
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
                UnitImageType = hasWater ? ActivatedUnitType : DeactivatedUnitType;
                UnitImage = ImageLibrary.Instance[UnitImageType];
                UnitNameColor = hasWater ? Color.Green : Color.Red;
                UnitNameText = $"{UnitType} {GetWaterLabelText()}";
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
        protected virtual void OnWaterStateChanged(bool hasWater)
        {
            DrumStateChanged?.Invoke(this, new DrumStateChangedEventArgs(hasWater));
        }

        #endregion
    }
}
