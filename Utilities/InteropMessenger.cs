using Newtonsoft.Json.Linq;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace WashingMachine
{
    public sealed class InteropMessenger
    {
        #region Delegates
        public delegate void StartWashingEventHandler(Guid MachineID);
        #endregion

        #region Events
        public event StartWashingEventHandler StartWashing;
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
        public void FireStartWashingMessage(Guid MachineID)
        {
            StartWashing?.Invoke(MachineID);
        }
        #endregion
    }
}
