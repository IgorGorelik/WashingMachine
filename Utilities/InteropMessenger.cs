using System;
using System.Runtime.Remoting.Messaging;

namespace WashingMachine
{
    public sealed class InteropMessenger
    {
        #region Delegates
        public delegate void StartWashingEventHandler(Guid machineID, string washMode);
        public delegate void EnableUnitEventHandler(Guid machineID, bool enable);
        public delegate void OpenWaterValveEventHandler(Guid machineID);
        public delegate void PumpWaterEventHandler(Guid machineID);
        public delegate void HeatWaterEventHandler(Guid machineID);
        public delegate void DispenseDetegentEventHandler(Guid machineID);
        public delegate void WashEventHandler(Guid machineID);
        public delegate void DrainEventHandler(Guid machineID);
        public delegate void SpinEventHandler(Guid machineID);
        public delegate void FinishEventHandler(Guid machineID);
        public delegate void ActionExecutedEventHandler(Guid machineID, WashingModes.WashingActions action);
        public delegate void ActionExecutionFinishedEventHandler(Guid machineID, WashingModes.WashingActions action);
        public delegate bool IsWashingStartedEventHandler(Guid machineID);
        public delegate void WaterTemperatureChangeEventHandler(Guid machineID, int temperature);
        public delegate void CancelProcessEventHandler(Guid machineID);
        #endregion

        #region Events
        public event StartWashingEventHandler StartWashing;
        public event EnableUnitEventHandler EnableUnit;
        public event OpenWaterValveEventHandler OpenWaterValve;
        public event PumpWaterEventHandler PumpWater;
        public event HeatWaterEventHandler HeatWater;
        public event DispenseDetegentEventHandler DispenseDetegent;
        public event WashEventHandler Wash;
        public event DrainEventHandler Drain;
        public event SpinEventHandler Spin;
        public event FinishEventHandler Finish;
        public event ActionExecutedEventHandler ActionExecuted;
        public event ActionExecutionFinishedEventHandler ActionExecutionFinished;
        public event IsWashingStartedEventHandler IsWashingStarted;
        public event WaterTemperatureChangeEventHandler WaterTemperatureChange;
        public event CancelProcessEventHandler CancelProcess;
        #endregion

        #region Properties
        private static readonly Lazy<InteropMessenger> singletonInstance = new Lazy<InteropMessenger>(() => new InteropMessenger());
        public static InteropMessenger Instance => singletonInstance.Value;
        #endregion

        #region Construction
        private InteropMessenger() { }
        public void Initialize()
        {
            
        }
        #endregion

        #region Fire message methods
        public void FireStartWashingMessage(Guid machineID, string washMode)
        {
            StartWashing?.Invoke(machineID, washMode);
        }

        public void FireEnableUnitMessage(Guid machineID, bool enable)
        {
            EnableUnit?.Invoke(machineID, enable);
        }

        public void FireOpenWaterValveMessage(Guid machineID)
        {
            OpenWaterValve?.Invoke(machineID);
        }

        public void FirePumpWaterMessage(Guid machineID)
        {
            PumpWater?.Invoke(machineID);
        }
        public void FireHeatWaterMessage(Guid machineID)
        {
            HeatWater?.Invoke(machineID);
        }
        public void FireDispenseDetegentMessage(Guid machineID)
        {
            DispenseDetegent?.Invoke(machineID);
        }
        public void FireWashMessage(Guid machineID)
        {
            Wash?.Invoke(machineID);
        }
        public void FireDrainMessage(Guid machineID)
        {
            Drain?.Invoke(machineID);
        }
        public void FireSpinMessage(Guid machineID)
        {
            Spin?.Invoke(machineID);
        }
        public void FireFinishMessage(Guid machineID)
        {
            Finish?.Invoke(machineID);
        }
        public void FireActionExecutedMessage(Guid machineID, WashingModes.WashingActions action)
        {
            ActionExecuted?.Invoke(machineID, action);
        }
        public void FireActionExecutionFinishedMessage(Guid machineID, WashingModes.WashingActions action)
        {
            ActionExecutionFinished?.Invoke(machineID, action);
        }
        public bool FireIsWashingStartedMessage(Guid machineID)
        {
            return (bool)IsWashingStarted?.Invoke(machineID);
        }
        public void FireWaterTemperatureChangeMessage(Guid machineID, int temperature)
        {
            WaterTemperatureChange?.Invoke(machineID, temperature);
        }
        public void FireCancelProcessMessage(Guid machineID)
        {
            CancelProcess?.Invoke(machineID);
        }
        #endregion
    }
}
