using CSCore.Codecs;
using CSCore.SoundOut;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WashingMachine.BaseUnit;
using static WashingMachine.OpenableUnitBase;
using static WashingMachine.SwitchableUnitBase;

namespace WashingMachine
{
    public class WashingMachine : TableLayoutPanel
    {
        #region Enums
        public enum UnitInitialState
        {
            On,
            Off,
            Opened,
            Closed,
            Empty,
            Filled,
            Irrelevant
        }
        #endregion

        #region Definitions
        public class UnitInfo
        {
            public Point Location { get; set; }
            public Type ClassType { get; set; }
            public UnitInitialState InitialState { get; set; }
            public bool Enabled { get; set; }
        }
        #endregion

        #region Constants
        protected readonly string layoutFileName = "UnitsLayout.json";
        #endregion

        #region Properties
        public Guid MachineID { get; set; } = Guid.NewGuid();
        private ConcurrentDictionary<BaseUnit.MachineUnitType, UnitInfo> UnitInfoList = new ConcurrentDictionary<BaseUnit.MachineUnitType, UnitInfo>();
        private ConcurrentDictionary<BaseUnit.MachineUnitType, BaseUnit> UnitList = new ConcurrentDictionary<BaseUnit.MachineUnitType, BaseUnit>();
        private List<WashingModes.WashingActions> WashModeActions = default;
        private string SelectedWashMode = string.Empty;
        private CancellationTokenSource TokenSource = default;
        #endregion

        #region Construction
        public WashingMachine()
        {
            InitUnitsLayout();
            InitLayoutFromFile();
            InitializeUnits();

            InitializeEvents();
        }
        #endregion

