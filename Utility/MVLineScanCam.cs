using HalconDotNet;
using MvFGCtrlC.NET;
using OpenCvSharp.Flann;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp.Dnn;

namespace WY_App.Utility
{
    public class MVLineScanCam
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlCopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        public enum SAVE_IAMGE_TYPE
        {
            Image_Undefined = 0,                    // ch:未定义的图像格式 | en:Undefined Image Type
            Image_Bmp = 1,                          // ch:BMP图像格式 | en:BMP Image Type
            Image_Jpeg = 2,                         // ch:JPEG图像格式 | en:Jpeg Image Type
        }

        // ch:判断用户自定义像素格式 | en:Determine custom pixel format
        public const Int32 CUSTOMER_PIXEL_FORMAT = unchecked((Int32)0x80000000);
        public UInt32 TRIGGER_MODE_ON = 1;    // ch:触发模式开 | en:Trigger mode on
        public const UInt32 TRIGGER_MODE_OFF = 0;   // ch:触发模式关 | en:Trigger mode off
        CSystem m_cSystem = new CSystem();          // ch:操作采集卡 | en:Interface operations
        CInterface m_cInterface = null;                  // ch:操作采集卡和设备 | en:Interface and device operation
        CDevice m_cDevice = null;                   // ch:操作设备和流 | en:Device and stream operation
        CStream m_cStream = null;                   // ch:操作流和缓存 | en:Stream and buffer operation
        public uint m_nInterfaceNum = 0;                   // ch:采集卡数量 | en:Interface number
        public bool m_bIsIFOpen = false;                   // ch:采集卡是否打开 | en:Whether to open interface
        public bool m_bIsDeviceOpen = false;               // ch:设备是否打开 | en:Whether to open device
        public bool m_bIsGrabbing = false;                 // ch:是否在抓图 | en:Whether to start grabbing
        public uint m_nTriggerMode = TRIGGER_MODE_OFF;     // ch:触发模式 | en:Trigger Mode
        bool m_bThreadState = false;                // ch:线程状态 | en:Thread state
        Thread m_hGrabThread = null;                // ch:取流线程 | en:Grabbing thread
        IntPtr m_pDataBuf = IntPtr.Zero;            // ch:数据缓存 | en:Data buffer
        uint m_nDataBufSize = 0;                    // ch:数据缓存大小 | en:Length of data buffer
        IntPtr m_pSaveImageBuf = IntPtr.Zero;       // ch:图像缓存 | en:Image buffer
        uint m_nSaveImageBufSize = 0;               // ch:图像缓存大小 | en:Length of image buffer
        private static Object m_LockForSaveImage = new Object();    // ch:存图锁 | en:Lock for saving image
        MV_FG_INPUT_IMAGE_INFO m_stImageInfo = new MV_FG_INPUT_IMAGE_INFO();   // ch:图像信息 | en:Image info
        public HObject hObject = new HObject();
        delegate void ShowDisplayError(int nRet);
        public List<string> cmbInterfaceList = new List<string>();
        public List<string> cmbDeviceList = new List<string>();
        public AutoResetEvent ImageEvent = new AutoResetEvent(false);

        public MVLineScanCam()
        {
            Thread th = new Thread(ini_MVLineScanCam);
            th.IsBackground = true;
            th.Start();
        }
        public void ini_MVLineScanCam()
        {
            EnumInterface();
            Thread.Sleep(3000);
            while (true)
            {
                try
                {
                    if (!m_bIsIFOpen)
                    {
                        OpenInterface();
                    }
                    else
                    {
                        Thread.Sleep(3000);
                        EnumDevice();
                    }
                    Thread.Sleep(3000);
                    if (!m_bIsDeviceOpen)
                    {
                        OpenDevice();
                    }                   
                }
                catch (Exception ex)
                {
                    LogHelper.Log.WriteError(System.DateTime.Now.ToString() + "相机链接失败:", ex.Message);
                    MainForm.AlarmList.Add(System.DateTime.Now.ToString() + "相机链接失败:" + ex.Message);
                }
            }
        }


