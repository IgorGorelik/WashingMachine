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

            var test = new WashMotorUnit(TimeSpan.FromSeconds(20), 4, new Point(0, 0), new Size(300, 300));
            var test2 = new SpinMotorUnit(TimeSpan.FromSeconds(20), new Point(300, 0), new Size(300, 300));

            tableWashingMachine.Controls.Add(test, 0, 2);
            tableWashingMachine.Controls.Add(test2, 2, 2);

            test.ExecuteCycle();
            test2.ExecuteCycle();
        }
    }
}
