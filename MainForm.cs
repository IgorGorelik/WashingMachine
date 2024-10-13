using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace WashingMachine
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.White;

            Logger.Instance.Initialize();
            Logger.Instance.LogInformation($"Application started, version {Assembly.GetExecutingAssembly().GetName().Version}");

            ImageLibrary.Instance.Initialize();

            var washMotorUnit = new WashMotorUnit(new Size(300, 300));
            var spinMotorUnit = new SpinMotorUnit(new Size(300, 300));
            var doorUnit = new DoorUnit(new Size(300, 300));
            var drumUnit = new DrumUnit(new Size(300, 300));
            var powerSupplyUnit = new PowerSupplyUnit(new Size(300, 300));

            tableWashingMachine.Controls.Add(washMotorUnit, 0, 2);
            tableWashingMachine.Controls.Add(spinMotorUnit, 2, 2);
            tableWashingMachine.Controls.Add(doorUnit, 1, 1);
            tableWashingMachine.Controls.Add(drumUnit, 1, 2);
            tableWashingMachine.Controls.Add(powerSupplyUnit, 0, 0);

            WashingMachine machine = new WashingMachine();
        }
    }
}
