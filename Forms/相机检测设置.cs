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
using static WY_App.Utility.Parameters;
using WY_App.UserControls;
using Sunny.UI;
using System.Data.Common;
using OpenCvSharp;
using static System.Windows.Forms.MonthCalendar;
using System.Data.SqlTypes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using SevenZip.Compression.LZ;
using System.Threading.Tasks;
using HslCommunication;
using System.Security.Cryptography;
using IKapBoardClassLibrary;

namespace WY_App
{
    public partial class 相机检测设置 : Form
    {
        static HWindow hWindow = new HWindow();
        static HRect1[] BaseReault = new HRect1[4];
        static Rect2[] decetionReault = new Rect2[100];
        public static int baseNum = 0;
        public static int decetionNum = 0;
        public static int decetionCricleNum = 0;
        public 相机检测设置()
        {
            InitializeComponent();
            hWindow = hWindowControl1.HalconWindow;
            HOperatorSet.SetPart(hWindow, 0, 0, -1, -1);//设置窗体的规格           
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
        }
        System.Drawing.Point downPoint;

        private void panel4_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new System.Drawing.Point(this.Location.X + e.X - downPoint.X,
                    this.Location.Y + e.Y - downPoint.Y);
            }
        }

        private void panel4_MouseDown(object sender, MouseEventArgs e)
        {
            downPoint = new System.Drawing.Point(e.X, e.Y);
        }


        private void btn_Close_System_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Close_System_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        

        private void btn_DrawXbase_Click(object sender, EventArgs e)
        {
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            Halcon.DetectionDrawLineAOI(0, hWindow, MainForm.hImage, ref Parameters.detectionSpec);
        }

        private void 相机1检测设置_Load(object sender, EventArgs e)
        {
            HOperatorSet.SetPart(hWindow, 0, 0, -1, -1);//设置窗体的规格            
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            HOperatorSet.SetPart(hWindow, 0, 0, -1, -1);//设置窗体的规格
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            num_DetectionRect2Num.Value = Parameters.specifications.DetectionRect2Num;
            uiDoubleUpDown1.Value = Parameters.specifications.DetectionCricleNum;
            cmb_Indication.Items.Clear();
            uiComboBox1.Items.Clear();
            for (int i = 0; i < num_DetectionRect2Num.Value; i++)
            {
                cmb_Indication.Items.Add(i + 1);
            }
            for (int i = 0; i < uiDoubleUpDown1.Value; i++)
            {
                uiComboBox1.Items.Add(i + 1);
            }
            uiDoubleUpDown2.Value = Parameters.detectionSpec.分割阈值;
            chk_MeanImageEnabled.Checked = Parameters.specifications.MeanImageEnabled;
            cmb_MeanImageList.DataSource = Enum.GetNames(typeof(Parameters.MeanImageEnum));
            cmb_MeanImageList.SelectedIndex = Parameters.specifications.meanImageEnum;
            cmb_Indication.SelectedIndex = 0;
            chk_SaveOrigalImage.Checked = Parameters.specifications.SaveOrigalImage;
            chk_SaveDefeatImage.Checked = Parameters.specifications.SaveDefeatImage;
            checkBox2.Checked = Parameters.specifications.ContourLineEnabled;
            checkBox3.Checked = Parameters.specifications.DefectionEnabled;
            num_DetectionRect2Num.Value = Parameters.specifications.DetectionRect2Num;
            txt_PixelResolutionRow.Value = Parameters.specifications.PixelResolutionRow;
            txt_PixelResolutionColumn.Value = Parameters.specifications.PixelResolutionColum;
            checkBox1.Checked = Parameters.specifications.ImageVerifyEnabled;
            checkBox4.Checked= Parameters.specifications.DefectionAbled;
            DataGridViewRow row0 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row0);
            DataGridViewRow row1 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row1);
            DataGridViewRow row2 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row2);

            DataGridViewRow row3 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row3);
            DataGridViewRow row4 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row4);
            DataGridViewRow row5 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row5);

            uiDataGridView1.Rows[0].Cells[0].Value = "Y";
            uiDataGridView1.Rows[1].Cells[0].Value = "X";
            uiDataGridView1.Rows[2].Cells[0].Value = "Y距离";
            uiDataGridView1.Rows[3].Cells[0].Value = "X距离";
            uiDataGridView1.Rows[4].Cells[0].Value = "点距离";
            uiDataGridView1.Rows[5].Cells[0].Value = "线距离";
        }

        private void btn_OpenTestimage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();

            if (openfile.ShowDialog() == DialogResult.OK && (openfile.FileName != ""))
            {
                //HOperatorSet.SetPart(hWindow, 0, 0, -1, -1);//设置窗体的规格
                //HOperatorSet.SetPart(hWindow, 0, 0, -1, -1);//设置窗体的规格
                Halcon.ImgDisplay(openfile.FileName, hWindow); 
            }
            openfile.Dispose();
        }

        private void btn_DrawY1base_Click(object sender, EventArgs e)
        {
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            Halcon.DetectionDrawLineAOI(1, hWindow, MainForm.hImage, ref Parameters.detectionSpec);
        }

        private void btn_DrawY2base_Click(object sender, EventArgs e)
        {
            HOperatorSet.DispObj(MainForm.hImage,hWindow);
            Halcon.DetectionDrawLineAOI(2, hWindow, MainForm.hImage, ref Parameters.detectionSpec);
        }
        public static void showXBase(object sender, EventArgs e)
        {
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            Halcon.DetectionHalconLine(0, hWindow, MainForm.hImage, Parameters.detectionSpec, 800, ref BaseReault[0]);
        }

        public static void showY1Base(object sender, EventArgs e)
        {
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            Halcon.DetectionHalconLine(1, hWindow, MainForm.hImage, Parameters.detectionSpec, 800, ref BaseReault[1]);

        }   

        public static void showY2Base(object sender, EventArgs e)
        {
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            Halcon.DetectionHalconLine(2, hWindow, MainForm.hImage, Parameters.detectionSpec, 800, ref BaseReault[2]);
        }

        public static void ShowMeasurePos(ref Location location)
        {
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            HTuple IsOverlapping1 = new HTuple();
            HTuple IsOverlapping2 = new HTuple();
            try
            {
                Halcon.DetectionHalconLine(0, hWindow, MainForm.hImage, Parameters.detectionSpec, 800, ref BaseReault[0]);
                Halcon.DetectionHalconLine(1, hWindow, MainForm.hImage, Parameters.detectionSpec, 800, ref BaseReault[1]);
                Halcon.DetectionHalconLine(2, hWindow, MainForm.hImage, Parameters.detectionSpec, 800, ref BaseReault[2]);
                HOperatorSet.SetColor(hWindow, "red");
                HOperatorSet.IntersectionLines(BaseReault[0].Row1, BaseReault[0].Colum1, BaseReault[0].Row2, BaseReault[0].Colum2, BaseReault[1].Row1, BaseReault[1].Colum1, BaseReault[1].Row2, BaseReault[1].Colum2, out BaseReault[3].Row1, out BaseReault[3].Colum1, out IsOverlapping1);
                HOperatorSet.DispCross(hWindow, BaseReault[3].Row1, BaseReault[3].Colum1, 600, 0);
                HOperatorSet.IntersectionLines(BaseReault[0].Row1, BaseReault[0].Colum1, BaseReault[0].Row2, BaseReault[0].Colum2, BaseReault[2].Row1, BaseReault[2].Colum1, BaseReault[2].Row2, BaseReault[2].Colum2, out BaseReault[3].Row2, out BaseReault[3].Colum2, out IsOverlapping2);
                HOperatorSet.DispCross(hWindow, BaseReault[3].Row2, BaseReault[3].Colum2, 600, 0);

                if(Parameters.specifications.ImageVerifyEnabled)
                {
                    Halcon.DetectionMeasurePos(decetionNum,hWindow, MainForm.hImage, BaseReault[3], Parameters.detectionSpec.decetionRect2[decetionNum], ref location);
                }
                else
                {
                    HOperatorSet.DispCross(hWindow, Parameters.detectionSpec.decetionRect2[decetionNum].Row, Parameters.detectionSpec.decetionRect2[decetionNum].Colum, 60, 0);
                    HOperatorSet.SetColor(hWindow, "red");
                    location.Row = Parameters.detectionSpec.decetionRect2[decetionNum].Row * Parameters.specifications.PixelResolutionRow;
                    location.Colum = Parameters.detectionSpec.decetionRect2[decetionNum].Colum * Parameters.specifications.PixelResolutionColum;
                    location.Length1 = Math.Abs(Parameters.detectionSpec.decetionRect2[decetionNum].Row - BaseReault[3].Row1.D) * Parameters.specifications.PixelResolutionRow;
                    location.Length2 = Math.Abs(Parameters.detectionSpec.decetionRect2[decetionNum].Colum - BaseReault[3].Colum1.D) * Parameters.specifications.PixelResolutionColum;
                    location.Length3 = Math.Sqrt(location.Length1 * location.Length1 + location.Length2 * location.Length2);
                    HTuple Length4 = new HTuple();
                    HOperatorSet.DistancePl(Parameters.detectionSpec.decetionRect2[decetionNum].Row, Parameters.detectionSpec.decetionRect2[decetionNum].Colum, BaseReault[3].Row1, BaseReault[3].Colum1, BaseReault[3].Row2, BaseReault[3].Colum2, out Length4);
                    location.Length4 = Length4.D * Parameters.specifications.PixelResolutionColum;
                }               
                HOperatorSet.SetColor(hWindow, "green");
                HOperatorSet.SetTposition(hWindow, Parameters.detectionSpec.decetionRect2[decetionNum].Row, Parameters.detectionSpec.decetionRect2[decetionNum].Colum);
                HOperatorSet.WriteString(hWindow, "点:" + (decetionNum+1));
                HOperatorSet.SetTposition(hWindow, 200 * decetionNum, 100);
                HOperatorSet.WriteString(hWindow, "点:" + decetionNum + "DY:" + location.Length1.ToString("0.000") + "DX:" + location.Length2.ToString("0.000") + "PO:" + location.Length3.ToString("0.000") + "PL:" + location.Length4.ToString("0.000"));
            }
            catch
            {
                IsOverlapping1.Dispose();
                IsOverlapping2.Dispose();
                MessageBox.Show("检测失败，请确认");
                return;
            }

        }

        public static void ShowCriclePos(ref Location location)
        {
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            HTuple IsOverlapping1 = new HTuple();
            HTuple IsOverlapping2 = new HTuple();
            try
            {
                Halcon.DetectionHalconLine(0, hWindow, MainForm.hImage, Parameters.detectionSpec, 200, ref BaseReault[0]);
                Halcon.DetectionHalconLine(1, hWindow, MainForm.hImage, Parameters.detectionSpec, 200, ref BaseReault[1]);
                Halcon.DetectionHalconLine(2, hWindow, MainForm.hImage, Parameters.detectionSpec, 200, ref BaseReault[2]);
                HOperatorSet.SetColor(hWindow, "red");
                HOperatorSet.IntersectionLines(BaseReault[0].Row1, BaseReault[0].Colum1, BaseReault[0].Row2, BaseReault[0].Colum2, BaseReault[1].Row1, BaseReault[1].Colum1, BaseReault[1].Row2, BaseReault[1].Colum2, out BaseReault[3].Row1, out BaseReault[3].Colum1, out IsOverlapping1);
                HOperatorSet.DispCross(hWindow, BaseReault[3].Row1, BaseReault[3].Colum1, 60, 0);
                HOperatorSet.IntersectionLines(BaseReault[0].Row1, BaseReault[0].Colum1, BaseReault[0].Row2, BaseReault[0].Colum2, BaseReault[2].Row1, BaseReault[2].Colum1, BaseReault[2].Row2, BaseReault[2].Colum2, out BaseReault[3].Row2, out BaseReault[3].Colum2, out IsOverlapping2);
                HOperatorSet.DispCross(hWindow, BaseReault[3].Row2, BaseReault[3].Colum2, 60, 0);
                Halcon.DetectionHalconCricle(hWindow, MainForm.hImage, Parameters.detectionSpec.decetionCircle[decetionCricleNum], Parameters.detectionSpec.decetionCircle[decetionCricleNum].Row, Parameters.detectionSpec.decetionCircle[decetionCricleNum].Colum, 200, ref pointReaultCricle);
                Halcon.DetectionCriclePos((uint)(decetionCricleNum + 1), hWindow, MainForm.hImage, BaseReault[3], pointReaultCricle, ref location);
            }
            catch
            {
                IsOverlapping1.Dispose();
                IsOverlapping2.Dispose();
                MessageBox.Show("检测失败，请确认");
                return;
            }

        }
        public static List<Location> locationsResult = new List<Location>();
        private void btn_Detection_Click(object sender, EventArgs e)
        {
            string detectionTime = "" ;
            DateTime dtNow = System.DateTime.Now;  // 获取系统当前时间
            MainForm.strDateTime = dtNow.ToString("yyyyMMddHHmmss");
            MainForm.strDateTimeDay = dtNow.ToString("yyyy-MM-dd");
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            Location offSet = new Location();
            locationsResult.Clear();
            Detection(hWindow,MainForm.hImage,ref offSet, ref locationsResult, ref detectionTime);
            lab_detectionTime.Text = detectionTime;
        }

        /// <summary>
        /// 写入csv
        /// </summary>
        /// <param name="result">写入内容 ----单元格内容，单元格内容-----</param>
        public static void WriteCsv(List<Location> location)
        {
            string path = Parameters.commministion.ImageSavePath + "/Data/" + MainForm.strDateTimeDay + "/";//保存路径
            string fileName = path + MainForm.strDateTime + ".csv";//文件名
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(fileName))
            {
                StreamWriter sw = new StreamWriter(fileName, true, Encoding.UTF8);
                string str1 = "序号" + "," + "行(Y)坐标" + "," + "列(X)坐标" + "," + "行(Y)绝对值" + "," + "列(X)绝对值" + "," + "基准点距离" + "," + "基准线距离" +","+ "行(Y)标准差" + "," + "列(X)标准差" + "," + "基准点标准差" + "," + "基准线标准差" + "\t\n";
                sw.Write(str1);
                sw.Close();             
            }
            StreamWriter swl = new StreamWriter(fileName, true, Encoding.UTF8);
            try
            {
                for (int i = 0; i < Parameters.specifications.DetectionRect2Num; i++)
                {
                    string result;
                    result = location[i].Row.ToString("0.0000") + "," +
                    location[i].Colum.ToString("0.0000") + "," +
                    location[i].Length1.ToString("0.0000") + "," +
                    location[i].Length2.ToString("0.0000") + "," +
                    location[i].Length3.ToString("0.0000") + "," +
                    location[i].Length4.ToString("0.0000") + "," +
                    (Parameters.cursorLocation.Location[i].Length1 - location[i].Length1).ToString("0.0000") + "," +
                    (Parameters.cursorLocation.Location[i].Length2 - location[i].Length2).ToString("0.0000") + "," +
                    (Parameters.cursorLocation.Location[i].Length3 - location[i].Length3).ToString("0.0000") + "," +
                    (Parameters.cursorLocation.Location[i].Length4 - location[i].Length4).ToString("0.0000") ;
                    string str = (i + 1) + "," + result + "\t\n";
                    swl.Write(str);
                }
            }
            catch(Exception ex)
            {

            }
           
            swl.Close();
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                Parameters.specifications.DetectionRect2Num =(int)num_DetectionRect2Num.Value;
                Parameters.specifications.DetectionCricleNum = (int)uiDoubleUpDown1.Value;
                Parameters.specifications.PixelResolutionRow = txt_PixelResolutionRow.Value;
                Parameters.specifications.PixelResolutionColum = txt_PixelResolutionColumn.Value;
                Parameters.specifications.MeanImageEnabled = chk_MeanImageEnabled.Checked;
                XMLHelper.serialize<Parameters.Specifications>(Parameters.specifications, Parameters.commministion.productName + "/Specifications.xml");
                XMLHelper.serialize<Parameters.DetectionSpec>(Parameters.detectionSpec, Parameters.commministion.productName + "/DetectionSpec.xml");
            }
            
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        static DetectionSpec decetionRect;
        public static bool Detection(HWindow hwindow, HObject hImage, ref Location Offsetlocations, ref List<Location> locations, ref string detectionTime)
        {
            System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //  开始监视代码运行时间
            if (Parameters.specifications.MeanImageEnabled)
            {
                Halcon.DetectionMeanImageint((MeanImageEnum)Parameters.specifications.meanImageEnum, MainForm.hImage, ref hImage);
                HOperatorSet.DispObj(hImage, hwindow);
            }
            HObject ho_RectangleReduce=new HObject(), ho_SortedContours = new HObject(); ;
            HObject hObject = new HObject();
            HTuple ho_phi = new HTuple();
            getMeasureBase(hImage, hwindow ,Parameters.detectionSpec);

            if (Parameters.specifications.ContourLineEnabled)
            {    
                HTuple IsOverlapping1 = new HTuple();
                HTuple IsOverlapping2 = new HTuple();
                HTuple phi = new HTuple();
                HTuple row = new HTuple();
                HTuple col = new HTuple();
                HTuple py = new HTuple();
                HTuple px = new HTuple();
                HTuple HomMat2DIdentity = new HTuple(), HomMat2DRotate = new HTuple();
                try
                {
                    phi = Parameters.detectionSpec.BasePoint.Radius - basePoint.Radius;
                    HOperatorSet.HomMat2dIdentity(out HomMat2DIdentity);
                    HOperatorSet.HomMat2dRotate(HomMat2DIdentity, phi,  7231.24, 9903.31, out HomMat2DRotate);                 
                    HOperatorSet.AffineTransImage(hImage, out hObject, HomMat2DRotate, "constant", "false");
                    HOperatorSet.DispObj(hObject, hwindow);
                    HomMat2DIdentity.Dispose();
                    HomMat2DRotate.Dispose();
                    HOperatorSet.AffineTransPoint2d(HomMat2DRotate, basePoint.Colum,basePoint.Row,out px,out py);                
                    HOperatorSet.TupleDeg(phi, out ho_phi) ;
                    if (HslCommunication.plc_connect_result && !Parameters.specifications.DefectionAbled)
                    {
                        OperateResult _connected = HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[31], (Int16)(ho_phi.D * 100));
                        LogHelper.WriteInfo(Parameters.plcParams.WriteAdd[31] + "写入R修正" + (Int16)(ho_phi.D) + _connected.IsSuccess);
                        HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.Trigger_Detection, 2);
                       
                    }
                    else if (HslCommunication.plc_connect_result && Parameters.specifications.DefectionAbled)
                    {
                        OperateResult _connected = HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[31], 0);
                        LogHelper.WriteInfo(Parameters.plcParams.WriteAdd[31] + "写入R修正" + 0 + _connected.IsSuccess);
                    }
                    py.Dispose();
                    px.Dispose();

                    HOperatorSet.ClearWindow(hwindow);
                    HOperatorSet.DispObj(hObject, hwindow);
                    getMeasureBase(hObject, hwindow,Parameters.detectionSpec);

                    row = (Parameters.detectionSpec.BasePoint.Row - basePoint.Row) * Parameters.specifications.PixelResolutionRow;
                    col = (Parameters.detectionSpec.BasePoint.Colum - basePoint.Colum) * Parameters.specifications.PixelResolutionColum;   
                    
                    
                    if (HslCommunication.plc_connect_result && !Parameters.specifications.DefectionAbled)
                    {
                        OperateResult _connected = HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[29], (Int16)(col.D * 95));
                        LogHelper.WriteInfo(Parameters.plcParams.WriteAdd[29] + "写入X修正" + (Int16)(col.D * 0.95) + _connected.IsSuccess);

                        _connected = HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[30], (Int16)(row.D * 105));
                        LogHelper.WriteInfo(Parameters.plcParams.WriteAdd[30] + "写入Y修正" + (Int16)(row.D * 1.05) + _connected.IsSuccess);

                        LogHelper.WriteInfo(Parameters.plcParams.Trigger_Detection + "位置矫正信号写入2" + _connected.IsSuccess);
                        MainForm.ServerEvent.Set();
                    }
                    else if (HslCommunication.plc_connect_result && Parameters.specifications.DefectionAbled)
                    {
                        OperateResult _connected = HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[29], 0);
                        LogHelper.WriteInfo(Parameters.plcParams.WriteAdd[29] + "写入X修正" + 0 + _connected.IsSuccess);

                        _connected = HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.WriteAdd[30], 0);
                        LogHelper.WriteInfo(Parameters.plcParams.WriteAdd[30] + "写入Y修正" + 0 + _connected.IsSuccess);

                        HslCommunication._NetworkTcpDevice.Write(Parameters.plcParams.Trigger_Detection, 2);
                        LogHelper.WriteInfo(Parameters.plcParams.Trigger_Detection + "位置矫正信号写入2" + _connected.IsSuccess);
                        MainForm.ServerEvent.Set();
                    }
                }
                catch
                {
                    ho_phi.Dispose();
                    phi.Dispose();
                    row.Dispose();
                    col.Dispose();
                    py.Dispose();
                    px.Dispose();
                    IsOverlapping1.Dispose();
                    IsOverlapping2.Dispose();
                    HomMat2DIdentity.Dispose();
                    HomMat2DRotate.Dispose();
                    MessageBox.Show("矫正基准检测失败，请确认", "矫正基准检测失败提示", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return false;
                }
                try
                {
                    HOperatorSet.HomMat2dIdentity(out HomMat2DIdentity);
                    HOperatorSet.HomMat2dTranslate(HomMat2DIdentity,  Parameters.detectionSpec.BasePoint.Row - basePoint.Row, Parameters.detectionSpec.BasePoint.Colum - basePoint.Colum,out HomMat2DRotate);
                    HOperatorSet.AffineTransImage(hObject, out hImage, HomMat2DRotate, "constant", "false");
                    HOperatorSet.ClearWindow(hwindow);
                    HOperatorSet.DispObj(hImage, hwindow);
                    decetionRect = new DetectionSpec();
                    decetionRect = Parameters.detectionSpec;
                    HOperatorSet.AffineTransPoint2d(HomMat2DIdentity, Parameters.detectionSpec.baseRect1[2].Colum1, Parameters.detectionSpec.baseRect1[2].Row1,out col, out row);
                    decetionRect.baseRect1[2].Colum1=col;
                    decetionRect.baseRect1[2].Row1 = row;

                    HOperatorSet.AffineTransPoint2d(HomMat2DIdentity, Parameters.detectionSpec.baseRect1[2].Colum2, Parameters.detectionSpec.baseRect1[2].Row2, out col, out row);

                    decetionRect.baseRect1[2].Colum2 = col;
                    decetionRect.baseRect1[2].Row2 = row;

                    getMeasureBase(hImage, hwindow, decetionRect);
                    DetectAOI(hImage, hWindow, out ho_RectangleReduce, out ho_SortedContours);

                    row = (Parameters.detectionSpec.BasePoint.Row - basePoint.Row) * Parameters.specifications.PixelResolutionRow;
                    col = (Parameters.detectionSpec.BasePoint.Colum - basePoint.Colum) * Parameters.specifications.PixelResolutionColum;
                    phi = Parameters.detectionSpec.BasePoint.Radius - basePoint.Radius;
                    HOperatorSet.SetColor(hwindow, "red");
                    HOperatorSet.IntersectionLines(BaseReault[0].Row1, BaseReault[0].Colum1, BaseReault[0].Row2, BaseReault[0].Colum2, BaseReault[1].Row1, BaseReault[1].Colum1, BaseReault[1].Row2, BaseReault[1].Colum2, out BaseReault[3].Row1, out BaseReault[3].Colum1, out IsOverlapping1);
                    HOperatorSet.SetColor(hwindow, "yellow");
                    HOperatorSet.DispCross(hwindow, Parameters.detectionSpec.BasePhi.Row1, Parameters.detectionSpec.BasePhi.Colum1, 600, 0);
                    HOperatorSet.SetColor(hwindow, "red");
                    HOperatorSet.DispCross(hwindow, BaseReault[3].Row1, BaseReault[3].Colum1, 600, 0);

                    HOperatorSet.IntersectionLines(BaseReault[0].Row1, BaseReault[0].Colum1, BaseReault[0].Row2, BaseReault[0].Colum2, BaseReault[2].Row1, BaseReault[2].Colum1, BaseReault[2].Row2, BaseReault[2].Colum2, out BaseReault[3].Row2, out BaseReault[3].Colum2, out IsOverlapping2);
                    HOperatorSet.SetColor(hwindow, "yellow");
                    HOperatorSet.DispCross(hwindow, Parameters.detectionSpec.BasePhi.Row2, Parameters.detectionSpec.BasePhi.Colum2, 300, 0);
                    HOperatorSet.SetColor(hwindow, "red");
                    HOperatorSet.DispCross(hwindow, BaseReault[3].Row2, BaseReault[3].Colum2, 300, 0);

                    HOperatorSet.SetTposition(hwindow, BaseReault[3].Row1 - 800, BaseReault[3].Colum1 + 200);
                    HOperatorSet.WriteString(hwindow, "X修正后:" + col.D.ToString("0.00"));
                    HOperatorSet.SetTposition(hwindow, BaseReault[3].Row1 - 400, BaseReault[3].Colum1 + 200);
                    HOperatorSet.WriteString(hwindow, "Y修正后:" + row.D.ToString("0.00"));
                    HOperatorSet.SetTposition(hwindow, BaseReault[3].Row1, BaseReault[3].Colum1 + 200);
                    HOperatorSet.WriteString(hwindow, " R修正后:" + phi.D.ToString("0.00"));
                    phi.Dispose();
                }
                catch
                {
                    ho_phi.Dispose();
                    phi.Dispose();
                    row.Dispose();
                    col.Dispose();
                    py.Dispose();
                    px.Dispose();
                    IsOverlapping1.Dispose();
                    IsOverlapping2.Dispose();
                    HomMat2DIdentity.Dispose();
                    HomMat2DRotate.Dispose();
                    MessageBox.Show("基准检测失败，请确认", "基准检测失败提示", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return false;
                }
     
                if (Parameters.specifications.ImageVerifyEnabled)
                {
                    try
                    {
                        HTuple Angles=new HTuple();
                        HTuple phis = new HTuple();
                        HOperatorSet.GetContourAngleXld(ho_SortedContours,"abs","range",3,out Angles);
                        HOperatorSet.TupleRad(90, out phis);
                        for (int i = 0; i < Parameters.specifications.DetectionRect2Num; i++)
                        {
                            Location location = new Location();

                            HOperatorSet.AffineTransPoint2d(HomMat2DRotate, Parameters.detectionSpec.decetionRect2[i].Row, Parameters.detectionSpec.decetionRect2[i].Colum, out py, out px);
                            Parameters.detectionSpec.decetionRect2[i].Phi = Angles.TupleSelect(Angles.Length / Parameters.specifications.DetectionRect2Num * i) + phis.D;
                            Halcon.DetectionMeasurePos (i+1,hwindow, hImage, BaseReault[3], Parameters.detectionSpec.decetionRect2[i], ref location);
                            locations.Add (location);
                        }
                        for (int i = 0; i < Parameters.specifications.DetectionCricleNum; i++)
                        {
                            Halcon.DetectionHalconCricle(hwindow, hImage, Parameters.detectionSpec.decetionCircle[i], Parameters.detectionSpec.decetionCircle[i].Row, Parameters.detectionSpec.decetionCircle[i].Colum, 50, ref pointReaultCricle);
                            Location location = new Location();
                            Halcon.DetectionCriclePos((uint)(i + 1), hwindow, hImage, BaseReault[3], pointReaultCricle, ref location);
                            locations.Add(location);
                        }
                    }
                    catch
                    {
                        IsOverlapping1.Dispose();
                        IsOverlapping2.Dispose();
                        MessageBox.Show("检测失败，请确认");
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        Halcon.DetectionLocation(hwindow, hImage, ho_SortedContours, BaseReault[3], ref locations);
                        for (uint i = 0; i < Parameters.specifications.DetectionCricleNum; i++)
                        {
                            Halcon.DetectionHalconCricle(hwindow, ho_RectangleReduce, Parameters.detectionSpec.decetionCircle[i], Parameters.detectionSpec.decetionCircle[i].Row, Parameters.detectionSpec.decetionCircle[i].Colum, 100, ref pointReaultCricle);
                            Location location = new Location();
                            Halcon.DetectionCriclePos((uint)(i + 1), hwindow, ho_RectangleReduce, BaseReault[3], pointReaultCricle, ref location);
                            locations.Add(location);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("检测失败，请确认");
                        return false;
                    }
                    

                    //Halcon.DetectionLinePos(hWindow,hImage, ho_SortedContours, BaseReault[3], ref locations);
                }
                phi.Dispose();
                row.Dispose();
                col.Dispose();
                IsOverlapping1.Dispose();
                IsOverlapping2.Dispose();
                HomMat2DIdentity.Dispose();
                HomMat2DRotate.Dispose();
                WriteCsv(locations);
            }
            if (Parameters.specifications.DefectionEnabled)
            {
                Halcon.DetectionMeasure(hwindow, hImage, ho_SortedContours);
            }
            ho_SortedContours.Dispose();
            ho_RectangleReduce.Dispose();
            stopwatch.Stop(); //  停止监视
            TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
            detectionTime = timespan.TotalMilliseconds.ToString();  //  总毫秒数           
            return true;
        }
        private void btn_DrawAOI_Click(object sender, EventArgs e)
        {
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            Halcon.DetectionDrawRectAOI(hWindow, MainForm.hImage,  ref Parameters.specifications.检测矩形);
        }

        private void btn_SaveAOI_Click(object sender, EventArgs e)
        {
            Halcon.DetectionSaveAOI(Parameters.commministion.productName + "/halcon/hoRegion.tiff", MainForm.hoRegions);
        }

        private void chk_SaveDefeatImage_CheckedChanged(object sender, EventArgs e)
        {
            Parameters.specifications.SaveDefeatImage = chk_SaveDefeatImage.Checked;
        }

        private void chk_SaveOrigalImage_CheckedChanged(object sender, EventArgs e)
        {
            Parameters.specifications.SaveOrigalImage = chk_SaveOrigalImage.Checked;

        }
        private void btn_XBaseSetting_Click(object sender, EventArgs e)
        {
            baseNum = 0;
            卡尺设置 flg = new  卡尺设置();
            flg.ShowDialog();
        }
        private void btn_Y1BaseSetting_Click(object sender, EventArgs e)
        {
            baseNum = 1;
            卡尺设置 flg = new 卡尺设置();
            flg.ShowDialog();
        }

        private void btn_Y2BaseSetting_Click(object sender, EventArgs e)
        {
            baseNum = 2;
            卡尺设置 flg = new 卡尺设置();
            flg.ShowDialog();
        }

        private static void DetectAOI(HObject hImage, HWindow hWindow,  out HObject ho_ImageReduced,  out HObject ho_SortedContours)
        {
            HObject ho_Rectangle = new HObject();
            HObject ho_Region = new HObject();
            HObject ho_ConnectedRegions = new HObject();
            HObject ho_SelectedRegions = new HObject();
            HObject ho_RegionFillUp;
            HObject ho_ConnectedRegions1, ho_SelectedRegions1, ho_Contours;
            HObject ho_SmoothedContours;
            HTuple hv_Area = new HTuple(), hv_Row1 = new HTuple();
            HTuple hv_Column = new HTuple(), hv_Row4 = new HTuple();
            HTuple hv_Column4 = new HTuple(), hv_Phi1 = new HTuple();
            HTuple hv_Length11 = new HTuple(), hv_Length21 = new HTuple();
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_Contours);
            HOperatorSet.GenEmptyObj(out ho_SmoothedContours);
            ho_Rectangle.Dispose();
            HOperatorSet.SetDraw(hWindow, "margin");
            HOperatorSet.SetColor(hWindow, "green");
            HOperatorSet.GenRectangle1(out ho_Rectangle, Parameters.specifications.检测矩形.Row1, Parameters.specifications.检测矩形.Colum1, Parameters.specifications.检测矩形.Row2, Parameters.specifications.检测矩形.Colum2);
            //显示一下看效果
            HOperatorSet.DispObj(ho_Rectangle, hWindow);

            HOperatorSet.GenRectangle1(out ho_Rectangle, Parameters.specifications.检测矩形.Row1, Parameters.specifications.检测矩形.Colum1, Parameters.specifications.检测矩形.Row2, Parameters.specifications.检测矩形.Colum2);
            //HOperatorSet.AffineTransImage(ho_Rectangle, out ho_Rectangle, HomMat2DRotate, "constant", "false");
            HOperatorSet.ReduceDomain(hImage, ho_Rectangle, out ho_ImageReduced);
            ho_Region.Dispose();
            HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, 5, Parameters.detectionSpec.分割阈值);
            ho_ConnectedRegions1.Dispose();
            HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions1);
            hv_Area.Dispose(); hv_Row1.Dispose(); hv_Column.Dispose();
            HOperatorSet.AreaCenter(ho_ConnectedRegions1, out hv_Area, out hv_Row1, out hv_Column);
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                ho_SelectedRegions1.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions1, out ho_SelectedRegions1, "area", "and", hv_Area.TupleMax(), 999990000);
            }
            ho_RegionFillUp.Dispose();
            HOperatorSet.FillUp(ho_SelectedRegions1, out ho_RegionFillUp);
            hv_Row4.Dispose(); hv_Column4.Dispose(); hv_Phi1.Dispose(); hv_Length11.Dispose(); hv_Length21.Dispose();
            HOperatorSet.SmallestRectangle2(ho_RegionFillUp, out hv_Row4, out hv_Column4, out hv_Phi1, out hv_Length11, out hv_Length21);
            HOperatorSet.GenRectangle2(out ho_Rectangle,  hv_Row4,  hv_Column4, hv_Phi1,  hv_Length11,  hv_Length21);

            HOperatorSet.DispCross(hWindow, hv_Row4, hv_Column4,200,0);
            HOperatorSet.SetTposition(hWindow, hv_Row4, hv_Column4);
            HOperatorSet.WriteString(hWindow, "Phi" + hv_Phi1);
            HOperatorSet.SetTposition(hWindow, hv_Row4+200, hv_Column4);
            HOperatorSet.WriteString(hWindow, "L1:" + hv_Length11 * Parameters.specifications.PixelResolutionColum);
            HOperatorSet.SetTposition(hWindow, hv_Row4+400, hv_Column4);
            HOperatorSet.WriteString(hWindow, "L2:" + hv_Length21 * Parameters.specifications.PixelResolutionRow);
            HOperatorSet.SetLineWidth(hWindow, 1);
            HOperatorSet.DispObj(ho_Rectangle ,hWindow );
            ho_Contours.Dispose();
            HOperatorSet.GenContourRegionXld(ho_RegionFillUp, out ho_Contours, "border");
            ho_SmoothedContours.Dispose();
            HOperatorSet.SmoothContoursXld(ho_Contours, out ho_SmoothedContours, 5);
            HOperatorSet.SortContoursXld(ho_SmoothedContours, out ho_SortedContours, "upper_left", "true", "row");

            HOperatorSet.DispObj(ho_SortedContours, hWindow);
            ho_Region.Dispose();
            ho_RegionFillUp.Dispose();
            ho_ConnectedRegions1.Dispose();
            ho_SelectedRegions1.Dispose();
            ho_Contours.Dispose();
            ho_SmoothedContours.Dispose();
            hv_Area.Dispose();
            hv_Row1.Dispose();
            hv_Column.Dispose();
            hv_Row4.Dispose();
            hv_Column4.Dispose();
            hv_Phi1.Dispose();
            hv_Length11.Dispose();
            hv_Length21.Dispose();

            ho_Rectangle.Dispose();
            ho_Region.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_SelectedRegions.Dispose();
            ho_Contours.Dispose();
            ho_Rectangle.Dispose();
            ho_Region.Dispose();
        }

        private void btn_ShowAOI_Click(object sender, EventArgs e)
        {
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            HObject ho_ImageReduced,  ho_SortedContours;
            DetectAOI(MainForm.hImage, hWindow,out ho_ImageReduced, out ho_SortedContours );
            ho_SortedContours.Dispose();
            ho_ImageReduced.Dispose();
        }


        private void btnn_IndicationTest_Click(object sender, EventArgs e)
        {
            Location location= new Location();
            
            ShowMeasurePos(ref location);
            uiDataGridView1.Rows[0].Cells[1].Value = Parameters.cursorLocation.Location[decetionNum].Row.ToString("0.0");
            uiDataGridView1.Rows[1].Cells[1].Value = Parameters.cursorLocation.Location[decetionNum].Colum.ToString("0.0");
            uiDataGridView1.Rows[2].Cells[1].Value = Parameters.cursorLocation.Location[decetionNum].Length1.ToString("0.0");
            uiDataGridView1.Rows[3].Cells[1].Value = Parameters.cursorLocation.Location[decetionNum].Length2.ToString("0.0");
            uiDataGridView1.Rows[4].Cells[1].Value = Parameters.cursorLocation.Location[decetionNum].Length3.ToString("0.0");
            uiDataGridView1.Rows[5].Cells[1].Value = Parameters.cursorLocation.Location[decetionNum].Length4.ToString("0.0");

            uiDataGridView1.Rows[0].Cells[2].Value = location.Row.ToString("0.0");
            uiDataGridView1.Rows[1].Cells[2].Value = location.Colum.ToString("0.0");
            uiDataGridView1.Rows[2].Cells[2].Value = location.Length1.ToString("0.0");
            uiDataGridView1.Rows[3].Cells[2].Value = location.Length2.ToString("0.0");
            uiDataGridView1.Rows[4].Cells[2].Value = location.Length3.ToString("0.0");
            uiDataGridView1.Rows[5].Cells[2].Value = location.Length4.ToString("0.0");

            uiDataGridView1.Rows[0].Cells[3].Value = (Parameters.cursorLocation.Location[decetionNum].Row - location.Row).ToString("0.0");
            uiDataGridView1.Rows[1].Cells[3].Value = (Parameters.cursorLocation.Location[decetionNum].Colum - location.Colum).ToString("0.0");
            uiDataGridView1.Rows[2].Cells[3].Value = (Parameters.cursorLocation.Location[decetionNum].Length1 - location.Length1).ToString("0.0");
            uiDataGridView1.Rows[3].Cells[3].Value = (Parameters.cursorLocation.Location[decetionNum].Length2 - location.Length2).ToString("0.0");
            uiDataGridView1.Rows[4].Cells[3].Value = (Parameters.cursorLocation.Location[decetionNum].Length3 - location.Length3).ToString("0.0");
            uiDataGridView1.Rows[5].Cells[3].Value = (Parameters.cursorLocation.Location[decetionNum].Length4 - location.Length4).ToString("0.0");
        }

        private void btn_AddKind_Click(object sender, EventArgs e)
        {

        }

        private void cmb_Indication_SelectedIndexChanged(object sender, EventArgs e)
        {
            相机检测设置.decetionNum = cmb_Indication.SelectedIndex;
        }

        private void num_lengthWidthRatio_ValueChanged(object sender, double value)
        {
            cmb_Indication.Items.Clear();
            for (int i = 0; i < num_DetectionRect2Num.Value; i++)
            {
                cmb_Indication.Items.Add(i+1);
            }    
        }

       

        private void btn_MeanImageTest_Click(object sender, EventArgs e)
        {
            HObject hObject = new HObject();
            Halcon.DetectionMeanImageint((MeanImageEnum)cmb_MeanImageList.SelectedIndex, MainForm.hImage,ref hObject);
            HOperatorSet.DispObj(hObject, hWindow);
            hObject.Dispose();
        }

        private void cmb_MeanImageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Parameters.specifications.meanImageEnum = cmb_MeanImageList.SelectedIndex;
        }

        private void chk_MeanImageEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Parameters.specifications.MeanImageEnabled = chk_MeanImageEnabled.Checked;
        }

        private void btn_Minimizid_System_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btn_showXBase_Click(object sender, EventArgs e)
        {
            showXBase(sender, e);
        }

        private void btn_showYBase_Click(object sender, EventArgs e)
        {
            相机检测设置.showY1Base(sender, e);
        }

        private void btn_showY2Base_Click(object sender, EventArgs e)
        {
            相机检测设置.showY2Base(sender, e);
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            try
            {
                Halcon.DetectionDrawRect2AOI(hWindow, ref Parameters.detectionSpec.decetionRect2[decetionNum]);
            }
            catch
            {
                MessageBox.Show("请选择需要检测的边序号");
            }
        }

        private void show_DecetionRect2_Click(object sender, EventArgs e)
        {
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            try
            {
                Halcon.DetectionShowRect2(hWindow, Parameters.detectionSpec.decetionRect2[decetionNum]);
            }
            catch
            {
                MessageBox.Show("请选择需要检测的边序号");
            }
           
        }

        private void hWindowControl1_HMouseWheel(object sender, HMouseEventArgs e)
        {
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(LMap_MouseWheel);
        }

        private void hWindowControl1_HMouseMove(object sender, HMouseEventArgs e)
        {
            
        }

        private void 相机检测设置_DoubleClick(object sender, EventArgs e)
        {

        }
        
        public void LMap_MouseWheel(object sender, MouseEventArgs e)
        {
            //当e.Delta > 0时鼠标滚轮是向上滚动，e.Delta < 0时鼠标滚轮向下滚动
            if (e.Delta > 0)//滚轮向上
            {
                Halcon.ImgZoom(MainForm.hImage, hWindow, 1);
            }
            else
            {
                Halcon.ImgZoom(MainForm.hImage, hWindow, 0);
            }
        }

        private void btn_CursorLocation_Click(object sender, EventArgs e)
        { 
            for (int i = 0;i< locationsResult.Count;i++)
            {
                Parameters.cursorLocation.Location[i].Row = locationsResult[i].Row;
                Parameters.cursorLocation.Location[i].Colum = locationsResult[i].Colum;
                Parameters.cursorLocation.Location[i].Length1 = locationsResult[i].Length1;
                Parameters.cursorLocation.Location[i].Length2 = locationsResult[i].Length2;
                Parameters.cursorLocation.Location[i].Length3 = locationsResult[i].Length3;
                Parameters.cursorLocation.Location[i].Length4 = locationsResult[i].Length4;
            }

            Parameters.detectionSpec.BasePoint.Row = basePoint.Row;
            Parameters.detectionSpec.BasePoint.Colum = basePoint.Colum;
            Parameters.detectionSpec.BasePoint.Radius = basePoint.Radius;

            Parameters.detectionSpec.BasePhi.Row1 = BaseReault[3].Row1;
            Parameters.detectionSpec.BasePhi.Colum1 = BaseReault[3].Colum1;
            Parameters.detectionSpec.BasePhi.Row2 = BaseReault[3].Row2;
            Parameters.detectionSpec.BasePhi.Colum2 = BaseReault[3].Colum2;

            XMLHelper.serialize<Parameters.DetectionSpec>(Parameters.detectionSpec, Parameters.commministion.productName + "/DetectionSpec.xml");
            XMLHelper.serialize<Parameters.CursorLocation>(Parameters.cursorLocation, Parameters.commministion.productName + "/CursorLocation.xml");
        }

        private void btn_SettingMeasurePos_Click(object sender, EventArgs e)
        {
            寻边设置 flg = new 寻边设置();
            decetionNum = cmb_Indication.SelectedIndex;
            flg.ShowDialog();
        }
        static Parameters.Cricle pointReaultCricle = new Parameters.Cricle();
        private void uiButton2_Click(object sender, EventArgs e)
        {
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            try
            {
                Halcon.DetectionHalconCricle(hWindow, MainForm.hImage, Parameters.detectionSpec.decetionCircle[cmb_Indication.SelectedIndex], Parameters.detectionSpec.decetionCircle[cmb_Indication.SelectedIndex].Row, Parameters.detectionSpec.decetionCircle[cmb_Indication.SelectedIndex].Colum, 20, ref pointReaultCricle);

            }
            catch
            {

            }
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {
            try
            {
                HOperatorSet.DispObj(MainForm.hImage, hWindow);
                Halcon.DetectionDrawCriclaAOI(hWindow, MainForm.hImage, ref Parameters.detectionSpec.decetionCircle[uiComboBox1.SelectedIndex]);
            }
            catch { }
            
        }
        static Cricle basePoint=new Cricle();
        private static void getMeasureBase(HObject hImage, HWindow hWindow ,DetectionSpec detectionSpec)
        {
            HOperatorSet.DispObj(hImage, hWindow);
            HTuple IsOverlapping = new HTuple();
            HTuple phi = new HTuple();
            try
            {
                Halcon.DetectionHalconLine(0, hWindow, hImage, detectionSpec, 300, ref BaseReault[0]);
                Halcon.DetectionHalconLine(1, hWindow, hImage, detectionSpec, 300, ref BaseReault[1]);
                Halcon.DetectionHalconLine(2, hWindow, hImage, detectionSpec, 300, ref BaseReault[2]);

                HOperatorSet.SetColor(hWindow, "red");
                HOperatorSet.IntersectionLines(BaseReault[0].Row1, BaseReault[0].Colum1, BaseReault[0].Row2, BaseReault[0].Colum2,
                    BaseReault[1].Row1, BaseReault[1].Colum1, BaseReault[1].Row2, BaseReault[1].Colum2, out BaseReault[3].Row1, out BaseReault[3].Colum1, out IsOverlapping);
                HOperatorSet.DispCross(hWindow, BaseReault[3].Row1, BaseReault[3].Colum1, 600, 0);

                HOperatorSet.IntersectionLines(BaseReault[0].Row1, BaseReault[0].Colum1, BaseReault[0].Row2, BaseReault[0].Colum2,
                    BaseReault[2].Row1, BaseReault[2].Colum1, BaseReault[2].Row2, BaseReault[2].Colum2, out BaseReault[3].Row2, out BaseReault[3].Colum2, out IsOverlapping);
                HOperatorSet.DispCross(hWindow, BaseReault[3].Row2, BaseReault[3].Colum2, 400, 0);               
                IsOverlapping.Dispose();
                HOperatorSet.AngleLx(BaseReault[0].Row1, BaseReault[0].Colum1,BaseReault[0].Row2, BaseReault[0].Colum2,out phi);
                basePoint.Row = BaseReault[3].Row1;
                basePoint.Colum = BaseReault[3].Colum1;
                basePoint.Radius = phi;
                
                
            }
            catch
            {
                IsOverlapping.Dispose();
                MessageBox.Show("基准检测失败，请确认");
            }
        }

        private void uiButton6_Click(object sender, EventArgs e)
        {
            try
            {
                getMeasureBase(MainForm.hImage, hWindow,Parameters.detectionSpec);
            }
            catch { }
           
            XMLHelper.serialize<Parameters.Specifications>(Parameters.specifications, Parameters.commministion.productName + "/Specifications.xml");
        }

        private void uiDoubleUpDown1_ValueChanged(object sender, double value)
        {
            uiComboBox1.Items.Clear();
            
            for (int i = 0; i < uiDoubleUpDown1.Value; i++)
            {
                uiComboBox1.Items.Add(i + 1);
            }
        }

        private void txt_PixelResolutionColumn_ValueChanged(object sender, double value)
        {

        }

        private void uiButton5_Click(object sender, EventArgs e)
        {
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            try
            {
                Location location = new Location();
                ShowCriclePos(ref location);
                uiDataGridView1.Rows[0].Cells[1].Value = Parameters.cursorLocation.Location[Parameters.specifications.DetectionRect2Num + decetionCricleNum].Row.ToString("0.0000");
                uiDataGridView1.Rows[1].Cells[1].Value = Parameters.cursorLocation.Location[Parameters.specifications.DetectionRect2Num + decetionCricleNum].Colum.ToString("0.0000");
                uiDataGridView1.Rows[2].Cells[1].Value = Parameters.cursorLocation.Location[Parameters.specifications.DetectionRect2Num + decetionCricleNum].Length1.ToString("0.0000");
                uiDataGridView1.Rows[3].Cells[1].Value = Parameters.cursorLocation.Location[Parameters.specifications.DetectionRect2Num + decetionCricleNum].Length2.ToString("0.0000");
                uiDataGridView1.Rows[4].Cells[1].Value = Parameters.cursorLocation.Location[Parameters.specifications.DetectionRect2Num + decetionCricleNum].Length3.ToString("0.0000");
                uiDataGridView1.Rows[5].Cells[1].Value = Parameters.cursorLocation.Location[Parameters.specifications.DetectionRect2Num + decetionCricleNum].Length4.ToString("0.0000");

                uiDataGridView1.Rows[0].Cells[2].Value = location.Row.ToString("0.0000");
                uiDataGridView1.Rows[1].Cells[2].Value = location.Colum.ToString("0.0000");
                uiDataGridView1.Rows[2].Cells[2].Value = location.Length1.ToString("0.0000");
                uiDataGridView1.Rows[3].Cells[2].Value = location.Length2.ToString("0.0000");
                uiDataGridView1.Rows[4].Cells[2].Value = location.Length3.ToString("0.0000");
                uiDataGridView1.Rows[5].Cells[2].Value = location.Length4.ToString("0.0000");

                uiDataGridView1.Rows[0].Cells[3].Value = (Parameters.cursorLocation.Location[Parameters.specifications.DetectionRect2Num + decetionCricleNum].Row - location.Row).ToString("0.0000");
                uiDataGridView1.Rows[1].Cells[3].Value = (Parameters.cursorLocation.Location[Parameters.specifications.DetectionRect2Num + decetionCricleNum].Colum - location.Colum).ToString("0.0000");
                uiDataGridView1.Rows[2].Cells[3].Value = (Parameters.cursorLocation.Location[Parameters.specifications.DetectionRect2Num + decetionCricleNum].Length1 - location.Length1).ToString("0.0000");
                uiDataGridView1.Rows[3].Cells[3].Value = (Parameters.cursorLocation.Location[Parameters.specifications.DetectionRect2Num + decetionCricleNum].Length2 - location.Length2).ToString("0.0000");
                uiDataGridView1.Rows[4].Cells[3].Value = (Parameters.cursorLocation.Location[Parameters.specifications.DetectionRect2Num + decetionCricleNum].Length3 - location.Length3).ToString("0.0000");
                uiDataGridView1.Rows[5].Cells[3].Value = (Parameters.cursorLocation.Location[Parameters.specifications.DetectionRect2Num + decetionCricleNum].Length4 - location.Length4).ToString("0.0000");
            }
            catch
            {

            }
        }

        private void uiComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            decetionCricleNum = uiComboBox1.SelectedIndex;
        }

        private void PixelResolutionRow_Paint(object sender, PaintEventArgs e)
        {

        }

        private void uiButton3_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Parameters.specifications.ImageVerifyEnabled = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Parameters.specifications.ContourLineEnabled = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Parameters.specifications.DefectionEnabled = checkBox3.Checked;
        }

        private void hWindowControl1_HMouseDown(object sender, HMouseEventArgs e)
        {
            if (e.Clicks == 2)
            {
                HOperatorSet.SetPart(hWindow, 0, 0, -1, -1);//设置窗体的规格
                HOperatorSet.DispObj(MainForm.hImage, hWindow);//显示图片
            }
        }

        private void uiDataGridView2_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            e.Row.HeaderCell.Value = string.Format("{0}", e.Row.Index + 1);
        }

        private void uiDoubleUpDown2_ValueChanged(object sender, double value)
        {
            Parameters.detectionSpec.分割阈值 = (int)value;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Parameters.specifications.DefectionAbled = checkBox4.Checked;
        }
    }
}
