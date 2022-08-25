using BO4Console;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WW2控制台
{
    public partial class Form1 : Form
    {
		private memory m = new memory();
		private int basea;
		public Form1()
        {
            InitializeComponent();
        }


        private void timer3_Tick(object sender, EventArgs e)
        {
			if (m.IsOpen())
			{
				label1.Text = "找到进程";
				process.Interval = 500;
				if (basea == 0)
				{
					address.setadd();
					this.basea = 1;
					return;
				}
			}
			else
			{
				this.m.AttackProcess("s2_mp64_ship");
				this.label1.Text = "未找到进程";
				this.process.Interval = 100;
			}

		}
		private void CbufAddText(string str)
		{
			if (m.IsOpen())
			{
				m.WriteString(m.BaseModule.ToInt64() + 180819776L, str + "exec mp/stats_init_ranked.cfg");
				m.WriteByte(m.BaseModule.ToInt64() + 180778181L, 1);
			}
		}

        private void cbuffaddtext_Click(object sender, EventArgs e)
        {
			CbufAddText(textBox1.Text);
		}

		private void cbuffaddtext_KeyDown(object sender, KeyEventArgs e)
		{

		}
		private void cbuff_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				this.CbufAddText(this.textBox1.Text);
			}
		}

        private void pos_Tick(object sender, EventArgs e)
        {

        }

        private void process_Tick(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
