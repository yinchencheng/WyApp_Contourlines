using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WY_App.Utility;

namespace WY_App
{
    public partial class 卡尺设置 : Form
    {
        public 卡尺设置()
        {
            InitializeComponent();
        }

        private void 卡尺工具设置_Load(object sender, EventArgs e)
        {
            num_MeasureLength1.Value = (decimal)Parameters.detectionSpec.baseRect1[相机检测设置.baseNum].Length1;
            num_MeasureLength2.Value = (decimal)Parameters.detectionSpec.baseRect1[相机检测设置.baseNum].Length2;
            num_MeasureSigma.Value = (decimal)Parameters.detectionSpec.baseRect1[相机检测设置.baseNum].simga;
            num_MeasureThreshold.Value = (decimal)Parameters.detectionSpec.baseRect1[相机检测设置.baseNum].阈值;
            num_MeasureTransition.Text = Parameters.detectionSpec.baseRect1[相机检测设置.baseNum].极性;

        }

        private void btn_Close_System_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

        private void 保存_Click(object sender, EventArgs e)
        {
            Parameters.detectionSpec.baseRect1[相机检测设置.baseNum].Length1 = (int)num_MeasureLength1.Value;
            Parameters.detectionSpec.baseRect1[相机检测设置.baseNum].Length2 = (int)num_MeasureLength2.Value;
            Parameters.detectionSpec.baseRect1[相机检测设置.baseNum].simga = (double)num_MeasureSigma.Value;
            Parameters.detectionSpec.baseRect1[相机检测设置.baseNum].阈值 = (int)num_MeasureThreshold.Value;
            Parameters.detectionSpec.baseRect1[相机检测设置.baseNum].极性 = num_MeasureTransition.Text;
            XMLHelper.serialize<Parameters.DetectionSpec>(Parameters.detectionSpec, Parameters.commministion.productName + "/DetectionSpec.xml");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (相机检测设置.baseNum)
            {
                case 0:
                    {
                        相机检测设置.showXBase(sender, e);
                        break;
                    }
                case 1:
                    {
                        相机检测设置.showY1Base(sender, e);
                        break;
                    }
                case 2:
                    {
                        相机检测设置.showY2Base(sender, e);
                        break;
                    }
            }
            
        }
        Point downPoint;
        private void panel4_MouseDown(object sender, MouseEventArgs e)
        {
            downPoint = new Point(e.X, e.Y);
        }

        private void panel4_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - downPoint.X,
                    this.Location.Y + e.Y - downPoint.Y);
            }
        }
    }
}
