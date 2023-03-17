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
        public static HTuple hv_Height ;
        public static void initalCamera(HTuple hv_AcqHandle, string CameraId, out bool camera_opend)
        {
            try
            {
                //获取相机句柄
                HOperatorSet.OpenFramegrabber("GigEVision2", 0, 0, 0, 0, 0, 0, "progressive",-1, "default", -1, "false", "default", 
                    "34bd20134a54_Hikrobot_MVCS01610GM",0, -1, out hv_AcqHandle);
                HOperatorSet.GrabImageStart(hv_AcqHandle, -1);
                camera_opend = true;
            }
            catch
            {
                camera_opend = false;
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

        public static void DetectionShowAOI(int CamNum, HWindow hWindow, out HObject hv_Region)
        {
            HOperatorSet.ReadRegion(out hv_Region, Parameters.commministion.productName + "/halcon/hoRegion" + CamNum + ".tiff");
            HOperatorSet.SetColor(hWindow, "green");
            HOperatorSet.SetDraw(hWindow, "margin");
            HOperatorSet.DispObj(hv_Region, hWindow);
        }

        public static void DetectionDrawAOI(HWindow hWindow, out HObject hv_Region)
        {
            HOperatorSet.SetColor(hWindow, "green");
            HOperatorSet.SetDraw(hWindow, "margin");
            HOperatorSet.DrawRegion(out hv_Region, hWindow);
            HOperatorSet.DispObj(hv_Region, hWindow);
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
        public static void DetectionDrawLineAOI(int BaseNum, HWindow hWindow, HObject hImage, ref Parameters.DetectionSpec rect1)
        {
            HOperatorSet.SetColor(hWindow, "green");
            HOperatorSet.SetDraw(hWindow, "margin");
            hWindow.DrawLine(out rect1.baseRect1[BaseNum].Row1,out rect1.baseRect1[BaseNum].Colum1, out rect1.baseRect1[BaseNum].Row2, out rect1.baseRect1[BaseNum].Colum2);
            HOperatorSet.DispLine(hWindow, rect1.baseRect1[BaseNum].Row1, rect1.baseRect1[BaseNum].Colum1, rect1.baseRect1[BaseNum].Row2, rect1.baseRect1[BaseNum].Colum2);
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
                rect2Result.Length3 = Math.Sqrt(rect2Result.Length1* rect2Result.Length1 + rect2Result.Length2* rect2Result.Length2);
                HTuple Length4 = new HTuple();
                HOperatorSet.DistancePl(hv_RowEdge1, hv_ColumnEdge1, hRect1.Row1, hRect1.Colum1, hRect1.Row2, hRect1.Colum2, out Length4);
                rect2Result.Length4 = Length4.D * Parameters.specifications.PixelResolutionColum;
                HOperatorSet.DispCross(hWindow, hv_RowEdge1, hv_ColumnEdge1, 60, Decetionrect2.Phi);
                HOperatorSet.SetColor(hWindow, "black");
                HOperatorSet.SetTposition(hWindow, hv_RowEdge1, hv_ColumnEdge1 - 80);
                HOperatorSet.WriteString(hWindow, "点:" + i);
                HOperatorSet.SetTposition(hWindow, hv_RowEdge1 + 60, hv_ColumnEdge1 - 80);
                HOperatorSet.WriteString(hWindow, "DY:" + rect2Result.Length1.ToString("0.000"));
                HOperatorSet.SetTposition(hWindow, hv_RowEdge1 + 120, hv_ColumnEdge1 - 80);
                HOperatorSet.WriteString(hWindow, "DX:" + rect2Result.Length2.ToString("0.000"));
                HOperatorSet.SetTposition(hWindow, hv_RowEdge1 + 180, hv_ColumnEdge1 - 80);
                HOperatorSet.WriteString(hWindow, "PO:" + rect2Result.Length3.ToString("0.000"));
                HOperatorSet.SetTposition(hWindow, hv_RowEdge1 + 240, hv_ColumnEdge1 - 80);
                HOperatorSet.WriteString(hWindow, "PL:" + rect2Result.Length4.ToString("0.000"));
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
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, length, 3, 1, 30, new HTuple(), new HTuple(), out hv_Index);

                //执行测量，获取边缘点集
                HOperatorSet.SetColor(hWindow, "yellow");
                HOperatorSet.ApplyMetrologyModel(hImage, hv_MetrologyHandle);
                hv_Row.Dispose(); hv_Column.Dispose();
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                //HOperatorSet.DispObj(ho_Contours, hWindow);
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
    }
}
