using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using MvFGCtrlC.NET;
using WY_App.Utility;

namespace WY_App
{
    public partial class 通讯设置 : Form
    {
        public 通讯设置()
        {
            InitializeComponent();
        }

        private void btn_Close_System_Click(object sender, EventArgs e)
        {           
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

       
        private void ParamSettings_Load(object sender, EventArgs e)
        {
            txt_PlcIpAddress.Text = Parameters.commministion.PlcIpAddress;
            num_PLCPort.Value = Parameters.commministion.PlcIpPort;
            txt_PlcDevice.Text = Parameters.commministion.PlcDevice;
            txt_PlcType.Text = Parameters.commministion.PlcType;
            chk_PLCEnabled.Checked = Parameters.commministion.PlcEnable;

            txt_ImageSavePath.Text = Parameters.commministion.ImageSavePath;
            txt_ServerIP.Text = Parameters.commministion.TcpServerIpAddress;
            num_ServerPort.Value = Parameters.commministion.TcpServerIpPort;
            chk_ServerEnabled.Checked = Parameters.commministion.TcpServerEnable;

            txt_ClientIP.Text = Parameters.commministion.TcpClientIpAddress;
            num_ClientPort.Value = Parameters.commministion.TcpClientIpPort;
            chk_ClientEnabled.Checked = Parameters.commministion.TcpClientEnable;
            num_LogSaveDays.Value = Parameters.commministion.LogFileExistDay;

            txt_Trigger_Detection.Text = Parameters.plcParams.Trigger_Detection;
            txt_HeartBeat_Add.Text = Parameters.plcParams.HeartBeatAdd;
            txt_StartAdd.Text = Parameters.plcParams.StartAdd;
            txt_Completion_Add.Text = Parameters.plcParams.Completion;

            WriteAdd0.Text = Parameters.plcParams.WriteAdd[0];
            WriteAdd1.Text = Parameters.plcParams.WriteAdd[1];
            WriteAdd2.Text = Parameters.plcParams.WriteAdd[2];
            WriteAdd3.Text = Parameters.plcParams.WriteAdd[3];
            WriteAdd4.Text = Parameters.plcParams.WriteAdd[4];

            WriteAdd5.Text = Parameters.plcParams.WriteAdd[5];
            WriteAdd6.Text = Parameters.plcParams.WriteAdd[6];
            WriteAdd7.Text = Parameters.plcParams.WriteAdd[7];
            WriteAdd8.Text = Parameters.plcParams.WriteAdd[8];
            WriteAdd9.Text = Parameters.plcParams.WriteAdd[9];

            WriteAdd10.Text = Parameters.plcParams.WriteAdd[10];
            WriteAdd11.Text = Parameters.plcParams.WriteAdd[11];
            WriteAdd12.Text = Parameters.plcParams.WriteAdd[12];
            WriteAdd13.Text = Parameters.plcParams.WriteAdd[13];
            WriteAdd14.Text = Parameters.plcParams.WriteAdd[14];

            WriteAdd15.Text = Parameters.plcParams.WriteAdd[15];
            WriteAdd16.Text = Parameters.plcParams.WriteAdd[16];
            WriteAdd17.Text = Parameters.plcParams.WriteAdd[17];
            WriteAdd18.Text = Parameters.plcParams.WriteAdd[18];
            WriteAdd19.Text = Parameters.plcParams.WriteAdd[19];

            WriteAdd20.Text = Parameters.plcParams.WriteAdd[20];
            WriteAdd21.Text = Parameters.plcParams.WriteAdd[21];
            WriteAdd22.Text = Parameters.plcParams.WriteAdd[22];
            WriteAdd23.Text = Parameters.plcParams.WriteAdd[23];
            WriteAdd24.Text = Parameters.plcParams.WriteAdd[24];

            WriteAdd25.Text = Parameters.plcParams.WriteAdd[25];
            WriteAdd26.Text = Parameters.plcParams.WriteAdd[26];
            WriteAdd27.Text = Parameters.plcParams.WriteAdd[27];
            WriteAdd28.Text = Parameters.plcParams.WriteAdd[28];
            WriteAdd29.Text = Parameters.plcParams.WriteAdd[29];

            WriteAdd30.Text = Parameters.plcParams.WriteAdd[30];
            WriteAdd31.Text = Parameters.plcParams.WriteAdd[31];
            WriteAdd32.Text = Parameters.plcParams.WriteAdd[32];
            WriteAdd33.Text = Parameters.plcParams.WriteAdd[33];
            WriteAdd34.Text = Parameters.plcParams.WriteAdd[34];
            WriteAdd35.Text = Parameters.plcParams.WriteAdd[35];

            WriteAdd36.Text = Parameters.plcParams.WriteAdd[36];
            WriteAdd37.Text = Parameters.plcParams.WriteAdd[37];
            WriteAdd38.Text = Parameters.plcParams.WriteAdd[38];
            WriteAdd39.Text = Parameters.plcParams.WriteAdd[39];
            WriteAdd40.Text = Parameters.plcParams.WriteAdd[40];
            WriteAdd41.Text = Parameters.plcParams.WriteAdd[41];
            WriteAdd42.Text = Parameters.plcParams.WriteAdd[42];
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btn_ChangePassword_Click(object sender, EventArgs e)
        {
            txt_PlcIpAddress.Enabled = true;
            num_PLCPort.Enabled = true;
            txt_PlcDevice.Enabled = true;
            txt_PlcType.Enabled = true;
            chk_PLCEnabled.Enabled = true;
            txt_ServerIP.Enabled = true;
            num_ServerPort.Enabled = true;
            chk_ServerEnabled.Enabled = true;
            txt_ClientIP.Enabled = true;
            num_ClientPort.Enabled = true;
            chk_ClientEnabled.Enabled = true;
            txt_Trigger_Detection.Enabled = true;
            txt_HeartBeat_Add.Enabled = true;
            txt_StartAdd.Enabled = true;
            txt_Completion_Add.Enabled = true;
            SNReadAdd.Enabled= true;
            num_LogSaveDays.Enabled = true;
            txt_ImageSavePath.Enabled = true;
            WriteAdd0.Enabled = true;
            WriteAdd1.Enabled = true;
            WriteAdd2.Enabled = true;
            WriteAdd3.Enabled = true;
            WriteAdd4.Enabled = true;                     
            WriteAdd5.Enabled = true;
            WriteAdd6.Enabled = true;
            WriteAdd7.Enabled = true;
            WriteAdd8.Enabled = true;
            WriteAdd9.Enabled = true;
            WriteAdd10.Enabled = true;
            WriteAdd11.Enabled = true;
            WriteAdd12.Enabled = true;
            WriteAdd13.Enabled = true;
            WriteAdd14.Enabled = true;
            WriteAdd15.Enabled = true;
            WriteAdd16.Enabled = true;
            WriteAdd17.Enabled = true;
            WriteAdd18.Enabled = true;
            WriteAdd19.Enabled = true;
            WriteAdd20.Enabled = true;
            WriteAdd21.Enabled = true;
            WriteAdd22.Enabled = true;
            WriteAdd23.Enabled = true;
            WriteAdd24.Enabled = true;
            WriteAdd25.Enabled = true;
            WriteAdd26.Enabled = true;
            WriteAdd27.Enabled = true;
            WriteAdd28.Enabled = true;
            WriteAdd29.Enabled = true;
            WriteAdd30.Enabled = true;
            WriteAdd31.Enabled = true;
            WriteAdd32.Enabled = true;
            WriteAdd33.Enabled = true;
            WriteAdd34.Enabled = true;
            WriteAdd35.Enabled = true;
            WriteAdd36.Enabled = true;
            WriteAdd37.Enabled = true;
            WriteAdd38.Enabled = true;
            WriteAdd39.Enabled = true;
            WriteAdd40.Enabled = true;
            WriteAdd41.Enabled = true;
            WriteAdd42.Enabled = true;
            btn_Save.Enabled = true;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            Parameters.commministion.PlcIpAddress = txt_PlcIpAddress.Text;
            Parameters.commministion.PlcIpPort = (int)num_PLCPort.Value;
            Parameters.commministion.PlcDevice = txt_PlcDevice.Text;
            Parameters.commministion.PlcType = txt_PlcType.Text;
            Parameters.commministion.PlcEnable = chk_PLCEnabled.Checked;

            Parameters.commministion.ImageSavePath = txt_ImageSavePath.Text; ;
            Parameters.commministion.TcpServerIpAddress = txt_ServerIP.Text;
            Parameters.commministion.TcpServerIpPort = (int)num_ServerPort.Value;
            Parameters.commministion.TcpServerEnable = chk_ServerEnabled.Checked;

            Parameters.commministion.TcpClientIpAddress = txt_ClientIP.Text;
            Parameters.commministion.TcpClientIpPort = (int)num_ClientPort.Value;
            Parameters.commministion.TcpClientEnable = chk_ClientEnabled.Checked;
            Parameters.commministion.LogFileExistDay = (int)num_LogSaveDays.Value;

            XMLHelper.serialize<Parameters.Commministion>(Parameters.commministion, "Parameter/Commministion.xml");

            Parameters.plcParams.Trigger_Detection = txt_Trigger_Detection.Text;
            Parameters.plcParams.HeartBeatAdd = txt_HeartBeat_Add.Text;
            Parameters.plcParams.StartAdd = txt_StartAdd.Text;
            Parameters.plcParams.Completion = txt_Completion_Add.Text;
            Parameters.plcParams.SNReadAdd = SNReadAdd.Text;

            Parameters.plcParams.WriteAdd[0] = WriteAdd0.Text;
            Parameters.plcParams.WriteAdd[1] = WriteAdd1.Text;
            Parameters.plcParams.WriteAdd[2] = WriteAdd2.Text;
            Parameters.plcParams.WriteAdd[3] = WriteAdd3.Text;
            Parameters.plcParams.WriteAdd[4] = WriteAdd4.Text;

            Parameters.plcParams.WriteAdd[5] = WriteAdd5.Text;
            Parameters.plcParams.WriteAdd[6] = WriteAdd6.Text;
            Parameters.plcParams.WriteAdd[7] = WriteAdd7.Text;
            Parameters.plcParams.WriteAdd[8] = WriteAdd8.Text;
            Parameters.plcParams.WriteAdd[9] = WriteAdd9.Text;

            Parameters.plcParams.WriteAdd[10] = WriteAdd10.Text;
            Parameters.plcParams.WriteAdd[11] = WriteAdd11.Text;
            Parameters.plcParams.WriteAdd[12] = WriteAdd12.Text;
            Parameters.plcParams.WriteAdd[13] = WriteAdd13.Text;
            Parameters.plcParams.WriteAdd[14] = WriteAdd14.Text;

            Parameters.plcParams.WriteAdd[15] = WriteAdd15.Text;
            Parameters.plcParams.WriteAdd[16] = WriteAdd16.Text;
            Parameters.plcParams.WriteAdd[17] = WriteAdd17.Text;
            Parameters.plcParams.WriteAdd[18] = WriteAdd18.Text;
            Parameters.plcParams.WriteAdd[19] = WriteAdd19.Text;

            Parameters.plcParams.WriteAdd[20] = WriteAdd20.Text;
            Parameters.plcParams.WriteAdd[21] = WriteAdd21.Text;
            Parameters.plcParams.WriteAdd[22] = WriteAdd22.Text;
            Parameters.plcParams.WriteAdd[23] = WriteAdd23.Text;
            Parameters.plcParams.WriteAdd[24] = WriteAdd24.Text;

            Parameters.plcParams.WriteAdd[25] = WriteAdd25.Text;
            Parameters.plcParams.WriteAdd[26] = WriteAdd26.Text;
            Parameters.plcParams.WriteAdd[27] = WriteAdd27.Text;
            Parameters.plcParams.WriteAdd[28] = WriteAdd28.Text;
            Parameters.plcParams.WriteAdd[29] = WriteAdd29.Text;

            Parameters.plcParams.WriteAdd[30] = WriteAdd30.Text;
            Parameters.plcParams.WriteAdd[31] = WriteAdd31.Text;
            Parameters.plcParams.WriteAdd[32] = WriteAdd32.Text;
            Parameters.plcParams.WriteAdd[33] = WriteAdd33.Text;
            Parameters.plcParams.WriteAdd[34] = WriteAdd34.Text;
            Parameters.plcParams.WriteAdd[35] = WriteAdd35.Text;

            Parameters.plcParams.WriteAdd[36] = WriteAdd36.Text;
            Parameters.plcParams.WriteAdd[37] = WriteAdd37.Text;
            Parameters.plcParams.WriteAdd[38] = WriteAdd38.Text;
            Parameters.plcParams.WriteAdd[39] = WriteAdd39.Text;
            Parameters.plcParams.WriteAdd[40] = WriteAdd40.Text;
            Parameters.plcParams.WriteAdd[41] = WriteAdd41.Text;
            Parameters.plcParams.WriteAdd[42] = WriteAdd42.Text;
            XMLHelper.serialize<Parameters.PLCParams>(Parameters.plcParams, "Parameter/PLCParams.xml");
            MessageBox.Show("系统参数修改，请重启软件");
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

     

        private void uiCheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void uiRadioButton5_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void uiRadioButton6_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
           
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {
           
        }

        private void label56_Click(object sender, EventArgs e)
        {
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[36], 1);
        }

        private void label64_Click(object sender, EventArgs e)
        {
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[37], 1);
        }

        private void label65_Click(object sender, EventArgs e)
        {
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[38], 1);
        }

        private void label62_Click(object sender, EventArgs e)
        {
            HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[39], 1);
        }

        private void label57_Click(object sender, EventArgs e)
        {
            int int16 = HslCommunication._NetworkTcpDevice.ReadInt16(Parameters.plcParams.WriteAdd[41]).Content;
            if(int16==0)
            {
                HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[41], 1);
            }
           else
            {
                HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[41], 0);
            }
        }
    }
}
