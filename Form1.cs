using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using _CustomNethook;

namespace CustomNethook
{
    public partial class Form1 : Form
    {
        int? mcolumn = null;
        int? row = null;
        int? fcolumn = null;

        public Form1()
        {
            InitializeComponent();
        }


        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void OK_click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MaleLandWidthWithGrainBtn_click(object sender, EventArgs e)
        {
            mcolumn = 1;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var mtempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = mtempControl;
            }
        }

        private void MaleLandWidthAgainstGrainBtn_click(object sender, EventArgs e)
        {
            mcolumn = 2;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var mtempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = mtempControl;
            }

        }

        private void FemaleLandWidthWithGrainBtn_click(object sender, EventArgs e)
        {
            fcolumn = 3;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }

        private void FealeLandWidthAgainstGrainBtn_click(object sender, EventArgs e)
        {
            fcolumn = 4;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }

        private void BC10_click(object sender, EventArgs e)
        {
            row = 2;
            if (row != null && mcolumn != null && fcolumn != null){
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }
        }
        private void BC11_click(object sender, EventArgs e)
        {
            row = 3;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC12_click(object sender, EventArgs e)
        {
            row = 4;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC13_click(object sender, EventArgs e)
        {
            row = 5;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC14_click(object sender, EventArgs e)
        {
            row = 6;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC15_click(object sender, EventArgs e)
        {
            row = 7;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC16_click(object sender, EventArgs e)
        {
            row = 8;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC17_click(object sender, EventArgs e)
        {
            row = 9;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC18_click(object sender, EventArgs e)
        {
            row = 10;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC19_click(object sender, EventArgs e)
        {
            row = 11;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC20_click(object sender, EventArgs e)
        {
            row = 12;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC21_click(object sender, EventArgs e)
        {
            row = 13;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC22_click(object sender, EventArgs e)
        {
            row = 14;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC23_click(object sender, EventArgs e)
        {
            row = 15;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC24_click(object sender, EventArgs e)
        {
            row = 16;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC25_click(object sender, EventArgs e)
        {
            row = 17;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC26_click(object sender, EventArgs e)
        {
            row = 18;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC27_click(object sender, EventArgs e)
        {
            row = 19;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC28_click(object sender, EventArgs e)
        {
            row = 20;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC29_click(object sender, EventArgs e)
        {
            row = 21;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC30_click(object sender, EventArgs e)
        {
            row = 22;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC31_click(object sender, EventArgs e)
        {
            row = 23;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC32_click(object sender, EventArgs e)
        {
            row = 24;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC33_click(object sender, EventArgs e)
        {
            row = 25;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC34_click(object sender, EventArgs e)
        {
            row = 26;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC35_click(object sender, EventArgs e)
        {
            row = 27;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC36_click(object sender, EventArgs e)
        {
            row = 28;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC37_click(object sender, EventArgs e)
        {
            row = 29;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }
        private void BC38_click(object sender, EventArgs e)
        {
            row = 30;
            if (row != null && mcolumn != null && fcolumn != null)
            {
                var tempControl = tableLayoutPanel1.GetControlFromPosition((int)mcolumn, (int)row).Text;
                maleLandWidthPicked.Text = tempControl;
                var ftempControl = tableLayoutPanel1.GetControlFromPosition((int)fcolumn, (int)row).Text;
                femaleLandWidthPicked.Text = ftempControl;
            }

        }

    }
}
