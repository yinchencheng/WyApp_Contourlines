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
using HalconDotNet;
using System.Data.SqlTypes;

namespace WY_App
{
    public partial class 相机配置界面 : Form
    {
       

        public 相机配置界面()
        {
            InitializeComponent();
            
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


        private void btn_Close_System_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 相机配置界面_Load(object sender, EventArgs e)
        {
            EnableControls();
        }

        private void btnEnumInterface_Click(object sender, EventArgs e)
        {
            MainForm.HivCam.EnumInterface();
            for(int i=0;i< MainForm.HivCam.cmbInterfaceList.Count;i++)
            {
                cmbInterfaceList.Items.Add(MainForm.HivCam.cmbInterfaceList[i]);
            }    
            
            EnableControls();
        }

        private void btnOpenInterface_Click(object sender, EventArgs e)
        {
            MainForm.HivCam.OpenInterface();
            EnableControls();
        }

        private void btnCloseInterface_Click(object sender, EventArgs e)
        {
            MainForm.HivCam.CloseInterface();
            EnableControls();
        }

        private void btnEnumDevice_Click(object sender, EventArgs e)
        {
            MainForm.HivCam.EnumDevice();
            for (int i = 0; i < MainForm.HivCam.cmbDeviceList.Count; i++)
            {
                cmbDeviceList.Items.Add(MainForm.HivCam.cmbDeviceList[i]);
            }
                
            EnableControls();
        }

        private void btnOpenDevice_Click(object sender, EventArgs e)
        {
            MainForm.HivCam.OpenDevice();
            EnableControls();
        }

        private void btnCloseDevice_Click(object sender, EventArgs e)
        {
            MainForm.HivCam.CloseDevice();
            EnableControls();
        }

        private void bnContinuesMode_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.HivCam.ContinuesMode();
        }

        private void bnTriggerMode_CheckedChanged(object sender, EventArgs e)
        {
            Parameters.cameraParam.CamContinuesMode = bnTriggerMode.Checked;
            MainForm.HivCam.TriggerMode(Parameters.cameraParam.CamContinuesMode);
            EnableControls();
        }

        private void cbSoftTrigger_CheckedChanged(object sender, EventArgs e)
        {
            Parameters.cameraParam.CamSoftwareMode= cbSoftTrigger.Checked; 
            MainForm.HivCam.SoftTrigger(Parameters.cameraParam.CamSoftwareMode);
            EnableControls();
        }

        private void btnStartGrab_Click(object sender, EventArgs e)
        {
            MainForm.HivCam.StartGrab();
            EnableControls();
        }

        private void btnStopGrab_Click(object sender, EventArgs e)
        {
            MainForm.HivCam.StopGrab();
            EnableControls();
        }

        private void btnTriggerExec_Click(object sender, EventArgs e)
        {
            MainForm.HivCam.TriggerExec();
            MainForm.HivCam.ImageEvent.WaitOne();
            HOperatorSet.DispObj(MainForm.hImage,hWindowControl1.HalconWindow);
        }

        private void btnSaveBmp_Click(object sender, EventArgs e)
        {
            MainForm.HivCam.SaveBmp();
        }

        private void EnableControls()
        {
            this.btnEnumInterface.Enabled = !MainForm.HivCam.m_bIsIFOpen;
            this.btnOpenInterface.Enabled = (MainForm.HivCam.m_nInterfaceNum > 0) && !MainForm.HivCam.m_bIsIFOpen;
            this.btnCloseInterface.Enabled = MainForm.HivCam.m_bIsIFOpen && !MainForm.HivCam.m_bIsDeviceOpen;

            this.btnEnumDevice.Enabled = MainForm.HivCam.m_bIsIFOpen && !MainForm.HivCam.m_bIsDeviceOpen;
            this.btnOpenDevice.Enabled = MainForm.HivCam.m_bIsIFOpen && (MainForm.HivCam.m_nInterfaceNum > 0) && !MainForm.HivCam.m_bIsDeviceOpen;
            this.btnCloseDevice.Enabled = MainForm.HivCam.m_bIsIFOpen && MainForm.HivCam.m_bIsDeviceOpen;

            this.bnContinuesMode.Enabled = MainForm.HivCam.m_bIsDeviceOpen;
            this.bnTriggerMode.Enabled = MainForm.HivCam.m_bIsDeviceOpen;

            this.btnStartGrab.Enabled = MainForm.HivCam.m_bIsDeviceOpen && !MainForm.HivCam.m_bIsGrabbing;
            this.btnStopGrab.Enabled = MainForm.HivCam.m_bIsDeviceOpen && MainForm.HivCam.m_bIsGrabbing;

            this.cbSoftTrigger.Enabled = MainForm.HivCam.m_bIsDeviceOpen && (MainForm.HivCam.m_nTriggerMode == MainForm.HivCam.TRIGGER_MODE_ON);
            this.btnTriggerExec.Enabled = this.cbSoftTrigger.Checked && this.bnTriggerMode.Checked && MainForm.HivCam.m_bIsGrabbing;

            this.btnSaveBmp.Enabled = MainForm.HivCam.m_bIsGrabbing;
            this.btnSaveJpg.Enabled = MainForm.HivCam.m_bIsGrabbing;
        }
    

        private void button2_Click(object sender, EventArgs e)
        {
            string stfFileNameOut = "Out" + System.DateTime.Now.ToString("yyyyMMdd");  // 默认的图像保存名称
            string pathOut = "/Image/";
            if (!System.IO.Directory.Exists(pathOut))
            {
                System.IO.Directory.CreateDirectory(pathOut);//不存在就创建文件夹
            }
            HOperatorSet.WriteImage(MainForm.hImage, "bmp", 0, pathOut + stfFileNameOut + ".bmp");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            HOperatorSet.SetPart(hWindowControl1.HalconWindow, 0, 0, -1, -1);
            HOperatorSet.DispObj(MainForm.hImage, hWindowControl1.HalconWindow);
        }


        private void btnSaveJpg_Click(object sender, EventArgs e)
        {
            string stfFileNameOut = "Out" + System.DateTime.Now.ToString("yyyyMMddHHmmss");  // 默认的图像保存名称
            string pathOut = "Image/";
            if (!System.IO.Directory.Exists(pathOut))
            {
                System.IO.Directory.CreateDirectory(pathOut);//不存在就创建文件夹
            }
            HOperatorSet.WriteImage(MainForm.hImage, "bmp", 0, pathOut + stfFileNameOut + ".bmp");
        }
    }
}
