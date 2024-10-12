using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WashingMachine.SwitchableUnit;

namespace WashingMachine
{
    public abstract class BaseUnit : PictureBox
    {
        #region Enums
        public enum MachineUnitType
        {
            PowerSupply,
            ControlUnit,
            WaterInletValve,
            Drum,
            WashMotor,
            SpinMotor,
            Heater,
            DetergentDispenser,
            Pump,
            DisplayPanel
        }
        #endregion
        #region Properties
        public MachineUnitType UnitType { get; set; }
        #endregion

        #region Construction
        public BaseUnit(MachineUnitType unitType, Point location, Size size)
        {
            SizeMode = PictureBoxSizeMode.Zoom;
            UnitType = unitType;
            Location = location;
            Size = size;

            Image = ImageLibrary.Instance[unitType];
            Logger.Instance.LogInformation($"Unit '{UnitType}' initialized.");
        }
        #endregion
    }

    public abstract class SwitchableUnit : BaseUnit
    {
        #region Definitions
        public class OperationStateChangedEventArgs : EventArgs
        {
            #region Properties
            public TimeSpan TimeRemaining { get; set; }
            #endregion

            #region Construction
            public OperationStateChangedEventArgs(TimeSpan timeRemaining) { TimeRemaining = timeRemaining; }
            #endregion
        }
        #endregion

        #region Delegates
        public delegate void ExecutionFinishedEventHandler(object sender, EventArgs e);
        public delegate void OperationFaultEventHandler(object sender, EventArgs e);
        public delegate void OperationStateChangedHandler(object sender, EventArgs e);
        #endregion

        #region Events
        public event ExecutionFinishedEventHandler ExecutionFinished;
        public event OperationFaultEventHandler OperationFault;
        public event OperationStateChangedHandler OperationStateChanged;
        #endregion

        #region Properties
        public bool TurnedOn { get; set; } = false;
        protected CancellationTokenSource TokenSource = default;
        #endregion

        #region Construction
        public SwitchableUnit(MachineUnitType unitType, Point location, Size size) : base(unitType, location, size)
        {
            TokenSource = new CancellationTokenSource();
        }
        #endregion

        #region Methods
        public void CancelOperation()
        {
            TokenSource.Cancel();
        }
        #endregion

        #region Handlers
        protected void OnExecutionFinished()
        {
            ExecutionFinished?.Invoke(this, new EventArgs());
        }

        protected void OnOperationFault()
        {
            OperationFault?.Invoke(this, new EventArgs());
        }

        protected void OnOperationStateChanged(EventArgs args)
        {
            OperationStateChanged?.Invoke(this, args);
        }
        #endregion

        #region Overrides
        #endregion
    }
}