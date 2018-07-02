using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Banana.AutoCode
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.banana_64px;

            this.BackColor = Color.White;
            pictureBox1.Image = Properties.Resources.banana;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            linkEmail.Click += LinkEmail_Click;
        }

        private void LinkEmail_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:cnzhangw@sina.com?Subject=关于Banana.AutoCode的反馈");
        }
    }
}
