using Serilog.Parsing;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace WashingMachine
{
    public abstract class BaseUnit : Control
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
            DisplayPanel,
            DoorOpened,
            DoorClosed
        }
        #endregion
        #region Properties
        public MachineUnitType UnitType { get; set; }
        private PictureBox UnitImage = new PictureBox();
        private Label UnitNameLabel = new Label();
        private TableLayoutPanel UnitNameTableLayoutPanel = new TableLayoutPanel();
        protected string UnitNameText
        {
            get { return UnitNameLabel.Text; }
            set { UnitNameLabel.Text = value; }
        }
        protected Color UnitNameColor
        {
            get { return UnitNameLabel.ForeColor; }
            set { UnitNameLabel.ForeColor = value; }
        }
        #endregion

        #region Construction
        public BaseUnit(MachineUnitType unitType, Point location, Size size)
        {
            Dock = DockStyle.Fill;
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            UnitType = unitType;
            Size = size;

            UnitNameTableLayoutPanel.Dock = DockStyle.Fill;
            UnitNameTableLayoutPanel.RowCount = 2;
            UnitNameTableLayoutPanel.ColumnCount = 1;
            UnitNameTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 70));
            UnitNameTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            UnitNameTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            this.Controls.Add(UnitNameTableLayoutPanel);

            UnitImage.SizeMode = PictureBoxSizeMode.Zoom;
            UnitImage.Dock = DockStyle.Fill;
            UnitImage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            UnitImage.Size = size;
            UnitImage.Image = ImageLibrary.Instance[unitType];

            #region Label initialization
            UnitNameLabel.BackColor = Color.Transparent;
            UnitNameLabel.ForeColor = Color.Blue;
            UnitNameLabel.Font = new Font("Arial", 12, FontStyle.Italic | FontStyle.Bold);
            UnitNameLabel.Text = unitType.ToString();
            UnitNameLabel.TextAlign = ContentAlignment.MiddleCenter;
            UnitNameLabel.AutoSize = true;
            UnitNameLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            UnitNameLabel.Dock = DockStyle.Fill;

            UnitNameTableLayoutPanel.Controls.Add(UnitImage, 0, 0);
            UnitNameTableLayoutPanel.Controls.Add(UnitNameLabel, 0, 1);
            #endregion

            Logger.Instance.LogInformation($"Unit '{UnitType}' initialized.");
        }
        #endregion

        #region Methods
        protected abstract void UpdateUnitState(EventArgs e);
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
        public delegate void OperationFinishedEventHandler(object sender, EventArgs e);
        public delegate void OperationFaultEventHandler(object sender, EventArgs e);
        public delegate void OperationStateChangedHandler(object sender, EventArgs e);
        #endregion

        #region Events
        public event OperationFinishedEventHandler OperationFinished;
        public event OperationFaultEventHandler OperationFault;
        public event OperationStateChangedHandler OperationStateChanged;
        #endregion

        #region Properties
        protected bool turnedOn = false;
        public bool TurnedOn
        {
            get { return turnedOn; }
            set
            {
                turnedOn = value;
                UnitNameColor = turnedOn ? Color.Green : Color.Blue;
            }
        }
        protected CancellationTokenSource TokenSource = default;
        #endregion

        #region Construction
        public SwitchableUnit(MachineUnitType unitType, Point location, Size size) : base(unitType, location, size)
        {
            TokenSource = new CancellationTokenSource();
            this.OperationStateChanged += SwitchableUnit_OperationStateChanged;
            this.OperationFinished += SwitchableUnit_OperationFinished;
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
            OperationFinished?.Invoke(this, new EventArgs());
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
        private void SwitchableUnit_OperationStateChanged(object sender, EventArgs e)
        {
            UpdateUnitState(e);
        }

        private void SwitchableUnit_OperationFinished(object sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                TurnedOn = false;
                UnitNameText = $"{UnitType.ToString()} finished";
            }));
        }
        #endregion
    }
}