using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WashingMachine
{
    internal class WashingMachine
    {
        #region Definitions
        public class UnitInfo
        {
            public Point Location { get; set; }
            public Size Dimensions { get; set; }
        }
        #endregion

        #region Constants
        protected readonly string layoutFileName = "UnitsLayout.json";
        #endregion

        #region Properties
        private TableLayoutPanel UnitsLayoutPanel = new TableLayoutPanel();
        private ConcurrentDictionary<BaseUnit.MachineUnitType, UnitInfo> UnitList = new ConcurrentDictionary<BaseUnit.MachineUnitType, UnitInfo>();
        #endregion

        #region Construction
        public WashingMachine()
        {
            InitUnitsLayout();
            InitLayoutFromFile();
        }
        #endregion

        #region Methods
        private void InitUnitsLayout()
        {
            try
            {
                UnitsLayoutPanel.Dock = DockStyle.Fill;

                UnitsLayoutPanel.RowCount = 3;
                UnitsLayoutPanel.ColumnCount = 3;

                UnitsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
                UnitsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
                UnitsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 34));

                UnitsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
                UnitsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
                UnitsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
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
                    var layout = JsonConvert.DeserializeObject<ConcurrentDictionary<BaseUnit.MachineUnitType, UnitInfo>>(jsonString);
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
