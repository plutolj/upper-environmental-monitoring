using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WFA_AUTO_oo
{
    public partial class JD_SET : Form
    {
        public JD_SET()
        {
            InitializeComponent();
        }
        public int n_JD = 2;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                
                n_JD = Math.Min(100, int.Parse(textBox1.Text));
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            catch(Exception exx)
            {
                MessageBox.Show(exx.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
