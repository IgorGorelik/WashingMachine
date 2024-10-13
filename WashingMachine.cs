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
        private ConcurrentDictionary<BaseUnit.MachineUnitType, UnitInfo> UnitList = new ConcurrentDictionary<BaseUnit.MachineUnitType, UnitInfo>();
        #endregion

        #region Construction
        public WashingMachine()
        {
            InitUnitsLayout();
            InitLayoutFromFile();
            InitializeUnits();
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
                foreach (var unitInfo in UnitList)
                {
                    this.Controls.Add(CreateUnitByType(unitInfo.Key, unitInfo.Value.InitialState), unitInfo.Value.Location.X, unitInfo.Value.Location.Y);
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
                string jsonFilePath = Path.Combine(Application.StartupPath, layoutFileName);
                using (var reader = new StreamReader(jsonFilePath))
                {
                    string jsonString = reader.ReadToEnd();
                    UnitList = JsonConvert.DeserializeObject<ConcurrentDictionary<BaseUnit.MachineUnitType, UnitInfo>>(jsonString);
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
                    return new PowerSupplyUnit() { IsOn = initialState == UnitInitialState.On };

                case MachineUnitType.ControlUnit:
                    break;

                case MachineUnitType.WaterInletValve:
                    return new WaterInletValveUnit() { IsOn = initialState == UnitInitialState.On };

                case MachineUnitType.Drum:
                    return new DrumUnit() { HasWater = initialState == UnitInitialState.Filled };

                case MachineUnitType.WashMotor:
                    return new WashMotorUnit() { TurnedOn = initialState == UnitInitialState.On };

                case MachineUnitType.SpinMotor:
                    return new SpinMotorUnit() { TurnedOn = initialState == UnitInitialState.On };

                case MachineUnitType.Heater:
                    return new HeaterUnit() { IsOn = initialState == UnitInitialState.On };

                case MachineUnitType.DetergentDispenser:
                    return new DetergentDispenserUnit();

                case MachineUnitType.Pump:
                    return new PumpUnit() { IsOn = initialState == UnitInitialState.On };

                case MachineUnitType.DisplayPanel:
                    return new DisplayPanelUnit();

                case MachineUnitType.Door:
                    return new DoorUnit() { Opened = initialState == UnitInitialState.Opened };
            }

            return default;
        }
        #endregion
    }
}
