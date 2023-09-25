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
using System.Threading.Tasks;
using HslCommunication;

namespace WY_App
{
    public partial class 示教器 : Form
    {
        Thread myThread;

        public 示教器()
        {
            InitializeComponent();
            myThread = new Thread(readPLCAll);
            myThread.IsBackground = true;
            myThread.Start();
        }
        Point downPoint;
        private void readPLCAll()
        {
            while (true)
            {
                Thread.Sleep(100);
                Task task = new Task(() =>
                {
                    MethodInvoker start = new MethodInvoker(() =>
                    {
                        //lab_X.Text
                        lab_X.Text = HslCommunication._NetworkTcpDevice.ReadFloat(Parameters.plcParams.WriteAdd[24]).Content.ToString();
                        lab_Y.Text = HslCommunication._NetworkTcpDevice.ReadFloat(Parameters.plcParams.WriteAdd[25]).Content.ToString();
                        lab_R.Text = HslCommunication._NetworkTcpDevice.ReadFloat(Parameters.plcParams.WriteAdd[26]).Content.ToString();
                        toolStripStatusLabel3.Text = HslCommunication._NetworkTcpDevice.ReadFloat(Parameters.plcParams.WriteAdd[40]).Content.ToString();
                    });
                    this.BeginInvoke(start);
                });
                task.Start();
            }

        }
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

        private void btn_Close_System_Click(object sender, EventArgs e)
        {
            myThread.Abort();
            this.Close();
        }

        private void btn_JOG_Home_Click(object sender, EventArgs e)
        {
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[20], 1);
        }

        private void btn_JOG_Xadd_Click(object sender, EventArgs e)
        {
            //OperateResult _connected = HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[9],1);
        }

        private void btn_JOG_Xred_Click(object sender, EventArgs e)
        {
            //HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[10], 1);
        }

        private void btn_JOG_Yadd_Click(object sender, EventArgs e)
        {
            //HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[11], 1);
        }

        private void btn_JOG_Yred_Click(object sender, EventArgs e)
        {
            //HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[12], 1);
        }

        private void btn_JOG_Zadd_Click(object sender, EventArgs e)
        {
            //HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[13], 1);
        }

        private void btn_JOG_Zred_Click(object sender, EventArgs e)
        {
            //HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[14], 1);
        }

        private void btn_JOG_Change_Click(object sender, EventArgs e)
        {
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[0], (float) num_X_JOG_Speed.Value);
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[1], (float)num_Y_JOG_Speed.Value);
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[2], (float)num_Z_JOG_Speed.Value);
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[27], (float)num_Back_Step.Value);

        }

        private void btn_Run_Change_Click(object sender, EventArgs e)
        {
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[3], (float)num_X_Run_Speed.Value);
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[4], (float)num_Y_Run_Speed.Value);
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[5], (float)num_Z_Run_Speed.Value);
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[32], (float)uiDoubleUpDown2.Value);
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[33], (float)uiDoubleUpDown1.Value);            
        }

        private void btn_Point_Move_Click(object sender, EventArgs e)
        {
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[15], (float)num_X_Run_Point.Value);
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[16], (float)num_Y_Run_Point.Value);
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[17], (float)num_R_Run_Point.Value);
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[42], (float)uiDoubleUpDown3.Value);
            
        }

        private void label41_Click(object sender, EventArgs e)
        {

        }

        private void btn_JOG_Xadd_MouseUp(object sender, MouseEventArgs e)
        {
            OperateResult _connected = HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[9], 0);
        }

        private void btn_JOG_Xadd_MouseDown(object sender, MouseEventArgs e)
        {
            OperateResult _connected = HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[9], 1);
        }

        private void btn_JOG_Yadd_MouseDown(object sender, MouseEventArgs e)
        {
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[11], 1);
        }

        private void btn_JOG_Yadd_MouseUp(object sender, MouseEventArgs e)
        {
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[11], 0);
        }

        private void btn_JOG_Xred_MouseDown(object sender, MouseEventArgs e)
        {
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[10], 1);
        }

        private void btn_JOG_Xred_MouseUp(object sender, MouseEventArgs e)
        {
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[10], 0);
        }

        private void btn_JOG_Yred_MouseDown(object sender, MouseEventArgs e)
        {
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[12], 1);
        }

        private void btn_JOG_Yred_MouseUp(object sender, MouseEventArgs e)
        {
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[12], 0);
        }

        private void btn_JOG_Zadd_MouseDown(object sender, MouseEventArgs e)
        {
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[13], 1);
        }

        private void btn_JOG_Zadd_MouseUp(object sender, MouseEventArgs e)
        {
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[13], 0);
        }

        private void btn_JOG_Zred_MouseDown(object sender, MouseEventArgs e)
        {
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[14], 1);
        }

        private void btn_JOG_Zred_MouseUp(object sender, MouseEventArgs e)
        {
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[14], 0);
        }

        private void 示教器_Load(object sender, EventArgs e)
        {
            num_X_Run_Speed.Value = HslCommunication._NetworkTcpDevice.ReadFloat(Parameters.plcParams.WriteAdd[3]).Content;
            num_Y_Run_Speed.Value = HslCommunication._NetworkTcpDevice.ReadFloat(Parameters.plcParams.WriteAdd[4]).Content;
            num_Z_Run_Speed.Value = HslCommunication._NetworkTcpDevice.ReadFloat(Parameters.plcParams.WriteAdd[5]).Content;
            uiDoubleUpDown2.Value = HslCommunication._NetworkTcpDevice.ReadFloat(Parameters.plcParams.WriteAdd[32]).Content;
            uiDoubleUpDown1.Value = HslCommunication._NetworkTcpDevice.ReadFloat(Parameters.plcParams.WriteAdd[33]).Content;

            num_X_JOG_Speed.Value= HslCommunication._NetworkTcpDevice.ReadFloat(Parameters.plcParams.WriteAdd[0]).Content;
            num_Y_JOG_Speed.Value= HslCommunication._NetworkTcpDevice.ReadFloat(Parameters.plcParams.WriteAdd[1]).Content;
            num_Z_JOG_Speed.Value= HslCommunication._NetworkTcpDevice.ReadFloat(Parameters.plcParams.WriteAdd[2]).Content;
            num_Back_Step.Value = HslCommunication._NetworkTcpDevice.ReadFloat(Parameters.plcParams.WriteAdd[27]).Content;

            num_X_Run_Point.Value = HslCommunication._NetworkTcpDevice.ReadFloat(Parameters.plcParams.WriteAdd[15]).Content;
            num_Y_Run_Point.Value = HslCommunication._NetworkTcpDevice.ReadFloat(Parameters.plcParams.WriteAdd[16]).Content;
            num_R_Run_Point.Value = HslCommunication._NetworkTcpDevice.ReadFloat(Parameters.plcParams.WriteAdd[17]).Content;
            uiDoubleUpDown3.Value = HslCommunication._NetworkTcpDevice.ReadFloat(Parameters.plcParams.WriteAdd[42]).Content;
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {

        }
    }
}
