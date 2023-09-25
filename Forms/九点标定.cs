using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Collections.ObjectModel;
using WY_App.Utility;
using OpenCvSharp.XImgProc;
using WY_App.UserControls;

namespace WY_App
{
    public partial class 九点标定 : Form
    {
       
        numControl[] numControls;
        public 九点标定()
        {
            InitializeComponent();
            numControls = new numControl[18] { numControl01, numControl02, numControl03, numControl04, numControl05, numControl06, numControl07, numControl08, numControl09,
                                               numControl11, numControl12, numControl13, numControl14, numControl15, numControl16, numControl17, numControl18, numControl19 };
        }
        Point downPoint;

        private void panel4_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - downPoint.X,
                    this.Location.Y + e.Y - downPoint.Y);
            }
        }

        private void panel4_MouseDown(object sender, MouseEventArgs e)
        {
            downPoint = new Point(e.X, e.Y);
        }


        private void btn_Close_System_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {

        }

        private void uiButton1_Click(object sender, EventArgs e)
        {

        }
    }
}
