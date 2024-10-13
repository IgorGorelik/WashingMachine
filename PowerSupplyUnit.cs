using System.Drawing;
using System.Windows.Forms;
using System;

namespace WashingMachine
{
    public class PowerSupplyUnit : BaseUnit
    {
        #region Definitions
        public class PowerSupplyStateChangedEventArgs : EventArgs
        {
            #region Properties
            public bool IsOn { get; set; }
            #endregion

            #region Construction
            public PowerSupplyStateChangedEventArgs(bool isOn) { IsOn = isOn; }
            #endregion
        }
        #endregion

        #region Delegates
        public delegate void PowerSupplyStateChangedEventHandler(object sender, EventArgs e);
        #endregion

        #region Events
        public event PowerSupplyStateChangedEventHandler PowerSupplyStateChanged;
        #endregion

        #region Properties
        private bool isOn = true;
        public bool IsOn
        {
            get { return isOn; }
            set
            {
                isOn = value;
                SwitchPowerSupplyState();
                OnPowerSupplyStateChanged(isOn);
            }
        }

        private void SwitchPowerSupplyState()
        {
            try
            {
                UnitType = isOn ? MachineUnitType.PowerSupplyOn : MachineUnitType.PowerSupplyOff;
                UnitImage = ImageLibrary.Instance[UnitType];
                UnitNameColor = isOn ? Color.Green : Color.Red;
                UnitNameText = UnitType.ToString();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }
        #endregion

        #region Construction
        public PowerSupplyUnit(Size size) : base(MachineUnitType.PowerSupplyOff, size)
        {
            AllowUserClick = true;
        }
        #endregion

        #region Overrides
        protected override void InitUnitLabel(MachineUnitType unitType)
        {
            base.InitUnitLabel(unitType);
            UnitNameColor = Color.Red;
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            IsOn = !IsOn;
        }

        protected override void UpdateUnitState(EventArgs e)
        {

        }
        #endregion

        #region Handlers
        protected virtual void OnPowerSupplyStateChanged(bool opened)
        {
            PowerSupplyStateChanged?.Invoke(this, new PowerSupplyStateChangedEventArgs(opened));
        }
        #endregion    
    }
}
