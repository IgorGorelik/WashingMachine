using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WashingMachine.WashingModes;

namespace WashingMachine
{
    public sealed class OperationActions
    {
        #region Properties
        public WashingActions Action { get; set; }
        #endregion

        #region Properties
        private static readonly Lazy<OperationActions> singletonInstance = new Lazy<OperationActions>(() => new OperationActions());
        public static OperationActions Instance => singletonInstance.Value;
        #endregion

        #region Construction
        OperationActions() { }
        #endregion

        #region Methods
        public void ExecuteAction(WashingActions action, Guid machineID)
        {
            switch (action)
            {
                case WashingActions.OpenWaterValve:
                    InteropMessenger.Instance.FireOpenWaterValveMessage(machineID);
                    break;

                case WashingActions.PumpWater:
                    InteropMessenger.Instance.FirePumpWaterMessage(machineID);
                    break;

                case WashingActions.Heat:
                    InteropMessenger.Instance.FireHeatWaterMessage(machineID);
                    break;

                case WashingActions.DispenseDetegent:
                    InteropMessenger.Instance.FireDispenseDetegentMessage(machineID);
                    break;

                case WashingActions.Wash:
                    InteropMessenger.Instance.FireWashMessage(machineID);
                    break;

                case WashingActions.Drain:
                    InteropMessenger.Instance.FireDrainMessage(machineID);
                    break;

                case WashingActions.Spin:
                    InteropMessenger.Instance.FireSpinMessage(machineID);
                    break;

                case WashingActions.Finish:
                    InteropMessenger.Instance.FireFinishMessage(machineID);
                    break;
            }

            InteropMessenger.Instance.FireActionExecutedMessage(machineID, action);
        }
        #endregion
    }
}
