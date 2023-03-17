using HalconDotNet;
using System;

namespace WY_App.Utility
{
    public class Parameters
    {
        /// <summary>
        /// 日志等级
        /// </summary>
        public enum LogLevelEnum
        {
            Debug = 0,
            Info = 1,
            Warn = 2,
            Error = 3,
            Fatal = 4
        }

        public enum MeanImageEnum
        {
            直方图均值化 = 0,
            增强对比度 = 1,
            均值滤波 = 2,
            中值滤波 = 3,
            高斯滤波 = 4,
            无滤波处理 = 5
        }
        

        public class CameraParam
        {
            /// <summary>
            /// 采集卡
            /// </summary>
            public string[] CamInterface = new string[3];
            /// <summary>
            /// 相机ID
            /// </summary>
            public string[] CamDeviceID = new string[3];
            /// <summary>
            /// 相机曝光
            /// </summary>
            public double[] CamShutter = new double[3];
            /// <summary>
            /// 相机补偿
            /// </summary>
            public double[] CamGain = new double[3];
            /// <summary>
            /// 相机软触发
            /// </summary>
            public bool CamSoftwareMode;

            /// <summary>
            /// 相机触发采集模式/连续采集模式
            /// </summary>
            public bool CamContinuesMode;

            public CameraParam()
            {
                CamSoftwareMode = false;
                CamContinuesMode = false;
                for (int i = 0; i < 3; i++)
                {
                    CamInterface[i] = "采集卡" + i;
                    CamDeviceID[i] = "Cam" + i;
                    CamShutter[i] = 1000;
                    CamGain[i] = 1;
                }
            }
        }
        public static CameraParam cameraParam = new CameraParam();
        public struct Rect1
        {
            public double Row1;           
            public double Colum1;
            public double Row2;
            public double Colum2;
        }
        public struct Location
        {
            public double Row;
            public double Colum;
            public double Length1;
            public double Length2;
            public double Length3;
            public double Length4;
        }
        public class CursorLocation
        {
            public Location[] Location = new Location[100];
            public CursorLocation()
            {
                for (int i = 0; i < 100; i++)
                {
                    Location[i].Row = 0;
                    Location[i].Colum = 0;
                    Location[i].Length1 = 0;
                    Location[i].Length2 = 0;
                    Location[i].Length3 = 0;
                }
            }
        }

        public static CursorLocation cursorLocation = new CursorLocation();
        public struct HRect1
        {
            public HTuple Row1;
            public HTuple Colum1;
            public HTuple Row2;
            public HTuple Colum2;
        }

        public struct Rect2
        {
            public double Row;
            public double Colum;
            public double Phi;
            public double Length1;
            public double Length2;
            public double 阈值;
            public string 极性;
            public double simga;
        }

        public class DetectionSpec
        {
            public Rect1[] baseRect1 = new Rect1[3];
            public Rect2[] decetionRect2 = new Rect2[100];
            public DetectionSpec()
            {                
                for (int i = 0; i < 3; i++)
                {
                    baseRect1[i].Row1 = 100;
                    baseRect1[i].Colum1 = 100;
                    baseRect1[i].Row2 = 100;
                    baseRect1[i].Colum2 = 100;
                }

                for (int i = 0; i < 100; i++)
                {
                    decetionRect2[i].Row = 500;
                    decetionRect2[i].Colum = 500;
                    decetionRect2[i].Phi = 0;
                    decetionRect2[i].Length1 = 100;
                    decetionRect2[i].Length2 = 200;
                    decetionRect2[i].阈值 = 15;
                    decetionRect2[i].极性 = "negative";
                    decetionRect2[i].simga = 1;
                }
            }
        }

        public static DetectionSpec detectionSpec = new DetectionSpec();

        public class Specifications
        {
            public bool SaveOrigalImage;

            public bool SaveDefeatImage;

            public bool SaveCropImage;

            public int CropImagelength;

            public bool MeanImageEnabled;

            public int DetectionRect2Num;

            /// <summary>
            ///Y方向放大比例
            /// </summary>
            public double PixelResolutionRow;
            /// <summary>
            ///X方向放大比例
            /// </summary>
            public double PixelResolutionColum;

