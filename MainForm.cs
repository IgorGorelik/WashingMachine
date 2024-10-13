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
            Controls.Add(new WashingMachine());
        }
    }
}
