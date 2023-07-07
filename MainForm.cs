using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication;
using WY_App.Utility;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing.Imaging;
using HalconDotNet;
using TcpClient = WY_App.Utility.TcpClient;
using Sunny.UI.Win32;
using Newtonsoft.Json.Linq;
using Sunny.UI;
using static WY_App.Utility.Parameters;
using OpenCvSharp.Dnn;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static ODT.Common.LanguageTranslationConstants;
using HslCommunication.BasicFramework;

namespace WY_App
{
    public partial class MainForm : Form
    {
        HslCommunication hslCommunication;
        public static string Alarm = "";
        public static List<string> AlarmList = new List<string>();
        Thread myThread;
        Thread MainThread;
        Halcon halcon = new Halcon();
        HWindow hWindow=new HWindow();

        public static List<HObject> ho_Image = new List<HObject>();
        public static List<HObject> ho_DefectImage = new List<HObject>();
        public static List<HObject> ho_OrigalImage = new List<HObject>();
        public static HObject hImage = new HObject();
        public static HTuple[] hv_AcqHandle = new HTuple[4];
        double[] defectionValues = new double[4];

        private static Queue<Func<int>> m_List = new Queue<Func<int>>();
        private static object m_obj = new object();
        private bool isExit = false;
        static HObject hObjectIn = new HObject();
        static HObject hObjectOut = new HObject();
        Location[] location;
        public static HObject hoRegions = new HObject();
        public static AutoResetEvent ImageEvent = new AutoResetEvent(false);
        public static AutoResetEvent ImageWait = new AutoResetEvent(false);
        public static IKapLineCam grab;
        bool CamConnectResult = false;
        private delegate void SetTextValueCallBack(int i, HObject hObject, string path);
        //声明回调
        private SetTextValueCallBack setCallBack;
        public MainForm()
        {
            InitializeComponent();  
            pictureBox1.Load(Application.StartupPath + "/image/logo.png");
            #region 读取配置文件
           
            try
            {
                Parameters.deviceName = XMLHelper.BackSerialize<Parameters.DeviceName>(@"D:\\DeviceName.xml");
            }
            catch
            {
                Parameters.deviceName = new Parameters.DeviceName();
                XMLHelper.serialize<Parameters.DeviceName>(Parameters.deviceName, @"D:\\DeviceName.xml");
            }
            if (!EnumDivice(Parameters.deviceName.DeviceID))
            {
                注册机器 flg = new 注册机器();
                flg.TransfEvent += DeviceID_TransfEvent;
                flg.ShowDialog();
                if (!EnumDivice(DeviceID))
                {
                    Environment.Exit(1);
                    return;
                }
            }
            try
            {
                Parameters.counts = XMLHelper.BackSerialize<Parameters.Counts>(Parameters.commministion.productName + "/CountsParams.xml");
            }
            catch
            {
                Parameters.counts = new Parameters.Counts();
                XMLHelper.serialize<Parameters.Counts>(Parameters.counts, Parameters.commministion.productName + "/CountsParams.xml");
            }
            try
            {
                Parameters.counts = XMLHelper.BackSerialize<Parameters.Counts>(Parameters.commministion.productName + "/CountsParams.xml");
            }
            catch
            {
                Parameters.cameraParam = new Parameters.CameraParam();
                XMLHelper.serialize<Parameters.CameraParam>(Parameters.cameraParam, Parameters.commministion.productName + "/CameraParam.xml");
            }
            try
            {
                Parameters.specifications = XMLHelper.BackSerialize<Parameters.Specifications>(Parameters.commministion.productName + "/Specifications.xml");
            }
            catch
            {
                Parameters.specifications = new Parameters.Specifications();
                XMLHelper.serialize<Parameters.Specifications>(Parameters.specifications, Parameters.commministion.productName + "/Specifications.xml");
            }
            try
            {
                Parameters.detectionSpec = XMLHelper.BackSerialize<Parameters.DetectionSpec>(Parameters.commministion.productName + "/DetectionSpec.xml");
            }
            catch
            {
                Parameters.detectionSpec = new Parameters.DetectionSpec();
                XMLHelper.serialize<Parameters.DetectionSpec>(Parameters.detectionSpec, Parameters.commministion.productName + "/DetectionSpec.xml");
            }
            try
            {
                Parameters.cursorLocation = XMLHelper.BackSerialize<Parameters.CursorLocation>(Parameters.commministion.productName + "/CursorLocation.xml");
            }
            catch
            {
                Parameters.cursorLocation = new Parameters.CursorLocation();
                XMLHelper.serialize<Parameters.CursorLocation>(Parameters.cursorLocation, Parameters.commministion.productName + "/CursorLocation.xml");
            }
            try
            {
                Parameters.commministion = XMLHelper.BackSerialize<Parameters.Commministion>("Parameter/Commministion.xml");
            }
            catch
            {
                Parameters.commministion = new Parameters.Commministion();
                XMLHelper.serialize<Parameters.Commministion>(Parameters.commministion, "Parameter/Commministion.xml");
            }
            HOperatorSet.ReadImage(out hImage, Parameters.commministion.productName + "/N1.jpg");
            HOperatorSet.GetImageSize(MainForm.hImage, out Halcon.hv_Width, out Halcon.hv_Height);//获取图片大小规格   
            hWindow = hWindowControl1.HalconWindow;
            hWindow.SetPart(0, 0, -1, -1);
            HOperatorSet.DispObj(hImage, hWindow);
            try
            {
                grab = new IKapLineCam();
                CamConnectResult = IKapLineCam.OpenDevice(grab);
            }
            catch
            {

            }
            #endregion
            myThread = new Thread(initAll);
            myThread.IsBackground = true;
            myThread.Start();
        }
        private bool EnumDivice(string DiviceSn)
        {
            SoftAuthorize softAuthorize = new SoftAuthorize();
            if (!softAuthorize.CheckAuthorize(DiviceSn, AuthorizeEncrypted))
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        private string AuthorizeEncrypted(string origin)
        {
            return SoftSecurity.MD5Encrypt(origin, "12345678");
        }

        public static string DeviceID = "";
        void DeviceID_TransfEvent(string value)
        {
            DeviceID = value;
        }
        /// <summary>
        /// skinPictureBox1滚动缩放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {


            }
            catch (Exception x)
            {
                LogHelper.Log.WriteError("缩放异常：" + x.Message);
            }
        }