            public int meanImageEnum { get; set; }

            public Specifications()
            {
                PixelResolutionRow = 1;
                PixelResolutionColum = 1;
                SaveOrigalImage = false;
                SaveDefeatImage = false;
                SaveCropImage = false;
                CropImagelength = 500;
                MeanImageEnabled = false;
                meanImageEnum = 0;
                DetectionRect2Num = 40;
            }
        }
        public static Specifications specifications = new Specifications();

        public class Commministion
        {
            /// <summary>
            /// 当前保存日志等级
            /// </summary>
            public LogLevelEnum LogLevel;

            /// <summary>
            /// 日志存放路径
            /// </summary>
            public string LogFilePath;

            /// <summary>
            /// 日志存放天数
            /// </summary>
            public int LogFileExistDay;

            /// <summary>
            /// plc启用标志
            /// </summary>
            public bool PlcEnable;

            /// <summary>
            /// plc型号
            /// </summary>
            public string PlcType;

            /// <summary>
            /// plc ip地址
            /// </summary>
            public string PlcIpAddress;

            /// <summary>
            /// plc ip端口号
            /// </summary>
            public int PlcIpPort;

            /// <summary>
            /// plc 站号/型号
            /// </summary>
            public string PlcDevice;

            /// <summary>
            /// Tcp客户端启用标志
            /// </summary>
            public bool TcpClientEnable;

            /// <summary>
            /// tcp ip地址
            /// </summary>
            public string TcpClientIpAddress;

            /// <summary>
            /// tcp ip端口号
            /// </summary>
            public int TcpClientIpPort;

            /// <summary>
            /// Tcp服务端启用标志
            /// </summary>
            public bool TcpServerEnable;

            /// <summary>
            /// tcp服务器 ip地址
            /// </summary>
            public string TcpServerIpAddress;

            /// <summary>
            /// tcp服务器 ip端口号
            /// </summary>
            public int TcpServerIpPort;
            /// <summary>
            /// path
            /// </summary>
            public string ImagePath;

            /// <summary>
            /// path
            /// </summary>
            public string ImageSavePath;

            /// <summary>
            /// path
            /// </summary>
            public string productName;

            /// <summary>
            /// 联机参数设置
            /// </summary>
            public Commministion()
            {
                //PLC启用标志
                PlcEnable = false;
                //--PLC参数设置--
                //--欧姆龙Omron.OmronFinsNet--
                //--西门子Siemens.SiemensS7Net--
                //--三菱Melsec.MelsecMcNet--
                //--汇川Inovance.InovanceSerialOverTcp--
                //--ModbusTcp

                //PLC 类型
                PlcType = "Omron.OmronFinsNet";
                //PLC 地址
                PlcIpAddress = "127.0.0.1";
                //PLC站号/型号
                PlcDevice = "200";
                //PLC 端口号
                PlcIpPort = 9600;

                //--TCP客户端参数设置--
                //Tcp客户端启用标志
                TcpClientEnable = true;
                //TCP 客户端 地址
                TcpClientIpAddress = "127.0.0.1";
                //TCP 客户端 端口号
                TcpClientIpPort = 9600;

                //--TCP服务器参数设置--
                //Tcp服务器启用标志
                TcpServerEnable = false;
                //TCP服务器 地址
                TcpServerIpAddress = "127.0.0.1";
                //TCP服务器 端口号
                TcpServerIpPort = 9600;

                ImagePath = @"D:\VisionDetect\InspectImage\";
                ImageSavePath = @"D:\Image\";
                productName = "55";
;           }
        }

        public static Commministion commministion = new Commministion();

        public class PLCParams
        {
            public string Trigger_Detection;
            public string Completion;
            public string HeartBeatAdd;
            public string StartAdd;
           
            public PLCParams()
            {
                Trigger_Detection = "D100";
                Completion = "D100";
                HeartBeatAdd = "D102";
                HeartBeatAdd = "D104";                               
            }
        }

        public class Counts
        {
            public int[] Count = new int[10];

            public Counts()
            {
              
                
            }
        }

        public static PLCParams plcParams = new PLCParams();
        public static Counts counts = new Counts();
    }
}