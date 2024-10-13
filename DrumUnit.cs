using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WashingMachine.DoorUnit;
using static WashingMachine.DrumUnit;

namespace WashingMachine
{
    public class DrumUnit : BaseUnit
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
        private bool hasWater = false;
        public bool HasWater
        {
            get { return hasWater; }
            set
            {
                hasWater = value;
                SwitchWaterState();
                OnDoorStateChanged(hasWater);
            }
        }
        #endregion

        #region Construction
        public DrumUnit(Size size) : base(MachineUnitType.Drum, size)
        {
        }
        #endregion

        #region Methods
        public virtual void SwitchWaterState()
        {
            UnitType = HasWater ? MachineUnitType.DrumWithWater : MachineUnitType.DrumWithoutWater;
            UnitImage = ImageLibrary.Instance[UnitType];
            UnitNameColor = HasWater ? Color.Green : Color.Blue;
            UnitNameText = UnitType.ToString();
        }

        #endregion

        #region Overrides
        protected override void UpdateUnitState(EventArgs e)
        {
        }
        #endregion

        #region Handlers
        protected virtual void OnDoorStateChanged(bool hasWater)
        {
            DrumStateChanged?.Invoke(this, new DrumStateChangedEventArgs(hasWater));
        }
        #endregion
    }
}