        public void EnumInterface()
        {
            int nRet = 0;
            bool bChanged = false;

            // ch:枚举采集卡 | en:Enum interface
            nRet = m_cSystem.UpdateInterfaceList(
                CParamDefine.MV_FG_CAMERALINK_INTERFACE | CParamDefine.MV_FG_GEV_INTERFACE | CParamDefine.MV_FG_CXP_INTERFACE,
                ref bChanged);
            if (CErrorCode.MV_FG_SUCCESS != nRet)
            {
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":枚举采集卡失败，故障码: " + nRet.ToString("X"));
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":枚举采集卡失败，故障码: " + nRet.ToString("X"));
                return;
            }
            m_nInterfaceNum = 0;

            // ch:获取采集卡数量 | en:Get interface num
            nRet = m_cSystem.GetNumInterfaces(ref m_nInterfaceNum);
            if (CErrorCode.MV_FG_SUCCESS != nRet)
            {
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":获取采集卡数量失败，故障码:" + nRet.ToString("X"));
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":获取采集卡数量失败，故障码:" + nRet.ToString("X"));
                return;
            }
            if (0 == m_nInterfaceNum)
            {
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":未发现采集卡");
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":未发现采集卡");
                return;
            }

            if (bChanged)
            {
                // ch:向下拉框添加采集卡信息 | en:Add interface info in Combo
                MV_FG_INTERFACE_INFO stIfInfo = new MV_FG_INTERFACE_INFO();
                for (uint i = 0; i < m_nInterfaceNum; i++)
                {
                    // ch:获取采集卡信息 | en:Get interface info
                    nRet = m_cSystem.GetInterfaceInfo(i, ref stIfInfo);
                    if (CErrorCode.MV_FG_SUCCESS != nRet)
                    {
                        LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
                        MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
                        return;
                    }

                    string strShowIfInfo = null;
                    switch (stIfInfo.nTLayerType)
                    {
                        case CParamDefine.MV_FG_GEV_INTERFACE:
                            {
                                MV_GEV_INTERFACE_INFO stGevIFInfo = (MV_GEV_INTERFACE_INFO)CAdditional.ByteToStruct(
                                    stIfInfo.SpecialInfo.stGevIfInfo, typeof(MV_GEV_INTERFACE_INFO));
                                strShowIfInfo += "GEV[" + i.ToString() + "]: " + stGevIFInfo.chDisplayName + " | " +
                                    stGevIFInfo.chInterfaceID + " | " + stGevIFInfo.chSerialNumber;
                                break;
                            }
                        case CParamDefine.MV_FG_CXP_INTERFACE:
                            {
                                MV_CXP_INTERFACE_INFO stCxpIFInfo = (MV_CXP_INTERFACE_INFO)CAdditional.ByteToStruct(
                                    stIfInfo.SpecialInfo.stCXPIfInfo, typeof(MV_CXP_INTERFACE_INFO));
                                strShowIfInfo += "CXP[" + i.ToString() + "]: " + stCxpIFInfo.chDisplayName + " | " +
                                    stCxpIFInfo.chInterfaceID + " | " + stCxpIFInfo.chSerialNumber;
                                break;
                            }
                        case CParamDefine.MV_FG_CAMERALINK_INTERFACE:
                            {
                                MV_CML_INTERFACE_INFO stCmlIFInfo = (MV_CML_INTERFACE_INFO)CAdditional.ByteToStruct(
                                    stIfInfo.SpecialInfo.stCMLIfInfo, typeof(MV_CML_INTERFACE_INFO));
                                strShowIfInfo += "CML[" + i.ToString() + "]: " + stCmlIFInfo.chDisplayName + " | " +
                                    stCmlIFInfo.chInterfaceID + " | " + stCmlIFInfo.chSerialNumber;
                                break;
                            }
                        default:
                            {
                                strShowIfInfo += "未知采集卡[" + i.ToString() + "]";
                                break;
                            }
                    }
                    cmbInterfaceList.Add(strShowIfInfo);
                }
            }
        }

        /// <summary>
        /// 打开采集卡
        /// </summary>
        public void OpenInterface()
        {
            int nRet = m_cSystem.OpenInterface(Convert.ToUInt32(0), out m_cInterface);
            if (CErrorCode.MV_FG_SUCCESS != nRet)
            {
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
                MessageBox.Show("打开采集卡失败，故障代码:" + nRet.ToString("X"));
                return;
            }
            m_bIsIFOpen = true;
        }

        /// <summary>
        /// 关闭采集卡
        /// </summary>
        public void CloseInterface()
        {

            if (m_bIsIFOpen && m_bIsDeviceOpen)
            {
                CloseDevice();
                // ch:关闭采集卡 | en:Close interface
                int nRet = m_cInterface.CloseInterface();
                if (CErrorCode.MV_FG_SUCCESS != nRet)
                {
                    LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
                    MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
                    MessageBox.Show("关闭采集卡失败，故障代码:" + nRet.ToString("X"));
                }
            }
            m_bIsIFOpen = false;
        }
        public void EnumDevice()
        {
            if (m_bIsIFOpen)
            {
                int nRet = 0;
                bool bChanged = false;
                uint nDeviceNum = 0;

                // ch:枚举采集卡上的相机 | en:Enum camera of interface
                nRet = m_cInterface.UpdateDeviceList(ref bChanged);
                if (CErrorCode.MV_FG_SUCCESS != nRet)
                {
                    LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
                    MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
                    MessageBox.Show("相机设备列表更新失败，故障码:" + nRet.ToString("X"));
                    return;
                }

                // ch:获取设备数量 | en:Get device number
                nRet = m_cInterface.GetNumDevices(ref nDeviceNum);
                if (CErrorCode.MV_FG_SUCCESS != nRet)
                {
                    LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
                    MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
                    MessageBox.Show("获取相机设备数量失败，故障码:" + nRet.ToString("X"));
                    return;
                }
                if (0 == nDeviceNum)
                {
                    LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
                    MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
                    MessageBox.Show("未发现相机设备");
                    return;
                }

                if (bChanged)
                {
                    cmbDeviceList.Clear();
                    // ch:向下拉框添加设备信息 | en:Add device info in Combo
                    MV_FG_DEVICE_INFO stDeviceInfo = new MV_FG_DEVICE_INFO();
                    for (uint i = 0; i < nDeviceNum; i++)
                    {
                        // ch:获取设备信息 | en:Get device info
                        nRet = m_cInterface.GetDeviceInfo(i, ref stDeviceInfo);
                        if (CErrorCode.MV_FG_SUCCESS != nRet)
                        {
                            cmbDeviceList.Clear();
                            LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
                            MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
                            MessageBox.Show("获取相机设备信息失败，故障码:" + nRet.ToString("X"));
                            return;
                        }

                        string strShowDevInfo = null;
                        switch (stDeviceInfo.nDevType)
                        {
                            case CParamDefine.MV_FG_GEV_DEVICE:
                                {
                                    MV_GEV_DEVICE_INFO stGevDevInfo = (MV_GEV_DEVICE_INFO)CAdditional.ByteToStruct(
                                        stDeviceInfo.DevInfo.stGEVDevInfo, typeof(MV_GEV_DEVICE_INFO));
                                    strShowDevInfo += "GEV[" + i.ToString() + "]: " + stGevDevInfo.chUserDefinedName + " | " +
                                        stGevDevInfo.chModelName + " | " + stGevDevInfo.chSerialNumber;
                                    break;
                                }
                            case CParamDefine.MV_FG_CXP_DEVICE:
                                {
                                    MV_CXP_DEVICE_INFO stCxpDevInfo = (MV_CXP_DEVICE_INFO)CAdditional.ByteToStruct(
                                        stDeviceInfo.DevInfo.stCXPDevInfo, typeof(MV_CXP_DEVICE_INFO));
                                    strShowDevInfo += "CXP[" + i.ToString() + "]: " + stCxpDevInfo.chUserDefinedName + " | " +
                                        stCxpDevInfo.chModelName + " | " + stCxpDevInfo.chSerialNumber;
                                    break;
                                }
                            case CParamDefine.MV_FG_CAMERALINK_DEVICE:
                                {
                                    MV_CML_DEVICE_INFO stCmlDevInfo = (MV_CML_DEVICE_INFO)CAdditional.ByteToStruct(
                                        stDeviceInfo.DevInfo.stCMLDevInfo, typeof(MV_CML_DEVICE_INFO));
                                    strShowDevInfo += "CML[" + i.ToString() + "]: " + stCmlDevInfo.chUserDefinedName + " | " +
                                        stCmlDevInfo.chModelName + " | " + stCmlDevInfo.chSerialNumber;
                                    break;
                                }
                            default:
                                {
                                    strShowDevInfo += "未知相机设备[" + i.ToString() + "]";
                                    break;
                                }
                        }
                        cmbDeviceList.Add(strShowDevInfo);
                    }
                }
                
            }
            else
            {
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":采集卡未打开");
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":采集卡未打开");
                MessageBox.Show("采集卡未打开！");
            }
        }
        /// <summary>
        /// 打开相机
        /// </summary>
        public void OpenDevice()
        {
            if (m_bIsIFOpen&& !m_bIsDeviceOpen)
            {
                // ch:打开设备，获得设备句柄 | en:Open device, get handle
                int nRet = m_cInterface.OpenDevice(Convert.ToUInt32(0), out m_cDevice);
                if (CErrorCode.MV_FG_SUCCESS != nRet)
                {
                    LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":打开相机设备失败，故障码:" + nRet.ToString("X"));
                    MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":打开相机设备失败，故障码:" + nRet.ToString("X"));                    
                    m_bIsDeviceOpen= false;
                    return;
                }
                // ch:设置连续采集模式 | en:Set Continuous Aquisition Mode
                CParam cDeviceParam = new CParam(m_cDevice);
                cDeviceParam.SetEnumValue("AcquisitionMode", 0);  // 0 - SingleFrame, 2 - Continuous
                cDeviceParam.SetEnumValue("TriggerMode", 0);      // ch:触发模式关 | en:Trigger mode off  
                m_bIsDeviceOpen = true;
            }
        }

        /// <summary>
        /// 关闭相机
        /// </summary>
        public void CloseDevice()
        {
            if (m_bThreadState)
            {
                m_bThreadState = false;
                m_hGrabThread.Join();
            }

            int nRet = 0;

            if (m_bIsGrabbing)
            {
                // ch:停止取流 | en:Stop Acquisition
                nRet = m_cStream.StopAcquisition();
                if (CErrorCode.MV_FG_SUCCESS != nRet)
                {
                    LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":停止取流失败，故障码:" + nRet.ToString("X"));
                    MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":停止取流失败，故障码:" + nRet.ToString("X"));
                }
                m_bIsGrabbing = false;
            }

            if (null != m_cStream)
            {
                // ch:关闭流通道 | en:Close stream channel
                nRet = m_cStream.CloseStream();
                if (CErrorCode.MV_FG_SUCCESS != nRet)
                {
                    LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":关闭流通道失败，故障码:" + nRet.ToString("X"));
                    MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":关闭流通道失败，故障码:" + nRet.ToString("X"));
                }
                m_cStream = null;
            }

            if (null != m_cDevice)
            {
                // ch:关闭设备 | en:Close device
                nRet = m_cDevice.CloseDevice();
                if (CErrorCode.MV_FG_SUCCESS != nRet)
                {
                    LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":关闭相机设备失败，故障码:" + nRet.ToString("X"));
                    MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":关闭相机设备失败，故障码:" + nRet.ToString("X"));
                }
                m_cDevice = null;
            }

            m_bIsDeviceOpen = false;

            if (IntPtr.Zero != m_pDataBuf)
            {
                Marshal.FreeHGlobal(m_pDataBuf);
                m_pDataBuf = IntPtr.Zero;
            }
            m_nDataBufSize = 0;

            if (IntPtr.Zero != m_pSaveImageBuf)
            {
                Marshal.FreeHGlobal(m_pSaveImageBuf);
                m_pSaveImageBuf = IntPtr.Zero;
            }
            m_nSaveImageBufSize = 0;
        }

        /// <summary>
        /// 开始采集
        /// </summary>
        public void StartGrab()
        {
            if (!m_bIsDeviceOpen)
            {
                MessageBox.Show("请确认相机连接状态");
                return;
            }

            if (m_bIsGrabbing)
            {
                return;
            }

            // ch:获取流通道个数 | en:Get number of stream
            uint nStreamNum = 0;
            int nRet = m_cDevice.GetNumStreams(ref nStreamNum);
            if (CErrorCode.MV_FG_SUCCESS != nRet)
            {
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":获取流通道数量失败，故障码:" + nRet.ToString("X"));
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":获取流通道数量失败，故障码:" + nRet.ToString("X"));
                return;
            }
            if (0 == nStreamNum)
            {
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":没有可用的数据通道");
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":没有可用的数据通道");
                return;
            }

            // ch:打开流通道(目前只支持单个通道) | en:Open stream(Only a single stream is supported now)
            nRet = m_cDevice.OpenStream(0, out m_cStream);
            if (CErrorCode.MV_FG_SUCCESS != nRet)
            {
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":打开流通道失败，故障码:" + nRet.ToString("X"));
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":打开流通道失败，故障码:" + nRet.ToString("X"));
                return;
            }

            // ch:设置SDK内部缓存数量 | en:Set internal buffer number
            const uint nBufNum = 5;
            nRet = m_cStream.SetBufferNum(nBufNum);
            if (CErrorCode.MV_FG_SUCCESS != nRet)
            {
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":设置缓存数量失败，故障码:" + nRet.ToString("X"));
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":设置缓存数量失败，故障码:" + nRet.ToString("X"));
                return;
            }

            // ch:创建取流线程 | en:Create acquistion thread
            m_bThreadState = true;
            m_hGrabThread = new Thread(ReceiveThreadProcess);
            if (null == m_hGrabThread)
            {
                m_bThreadState = false;
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":创建流通道失败，故障码:" + nRet.ToString("X"));
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":创建流通道失败，故障码:" + nRet.ToString("X"));
                return;
            }
            m_hGrabThread.Start();

            // ch:开始取流 | en:Start Acquisition
            nRet = m_cStream.StartAcquisition();
            if (CErrorCode.MV_FG_SUCCESS != nRet)
            {
                m_bThreadState = false;
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":开始取流失败，故障码:" + nRet.ToString("X"));
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":开始取流失败，故障码:" + nRet.ToString("X"));
                return;
            }
            m_bIsGrabbing = true;
        }

        /// <summary>
        /// 停止采集
        /// </summary>
        public void StopGrab()
        {
            if (false == m_bIsDeviceOpen || false == m_bIsGrabbing)
            {
                return;
            }

            // ch:标志位设为false | en:Set flag bit false
            m_bThreadState = false;
            m_hGrabThread.Join();

            // ch:停止取流 | en:Stop Acquisition
            int nRet = m_cStream.StopAcquisition();
            if (CErrorCode.MV_FG_SUCCESS != nRet)
            {
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":停止取流失败，故障码:" + nRet.ToString("X"));
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":停止取流失败，故障码:" + nRet.ToString("X"));               
                return;
            }
            m_bIsGrabbing = false;

            // ch:关闭流通道 | en:Close stream channel
            nRet = m_cStream.CloseStream();
            if (CErrorCode.MV_FG_SUCCESS != nRet)
            {
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":关闭取流通道失败，故障码:" + nRet.ToString("X"));
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":关闭取流通道失败，故障码:" + nRet.ToString("X"));
            }
            m_cStream = null;
        }

        /// <summary>
        /// 触发采集
        /// </summary>
        public void TriggerExec()
        {

            if (true != m_bIsGrabbing)
            {
                return;
            }

            CParam cDeviceParam = new CParam(m_cDevice);

            // ch:触发命令 | en:Trigger command
            int nRet = cDeviceParam.SetCommandValue("TriggerSoftware");
            if (CErrorCode.MV_FG_SUCCESS != nRet)
            {
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":设置相机软触发失败，故障码:" + nRet.ToString("X"));
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":设置相机软触发失败，故障码:" + nRet.ToString("X"));
                return;
            }
        }

        /// <summary>
        /// 设置触发模式
        /// </summary>
        public void TriggerMode(bool mode)
        {
            CParam cDeviceParam = new CParam(m_cDevice);
            // ch:打开触发模式 | en:Open Trigger Mode
            int nRet = cDeviceParam.SetEnumValue("TriggerMode", TRIGGER_MODE_ON);
            if (CErrorCode.MV_FG_SUCCESS != nRet)
            {
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":设置相机触发采集失败，故障码:" + nRet.ToString("X"));
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":设置相机触发采集失败，故障码:" + nRet.ToString("X"));
                MessageBox.Show("相机打开触发采集模式失败, 故障码:" + nRet.ToString("X"));
                return;
            }
            m_nTriggerMode = TRIGGER_MODE_ON;

            // ch:触发源选择:0 - Line0; | en:Trigger source select:0 - Line0;
            //           1 - Line1;
            //           2 - Line2;
            //           3 - Line3;
            //           4 - Counter;
            //           7 - Software;
            if (mode)
            {
                cDeviceParam.SetEnumValue("TriggerSource", (uint)7);
            }
            else
            {
                cDeviceParam.SetEnumValue("TriggerSource", (uint)0);

            }
        }

        public void SoftTrigger(bool mode)
        {

            CParam cDeviceParam = new CParam(m_cDevice);

            // ch:触发源设置 | en:Set trigger source
            // ch:触发源选择:0 - Line0; | en:Trigger source select:0 - Line0;
            //           1 - Line1;
            //           2 - Line2;
            //           3 - Line3;
            //           4 - Counter;
            //           7 - Software;
            if (mode)
            {
                int nRet = cDeviceParam.SetEnumValue("TriggerSource", (uint)7);
                if (CErrorCode.MV_FG_SUCCESS != nRet)
                {
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
                MessageBox.Show("设置相机软触发失败, 故障码:" + nRet.ToString("X"));
                    return;
                }
            }
            else
            {
                int nRet = cDeviceParam.SetEnumValue("TriggerSource", (uint)0);
                if (CErrorCode.MV_FG_SUCCESS != nRet)
                {
                    MessageBox.Show("设置相机硬触发失败, 故障码:" + nRet.ToString("X"));
                }
            }

        }

        /// <summary>
        /// 设置连续采集模式
        /// </summary>
        public void ContinuesMode()
        {
            CParam cDeviceParam = new CParam(m_cDevice);
            // ch:关闭触发模式 | en:Turn off Trigger Mode
            int nRet = cDeviceParam.SetEnumValue("TriggerMode", TRIGGER_MODE_OFF);
            if (CErrorCode.MV_FG_SUCCESS != nRet)
            {
                MessageBox.Show("设置相机连续采集失败, 故障码:" + nRet.ToString("X"));
                return;
            }
            m_nTriggerMode = TRIGGER_MODE_OFF;
        }

        public void ReceiveThreadProcess()
        {
            CImageProcess cImgProc = new CImageProcess(m_cStream);
            MV_FG_BUFFER_INFO stFrameInfo = new MV_FG_BUFFER_INFO();          // ch:图像信息 | en:Frame info
            MV_FG_INPUT_IMAGE_INFO stDisplayInfo = new MV_FG_INPUT_IMAGE_INFO();   // ch:显示的图像信息 | en:Display frame info
            const uint nTimeout = 10000;
            int nRet = 0;

            while (m_bThreadState)
            {
                if (m_bIsGrabbing)
                {
                    // ch:获取一帧图像缓存信息 | en:Get one frame buffer's info
                    nRet = m_cStream.GetFrameBuffer(ref stFrameInfo, nTimeout);
                    if (CErrorCode.MV_FG_SUCCESS == nRet)
                    {
                        // 用于保存图片
                        lock (m_LockForSaveImage)
                        {
                            if (IntPtr.Zero == m_pDataBuf || m_nDataBufSize < stFrameInfo.nFilledSize)
                            {
                                if (IntPtr.Zero != m_pDataBuf)
                                {
                                    Marshal.FreeHGlobal(m_pDataBuf);
                                    m_pDataBuf = IntPtr.Zero;
                                }

                                m_pDataBuf = Marshal.AllocHGlobal((Int32)stFrameInfo.nFilledSize);
                                if (IntPtr.Zero == m_pDataBuf)
                                {
                                    m_cStream.ReleaseFrameBuffer(stFrameInfo);
                                    break;
                                }
                                m_nDataBufSize = stFrameInfo.nFilledSize;
                            }
                            CopyMemory(m_pDataBuf, stFrameInfo.pBuffer, stFrameInfo.nFilledSize);

                            m_stImageInfo.nWidth = stFrameInfo.nWidth;
                            m_stImageInfo.nHeight = stFrameInfo.nHeight;
                            m_stImageInfo.enPixelType = stFrameInfo.enPixelType;
                            m_stImageInfo.pImageBuf = m_pDataBuf;
                            m_stImageInfo.nImageBufLen = stFrameInfo.nFilledSize;
                        }

                        // 自定义格式不支持显示
                        if (RemoveCustomPixelFormats(stFrameInfo.enPixelType))
                        {
                            m_cStream.ReleaseFrameBuffer(stFrameInfo);
                            continue;
                        }

                        // 配置显示图像的参数
                        stDisplayInfo.nWidth = stFrameInfo.nWidth;
                        stDisplayInfo.nHeight = stFrameInfo.nHeight;
                        stDisplayInfo.enPixelType = stFrameInfo.enPixelType;
                        stDisplayInfo.pImageBuf = stFrameInfo.pBuffer;
                        stDisplayInfo.nImageBufLen = stFrameInfo.nFilledSize;

                        try
                        {
                            HOperatorSet.GenImage1Extern(out hObject, "byte", stFrameInfo.nWidth, stFrameInfo.nHeight, (HTuple)stFrameInfo.pBuffer, IntPtr.Zero);
                            ImageEvent.Set();
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }

                        m_cStream.ReleaseFrameBuffer(stFrameInfo);
                    }
                    else
                    {
                        if (TRIGGER_MODE_ON == m_nTriggerMode)
                        {
                            Thread.Sleep(5);
                        }
                    }
                }
                else
                {
                    Thread.Sleep(5);
                }
                
                
            }
                    
        }

        public void SaveBmp()
        {
            int nRet = SaveImage(SAVE_IAMGE_TYPE.Image_Bmp);
            if (CErrorCode.MV_FG_SUCCESS != nRet)
            {
                LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
                MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
                MessageBox.Show("Save BMP failed, ErrorCode:" + nRet.ToString("X"));
                return;
            }
            LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
            MainForm.AlarmList.Add(System.DateTime.Now.ToString() + ":获取采集卡信息失败，故障码:" + nRet.ToString("X"));
            MessageBox.Show("Save BMP succeed");
        }

        private int SaveImage(SAVE_IAMGE_TYPE enSaveImageType)
        {
            int nRet = 0;

            lock (m_LockForSaveImage)
            {
                if (IntPtr.Zero == m_pDataBuf)
                {
                    return CErrorCode.MV_FG_ERR_NO_DATA;
                }

                if (RemoveCustomPixelFormats(m_stImageInfo.enPixelType))
                {
                    return CErrorCode.MV_FG_ERR_INVALID_VALUE;
                }

                uint nMaxImageLen = m_stImageInfo.nWidth * m_stImageInfo.nHeight * 4 + 2048; // 确保存图空间足够，包括图像头

                if (IntPtr.Zero == m_pSaveImageBuf || m_nSaveImageBufSize < nMaxImageLen)
                {
                    if (IntPtr.Zero != m_pSaveImageBuf)
                    {
                        Marshal.FreeHGlobal(m_pSaveImageBuf);
                        m_pSaveImageBuf = IntPtr.Zero;
                    }

                    m_pSaveImageBuf = Marshal.AllocHGlobal((Int32)nMaxImageLen);
                    if (IntPtr.Zero == m_pSaveImageBuf)
                    {
                        return CErrorCode.MV_FG_ERR_OUT_OF_MEMORY;
                    }
                    m_nSaveImageBufSize = nMaxImageLen;
                }

                CImageProcess cImgSave = new CImageProcess(m_cStream);
                System.DateTime currentTime = new System.DateTime();
                currentTime = System.DateTime.Now;

                do
                {
                    if (SAVE_IAMGE_TYPE.Image_Bmp == enSaveImageType)
                    {
                        MV_FG_SAVE_BITMAP_INFO stBmpInfo = new MV_FG_SAVE_BITMAP_INFO();

                        stBmpInfo.stInputImageInfo = m_stImageInfo;
                        stBmpInfo.pBmpBuf = m_pSaveImageBuf;
                        stBmpInfo.nBmpBufSize = m_nSaveImageBufSize;
                        stBmpInfo.enCfaMethod = MV_FG_CFA_METHOD.MV_FG_CFA_METHOD_OPTIMAL;

                        // ch:保存BMP图像 | en:Save to BMP
                        nRet = cImgSave.SaveBitmap(ref stBmpInfo);
                        if (CErrorCode.MV_FG_SUCCESS != nRet)
                        {
                            break;
                        }

                        // ch:将图像数据保存到本地文件 | en:Save image data to local file
                        byte[] byteData = new byte[stBmpInfo.nBmpBufLen];
                        Marshal.Copy(stBmpInfo.pBmpBuf, byteData, 0, (int)stBmpInfo.nBmpBufLen);

                        string strName = "Image_w" + stBmpInfo.stInputImageInfo.nWidth.ToString() +
                            "_h" + stBmpInfo.stInputImageInfo.nHeight.ToString() + "_" + currentTime.Minute.ToString() +
                            currentTime.Second.ToString() + currentTime.Millisecond.ToString() + ".bmp";

                        FileStream pFile = new FileStream(strName, FileMode.Create);
                        if (null == pFile)
                        {
                            nRet = CErrorCode.MV_FG_ERR_ERROR;
                            break;
                        }
                        pFile.Write(byteData, 0, byteData.Length);
                        pFile.Close();
                    }
                    else if (SAVE_IAMGE_TYPE.Image_Jpeg == enSaveImageType)
                    {
                        MV_FG_SAVE_JPEG_INFO stJpgInfo = new MV_FG_SAVE_JPEG_INFO();

                        stJpgInfo.stInputImageInfo = m_stImageInfo;
                        stJpgInfo.pJpgBuf = m_pSaveImageBuf;
                        stJpgInfo.nJpgBufSize = m_nSaveImageBufSize;
                        stJpgInfo.nJpgQuality = 80;                   // JPG编码质量(0-100]
                        stJpgInfo.enCfaMethod = MV_FG_CFA_METHOD.MV_FG_CFA_METHOD_OPTIMAL;

                        // ch:保存JPG图像 | en:Save to JPG
                        nRet = cImgSave.SaveJpeg(ref stJpgInfo);
                        if (CErrorCode.MV_FG_SUCCESS != nRet)
                        {
                            break;
                        }

                        // ch:将图像数据保存到本地文件 | en:Save image data to local file
                        byte[] byteData = new byte[stJpgInfo.nJpgBufLen];
                        Marshal.Copy(stJpgInfo.pJpgBuf, byteData, 0, (int)stJpgInfo.nJpgBufLen);

                        string strName = "Image_w" + stJpgInfo.stInputImageInfo.nWidth.ToString() +
                            "_h" + stJpgInfo.stInputImageInfo.nHeight.ToString() + "_" + currentTime.Minute.ToString() +
                            currentTime.Second.ToString() + currentTime.Millisecond.ToString() + ".jpg";

                        FileStream pFile = new FileStream(strName, FileMode.Create);
                        if (null == pFile)
                        {
                            nRet = CErrorCode.MV_FG_ERR_ERROR;
                            break;
                        }
                        pFile.Write(byteData, 0, byteData.Length);
                        pFile.Close();
                    }
                    else
                    {
                        nRet = CErrorCode.MV_FG_ERR_INVALID_PARAMETER;
                        break;
                    }
                } while (false);
            }

            return nRet;
        }

        // ch:去除自定义的像素格式 | en:Remove custom pixel formats
        private bool RemoveCustomPixelFormats(MV_FG_PIXEL_TYPE enPixelFormat)
        {
            Int32 nResult = ((int)enPixelFormat) & CUSTOMER_PIXEL_FORMAT;
            if (CUSTOMER_PIXEL_FORMAT == nResult)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
