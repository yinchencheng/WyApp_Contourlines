using HalconDotNet;
using OpenCvSharp;
using SevenZip.Compression.LZ;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WY_App;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static WY_App.Utility.Parameters;

namespace WY_App.Utility
{
    public class Halcon
    {
        public static HTuple hv_Width;
        public static HTuple hv_Height;
        public static HTuple hv_AcqHandle;
        public static bool CamConnect = false;

        public static bool initalCamera(string CamID, ref HTuple hv_AcqHandle)
        {
            try
            {
                //获取相机句柄
                HOperatorSet.OpenFramegrabber("GenICamTL", 0, 0, 0, 0, 0, 0, "progressive", -1, "default", -1, "false", "default", CamID, 0, -1, out hv_AcqHandle);
                HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerMode", "Off");
                HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerSource", "Software");
                HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "grab_timeout", 20000);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log.WriteError(System.DateTime.Now.ToString() + CamID + "相机链接失败" + ex.Message);
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + CamID + "相机链接失败" + ex.Message);
                return false;
            }

        }

        public Halcon()
        {

            Thread th = new Thread(ini_Cam);
            th.IsBackground = true;
            th.Start();

        }

        private void ini_Cam()
        {
            while (true)
            {
                Thread.Sleep(5000);
                while (!CamConnect)
                {
                    Thread.Sleep(5000);
                    if (!CamConnect)
                    {
                        CamConnect = initalCamera("LineCam0", ref hv_AcqHandle);
                    }
                    else
                    {
                        LogHelper.Log.WriteError(System.DateTime.Now.ToString() + "相机1链接成功");
                        MainForm.AlarmList.Add(System.DateTime.Now.ToString() + "相机1链接成功");
                    }
                }            
            }
        }

        public static bool ImgDisplay(string imgPath, HWindow Hwindow)
        {
            MainForm.hImage.Dispose();
            HOperatorSet.GenEmptyObj(out MainForm.hImage);
            
            HOperatorSet.ReadImage(out MainForm.hImage, imgPath);//读取图片存入到HalconImage           
            HOperatorSet.GetImageSize(MainForm.hImage, out hv_Width, out hv_Height);//获取图片大小规格
            HOperatorSet.SetPart(Hwindow, 0, 0, -1, -1);//设置窗体的规格
            HOperatorSet.DispObj(MainForm.hImage, Hwindow);//显示图片
            return true;
        }
        //        //-----------------------------------------------------------------------------
        public static void CloseFramegrabber(HTuple hv_AcqHandle)
        {
            HOperatorSet.CloseFramegrabber(hv_AcqHandle);
        }
        public static void TriggerModeOff(HTuple hv_AcqHandle)
        {
            HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerMode", "Off");
        }
        public static void TriggerModeOn(HTuple hv_AcqHandle)
        {
            HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerMode", "On");
        }
        public static void SetFramegrabberParam(HTuple hv_AcqHandle)
        {
            HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "gain", Parameters.cameraParam.CamGain[0]);
            HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "ExposureTime", Parameters.cameraParam.CamShutter[0]);

        }
        public static void GrabImageAsync(HTuple hv_AcqHandle, out HObject himage)
        {
            HOperatorSet.GrabImageAsync(out himage, hv_AcqHandle, -1);
        }
        public static void GrabImageStart(HTuple hv_AcqHandle)
        {
            HOperatorSet.GrabImageStart(hv_AcqHandle, -1);
        }

        public static void DetectionShowAOI(int CamNum, HWindow hWindow, Rect1 hv_Region)
        {
  
        }

        public static void DetectionDrawRectAOI(HWindow hWindow, HObject hImage, ref Parameters.Rect1 rect1)
        {            
            HOperatorSet.SetColor(hWindow, "green");
            HOperatorSet.SetDraw(hWindow, "margin");
            hWindow.DispObj(hImage);
            HObject Rectangle1;
            hWindow.DrawRectangle1(out rect1.Row1, out rect1.Colum1, out rect1.Row2, out rect1.Colum2);
            HOperatorSet.GenRectangle1(out Rectangle1, rect1.Row1, rect1.Colum1, rect1.Row2, rect1.Colum2);
            HOperatorSet.DispObj(Rectangle1, hWindow);           
            Rectangle1.Dispose();
        }
        public static void DetectionMeanImageint (Parameters.MeanImageEnum meanImageEnum, HObject hObject,ref HObject hObject1)
        {
            switch (meanImageEnum)
            {
                case MeanImageEnum.直方图均值化:
                    {
                        HOperatorSet.EquHistoImage(hObject, out hObject1);
                        break;
                    }

                case MeanImageEnum.增强对比度:
                    {
                        HOperatorSet.Emphasize(hObject, out hObject1, 10, 10, 1.5);
                        break;
                    }
                case MeanImageEnum.均值滤波:
                    {
                        HOperatorSet.MeanImage(hObject, out hObject1, 25, 25);
                        break;
                    }
                case MeanImageEnum.中值滤波:
                    {
                        HOperatorSet.MedianImage(hObject, out hObject1, "square", 1.5, "mirrored");
                        break;
                    }
                case MeanImageEnum.高斯滤波:
                    {
                        HOperatorSet.GaussFilter(hObject, out hObject1, 11);
                        break;
                    }
                case MeanImageEnum.无滤波处理:
                    {
                        HOperatorSet.CopyImage(hObject, out hObject1);
                        break;
                    }                
            }
        }
        public static bool DetectionHalconCricle(HWindow hWindow, HObject hImage, Cricle cricle, HTuple PointRow, HTuple PointColum, HTuple length, ref Cricle cricle1)
        {

            HObject ho_Contours, ho_Cross, ho_Contour;
            HObject ho_Circle;

            // Local control variables 

            HTuple hv_WindowHandle = new HTuple(), hv_shapeParam = new HTuple();
            HTuple hv_MetrologyHandle = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Parameter = new HTuple(), hv_RowBegin = new HTuple();
            HTuple hv_ColBegin = new HTuple(), hv_RowEnd = new HTuple();
            HTuple hv_ColEnd = new HTuple(), hv_Nr = new HTuple();
            HTuple hv_Nc = new HTuple(), hv_Dist = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Contours);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_Circle);
            //获取图像及图像尺寸
            //获取图像及图像尺寸
            //dev_close_window(...);

            HOperatorSet.SetLineWidth(hWindow, 1);

            //标记测量位置
            //draw_line (WindowHandle, Row1, Column1, Row2, Column2)


            hv_shapeParam.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(PointRow);
                hv_shapeParam = hv_shapeParam.TupleConcat(PointColum);
                hv_shapeParam = hv_shapeParam.TupleConcat(cricle.Radius);
            }
            //创建测量句柄
            hv_MetrologyHandle.Dispose();
            HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
            //添加测量对象
            HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, hv_Width, hv_Height);
            hv_Index.Dispose();
            HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "circle", hv_shapeParam, length, 3, 1, 20, new HTuple(), new HTuple(), out hv_Index);
            //执行测量，获取边缘点集
            HOperatorSet.SetColor(hWindow, "yellow");
            HOperatorSet.ApplyMetrologyModel(hImage, hv_MetrologyHandle);
            ho_Contours.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
            HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "negative", out hv_Row, out hv_Column);
            HOperatorSet.DispObj(ho_Contours, hWindow);
            HOperatorSet.SetColor(hWindow, "red");
            ho_Cross.Dispose();
            HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
            HOperatorSet.DispObj(ho_Cross, hWindow);
            //获取最终测量数据和轮廓线
            HOperatorSet.SetColor(hWindow, "green");
            HOperatorSet.SetLineWidth(hWindow, 1);
            hv_Parameter.Dispose();
            HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
            ho_Contour.Dispose();
            HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 1.5);

            hv_RowBegin.Dispose(); hv_ColBegin.Dispose(); hv_RowEnd.Dispose(); hv_ColEnd.Dispose(); hv_Nr.Dispose(); hv_Nc.Dispose(); hv_Dist.Dispose();
            HOperatorSet.FitLineContourXld(ho_Contour, "tukey", -1, 0, 5, 2, out hv_RowBegin, out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc, out hv_Dist);
            HOperatorSet.SetDraw(hWindow, "margin");
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                ho_Circle.Dispose();
                HOperatorSet.GenCircle(out ho_Circle, hv_Parameter.TupleSelect(0), hv_Parameter.TupleSelect(1), hv_Parameter.TupleSelect(2));
                cricle1.Row = hv_Parameter[0];
                cricle1.Colum = hv_Parameter[1];
                cricle1.Radius = hv_Parameter[2];

            }
            HOperatorSet.SetColor(hWindow, "blue");
            HOperatorSet.DispObj(ho_Circle, hWindow);
            //释放测量句柄
            HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
            ho_Contours.Dispose();
            ho_Cross.Dispose();
            ho_Contour.Dispose();
            ho_Circle.Dispose();

            hv_Width.Dispose();
            hv_Height.Dispose();
            hv_WindowHandle.Dispose();
            hv_shapeParam.Dispose();
            hv_MetrologyHandle.Dispose();
            hv_Index.Dispose();
            hv_Row.Dispose();
            hv_Column.Dispose();
            hv_Parameter.Dispose();
            hv_RowBegin.Dispose();
            hv_ColBegin.Dispose();
            hv_RowEnd.Dispose();
            hv_ColEnd.Dispose();
            hv_Nr.Dispose();
            hv_Nc.Dispose();
            hv_Dist.Dispose();
            return true;
        }
        public static void DetectionDrawCriclaAOI(HWindow hWindow, HObject hImage, ref Parameters.Cricle cricle)
        {
            HOperatorSet.SetDraw(hWindow, "margin");
            HOperatorSet.SetColor(hWindow, "green");
            HObject DrawCircle;
            HTuple Row;
            HTuple Column;
            HTuple Area;
            hWindow.DrawCircle(out cricle.Row, out cricle.Colum, out cricle.Radius);
            HOperatorSet.GenCircle(out DrawCircle, cricle.Row, cricle.Colum, cricle.Radius);
            HOperatorSet.DispObj(DrawCircle, hWindow);
            HOperatorSet.AreaCenter(DrawCircle, out Area, out Row, out Column);
        }
        public static void DetectionDrawLineAOI(int BaseNum, HWindow hWindow, HObject hImage, ref Parameters.DetectionSpec rect1)
        {
            HOperatorSet.SetColor(hWindow, "green");
            HOperatorSet.SetDraw(hWindow, "margin");
            hWindow.DrawLine(out rect1.baseRect1[BaseNum].Row1,out rect1.baseRect1[BaseNum].Colum1, out rect1.baseRect1[BaseNum].Row2, out rect1.baseRect1[BaseNum].Colum2);
            HOperatorSet.DispLine(hWindow, rect1.baseRect1[BaseNum].Row1, rect1.baseRect1[BaseNum].Colum1, rect1.baseRect1[BaseNum].Row2, rect1.baseRect1[BaseNum].Colum2);
        }

        public static bool DetectionCriclePos(uint i, HWindow hWindow, HObject hImage, HRect1 hRect1, Cricle cricle, ref Location rect2Result)
        {          
            try
            {                        
                rect2Result.Row = cricle.Row * Parameters.specifications.PixelResolutionRow;
                rect2Result.Colum = cricle.Colum * Parameters.specifications.PixelResolutionColum;
                rect2Result.Length1 = Math.Abs(cricle.Row - hRect1.Row1.D) * Parameters.specifications.PixelResolutionRow;
                rect2Result.Length2 = Math.Abs(cricle.Colum - hRect1.Colum1.D) * Parameters.specifications.PixelResolutionColum;
                rect2Result.Length3 = cricle.Radius * Parameters.specifications.PixelResolutionRow;
                HTuple Length4 = new HTuple();
                HOperatorSet.DistancePl(cricle.Row, cricle.Colum, hRect1.Row1, hRect1.Colum1, hRect1.Row2, hRect1.Colum2, out Length4);
                rect2Result.Length4 = Length4.D * Parameters.specifications.PixelResolutionColum;
                HOperatorSet.SetColor(hWindow, "red");
                HOperatorSet.DispCross(hWindow, cricle.Row, cricle.Colum, 60, 0);
                HOperatorSet.SetColor(hWindow, "black");
                HOperatorSet.SetTposition(hWindow, cricle.Row, cricle.Colum - 80);
                HOperatorSet.WriteString(hWindow, "圆:" + i);
                HOperatorSet.SetTposition(hWindow, cricle.Row + 60, cricle.Colum - 80);
                HOperatorSet.WriteString(hWindow, "DY:" + rect2Result.Length1.ToString("0.000"));
                HOperatorSet.SetTposition(hWindow, cricle.Row + 120, cricle.Colum - 80);
                HOperatorSet.WriteString(hWindow, "DX:" + rect2Result.Length2.ToString("0.000"));
                HOperatorSet.SetTposition(hWindow, cricle.Row + 180, cricle.Colum - 80);
                HOperatorSet.WriteString(hWindow, "DR:" + (cricle.Radius* Parameters.specifications.PixelResolutionRow).ToString("0.000"));
                HOperatorSet.SetTposition(hWindow, cricle.Row + 240, cricle.Colum - 80);
                HOperatorSet.WriteString(hWindow, "PL:" + rect2Result.Length4.ToString("0.000"));
            }
            catch
            {
                
                return false;
            }
            
            return true;
        }
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="hWindow"></param>
        /// <param name="hImage"></param>
        /// <param name="hRect1"></param>
        /// <param name="Decetionrect2"></param>
        /// <param name="rect2Result"></param>
        /// <returns></returns>
        public static bool DetectionMeasurePos(uint i,HWindow hWindow, HObject hImage,HRect1 hRect1, Rect2 Decetionrect2,ref Location rect2Result)
        {
            
            HOperatorSet.SetColor(hWindow, "green");
            HOperatorSet.SetDraw(hWindow, "margin");
            // Local iconic variables 

            HObject ho_Rectangle;

            // Local control variables 

            HTuple hv_WindowHandle = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple();
            HTuple hv_MeasureHandle1 = new HTuple(), hv_RowEdge1 = new HTuple();
            HTuple hv_ColumnEdge1 = new HTuple(), hv_Amplitude = new HTuple();
            HTuple hv_Distance = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            try
            {
                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(hImage, out hv_Width, out hv_Height);               
                HOperatorSet.GenRectangle2(out ho_Rectangle, Decetionrect2.Row, Decetionrect2.Colum, Decetionrect2.Phi, Decetionrect2.Length1, Decetionrect2.Length2);
                HOperatorSet.DispObj(ho_Rectangle, hWindow);
                hv_MeasureHandle1.Dispose();
                HOperatorSet.GenMeasureRectangle2(Decetionrect2.Row, Decetionrect2.Colum, Decetionrect2.Phi, Decetionrect2.Length1, Decetionrect2.Length2, hv_Width, hv_Height, "bilinear", out hv_MeasureHandle1);
                hv_RowEdge1.Dispose(); hv_ColumnEdge1.Dispose(); hv_Amplitude.Dispose(); hv_Distance.Dispose();
                HOperatorSet.MeasurePos(hImage, hv_MeasureHandle1, Decetionrect2.simga, Decetionrect2.阈值, Decetionrect2.极性, "first",out hv_RowEdge1, out hv_ColumnEdge1, out hv_Amplitude, out hv_Distance);
                HOperatorSet.SetColor(hWindow, "red");
                rect2Result.Row = hv_RowEdge1.D * Parameters.specifications.PixelResolutionRow;
                rect2Result.Colum = hv_ColumnEdge1.D * Parameters.specifications.PixelResolutionColum;
                rect2Result.Length1 = Math.Abs(hv_RowEdge1.D - hRect1.Row1.D) * Parameters.specifications.PixelResolutionRow;
                rect2Result.Length2 = Math.Abs(hv_ColumnEdge1.D - hRect1.Colum1.D) * Parameters.specifications.PixelResolutionColum;
                rect2Result.Length3 = Math.Sqrt(rect2Result.Length1 * rect2Result.Length1 + rect2Result.Length2 * rect2Result.Length2);
                HTuple Length4 = new HTuple();
                HOperatorSet.DistancePl(hv_RowEdge1, hv_ColumnEdge1, hRect1.Row1, hRect1.Colum1, hRect1.Row2, hRect1.Colum2, out Length4);
                rect2Result.Length4 = Length4.D * Parameters.specifications.PixelResolutionColum;
                HOperatorSet.DispCross(hWindow, hv_RowEdge1, hv_ColumnEdge1, 60, Decetionrect2.Phi);
                
                HOperatorSet.SetColor(hWindow, "green");
                HOperatorSet.SetFont(hWindow, "-黑体-8-*-1-*-*-1-");
                HOperatorSet.SetTposition(hWindow, hv_RowEdge1, hv_ColumnEdge1);
                HOperatorSet.WriteString(hWindow, "点:" + i);
                HOperatorSet.SetTposition(hWindow, 200 * i, 100);
                HOperatorSet.WriteString(hWindow, "点:" + i+ "DY:" + rect2Result.Length1.ToString("0.000")+ "DX:" + rect2Result.Length2.ToString("0.000")+ "PO:" + rect2Result.Length3.ToString("0.000")+ "PL:" + rect2Result.Length4.ToString("0.000"));                
            }
            catch
            {
                ho_Rectangle.Dispose();
                hv_WindowHandle.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();                
                hv_MeasureHandle1.Dispose();
                hv_RowEdge1.Dispose();
                hv_ColumnEdge1.Dispose();
                hv_Amplitude.Dispose();
                hv_Distance.Dispose();
                return false;
            }
            ho_Rectangle.Dispose();
            hv_WindowHandle.Dispose();
            hv_Width.Dispose();
            hv_Height.Dispose();           
            hv_MeasureHandle1.Dispose();
            hv_RowEdge1.Dispose();
            hv_ColumnEdge1.Dispose();
            hv_Amplitude.Dispose();
            hv_Distance.Dispose();
            return true;
        }

        public static bool DetectionLinePos( HWindow hWindow, HObject hImage, HRect1 hRect1,Rect1 Decetionrect2, ref Location[] rect2Result)
        {

            HOperatorSet.SetColor(hWindow, "green");
            HOperatorSet.SetDraw(hWindow, "margin");

            HObject ho_Rectangle, ho_ImageReduced1;
            HObject ho_Region, ho_RegionFillUp, ho_ConnectedRegions;
            HObject ho_SelectedRegions, ho_ImageReduced, ho_Contours;
            HObject ho_Cross = null, ho_Contour = null;

            // Local control variables 

            HTuple hv_WindowHandle = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple(), hv_Row11 = new HTuple();
            HTuple hv_Column1 = new HTuple(), hv_Row2 = new HTuple();
            HTuple hv_Column2 = new HTuple(), hv_Area = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_len = new HTuple(), hv_num_1 = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Col1 = new HTuple();
            HTuple hv_Length = new HTuple(), hv_index = new HTuple();
            HTuple hv_shapeParam = new HTuple(), hv_MetrologyHandle = new HTuple();
            HTuple hv_Index = new HTuple(), hv_Parameter = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Contours);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            //dev_close_window(...);
            //dev_open_window(...);

            hv_Width.Dispose(); hv_Height.Dispose();
            HOperatorSet.GetImageSize(hImage, out hv_Width, out hv_Height);
            ho_Rectangle.Dispose();
            HOperatorSet.GenRectangle1(out ho_Rectangle, Decetionrect2.Row1, Decetionrect2.Colum1, Decetionrect2.Row2, Decetionrect2.Colum2);
            ho_ImageReduced1.Dispose();
            HOperatorSet.ReduceDomain(hImage, ho_Rectangle, out ho_ImageReduced1);
            ho_Region.Dispose();


            HOperatorSet.FastThreshold(ho_ImageReduced1, out ho_Region, 0, 140, 5);

            //Segment a region containing the edges
            //基于全局阈值的图像快速阈值化
            //填充区域中的洞
            ho_RegionFillUp.Dispose();
            HOperatorSet.FillUp(ho_Region, out ho_RegionFillUp);
            hv_Area.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
            HOperatorSet.AreaCenter(ho_RegionFillUp, out hv_Area, out hv_Row, out hv_Column);
            //连接区域，连在一起的作为一个区域，这样就将不连接的区域打散
            ho_ConnectedRegions.Dispose();
            HOperatorSet.Connection(ho_RegionFillUp, out ho_ConnectedRegions);
            //按区域面积（以象素为单位的）选择目标区域
            //这里我们要找到设备操作书的位置
            ho_SelectedRegions.Dispose();
            HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                "and", 5000, 1000000000);
            //提取操作书区域的图像
            ho_ImageReduced.Dispose();
            HOperatorSet.ReduceDomain(hImage, ho_SelectedRegions, out ho_ImageReduced);
            HOperatorSet.SetDraw(hWindow, "margin");

            //显示一下看效果
            
            //HOperatorSet.DispObj(ho_ImageReduced, hWindow);
            HOperatorSet.DispCross(hWindow, hv_Row, hv_Column, 60, 0);
            ho_Contours.Dispose();
            HOperatorSet.GenContourRegionXld(ho_ImageReduced, out ho_Contours, "border");
            HOperatorSet.DispObj(ho_Contours,hWindow);
            hv_len.Dispose();
            HOperatorSet.LengthXld(ho_Contours, out hv_len);
            hv_num_1.Dispose();
            HOperatorSet.ContourPointNumXld(ho_Contours, out hv_num_1);
            hv_Row1.Dispose(); hv_Col1.Dispose();
            HOperatorSet.GetContourXld(ho_Contours, out hv_Row1, out hv_Col1);
            hv_Length.Dispose();
            HOperatorSet.TupleLength(hv_Row1, out hv_Length);
            HTuple end_val32 = hv_Length;
            HTuple step_val32 = hv_Length/ Parameters.specifications.DetectionRect2Num;
            rect2Result = new Location[100];
            for (hv_index = 1; hv_index.Continue(end_val32- step_val32, step_val32); hv_index = hv_index.TupleAdd(step_val32))
            {
                int index = hv_index / step_val32;
                hv_shapeParam.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_shapeParam = new HTuple();
                    hv_shapeParam = hv_shapeParam.TupleConcat((hv_Row1.TupleSelect(hv_index)) - 10);
                    hv_shapeParam = hv_shapeParam.TupleConcat((hv_Col1.TupleSelect(hv_index)) - 10);
                    hv_shapeParam = hv_shapeParam.TupleConcat((hv_Row1.TupleSelect(hv_index)) + 10);
                    hv_shapeParam = hv_shapeParam.TupleConcat((hv_Col1.TupleSelect(hv_index)) + 10);
                }

                //创建测量句柄
                hv_MetrologyHandle.Dispose();
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                //添加测量对象
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, hv_Width, hv_Height);
                hv_Index.Dispose();
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam,100, 2, 1, 20, new HTuple(), new HTuple(), out hv_Index);

                //执行测量，获取边缘点集
                HOperatorSet.SetColor(hWindow, "yellow");
                HOperatorSet.ApplyMetrologyModel(hImage, hv_MetrologyHandle);
                ho_Contours.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle,"all", "all", out hv_Row, out hv_Column);
                HOperatorSet.DispObj(ho_Contours, hWindow);
                HOperatorSet.SetColor(hWindow, "red");
                ho_Cross.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.DispObj(ho_Cross, hWindow);
                //获取最终测量数据和轮廓线
                HOperatorSet.SetColor(hWindow, "green");
                HOperatorSet.SetLineWidth(hWindow, 1);
                hv_Parameter.Dispose();
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type","all_param", out hv_Parameter);
                if(hv_Parameter.Length==4)
                {
                    rect2Result[index].Row = (hv_Parameter[0] + hv_Parameter[2]) / 2;
                    rect2Result[index].Colum = (hv_Parameter[1] + hv_Parameter[3]) / 2;
                    HOperatorSet.DispCross(hWindow, rect2Result[index].Row, rect2Result[index].Colum, 200, 0);
                    rect2Result[index].Row = rect2Result[index].Row * Parameters.specifications.PixelResolutionRow;
                    rect2Result[index].Colum = rect2Result[index].Colum * Parameters.specifications.PixelResolutionColum;
                    rect2Result[index].Length1 = Math.Abs(rect2Result[index].Row - hRect1.Row1.D) * Parameters.specifications.PixelResolutionRow;
                    rect2Result[index].Length2 = Math.Abs(rect2Result[index].Colum - hRect1.Colum1.D) * Parameters.specifications.PixelResolutionColum;
                    rect2Result[index].Length3 = Math.Sqrt(rect2Result[index].Length1 * rect2Result[index].Length1 + rect2Result[index].Length2 * rect2Result[index].Length2);
                    HTuple Length4 = new HTuple();
                    HOperatorSet.DistancePl(rect2Result[index].Row, rect2Result[index].Colum, hRect1.Row1, hRect1.Colum1, hRect1.Row2, hRect1.Colum2, out Length4);
                    rect2Result[index].Length4 = Length4.D * Parameters.specifications.PixelResolutionColum;
                    HOperatorSet.SetColor(hWindow, "green");
                    HOperatorSet.SetFont(hWindow, "-黑体-8-*-1-*-*-1-");
                    HOperatorSet.SetTposition(hWindow, rect2Result[index].Row, rect2Result[index].Colum);
                    HOperatorSet.WriteString(hWindow, "点:" + index);
                    HOperatorSet.SetTposition(hWindow, 200 * index, 100);
                    HOperatorSet.WriteString(hWindow, "点:" + index+ "DY:" + rect2Result[index].Length1.ToString("0.000")+ "DX:" + rect2Result[index].Length2.ToString("0.000")+ "PO:" + rect2Result[index].Length3.ToString("0.000")+ "PL:" + rect2Result[index].Length4.ToString("0.000"));                  
                }
                else
                {
                    rect2Result[index].Row = 0;
                    rect2Result[index].Colum = 0;
                }
                ho_Contour.Dispose();
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle,"all", "all", 1.5);
                HOperatorSet.DispObj(ho_Contour, hWindow);
                //释放测量句柄
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);

            }
            ho_Rectangle.Dispose();
            ho_ImageReduced1.Dispose();
            ho_Region.Dispose();
            ho_RegionFillUp.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_SelectedRegions.Dispose();
            ho_ImageReduced.Dispose();
            ho_Contours.Dispose();
            ho_Cross.Dispose();
            ho_Contour.Dispose();

            hv_WindowHandle.Dispose();
            hv_Width.Dispose();
            hv_Height.Dispose();
            hv_Row11.Dispose();
            hv_Column1.Dispose();
            hv_Row2.Dispose();
            hv_Column2.Dispose();
            hv_Area.Dispose();
            hv_Row.Dispose();
            hv_Column.Dispose();
            hv_len.Dispose();
            hv_num_1.Dispose();
            hv_Row1.Dispose();
            hv_Col1.Dispose();
            hv_Length.Dispose();
            hv_index.Dispose();
            hv_shapeParam.Dispose();
            hv_MetrologyHandle.Dispose();
            hv_Index.Dispose();
            hv_Parameter.Dispose();
            return true;
        }
        /// <summary>
        /// 直线卡尺工具
        /// </summary>
        /// <param name="hWindow"></param>
        /// <param name="hImage"></param>
        /// <param name="rect1"></param>
        /// <param name="PointXY"></param>
        /// <returns></returns>
        public static bool DetectionHalconLine(int BaseNum, HWindow hWindow, HObject hImage, Parameters.DetectionSpec rect1, HTuple length, ref HRect1 PointXY)
        {

            HObject ho_Contours, ho_Cross, ho_Contour;

            // Local control variables 

            HTuple hv_shapeParam = new HTuple();
            HTuple hv_MetrologyHandle = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Parameter = new HTuple();

            HTuple hv_Nc = new HTuple(), hv_Dist = new HTuple(), hv_Nr = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                HOperatorSet.SetLineWidth(hWindow, 1);
                //HOperatorSet.DispObj(hImage, hWindow);
                //标记测量位置         
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_shapeParam = new HTuple();
                    hv_shapeParam = hv_shapeParam.TupleConcat(rect1.baseRect1[BaseNum].Row1, rect1.baseRect1[BaseNum].Colum1, rect1.baseRect1[BaseNum].Row2, rect1.baseRect1[BaseNum].Colum2);
                }
                //创建测量句柄
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                //添加测量对象
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, hv_Width, hv_Height);
                hv_Index.Dispose();
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, rect1.baseRect1[BaseNum].Length1, rect1.baseRect1[BaseNum].Length2, rect1.baseRect1[BaseNum].simga, rect1.baseRect1[BaseNum].阈值, new HTuple(), new HTuple(), out hv_Index);

                //执行测量，获取边缘点集
                HOperatorSet.SetColor(hWindow, "yellow");
                HOperatorSet.ApplyMetrologyModel(hImage, hv_MetrologyHandle);
                hv_Row.Dispose(); hv_Column.Dispose();
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.DispObj(ho_Contours, hWindow);
                HOperatorSet.SetColor(hWindow, "red");
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 1, 0.785398);
                //获取最终测量数据和轮廓线
                HOperatorSet.SetColor(hWindow, "green");
                HOperatorSet.SetLineWidth(hWindow, 1);
                hv_Parameter.Dispose();
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 15);
                HOperatorSet.FitLineContourXld(ho_Contour, "tukey", -1, 0, 5, 2, out PointXY.Row1, out PointXY.Colum1, out PointXY.Row2, out PointXY.Colum2, out hv_Nr, out hv_Nc, out hv_Dist);
                HOperatorSet.DispObj(ho_Cross, hWindow);
                HOperatorSet.SetColor(hWindow, "blue");
                HOperatorSet.DispObj(ho_Contour, hWindow);
                ho_Contours.Dispose();
                ho_Cross.Dispose();
                ho_Contour.Dispose();
            }
            catch
            {
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle); ;

                hv_shapeParam.Dispose();
                hv_MetrologyHandle.Dispose();
                hv_Index.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Parameter.Dispose();
                return false;
            }
            
            //释放测量句柄
            HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle); ;
            
            hv_shapeParam.Dispose();
            hv_MetrologyHandle.Dispose();
            hv_Index.Dispose();
            hv_Row.Dispose();
            hv_Column.Dispose();
            hv_Parameter.Dispose();


            return true;
        }


        public static void DetectionSaveAOI(string pathName,HObject hv_Region)
        {
            HOperatorSet.WriteRegion(hv_Region, pathName);
        }

        public static void DetectionDrawRect2AOI(HWindow hWindow,ref Rect2 rect2)
        {
            HOperatorSet.SetColor(hWindow, "green");
            HOperatorSet.SetDraw(hWindow, "margin");
            HObject hrect2=new HObject();
            hWindow.DrawRectangle2(out rect2.Row, out rect2.Colum, out rect2.Phi, out rect2.Length1, out rect2.Length2);
            rect2.Length1 = 100;
            rect2.Length2 = 20;
            HOperatorSet.GenRectangle2(out hrect2, rect2.Row, rect2.Colum, rect2.Phi, rect2.Length1, rect2.Length2);
            HOperatorSet.DispObj(hrect2, hWindow);
            hrect2.Dispose();
        }

        public static void DetectionShowRect2(HWindow hWindow, Rect2 rect2)
        {
            HObject rectanl2 = new HObject();

            HOperatorSet.GenRectangle2(out rectanl2, rect2.Row, rect2.Colum, rect2.Phi, rect2.Length1, rect2.Length2);
            HOperatorSet.SetColor(hWindow, "green");
            HOperatorSet.SetDraw(hWindow, "margin");
            HOperatorSet.DispObj(rectanl2, hWindow);
        }

        //图片缩小放大，配合鼠标滚轮事件
        public static void ImgZoom(HObject L_Img, HTuple Hwindow, int Delta = 1)
        {
            HTuple Zoom = new HTuple(), Row = new HTuple(), Col = new HTuple(), L_Button = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple Row0 = new HTuple(), Column0 = new HTuple(), Row00 = new HTuple(), Column00 = new HTuple(), Ht = new HTuple(), Wt = new HTuple();
            HTuple[] Now_Pos = new HTuple[4];
            try
            {
                
                if (Delta > 0)//鼠标滚动格值，一般120
                {
                    Zoom = 1.01;//向上滚动,放大倍数
                }
                else
                {
                    Zoom = 0.99;//向下滚动,缩小倍数
                }
                HOperatorSet.GetMposition(Hwindow, out Row, out Col, out L_Button);//获取当前鼠标的位置
                HOperatorSet.GetPart(Hwindow, out Row0, out Column0, out Row00, out Column00);//获取当前窗体的大小规格
                HOperatorSet.GetImageSize(L_Img, out hv_Width, out hv_Height);//获取图片大小规格
                Ht = Row00 - Row0;
                Wt = Column00 - Column0;
                if (Ht * Wt < 32000 * 32000 || Zoom == 1.2)
                {
                    Now_Pos[0] = (Row0 + ((1 - (1.0 / Zoom)) * (Row - Row0)));
                    Now_Pos[1] = (Column0 + ((1 - (1.0 / Zoom)) * (Col - Column0)));
                    Now_Pos[2] = Now_Pos[0] + (Ht / Zoom);
                    Now_Pos[3] = Now_Pos[1] + (Wt / Zoom);
                    HOperatorSet.SetPart(Hwindow, Now_Pos[0], Now_Pos[1], Now_Pos[2], Now_Pos[3]);
                    HOperatorSet.ClearWindow(Hwindow);
                    HOperatorSet.DispObj(L_Img, Hwindow);
                    Now_Pos[0].Dispose();
                    Now_Pos[1].Dispose();
                    Now_Pos[2].Dispose();
                    Now_Pos[3].Dispose();
                }
                else
                {
                    ImgIsNotStretchDisplay(L_Img, Hwindow);//不拉伸显示
                }
                Zoom.Dispose();
                Row.Dispose(); Col.Dispose(); L_Button.Dispose();
                hv_Width.Dispose(); hv_Height.Dispose();
                Row0.Dispose(); Column0.Dispose(); Row00.Dispose(); Column00.Dispose(); Ht.Dispose(); Wt.Dispose();
                
            }
            catch 
            {
                Zoom.Dispose();
                Row.Dispose(); Col.Dispose(); L_Button.Dispose();
                hv_Width.Dispose(); hv_Height.Dispose();
                Row0.Dispose(); Column0.Dispose(); Row00.Dispose(); Column00.Dispose(); Ht.Dispose(); Wt.Dispose();
            }
           

        }

        //图片不拉伸显示
        public static void ImgIsNotStretchDisplay(HObject L_Img, HTuple Hwindow)
        {
            HTuple hv_Width, hv_Height;
            HTuple win_Width, win_Height, win_Col, win_Row, cwin_Width, cwin_Height;
            HOperatorSet.ClearWindow(Hwindow);
            HOperatorSet.GetImageSize(L_Img, out hv_Width, out hv_Height);//获取图片大小规格
            HOperatorSet.GetWindowExtents(Hwindow, out win_Row, out win_Col, out win_Width, out win_Height);//获取窗体大小规格
            cwin_Height = 1.0 * win_Height / win_Width * hv_Width;//宽不变计算高          
            if (cwin_Height > hv_Height)//宽不变高能容纳
            {
                cwin_Height = 1.0 * (cwin_Height - hv_Height) / 2;
                HOperatorSet.SetPart(Hwindow, -cwin_Height, 0, cwin_Height + hv_Height, hv_Width);//设置窗体的规格
            }
            else//高不变宽能容纳
            {
                cwin_Width = 1.0 * win_Width / win_Height * hv_Height;//高不变计算宽
                cwin_Width = 1.0 * (cwin_Width - hv_Width) / 2;
                HOperatorSet.SetPart(Hwindow, 0, -cwin_Width, hv_Height, cwin_Width + hv_Width);//设置窗体的规格
                cwin_Width.Dispose();
            }
            HOperatorSet.DispObj(L_Img, Hwindow);//显示图片
            hv_Width.Dispose(); hv_Height.Dispose();
            win_Width.Dispose(); win_Height.Dispose(); win_Col.Dispose(); win_Row.Dispose();  cwin_Height.Dispose();
        }


        static HTuple oldRow, oldColumn;
        //鼠标按下去拖着图像移动，配合鼠标坐标按下与移动事件
        public static void MouseDownMoveImg(HObject L_Img, HTuple Hwindow)
        {
            HTuple row1, col1, row2, col2, Row, Column, Button;
            HOperatorSet.GetMposition(Hwindow, out Row, out Column, out Button);
            double RowMove = Row - oldRow;
            double ColMove = Column - oldColumn;
            HOperatorSet.GetPart(Hwindow, out row1, out col1, out row2, out col2);//得到当前的窗口坐标
            HOperatorSet.SetPart(Hwindow, row1 - RowMove, col1 - ColMove, row2 - RowMove, col2 - ColMove);

            //防止刷新图片太快的时候闪烁
            HOperatorSet.SetSystem("flush_graphic", "false");
            HOperatorSet.ClearWindow(Hwindow);
            HOperatorSet.SetSystem("flush_graphic", "true");
            //

            HOperatorSet.DispObj(L_Img, Hwindow);
            row1.Dispose(); col1.Dispose(); row2.Dispose(); col2.Dispose(); Row.Dispose(); Column.Dispose(); Button.Dispose();
        }

        public void SaveMouseDownPosition(HTuple Hwindow)
        {
            HTuple Button;
            HOperatorSet.GetMposition(Hwindow, out oldRow, out oldColumn, out Button);

        }

        public static void set_display_font(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font,HTuple hv_Bold, HTuple hv_Slant)
        {

            HTuple hv_OS = new HTuple(), hv_Fonts = new HTuple();
            HTuple hv_Style = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_AvailableFonts = new HTuple(), hv_Fdx = new HTuple();
            HTuple hv_Indices = new HTuple();
            HTuple hv_Font_COPY_INP_TMP = new HTuple(hv_Font);
            HTuple hv_Size_COPY_INP_TMP = new HTuple(hv_Size);
            try
            {
               
                hv_OS.Dispose();
                HOperatorSet.GetSystem("operating_system", out hv_OS);
                if ((int)((new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                    new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(-1)))) != 0)
                {
                    hv_Size_COPY_INP_TMP.Dispose();
                    hv_Size_COPY_INP_TMP = 16;
                }
                if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
                {
                    //Restore previous behaviour
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Size = ((1.13677 * hv_Size_COPY_INP_TMP)).TupleInt()
                                ;
                            hv_Size_COPY_INP_TMP.Dispose();
                            hv_Size_COPY_INP_TMP = ExpTmpLocalVar_Size;
                        }
                    }
                }
                else
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Size = hv_Size_COPY_INP_TMP.TupleInt()
                                ;
                            hv_Size_COPY_INP_TMP.Dispose();
                            hv_Size_COPY_INP_TMP = ExpTmpLocalVar_Size;
                        }
                    }
                }
               
               
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Consolas";
                    hv_Fonts[1] = "Menlo";
                    hv_Fonts[2] = "Courier";
                    hv_Fonts[3] = "Courier 10 Pitch";
                    hv_Fonts[4] = "FreeMono";
                    hv_Fonts[5] = "Liberation Mono";
               
                hv_Style.Dispose();
                hv_Style = "";
                if ((int)(new HTuple(hv_Bold.TupleEqual("true"))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Style = hv_Style + "Bold";
                            hv_Style.Dispose();
                            hv_Style = ExpTmpLocalVar_Style;
                        }
                    }
                }
                else if ((int)(new HTuple(hv_Bold.TupleNotEqual("false"))) != 0)
                {
                    hv_Exception.Dispose();
                    hv_Exception = "Wrong value of control parameter Bold";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Slant.TupleEqual("true"))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Style = hv_Style + "Italic";
                            hv_Style.Dispose();
                            hv_Style = ExpTmpLocalVar_Style;
                        }
                    }
                }
                else if ((int)(new HTuple(hv_Slant.TupleNotEqual("false"))) != 0)
                {
                    hv_Exception.Dispose();
                    hv_Exception = "Wrong value of control parameter Slant";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Style.TupleEqual(""))) != 0)
                {
                    hv_Style.Dispose();
                    hv_Style = "Normal";
                }
                hv_AvailableFonts.Dispose();
                HOperatorSet.QueryFont(hv_WindowHandle, out hv_AvailableFonts);                
                for (hv_Fdx = 0; (int)hv_Fdx <= (int)((new HTuple(hv_Fonts.TupleLength())) - 1); hv_Fdx = (int)hv_Fdx + 1)
                {
                    hv_Indices.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Indices = hv_AvailableFonts.TupleFind(
                            hv_Fonts.TupleSelect(hv_Fdx));
                    }
                    if ((int)(new HTuple((new HTuple(hv_Indices.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        if ((int)(new HTuple(((hv_Indices.TupleSelect(0))).TupleGreaterEqual(0))) != 0)
                        {
                            hv_Font_COPY_INP_TMP.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Font_COPY_INP_TMP = hv_Fonts.TupleSelect(
                                    hv_Fdx);
                            }
                            break;
                        }
                    }
                }
                if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual(""))) != 0)
                {
                    throw new HalconException("Wrong value of control parameter Font");
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Font = (((hv_Font_COPY_INP_TMP + "-") + hv_Style) + "-") + hv_Size_COPY_INP_TMP;
                        hv_Font_COPY_INP_TMP.Dispose();
                        hv_Font_COPY_INP_TMP = ExpTmpLocalVar_Font;
                    }
                }
                HOperatorSet.SetFont(hv_WindowHandle, hv_Font_COPY_INP_TMP);

                hv_Font_COPY_INP_TMP.Dispose();
                hv_Size_COPY_INP_TMP.Dispose();
                hv_OS.Dispose();
                hv_Fonts.Dispose();
                hv_Style.Dispose();
                hv_Exception.Dispose();
                hv_AvailableFonts.Dispose();
                hv_Fdx.Dispose();
                hv_Indices.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Font_COPY_INP_TMP.Dispose();
                hv_Size_COPY_INP_TMP.Dispose();
                hv_OS.Dispose();
                hv_Fonts.Dispose();
                hv_Style.Dispose();
                hv_Exception.Dispose();
                hv_AvailableFonts.Dispose();
                hv_Fdx.Dispose();
                hv_Indices.Dispose();

                //throw HDevExpDefaultException;
            }
        }
    }
}
