using System;
using System.Drawing;
using System.Windows.Forms;

namespace WashingMachine
{
    public abstract class OpenableUnitBase : BaseUnit
    {
        #region Definitions
        public class OpenStateChangedEventArgs : EventArgs
        {
            #region Properties
            public bool Opened { get; set; }
            #endregion

            #region Construction
            public OpenStateChangedEventArgs(bool opened) { Opened = opened; }
            #endregion
        }
        #endregion

        #region Delegates
        public delegate void OpenStateChangedEventHandler(object sender, EventArgs e);
        #endregion

        #region Events
        public event OpenStateChangedEventHandler OpenStateChanged;
        #endregion

        #region Properties
        private bool unitOpened = true;
        public bool Opened
        {
            get { return unitOpened; }
            set
            {
                unitOpened = value;
                SwitchDoorState();
                OnDoorStateChanged(unitOpened);
            }
        }
        protected MachineUnitImageType OpenedUnitType { get; set; }
        protected MachineUnitImageType ClosedUnitType { get; set; }
        #endregion

        #region Construction
        public OpenableUnitBase(MachineUnitType unitType, MachineUnitImageType openedUnitType, MachineUnitImageType closedUnitType) : base(unitType, openedUnitType)
        {
            AllowUserClick = true;

            OpenedUnitType = openedUnitType;
            ClosedUnitType = closedUnitType;
        }
        #endregion

        #region Methods
        private void SwitchDoorState()
        {
            try
            {
                UnitImageType = unitOpened ? OpenedUnitType : ClosedUnitType;
                UnitImage = ImageLibrary.Instance[UnitImageType];
                UnitNameColor = unitOpened ? Color.Red : Color.Green;
                UnitNameText = UnitType.ToString();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }
        #endregion

        #region Handlers
        protected virtual void OnDoorStateChanged(bool opened)
        {
            OpenStateChanged?.Invoke(this, new OpenStateChangedEventArgs(opened));
        }
        #endregion

        #region Overrides
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            Opened = !Opened;
        }

        protected override void UpdateUnitState(EventArgs e)
        {
        }
        #endregion
    }
}
