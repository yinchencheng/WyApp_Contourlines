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
            chk_MeanImageEnabled.Checked = Parameters.specifications.MeanImageEnabled;
            cmb_MeanImageList.DataSource = Enum.GetNames(typeof(Parameters.MeanImageEnum));
            cmb_MeanImageList.SelectedIndex = Parameters.specifications.meanImageEnum;
            cmb_Indication.SelectedIndex = 0;
            chk_SaveOrigalImage.Checked = Parameters.specifications.SaveOrigalImage;
            chk_SaveDefeatImage.Checked = Parameters.specifications.SaveDefeatImage;

            num_DetectionRect2Num.Value = Parameters.specifications.DetectionRect2Num;
            txt_PixelResolutionRow.Value = Parameters.specifications.PixelResolutionRow;
            txt_PixelResolutionColumn.Value = Parameters.specifications.PixelResolutionColum;


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

            uiDataGridView1.Rows[0].Cells[0].Value = "行(Y)坐标";
            uiDataGridView1.Rows[1].Cells[0].Value = "列(X)坐标";
            uiDataGridView1.Rows[2].Cells[0].Value = "行(Y)绝对值";
            uiDataGridView1.Rows[3].Cells[0].Value = "列(X)绝对值";
            uiDataGridView1.Rows[4].Cells[0].Value = "基准点距离";
            uiDataGridView1.Rows[5].Cells[0].Value = "基准线距离";
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
            Halcon.DetectionHalconLine(0, hWindow, MainForm.hImage, Parameters.detectionSpec, 500, ref BaseReault[0]);
        }

        public static void showY1Base(object sender, EventArgs e)
        {
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            Halcon.DetectionHalconLine(1, hWindow, MainForm.hImage, Parameters.detectionSpec, 500, ref BaseReault[1]);

        }   

        public static void showY2Base(object sender, EventArgs e)
        {
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            Halcon.DetectionHalconLine(2, hWindow, MainForm.hImage, Parameters.detectionSpec, 500, ref BaseReault[2]);
        }

        public static void ShowMeasurePos(ref Location location)
        {
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            HTuple IsOverlapping1 = new HTuple();
            HTuple IsOverlapping2 = new HTuple();
            try
            {
                Halcon.DetectionHalconLine(0, hWindow, MainForm.hImage, Parameters.detectionSpec, 500, ref BaseReault[0]);
                Halcon.DetectionHalconLine(1, hWindow, MainForm.hImage, Parameters.detectionSpec, 500, ref BaseReault[1]);
                Halcon.DetectionHalconLine(2, hWindow, MainForm.hImage, Parameters.detectionSpec, 500, ref BaseReault[2]);
                HOperatorSet.SetColor(hWindow, "red");
                HOperatorSet.IntersectionLines(BaseReault[0].Row1, BaseReault[0].Colum1, BaseReault[0].Row2, BaseReault[0].Colum2, BaseReault[1].Row1, BaseReault[1].Colum1, BaseReault[1].Row2, BaseReault[1].Colum2, out BaseReault[3].Row1, out BaseReault[3].Colum1, out IsOverlapping1);
                HOperatorSet.DispCross(hWindow, BaseReault[3].Row1, BaseReault[3].Colum1, 60, 0);
                HOperatorSet.IntersectionLines(BaseReault[0].Row1, BaseReault[0].Colum1, BaseReault[0].Row2, BaseReault[0].Colum2, BaseReault[2].Row1, BaseReault[2].Colum1, BaseReault[2].Row2, BaseReault[2].Colum2, out BaseReault[3].Row2, out BaseReault[3].Colum2, out IsOverlapping2);
                HOperatorSet.DispCross(hWindow, BaseReault[3].Row2, BaseReault[3].Colum2, 60, 0);
                Halcon.DetectionMeasurePos((uint)(decetionNum + 1), hWindow, MainForm.hImage, BaseReault[3], Parameters.detectionSpec.decetionRect2[decetionNum], ref location);
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
                Halcon.DetectionHalconCricle(hWindow, MainForm.hImage, Parameters.detectionSpec.decetionCircle[decetionCricleNum], Parameters.detectionSpec.decetionCircle[decetionCricleNum].Row, Parameters.detectionSpec.decetionCircle[decetionCricleNum].Colum, 30, ref pointReaultCricle);
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
        private void btn_Detection_Click(object sender, EventArgs e)
        {
            string detectionTime = "" ;
            DateTime dtNow = System.DateTime.Now;  // 获取系统当前时间
            MainForm.strDateTime = dtNow.ToString("yyyyMMddHHmmss");
            MainForm.strDateTimeDay = dtNow.ToString("yyyy-MM-dd");
            Detection(hWindow,MainForm.hImage, ref locationsResult, ref detectionTime);
            WriteCsv(locationsResult);
            lab_detectionTime.Text = detectionTime;
        }

        /// <summary>
        /// 写入csv
        /// </summary>
        /// <param name="result">写入内容 ----单元格内容，单元格内容-----</param>
        public static void WriteCsv(Location[] location)
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
                string str1 = "序号" + "," + "行(Y)坐标" + "," + "列(X)坐标" + "," + "行(Y)绝对值" + "," + "列(X)绝对值" + "," + "基准点距离" + "," + "基准线距离" + "\t\n";
                sw.Write(str1);
                sw.Close();

               
            }
            StreamWriter swl = new StreamWriter(fileName, true, Encoding.UTF8);
            for (int i = 0; i < Parameters.specifications.DetectionRect2Num; i++)
            {
                string result;  
                result = location[i].Row.ToString("0.0000") + "," +
                location[i].Colum.ToString("0.0000") + "," +
                location[i].Length1.ToString("0.0000") + "," +
                location[i].Length2.ToString("0.0000") + "," +
                location[i].Length3.ToString("0.0000") + "," +
                location[i].Length4.ToString("0.0000");
                string str = (i+1) + "," + result + "\t\n";
                swl.Write(str);
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
                XMLHelper.serialize<Parameters.DetectionSpec>(Parameters.detectionSpec, Parameters.commministion.productName + "/DetectionSpec.xml");
                XMLHelper.serialize<Parameters.Specifications>(Parameters.specifications, Parameters.commministion.productName + "/Specifications.xml");
            }
            
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        static Location[] locationsResult; 
        public static void Detection(HWindow hwindow,HObject hImage ,ref Location[] locations,  ref string detectionTime)
        {
            locations = new Location[Parameters.specifications.DetectionRect2Num+ Parameters.specifications.DetectionCricleNum];
            System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //  开始监视代码运行时间

            HOperatorSet.DispObj(hImage, hwindow);

            HTuple IsOverlapping1 = new HTuple();
            HTuple IsOverlapping2 = new HTuple();
            try
            {
                Halcon.DetectionHalconLine(0, hwindow, hImage, Parameters.detectionSpec, 200, ref BaseReault[0]);
                Halcon.DetectionHalconLine(1, hwindow, hImage, Parameters.detectionSpec, 200, ref BaseReault[1]);
                Halcon.DetectionHalconLine(2, hwindow, hImage, Parameters.detectionSpec, 200, ref BaseReault[2]);

                HOperatorSet.SetColor(hWindow, "red");
                HOperatorSet.IntersectionLines(BaseReault[0].Row1, BaseReault[0].Colum1, BaseReault[0].Row2, BaseReault[0].Colum2, BaseReault[1].Row1, BaseReault[1].Colum1, BaseReault[1].Row2, BaseReault[1].Colum2, out BaseReault[3].Row1, out BaseReault[3].Colum1, out IsOverlapping1);
                HOperatorSet.DispCross(hWindow, BaseReault[3].Row1, BaseReault[3].Colum1, 60, 0);
                HOperatorSet.IntersectionLines(BaseReault[0].Row1, BaseReault[0].Colum1, BaseReault[0].Row2, BaseReault[0].Colum2, BaseReault[2].Row1, BaseReault[2].Colum1, BaseReault[2].Row2, BaseReault[2].Colum2, out BaseReault[3].Row2, out BaseReault[3].Colum2, out IsOverlapping2);
                HOperatorSet.DispCross(hWindow, BaseReault[3].Row2, BaseReault[3].Colum2, 60, 0);
            }
            catch
            {
                IsOverlapping1.Dispose();
                IsOverlapping2.Dispose();
                MessageBox.Show("基准检测失败，请确认");
                return;
            }
            if (Parameters.specifications.ImageVerifyEnabled)
            {              
                try
                {
                    for (uint i = 0; i < Parameters.specifications.DetectionRect2Num; i++)
                    {
                        Halcon.DetectionMeasurePos(i + 1, hwindow, hImage, BaseReault[3], Parameters.detectionSpec.decetionRect2[i], ref locations[i]);
                    }
                    for (uint i = 0; i < Parameters.specifications.DetectionCricleNum; i++)
                    {
                        Halcon.DetectionHalconCricle(hwindow, hImage, Parameters.detectionSpec.decetionCircle[i], Parameters.detectionSpec.decetionCircle[i].Row, Parameters.detectionSpec.decetionCircle[i].Colum, 50, ref pointReaultCricle);
                        Halcon.DetectionCriclePos((uint)(i + 1), hWindow, hImage, BaseReault[3], pointReaultCricle, ref locations[Parameters.specifications.DetectionRect2Num + i]);
                    }
                }
                catch
                {
                    IsOverlapping1.Dispose();
                    IsOverlapping2.Dispose();
                    MessageBox.Show("检测失败，请确认");
                    return;
                }
            }
            else
            {
                try
                {
                    for (uint i = 0; i < Parameters.specifications.DetectionCricleNum; i++)
                    {
                        Halcon.DetectionHalconCricle(hwindow, hImage, Parameters.detectionSpec.decetionCircle[i], Parameters.detectionSpec.decetionCircle[i].Row, Parameters.detectionSpec.decetionCircle[i].Colum, 50, ref pointReaultCricle);
                        Halcon.DetectionCriclePos((uint)(i + 1), hWindow, hImage, BaseReault[3], pointReaultCricle, ref locations[Parameters.specifications.DetectionRect2Num + i]);
                    }
                }
                catch
                {
                    MessageBox.Show("检测失败，请确认");
                    return;
                }
                Halcon.DetectionLinePos(hWindow, hImage, BaseReault[3], Parameters.specifications.检测矩形, ref locations);
            }
            IsOverlapping1.Dispose();
            IsOverlapping2.Dispose();
            stopwatch.Stop(); //  停止监视
            TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
            detectionTime = timespan.TotalMilliseconds.ToString();  //  总毫秒数           
            
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

        

        private void btn_ShowAOI_Click(object sender, EventArgs e)
        {
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            HObject ho_Rectangle=new HObject();
            ho_Rectangle.Dispose();
            HOperatorSet.GenRectangle1(out ho_Rectangle,Parameters.specifications.检测矩形.Row1, Parameters.specifications.检测矩形.Colum1, Parameters.specifications.检测矩形.Row2, Parameters.specifications.检测矩形.Colum2);
            //显示一下看效果
            HOperatorSet.DispObj(ho_Rectangle, hWindow);;
           
        }


        private void btnn_IndicationTest_Click(object sender, EventArgs e)
        {
            Location location= new Location();
            ShowMeasurePos(ref location);
            uiDataGridView1.Rows[0].Cells[1].Value = Parameters.cursorLocation.Location[decetionNum].Row.ToString("0.0000");
            uiDataGridView1.Rows[1].Cells[1].Value = Parameters.cursorLocation.Location[decetionNum].Colum.ToString("0.0000");
            uiDataGridView1.Rows[2].Cells[1].Value = Parameters.cursorLocation.Location[decetionNum].Length1.ToString("0.0000");
            uiDataGridView1.Rows[3].Cells[1].Value = Parameters.cursorLocation.Location[decetionNum].Length2.ToString("0.0000");
            uiDataGridView1.Rows[4].Cells[1].Value = Parameters.cursorLocation.Location[decetionNum].Length3.ToString("0.0000");
            uiDataGridView1.Rows[5].Cells[1].Value = Parameters.cursorLocation.Location[decetionNum].Length4.ToString("0.0000");

            uiDataGridView1.Rows[0].Cells[2].Value = location.Row.ToString("0.0000");
            uiDataGridView1.Rows[1].Cells[2].Value = location.Colum.ToString("0.0000");
            uiDataGridView1.Rows[2].Cells[2].Value = location.Length1.ToString("0.0000");
            uiDataGridView1.Rows[3].Cells[2].Value = location.Length2.ToString("0.0000");
            uiDataGridView1.Rows[4].Cells[2].Value = location.Length3.ToString("0.0000");
            uiDataGridView1.Rows[5].Cells[2].Value = location.Length4.ToString("0.0000");

            uiDataGridView1.Rows[0].Cells[3].Value = (Parameters.cursorLocation.Location[decetionNum].Row - location.Row).ToString("0.0000");
            uiDataGridView1.Rows[1].Cells[3].Value = (Parameters.cursorLocation.Location[decetionNum].Colum - location.Colum).ToString("0.0000");
            uiDataGridView1.Rows[2].Cells[3].Value = (Parameters.cursorLocation.Location[decetionNum].Length1 - location.Length1).ToString("0.0000");
            uiDataGridView1.Rows[3].Cells[3].Value = (Parameters.cursorLocation.Location[decetionNum].Length2 - location.Length2).ToString("0.0000");
            uiDataGridView1.Rows[4].Cells[3].Value = (Parameters.cursorLocation.Location[decetionNum].Length3 - location.Length3).ToString("0.0000");
            uiDataGridView1.Rows[5].Cells[3].Value = (Parameters.cursorLocation.Location[decetionNum].Length4 - location.Length4).ToString("0.0000");
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
            Parameters.cursorLocation.Location = locationsResult;
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
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            Halcon.DetectionDrawCriclaAOI(hWindow, MainForm.hImage, ref Parameters.detectionSpec.decetionCircle[uiComboBox1.SelectedIndex]);
        }

        private void uiButton6_Click(object sender, EventArgs e)
        {
            HOperatorSet.DispObj(MainForm.hImage, hWindow);
            HTuple IsOverlapping = new HTuple();
            try
            {
                Halcon.DetectionHalconLine(0, hWindow, MainForm.hImage, Parameters.detectionSpec, 300, ref BaseReault[0]);
                Halcon.DetectionHalconLine(1, hWindow, MainForm.hImage, Parameters.detectionSpec, 300, ref BaseReault[1]);
                Halcon.DetectionHalconLine(2, hWindow, MainForm.hImage, Parameters.detectionSpec, 300, ref BaseReault[2]);

                HOperatorSet.SetColor(hWindow, "red");
                HOperatorSet.IntersectionLines(BaseReault[0].Row1, BaseReault[0].Colum1, BaseReault[0].Row2, BaseReault[0].Colum2,
                    BaseReault[1].Row1, BaseReault[1].Colum1, BaseReault[1].Row2, BaseReault[1].Colum2, out BaseReault[3].Row1, out BaseReault[3].Colum1, out IsOverlapping);
                HOperatorSet.DispCross(hWindow, BaseReault[3].Row1, BaseReault[3].Colum1, 60, 0);

                HOperatorSet.IntersectionLines(BaseReault[0].Row1, BaseReault[0].Colum1, BaseReault[0].Row2, BaseReault[0].Colum2,
                    BaseReault[2].Row1, BaseReault[2].Colum1, BaseReault[2].Row2, BaseReault[2].Colum2, out BaseReault[3].Row2, out BaseReault[3].Colum2, out IsOverlapping);
                HOperatorSet.DispCross(hWindow, BaseReault[3].Row2, BaseReault[3].Colum2, 60, 0);

                Parameters.detectionSpec.BasePoint.Row = BaseReault[3].Row1;
                Parameters.detectionSpec.BasePoint.Colum = BaseReault[3].Colum1;
                HTuple hv_Angle;
                HOperatorSet.AngleLx(BaseReault[1].Row1, BaseReault[1].Colum1, BaseReault[1].Row2, BaseReault[1].Colum2, out hv_Angle);
                Parameters.detectionSpec.BasePoint.Radius = 0;
                hv_Angle.Dispose(); 
                IsOverlapping.Dispose();
            }
            catch
            {
                IsOverlapping.Dispose();             
                MessageBox.Show("基准检测失败，请确认");
                return;
            }
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

        private void uiButton7_Click(object sender, EventArgs e)
        {
            locationsResult = new Location[100];
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
            }
            catch
            {
                IsOverlapping1.Dispose();
                IsOverlapping2.Dispose();

                return;
            }
            Halcon.DetectionLinePos(hWindow, MainForm.hImage, BaseReault[3], Parameters.specifications.检测矩形, ref locationsResult);
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
    }
}
