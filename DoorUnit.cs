using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace WashingMachine
{
    public class DoorUnit : BaseUnit
    {
        #region Definitions
        public class DoorStateChangedEventArgs : EventArgs
        {
            #region Properties
            public bool Opened { get; set; }
            #endregion

            #region Construction
            public DoorStateChangedEventArgs(bool opened) { Opened = opened; }
            #endregion
        }
        #endregion

        #region Delegates
        public delegate void DoorStateChangedEventHandler(object sender, EventArgs e);
        #endregion

        #region Events
        public event DoorStateChangedEventHandler DoorStateChanged;
        #endregion

        #region Properties
        private bool doorOpened = true;
        public bool Opened
        {
            get { return doorOpened; }
            set
            {
                doorOpened = value;
                SwitchDoorState();
                OnDoorStateChanged(doorOpened);
            }
        }

        private void SwitchDoorState()
        {
            try
            {
                UnitType = doorOpened ? MachineUnitType.DoorOpened : MachineUnitType.DoorClosed;
                UnitImage = ImageLibrary.Instance[UnitType];
                UnitNameColor = doorOpened ? Color.Red : Color.Green;
                UnitNameText = UnitType.ToString();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }
        #endregion

        #region Construction
        public DoorUnit(Point location, Size size) : base(MachineUnitType.DoorOpened, location, size)
        {
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
            Opened = !Opened;
        }

        protected override void UpdateUnitState(EventArgs e)
        {

        }
        #endregion

        #region Handlers

        protected virtual void OnDoorStateChanged(bool opened)
        {
            DoorStateChanged?.Invoke(this, new DoorStateChangedEventArgs(opened));
        }
        #endregion
    }
}