        //鼠标移动
        int xPos = 0;
        int yPos = 0;
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            xPos = e.X;//当前x坐标.
            yPos = e.Y;//当前y坐标.
        }

        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox1_MouseMove1(object sender, MouseEventArgs e)
        {

        }


        private void initAll()
        {
            while (true)
            {
                Thread.Sleep(1);
                Task task = new Task(() =>
                {
                    MethodInvoker start = new MethodInvoker(() =>
                    {
                        lab_Time.Text = System.DateTime.Now.ToString();
                        if (Parameters.commministion.PlcEnable)
                        {
                            if (HslCommunication.plc_connect_result)
                            {
                                lab_PLCStatus.Text = "在线";
                                lab_PLCStatus.BackColor = Color.Green;
                            }
                            else
                            {
                                lab_PLCStatus.Text = "离线";
                                lab_PLCStatus.BackColor = Color.Red;
                            }
                        }
                        else
                        {
                            lab_PLCStatus.Text = "禁用";
                            lab_PLCStatus.BackColor = Color.Gray;
                        }
                        if (Parameters.commministion.TcpClientEnable)
                        {
                            if (TcpClient.TcpClientConnectResult)
                            {
                                lab_Client.Text = "在线";
                                lab_Client.BackColor = Color.Green;
                            }
                            else
                            {
                                lab_Client.Text = "等待";
                                lab_Client.BackColor = Color.Red;
                            }
                        }
                        else
                        {
                            lab_Client.Text = "禁用";
                            lab_Client.BackColor = Color.Gray;
                        }
                        if (Parameters.commministion.TcpServerEnable)
                        {
                            if (TcpServer.TcpServerConnectResult)
                            {
                                lab_Server.Text = "在线";
                                lab_Server.BackColor = Color.Green;
                            }
                            else
                            {
                                lab_Server.Text = "等待";
                                lab_Server.BackColor = Color.Red;
                            }
                        }
                        else
                        {
                            lab_Server.Text = "禁用";
                            lab_Server.BackColor = Color.Gray;
                        }
                        if (AlarmList.Count > 0)
                        {
                            lab_log.Items.Add(AlarmList[0]);
                            AlarmList.RemoveAt(0);
                        }
                        if (lab_log.Items.Count > 20)
                        {
                            lab_log.Items.RemoveAt(0);
                        }
                        if (Halcon.CamConnect || CamConnectResult)
                        {
                            lab_Cam1.Text = "在线";
                            lab_Cam1.BackColor = Color.Green;
                        }
                        else
                        {
                            lab_Cam1.Text = "等待";
                            lab_Cam1.BackColor = Color.Red;
                        }
                       
                        //lab_Product.Text = Parameters.commministion.productName;
                    });
                    this.BeginInvoke(start);
                });
                task.Start();
            }

        }
        bool m_Pause = true;
        bool DetectionResult = true;
        public static string productSN = "0000";
        private void MainRun()
        {
            while (true)
            {
                if (m_Pause)
                {
                    Int16 uInt16 = HslCommunication._NetworkTcpDevice.ReadInt16(Parameters.plcParams.Trigger_Detection).Content; // 读取寄存器100的ushort值             
                    //if (uInt16 == 1)
                    {
                        DateTime dtNow = System.DateTime.Now;  // 获取系统当前时间
                        strDateTime = dtNow.ToString("yyyyMMddHHmmss");
                        strDateTimeDay = dtNow.ToString("yyyy-MM-dd");
                        //productSN = HslCommunication._NetworkTcpDevice.ReadString(Parameters.plcParams.SNReadAdd, 2).Content;
                        System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start(); //  开始监视代码运行时间
                        if (Halcon.CamConnect)
                        {
                            HOperatorSet.GrabImage(out hImage, Halcon.hv_AcqHandle);
                        }
                        if(CamConnectResult)
                        {
                            ImageWait.Set();
                            ImageEvent.WaitOne();
                        }
                        if (Parameters.specifications.SaveOrigalImage)
                        {
                            setCallBack = SaveImages;
                            this.Invoke(setCallBack, 0, hImage, "IN-");
                        }
                        //HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.Trigger_Detection, 0);
                        DetectionResult = true;
                        HOperatorSet.GetImageSize(hImage, out Halcon.hv_Width, out Halcon. hv_Height);
                        HOperatorSet.DispObj(hImage, hWindow);
                        HOperatorSet.SetPart(hWindow, 0, 0, -1, -1);                        
                        if(detection(hWindow, hImage))
                        {

                        }
                        if (Parameters.specifications.SaveDefeatImage)
                        {
                            HOperatorSet.DumpWindowImage(out hObjectOut, hWindow);
                            setCallBack = SaveImages;
                            this.Invoke(setCallBack, 0, hObjectOut, "OUT-");
                        }                                               
                        stopwatch.Stop(); //  停止监视
                        TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
                        double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数           
                        AlarmList.Add(System.DateTime.Now.ToString() + "检测时间:" + milliseconds.ToString());
                        CleanFile(Parameters.commministion.ImageSavePath);
                    }
                }
            }
        }

        public static void SaveImages(int i, HObject hObject, string path)
        {
            string stfFileNameOut = path + i + "CAM-" + productSN + "-" + strDateTime;  // 默认的图像保存名称
            string pathOut = Parameters.commministion.ImageSavePath + "/" + strDateTimeDay + "/" + productSN + "/";
            if (!System.IO.Directory.Exists(pathOut))
            {
                System.IO.Directory.CreateDirectory(pathOut);//不存在就创建文件夹
            }
            HOperatorSet.WriteImage(hObject, "jpeg", 0, pathOut + stfFileNameOut + ".jpeg");
        }
        private static void CleanFile(String dir)
        {
            DirectoryInfo di = new DirectoryInfo(dir);
            FileSystemInfo[] fileinfo = di.GetFileSystemInfos();  //返回目录中所有文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)            //判断是否文件夹
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    DateTime dates = Convert.ToDateTime(i.CreationTime);
                    if (dates <= DateTime.Now.AddDays(-Parameters.commministion.LogFileExistDay))
                    {
                        subdir.Delete(true);          //删除子目录和文件
                    }
                }
                else
                {
                    DateTime dates = Convert.ToDateTime(i.CreationTime);
                    if (dates <= DateTime.Now.AddDays(-Parameters.commministion.LogFileExistDay))
                    {
                        File.Delete(i.FullName);      //删除指定文件
                    }

                }
            }
        }
        private void btn_Close_System_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定关闭程序吗？", "软件关闭提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                XMLHelper.serialize<Parameters.Counts>(Parameters.counts, "Parameter/CountsParams.xml");
                //Parameter.specifications.右短端.value = 10;
                //XMLHelper.serialize<Parameter.Specifications>(Parameter.specifications, "Specifications.xml");
                myThread.Abort();
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + "软件关闭。");
                this.Close();
            }
        }

        private void btn_Minimizid_System_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + "窗体最小化。");
            MainForm.AlarmList.Add(System.DateTime.Now.ToString() + "窗体最小化。");
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();//隐藏主窗体  
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + "主窗体隐藏。");
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + "主窗体隐藏。");
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)//当鼠标点击为左键时  
            {
                this.Show();//显示主窗体  
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + "主窗体恢复。");
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + "主窗体恢复。");
                this.WindowState = FormWindowState.Normal;//主窗体的大小为默认  
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Parameters.commministion.PlcEnable)
            {
                hslCommunication = new HslCommunication();
                Thread.Sleep(1000);
                if (HslCommunication.plc_connect_result)
                {
                    lab_PLCStatus.Text = "在线";
                    lab_PLCStatus.BackColor = Color.Green;
                }
                else
                {
                    lab_PLCStatus.Text = "离线";
                    lab_PLCStatus.BackColor = Color.Red;
                }
            }
            else
            {
                lab_PLCStatus.Text = "禁用";
                lab_PLCStatus.BackColor = Color.Gray;
            }

            if (Parameters.commministion.TcpClientEnable)
            {
                TcpClient tcpClientr = new TcpClient();
                Thread.Sleep(1000);
                if (TcpClient.TcpClientConnectResult)
                {
                    lab_Client.Text = "在线";
                    lab_Client.BackColor = Color.Green;
                }
                else
                {
                    lab_Client.Text = "等待";
                    lab_Client.BackColor = Color.Red;
                }
            }
            else
            {
                lab_Client.Text = "禁用";
                lab_Client.BackColor = Color.Gray;
            }

            if (Parameters.commministion.TcpServerEnable)
            {
                TcpServer tcpServer = new TcpServer();
                Thread.Sleep(1000);
                if (TcpServer.TcpServerConnectResult)
                {
                    lab_Server.Text = "在线";
                    lab_Server.BackColor = Color.Green;
                }
                else
                {
                    lab_Server.Text = "等待";
                    lab_Server.BackColor = Color.Red;
                }
            }
            else
            {
                lab_Server.Text = "禁用";
                lab_Server.BackColor = Color.Gray;
            }
            for (int i = 0; i < Parameters.specifications.DetectionRect2Num+ Parameters.specifications.DetectionCricleNum; i++)
            {
                uiDataGridView1.Rows.Add();
            }
            lab_Product.Text = Parameters.commministion.productName;
            UpdataUI();
            hWindow.SetPart(0, 0, -1, -1);
            HOperatorSet.DispObj(hImage, hWindow);
            this.uiDataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            if (CamConnectResult)
            {
                IKapLineCam.StartGrabImage(grab);
            }
            LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + "初始化完成");
            MainForm.AlarmList.Add(System.DateTime.Now.ToString() + "初始化完成");
        }

        #region 点击panel控件移动窗口
        System.Drawing.Point downPoint;
        private void panel4_MouseDown(object sender, MouseEventArgs e)
        {
            downPoint = new System.Drawing.Point(e.X, e.Y);
        }
        private void panel4_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new System.Drawing.Point(this.Location.X + e.X - downPoint.X,
                    this.Location.Y + e.Y - downPoint.Y);
            }
        }
        #endregion
        private void btn_SettingMean_Click(object sender, EventArgs e)
        {
            通讯设置 flg = new 通讯设置();
            flg.ShowDialog();
        }

        //    Directory.CreateDirectory(string path);//在指定路径中创建所有目录和子目录，除非已经存在
        //    Directory.Delete(string path);//从指定路径删除空目录
        //    Directory.Delete(string path, bool recursive);//布尔参数为true可删除非空目录
        //    Directory.Exists(string path);//确定路径是否存在
        //    Directory.GetCreationTime(string path);//获取目录创建日期和时间
        //    Directory.GetCurrentDirectory();//获取应用程序当前的工作目录
        //    Directory.GetDirectories(string path);//返回指定目录所有子目录名称，包括路径
        //    Directory.GetFiles(string path);//获取指定目录中所有文件的名称，包括路径
        //    Directory.GetFileSystemEntries(string path);//获取指定路径中所有的文件和子目录名称
        //    Directory.GetLastAccessTime(string path);//获取上次访问指定文件或目录的时间和日期
        //    Directory.GetLastWriteTime(string path);//返回上次写入指定文件或目录的时间和日期
        //    Directory.GetParent(string path);//检索指定路径的父目录，包括相对路径和绝对路径
        //    Directory.Move(string soureDirName, string destName);//将文件或目录及其内容移到新的位置
        //    Directory.SetCreationTime(string path);//为指定的目录或文件设置创建时间和日期
        //    Directory.SetCurrentDirectory(string path);//将应用程序工作的当前路径设为指定路径
        //    Directory.SetLastAccessTime(string path);//为指定的目录或文件设置上次访问时间和日期
        //    Directory.SetLastWriteTime(string path);//为指定的目录和文件设置上次访问时间和日期


        private void btn_Start_Click(object sender, EventArgs e)
        {
            //if (!Halcon.CamConnect)
            //{
            //    MessageBox.Show("相机链接异常，请检查!");
            //    return;
            //}           
            //if (HslCommunication.plc_connect_result)
            //{
            //    HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.StartAdd, 1);
            //    HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.Trigger_Detection, 0);
            //}
            //else
            //{
            //    MessageBox.Show("PLC链接异常，请检查!");
            //    return;
            //}
            if (!CamConnectResult)
            {
                if (MessageBox.Show("相机未连接，启动本地测试？", "开始检测提示", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    return;
                }
            }
            m_Pause = true;
            Permission = "访客";
            btn_Start.Enabled = false;
            btn_Stop.Enabled = true;
            btn_Connutius.Enabled = true;
            btn_Connect.Enabled = false;
            btn_MotorSetting.Enabled = false;
            btn_Login.Enabled = false;
            btn_Cam1.Enabled = false;
            btn_Close_System.Enabled = false;
            btn_changeProduct.Enabled = false;          
            MainThread = new Thread(MainRun);
            MainThread.IsBackground = true;
            MainThread.Start();       
        }


        private void btn_Stop_Click(object sender, EventArgs e)
        {
            if (HslCommunication.plc_connect_result)
            {
                HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.StartAdd, 0);             
            }
            else
            {
                MessageBox.Show("链接异常，请检查!");
                return;
            }
            btn_Start.Enabled = true;
            btn_Stop.Enabled = false;
            btn_Connutius.Enabled = false;
            btn_Connect.Enabled = false;
            btn_MotorSetting.Enabled = false;
            btn_Login.Enabled = true;
            btn_Cam1.Enabled = false;
            btn_Close_System.Enabled = false;
            if (MainThread != null)
            {
                MainThread.Abort();
            }           
        }

        private void btn_Connutius_Click(object sender, EventArgs e)
        {
            if (btn_Connutius.Text == "暂停")
            {
                m_Pause = false;
                btn_Connutius.Text = "继续";
            }
            else
            {
                m_Pause = true;
                btn_Connutius.Text = "暂停";
            }
        }
        #region 任务队列
        #endregion
        public static string strDateTime;
        public static string strDateTimeDay;

        private int ImgSaveOut()
        {
            #region 保存图片
            // 文件命名规则
            if (Parameters.specifications.SaveDefeatImage)
            {
                string stfFileNameOut ="Out" + strDateTime;  // 默认的图像保存名称
                string pathOut = Parameters.commministion.ImageSavePath + "Image/"+ strDateTimeDay + "/Out/";
                if (!System.IO.Directory.Exists(pathOut))
                {
                    System.IO.Directory.CreateDirectory(pathOut);//不存在就创建文件夹
                }
                HOperatorSet.WriteImage(hObjectOut, "jpeg", 0, pathOut + stfFileNameOut + ".jpeg");
            }
            if (Parameters.specifications.SaveOrigalImage)
            {
                string stfFileNameIn = "In" + strDateTime;  // 默认的图像保存名称
                string pathIn = Parameters.commministion.ImageSavePath + "Image/" + strDateTimeDay + "/In/";
                if (!System.IO.Directory.Exists(pathIn))
                {
                    System.IO.Directory.CreateDirectory(pathIn);//不存在就创建文件夹
                }
                HOperatorSet.WriteImage(hObjectIn, "jpeg", 0, pathIn + stfFileNameIn + ".jpeg");
            }

            return 0;
            #endregion
        }

        private void btn_检测设置1_Click(object sender, EventArgs e)
        {
            相机检测设置 flg = new 相机检测设置();
            flg.ShowDialog();
        }
        public static string User = "访客";
        public static string Permission = "访客";
        void User_TransfEvent(登陆界面.Users value)
        {
            User= value.Name;
            Permission = value.Permission;
        }
        private void UpdataUI()
        {
            if (Permission == "开发员")
            {
                btn_Start.Enabled = true;
                btn_Stop.Enabled = false;
                btn_Connutius.Enabled = true;
                btn_Login.Enabled = true;
                btn_Connect.Enabled = true;
                btn_MotorSetting.Enabled = true;
                btn_Cam1.Enabled = true;
                btn_Close_System.Enabled = true;
                btn_Minimizid_System.Enabled = true;
                lab_User.Text = Permission;
                btn_changeProduct.Enabled = true;
                btn_MotorSetting.Enabled= true;
            }
            else if (Permission == "管理员")
            {
                btn_Start.Enabled = true;
                btn_Stop.Enabled = false;
                btn_Connutius.Enabled = true;
                btn_Login.Enabled = true;
                btn_Connect.Enabled = true;
                btn_MotorSetting.Enabled = true;
                btn_Cam1.Enabled = true;
                btn_Close_System.Enabled = true;
                btn_Minimizid_System.Enabled = true;
                lab_User.Text = Permission;
                 btn_changeProduct.Enabled = true;
                btn_MotorSetting.Enabled = true;
            }
            else if (Permission == "操作员")
            {
                btn_Start.Enabled = false;
                btn_Stop.Enabled = false;
                btn_Connutius.Enabled = false;
                btn_Login.Enabled = true;
                btn_Connect.Enabled = false;
                btn_MotorSetting.Enabled = false;
                btn_Cam1.Enabled = false;
                btn_Close_System.Enabled = false;
                btn_Minimizid_System.Enabled = false;
                lab_User.Text = Permission;
                btn_changeProduct.Enabled = false;
                btn_MotorSetting.Enabled = false;
            }
            else
            {
                btn_Start.Enabled = true;
                btn_Stop.Enabled = false;
                btn_Connutius.Enabled = true;
                btn_Connect.Enabled = false;
                btn_MotorSetting.Enabled = false;
                btn_Login.Enabled = true;
                btn_Cam1.Enabled = false;
                btn_Close_System.Enabled = false;
                btn_Minimizid_System.Enabled = false;
                lab_User.Text = Permission;
                btn_changeProduct.Enabled = false;
                btn_MotorSetting.Enabled = false;
            }
        }
        private void btn_Login_Click(object sender, EventArgs e)
        {
            登陆界面 flg = new 登陆界面();
            flg.TransfEvent += User_TransfEvent;
            flg.ShowDialog();
            UpdataUI();
        }
        public static string Product = "初始化";
        void Product_TransfEvent(string value)
        {
            Product = value;
        }
        private void btn_changeProduct_Click(object sender, EventArgs e)
        {
            切换产品 flg = new 切换产品();
            flg.TransfEvent += Product_TransfEvent;
            flg.ShowDialog();
            lab_Product.Text = Product;
        }
        private delegate void InvokeHandler();
        private void btn_SpecicationSetting_Click(object sender, EventArgs e)
        {
            示教器 flg = new 示教器();
            flg.ShowDialog();
        }

        private bool detection(HWindow hWindow,HObject hImage  )
        {
            string time = "";
            相机检测设置.Detection(hWindow, hImage, ref location, ref time);
            this.Invoke(new InvokeHandler(delegate ()
            {
                DataGridViewRow[] dtRows = new DataGridViewRow[Parameters.specifications.DetectionRect2Num];
                for (int i = 0; i < Parameters.specifications.DetectionRect2Num; i++)
                {
                    dtRows[i] = new DataGridViewRow();
                    dtRows[i].CreateCells(uiDataGridView1);
                    dtRows[i].Cells[0].Value = location[i].Length1.ToString("0.0000");
                    dtRows[i].Cells[1].Value = location[i].Length2.ToString("0.0000");
                    dtRows[i].Cells[2].Value = location[i].Length3.ToString("0.0000");
                    dtRows[i].Cells[3].Value = location[i].Length4.ToString("0.0000");
                    
                    
                }
                uiDataGridView1.Rows.AddRange(dtRows);
                相机检测设置.WriteCsv(location);
            }));
            return true;
        }

        private void uiDataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            e.Row.HeaderCell.Value = string.Format("{0}", e.Row.Index + 1);
        }

        private void uiDataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,e.RowBounds.Location.Y,uiDataGridView1.RowHeadersWidth - 4,e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics,(e.RowIndex + 1).ToString(),uiDataGridView1.RowHeadersDefaultCellStyle.Font,rectangle,uiDataGridView1.RowHeadersDefaultCellStyle.ForeColor,TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
    }
}
