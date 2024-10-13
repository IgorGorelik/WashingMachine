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
            Door
        }
        public enum MachineUnitImageType
        {
            PowerSupplyOn,
            PowerSupplyOff,
            ControlUnit,
            WaterInletValveOn,
            WaterInletValveOff,
            WashMotor,
            SpinMotor,
            HeaterOn,
            HeaterOff,
            DetergentDispenser,
            PumpOn,
            PumpOff,
            DisplayPanel,
            DoorOpened,
            DoorClosed,
            DrumWithWater,
            DrumWithoutWater
        }
        #endregion

        #region Properties
        public MachineUnitType UnitType { get; protected set; }
        public MachineUnitImageType UnitImageType { get; protected set; }
        protected bool AllowUserClick = false;
        private PictureBox UnitImageBox = new PictureBox();
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
        protected Image UnitImage
        {
            set { UnitImageBox.Image = value; }
        }
        protected ContextMenuStrip UnitContextMenu
        {
            get { return UnitNameTableLayoutPanel.ContextMenuStrip; }
            set { UnitNameTableLayoutPanel.ContextMenuStrip = value; }
        }
        #endregion

        #region Construction
        public BaseUnit(MachineUnitType unitType, MachineUnitImageType unitImageType)
        {
            Dock = DockStyle.Fill;
            UnitType = unitType;
            UnitImageType = unitImageType;

            InitUnitLayout();
            this.Controls.Add(UnitNameTableLayoutPanel);

            InitUnitImage(UnitImageType);
            InitUnitLabel(unitType);

            UnitNameTableLayoutPanel.Controls.Add(UnitImageBox, 0, 0);
            UnitNameTableLayoutPanel.Controls.Add(UnitNameLabel, 0, 1);

            UnitImageBox.MouseDoubleClick += Handle_MouseDoubleClick;
            UnitNameLabel.MouseDoubleClick += Handle_MouseDoubleClick;

            this.SizeChanged += BaseUnit_SizeChanged;
            Logger.Instance.LogInformation($"Unit '{UnitType}' initialized.");
        }
        #endregion

        #region Methods
        protected abstract void UpdateUnitState(EventArgs e);
        protected virtual void InitUnitLayout()
        {
            UnitNameTableLayoutPanel.Dock = DockStyle.Fill;
            UnitNameTableLayoutPanel.RowCount = 2;
            UnitNameTableLayoutPanel.ColumnCount = 1;
            UnitNameTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 70));
            UnitNameTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            UnitNameTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        }
        protected virtual void InitUnitLabel(MachineUnitType unitType)
        {
            UnitNameLabel.BackColor = Color.Transparent;
            UnitNameLabel.ForeColor = Color.Blue;
            UnitNameLabel.Font = new Font("Arial", 12, FontStyle.Italic | FontStyle.Bold);
            UnitNameLabel.Text = unitType.ToString();
            UnitNameLabel.TextAlign = ContentAlignment.MiddleCenter;
            UnitNameLabel.AutoSize = true;
            UnitNameLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            UnitNameLabel.Dock = DockStyle.Fill;
        }
        protected virtual void InitUnitImage(MachineUnitImageType unitType)
        {
            UnitImageBox.SizeMode = PictureBoxSizeMode.Zoom;
            UnitImageBox.Dock = DockStyle.Fill;
            UnitImageBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            UnitImageBox.Image = ImageLibrary.Instance[unitType];
        }
        #endregion

        #region Handlers
        private void Handle_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (AllowUserClick)
            {
                OnMouseDoubleClick(e);
            }
        }

        private void BaseUnit_SizeChanged(object sender, EventArgs e)
        {
            UnitImageBox.Size = this.Size;
        }
        #endregion
    }

    public abstract class TurnableUnit : BaseUnit
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
        public TurnableUnit(MachineUnitType unitType, MachineUnitImageType unitImageType) : base(unitType, unitImageType)
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
        protected virtual void OnExecutionFinished()
        {
            OperationFinished?.Invoke(this, new EventArgs());
        }

        protected virtual void OnOperationFault()
        {
            OperationFault?.Invoke(this, new EventArgs());
        }

        protected virtual void OnOperationStateChanged(EventArgs args)
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