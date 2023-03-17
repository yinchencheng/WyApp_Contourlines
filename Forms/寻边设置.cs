using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WY_App.Utility;
using static WY_App.Utility.Parameters;

namespace WY_App
{
    public partial class 寻边设置 : Form
    {
        public 寻边设置()
        {
            InitializeComponent();
        }

        private void 卡尺工具设置_Load(object sender, EventArgs e)
        {
            try
            {
                num_Row.Value = (decimal)Parameters.detectionSpec.decetionRect2[相机检测设置.decetionNum].Row;
                num_Column.Value = (decimal)Parameters.detectionSpec.decetionRect2[相机检测设置.decetionNum].Colum;
                num_Phi.Value = (decimal)Parameters.detectionSpec.decetionRect2[相机检测设置.decetionNum].Phi;
                num_Length1.Value = (decimal)Parameters.detectionSpec.decetionRect2[相机检测设置.decetionNum].Length1;
                num_Length2.Value = (decimal)Parameters.detectionSpec.decetionRect2[相机检测设置.decetionNum].Length2;
                num_Sigma.Value = (decimal)Parameters.detectionSpec.decetionRect2[相机检测设置.decetionNum].simga;
                num_Threshold.Value = (decimal)Parameters.detectionSpec.decetionRect2[相机检测设置.decetionNum].阈值;
                num_Transition.Text = Parameters.detectionSpec.decetionRect2[相机检测设置.decetionNum].极性;
            }
            catch
            {
                MessageBox.Show("请选择需要检测的序号");
                this.Close();
            }
            
        }

        private void btn_Close_System_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }


        private void btn_Change_Click(object sender, EventArgs e)
        {
            num_Row.Enabled = true;
            num_Column.Enabled = true;
            num_Phi.Enabled = true;
            num_Length1.Enabled = true;
            num_Length2.Enabled = true;
            num_Sigma.Enabled = true;
            num_Threshold.Enabled = true;
            num_Transition.Enabled = true;
            btn_Save.Enabled = true;
        }

        private void btn_Test_Click(object sender, EventArgs e)
        {
            Location location = new Location();
            相机检测设置.ShowMeasurePos(ref location);

        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            Parameters.detectionSpec.decetionRect2[相机检测设置.decetionNum].Row =(double) num_Row.Value;
            Parameters.detectionSpec.decetionRect2[相机检测设置.decetionNum].Colum = (double)num_Column.Value;
            Parameters.detectionSpec.decetionRect2[相机检测设置.decetionNum].Phi = (double)num_Phi.Value;
            Parameters.detectionSpec.decetionRect2[相机检测设置.decetionNum].Length1 = (double)num_Length1.Value;
            Parameters.detectionSpec.decetionRect2[相机检测设置.decetionNum].Length2 = (double)num_Length2.Value;
            Parameters.detectionSpec.decetionRect2[相机检测设置.decetionNum].simga = (double)num_Sigma.Value;
            Parameters.detectionSpec.decetionRect2[相机检测设置.decetionNum].阈值 = (double)num_Threshold.Value;
            Parameters.detectionSpec.decetionRect2[相机检测设置.decetionNum].极性   = num_Transition.Text;
            XMLHelper.serialize<Parameters.DetectionSpec>(Parameters.detectionSpec, Parameters.commministion.productName + "/DetectionSpec.xml");
            this.Close();
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
