using System;
using System.Drawing;
using System.Windows.Forms;

namespace WashingMachine
{
    public abstract class SwitchableUnitBase : BaseUnit
    {
        #region Definitions
        public class ActiveStateChangedEventArgs : EventArgs
        {
            #region Properties
            public bool IsOn { get; set; }
            #endregion

            #region Construction
            public ActiveStateChangedEventArgs(bool isOn) { IsOn = isOn; }
            #endregion
        }
        #endregion

        #region Delegates
        public delegate void ActiveStateChangedEventHandler(object sender, EventArgs e);
        #endregion

        #region Events
        public event ActiveStateChangedEventHandler ActiveStateChanged;
        #endregion

        #region Properties
        private bool isOn = false;
        public virtual bool IsOn
        {
            get { return isOn; }
            set
            {
                isOn = value;
                SwitchState();
                OnStateChanged(isOn);
            }
        }

        protected MachineUnitImageType ActivatedUnitType { get; set; }
        protected MachineUnitImageType DeactivatedUnitType { get; set; }
        #endregion  

        #region Construction
        public SwitchableUnitBase(Guid machineID, MachineUnitType unitType, MachineUnitImageType activatedUnitType, MachineUnitImageType deactivetedUnitType) : base(machineID, unitType, deactivetedUnitType)
        {
            AllowUserClick = true;

            ActivatedUnitType = activatedUnitType;
            DeactivatedUnitType = deactivetedUnitType;
        }
        #endregion

        #region Overrides
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            IsOn = !IsOn;
        }
        #endregion

        #region Methods
        protected virtual void SwitchState()
        {
            try
            {
                UnitImageType = isOn ? ActivatedUnitType : DeactivatedUnitType;
                UnitImage = ImageLibrary.Instance[UnitImageType];
                UnitNameColor = isOn ? Color.Green : Color.Red;
                UnitNameText = UnitType.ToString();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }
        #endregion

        #region Handlers
        protected virtual void OnStateChanged(bool opened)
        {
            ActiveStateChanged?.Invoke(this, new ActiveStateChangedEventArgs(opened));
        }
        #endregion   
    }
}
