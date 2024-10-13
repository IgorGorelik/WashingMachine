using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static WashingMachine.BaseUnit;

namespace WashingMachine
{
    internal class WashingMachine : TableLayoutPanel
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
        }
        #endregion

        #region Constants
        protected readonly string layoutFileName = "UnitsLayout.json";
        #endregion

        #region Properties
        public Guid MachineID { get; set; } = Guid.NewGuid();
        private ConcurrentDictionary<BaseUnit.MachineUnitType, UnitInfo> UnitInfoList = new ConcurrentDictionary<BaseUnit.MachineUnitType, UnitInfo>();
        private ConcurrentDictionary<BaseUnit.MachineUnitType, BaseUnit> UnitList = new ConcurrentDictionary<BaseUnit.MachineUnitType, BaseUnit>();
        #endregion

        #region Construction
        public WashingMachine()
        {
            InitUnitsLayout();
            InitLayoutFromFile();
            InitializeUnits();

            InitializeMessengerEvents();
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
                    var unit = CreateUnitByType(unitInfo.Key, unitInfo.Value.InitialState);
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

        protected virtual BaseUnit CreateUnitByType(MachineUnitType unitType, UnitInitialState initialState)
        {
            switch (unitType)
            {
                case MachineUnitType.PowerSupply:
                    return new PowerSupplyUnit(MachineID) { IsOn = initialState == UnitInitialState.On };

                case MachineUnitType.ControlUnit:
                    break;

                case MachineUnitType.WaterInletValve:
                    return new WaterInletValveUnit(MachineID) { IsOn = initialState == UnitInitialState.On };

                case MachineUnitType.Drum:
                    return new DrumUnit(MachineID) { HasWater = initialState == UnitInitialState.Filled };

                case MachineUnitType.WashMotor:
                    return new WashMotorUnit(MachineID) { TurnedOn = initialState == UnitInitialState.On };

                case MachineUnitType.SpinMotor:
                    return new SpinMotorUnit(MachineID) { TurnedOn = initialState == UnitInitialState.On };

                case MachineUnitType.Heater:
                    return new HeaterUnit(MachineID) { IsOn = initialState == UnitInitialState.On };

                case MachineUnitType.DetergentDispenser:
                    return new DetergentDispenserUnit(MachineID);

                case MachineUnitType.Pump:
                    return new PumpUnit(MachineID) { IsOn = initialState == UnitInitialState.On };

                case MachineUnitType.DisplayPanel:
                    return new DisplayPanelUnit(MachineID);

                case MachineUnitType.Door:
                    return new DoorUnit(MachineID) { Opened = initialState == UnitInitialState.Opened };
            }

            return default;
        }
        private void InitializeMessengerEvents()
        {
            InteropMessenger.Instance.StartWashing += Instance_StartWashing;
        }
        #endregion

        #region Handlers
        private void Instance_StartWashing(Guid MachineID)
        {
            (UnitList[MachineUnitType.WashMotor] as WashMotorUnit).ExecuteCycle();
        }

        #endregion
    }
}