        #region Methods
        private void InitUnitsLayout()
        {
            try
            {
                this.Dock = DockStyle.Fill;

                this.RowCount = 3;
                this.ColumnCount = 4;

                this.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
                this.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
                this.RowStyles.Add(new RowStyle(SizeType.Percent, 34));

                this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
                this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
                this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
                this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }
        private void InitializeUnits()
        {
            try
            {
                foreach (var unitInfo in UnitInfoList)
                {
                    var unit = CreateUnitByType(unitInfo.Key, unitInfo.Value.InitialState, unitInfo.Value.Enabled);
                    UnitList[unitInfo.Key] = unit;

                    this.Controls.Add(unit, unitInfo.Value.Location.X, unitInfo.Value.Location.Y);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }
        private void InitLayoutFromFile()
        {

            try
            {
                string jsonFilePath = Path.Combine(Application.StartupPath, "Configuration", layoutFileName);
                using (var reader = new StreamReader(jsonFilePath))
                {
                    string jsonString = reader.ReadToEnd();
                    UnitInfoList = JsonConvert.DeserializeObject<ConcurrentDictionary<BaseUnit.MachineUnitType, UnitInfo>>(jsonString);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }

        protected virtual BaseUnit CreateUnitByType(MachineUnitType unitType, UnitInitialState initialState, bool enabled)
        {
            switch (unitType)
            {
                case MachineUnitType.PowerSupply:
                    return new PowerSupplyUnit(MachineID)
                    {
                        IsOn = initialState == UnitInitialState.On,
                        Enabled = enabled
                    };

                case MachineUnitType.ControlUnit:
                    break;

                case MachineUnitType.WaterInletValve:
                    return new WaterInletValveUnit(MachineID)
                    {
                        IsOn = initialState == UnitInitialState.On,
                        Enabled = enabled
                    };

                case MachineUnitType.Drum:
                    return new DrumUnit(MachineID)
                    {
                        HasWater = initialState == UnitInitialState.Filled,
                        Enabled = enabled
                    };

                case MachineUnitType.WashMotor:
                    return new WashMotorUnit(MachineID)
                    {
                        TurnedOn = initialState == UnitInitialState.On,
                        Enabled = enabled
                    };

                case MachineUnitType.SpinMotor:
                    return new SpinMotorUnit(MachineID)
                    {
                        TurnedOn = initialState == UnitInitialState.On,
                        Enabled = enabled
                    };

                case MachineUnitType.Heater:
                    return new HeaterUnit(MachineID)
                    {
                        IsOn = initialState == UnitInitialState.On,
                        Enabled = enabled
                    };

                case MachineUnitType.DetergentDispenser:
                    return new DetergentDispenserUnit(MachineID)
                    {
                        IsOn = initialState == UnitInitialState.On,
                        Enabled = enabled
                    };

                case MachineUnitType.Pump:
                    return new PumpUnit(MachineID)
                    {
                        IsOn = initialState == UnitInitialState.On,
                        Enabled = enabled
                    };

                case MachineUnitType.DisplayPanel:
                    return new DisplayPanelUnit(MachineID) { Enabled = enabled };

                case MachineUnitType.Door:
                    return new DoorUnit(MachineID)
                    {
                        Opened = initialState == UnitInitialState.Opened,
                        Enabled = enabled
                    };
            }

            return default;
        }
        private void InitializeEvents()
        {
            InteropMessenger.Instance.StartWashing += Handler_StartWashing;
            InteropMessenger.Instance.ActionExecutionFinished += Handler_ActionExecutionFinished;
            InteropMessenger.Instance.IsWashingStarted += Handler_IsWashingStarted;

            (UnitList[MachineUnitType.PowerSupply] as SwitchableUnitBase).ActiveStateChanged += PowerSupplyUnit_ActiveStateChanged;
            (UnitList[MachineUnitType.Door] as DoorUnit).OpenStateChanged += DoorUnit_OpenStateChanged;
        }
        #endregion

        #region Handlers
        private void DoorUnit_OpenStateChanged(object sender, OpenStateChangedEventArgs e)
        {
            var powerUnit = (UnitList[MachineUnitType.PowerSupply] as PowerSupplyUnit);
            var doorUnit = (UnitList[MachineUnitType.Door] as DoorUnit);

            InteropMessenger.Instance.FireEnableUnitMessage(MachineID, powerUnit.IsOn && !e.Opened);
            powerUnit.Enabled = true;
            doorUnit.Enabled = powerUnit.IsOn;
        }

        private void PowerSupplyUnit_ActiveStateChanged(object sender, ActiveStateChangedEventArgs e)
        {
            try
            {
                var doorUnit = (UnitList[MachineUnitType.Door] as DoorUnit);

                InteropMessenger.Instance.FireEnableUnitMessage(MachineID, e.IsOn && !doorUnit.Opened);
                doorUnit.Enabled = e.IsOn;
                UnitList[MachineUnitType.PowerSupply].Enabled = true;

                if (!e.IsOn)
                {
                    if (TokenSource != null)
                    {
                        TokenSource.Cancel();
                    }

                    InteropMessenger.Instance.FireCancelProcessMessage(MachineID);
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }

        private WashingModes.WashingActions MoveNextAction()
        {
            if (WashModeActions.Count == 0)
            {
                return WashingModes.WashingActions.Finish;
            }

            var currentAction = WashModeActions[0];
            WashModeActions.RemoveAt(0);

            return currentAction;
        }
        private void Handler_StartWashing(Guid machineID, string washMode)
        {
            try
            {
                if (MachineID == machineID)
                {
                    TokenSource = new CancellationTokenSource();
                    WashModeActions = WashingModes.Instance[washMode].Actions;
                    SelectedWashMode = washMode;

                    OperationActions.Instance.ExecuteAction(MoveNextAction(), MachineID);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }

        private bool Handler_IsWashingStarted(Guid machineID)
        {
            return (WashModeActions != null) && (WashModeActions.Count != 0) && !TokenSource.IsCancellationRequested;
        }

        private void Handler_ActionExecutionFinished(Guid MachineID, WashingModes.WashingActions action)
        {
            if(TokenSource == null)
            {
                TokenSource = new CancellationTokenSource();    
            }

            Task.Run(async () =>
            {
                try
                {
                    if (!TokenSource.IsCancellationRequested)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        var nextAction = MoveNextAction();

                        ApplyUnitValues(nextAction);

                        if (nextAction != WashingModes.WashingActions.Finish)
                        {
                            OperationActions.Instance.ExecuteAction(nextAction, MachineID);
                        }
                        else
                        {
                            InteropMessenger.Instance.FireFinishMessage(MachineID);
                            PlayFinishSound();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogError(ex);
                }
            });
        }

        private void PlayFinishSound()
        {
            try
            {
                var filePath = Path.Combine(Application.StartupPath, "Sounds", "Finish.wav");
                new SoundPlayer(filePath).Play();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }

        private void ApplyUnitValues(WashingModes.WashingActions nextAction)
        {
            try
            {
                var modeInfo = WashingModes.Instance[SelectedWashMode];

                switch (nextAction)
                {
                    case WashingModes.WashingActions.Wash:
                        {
                            var washUnit = (UnitList[MachineUnitType.WashMotor] as WashMotorUnit);
                            washUnit.ConfigureCycle(TimeSpan.FromSeconds(modeInfo.WashingCycleTime), modeInfo.WashingCycles);
                        }
                        break;


                    case WashingModes.WashingActions.Heat:
                        {
                            var heaterUnit = (UnitList[MachineUnitType.Heater] as HeaterUnit);
                            heaterUnit.Temperature = modeInfo.Temperature;
                        }
                        break;

                    case WashingModes.WashingActions.Spin:
                        {
                            var spinUnit = (UnitList[MachineUnitType.SpinMotor] as SpinMotorUnit);
                            spinUnit.ConfigureCycle(TimeSpan.FromSeconds(modeInfo.SpinCycle));
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }
        #endregion
    }
}
