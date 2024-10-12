using System;
using System.Drawing;
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
            ImageLibrary.Instance.Initialize();

            var test = new WashMotorUnit(TimeSpan.FromSeconds(20), 4, new Point(0, 0), new Size(300, 300));
            var test2 = new SpinMotorUnit(TimeSpan.FromSeconds(20), new Point(300, 0), new Size(300, 300));

            Controls.Add(test);
            Controls.Add(test2);
        }
    }
}
