using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WashingMachine
{
    public sealed class WashingModes
    {
        #region Enums
        public enum WashingActions
        {
            OpenWaterValve,
            PumpWater,
            Heat,
            DispenseDetegent,
            Wash,
            Drain,
            Spin,
            Finish
        }
        #endregion

        #region Definitions
        public class WashingModeInfo
        {
            #region Properties
            public int Temperature { get; set; }
            public int WashingCycleTime { get; set; }
            public int WashingCycles { get; set; }
            public int SpinCycle { get; set; }
            public List<WashingActions> Actions { get; set; }
            #endregion
        }
        #endregion

        #region Constants
        private const string WashingModesFileName = "WashingModes.json";
        #endregion

        #region Properties
        private static readonly Lazy<WashingModes> singletonInstance = new Lazy<WashingModes>(() => new WashingModes());
        public static WashingModes Instance => singletonInstance.Value;
        private ConcurrentDictionary<string, WashingModeInfo> WashingModesList = new ConcurrentDictionary<string, WashingModeInfo>();

        public List<string> WashModes
        {
            get
            {
                return WashingModesList.Keys.ToList<string>();
            }
        }

        public WashingModeInfo this[string mode]
        {
            get
            {
                return WashingModesList[mode];
            }
        }
        #endregion

        #region Construction
        private WashingModes() { }
        public bool Initialize()
        {
            return LoadFromFile();
        }
        #endregion

        #region Methods
        private bool LoadFromFile()
        {
            try
            {
                string jsonFilePath = Path.Combine(Application.StartupPath, "Configuration", WashingModesFileName);
                using (var reader = new StreamReader(jsonFilePath))
                {
                    string jsonString = reader.ReadToEnd();
                    WashingModesList = JsonConvert.DeserializeObject<ConcurrentDictionary<string, WashingModeInfo>>(jsonString);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
                return false;
            }

            Logger.Instance.LogInformation("Washing modes list loaded.");
            return true;
        }
        #endregion
    }
}
