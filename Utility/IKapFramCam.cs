using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IKapBoardClassLibrary;
using IKapC.NET;
using HalconDotNet;
using OpenCvSharp;

namespace WY_App.Utility
{
    public class IKapFramCam
    {

        public static bool m_hCameraResult = false;
        // 相机设备句柄。
        //
        // Camera device handle.
        public IntPtr m_hCamera = new IntPtr(-1);

        // 采集卡设备句柄。
        //
        // Frame grabber device handle.
        public IntPtr m_hBoard = new IntPtr(-1);

        // 保存图像的文件名。
        //
        // File name of image.
        public string m_strFileName = "D:/Image/CSharpImage.tiff";

        public string m_strCamName = Parameters.commministion.productName+"/CamParams.vlcf";
        // 当前帧索引。
        //
        // Current frame index.
        public int m_nCurFrameIndex = 0;

        // 图像缓冲区申请的帧数。
        //
        // The number of frames requested by buffer.
        public int m_nTotalFrameCount = 5;

        /* @brief：判断 IKapC 函数是否成功调用。
         * @param[in] res：函数返回值。
         *
         * @brief：Determine whether the IKapC function is called successfully.
         * @param[in] res：Function return value. */
        static void CheckIKapC(uint res)
        {
            if (res != (uint)ItkStatusErrorId.ITKSTATUS_OK)
            {
                Console.Error.WriteLine("Error Code: {0}.\n", res.ToString("x8"));
                IKapCLib.ItkManTerminate();
                Console.ReadLine();
                Environment.Exit(1);
            }
        }

        /* @brief：判断 IKapBoard 函数是否成功调用。
         * @param[in] ret：函数返回值。
         *
         * @brief：Determine whether the IKapBoard function is called successfully.
         * @param[in] ret：Function return value. */
        static void CheckIKapBoard(int ret)
        {
            if (ret != (int)IKapBoardClassLibrary.ErrorCode.IK_RTN_OK)
            {
                string sErrMsg = "";
                IKapBoard.IKAPERRORINFO tIKei = new IKapBoardClassLibrary.IKapBoard.IKAPERRORINFO();

                // 获取错误码信息。
                //
                // Get error code message.
                IKapBoard.IKapGetLastError(ref tIKei, true);

                // 打印错误信息。
                //
                // Print error message.
                sErrMsg = string.Concat("Error",
                                        sErrMsg,
                                        "Board Type\t = 0x", tIKei.uBoardType.ToString("X4"), "\n",
                                        "Board Index\t = 0x", tIKei.uBoardIndex.ToString("X4"), "\n",
                                        "Error Code\t = 0x", tIKei.uErrorCode.ToString("X4"), "\n"
                                        );
                Console.Write(sErrMsg);
                Console.ReadLine();
                Environment.Exit(1);
            }
        }

        #region Callback
        delegate void IKapCallBackProc(IntPtr pParam);

        /* @brief：本函数被注册为一个回调函数。当图像采集开始时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When starting grabbing images, the function will be called. */
        private IKapCallBackProc OnGrabStartProc;

        /* @brief：本函数被注册为一个回调函数。当采集丢帧时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When grabbing frame lost, the function will be called. */
        private IKapCallBackProc OnFrameLostProc;

        /* @brief：本函数被注册为一个回调函数。当图像采集超时时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When grabbing images time out, the function will be called. */
        private IKapCallBackProc OnTimeoutProc;

        /* @brief：本函数被注册为一个回调函数。当一帧图像采集完成时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When a frame of image grabbing ready, the function will be called. */
        private IKapCallBackProc OnFrameReadyProc;

        /* @brief：本函数被注册为一个回调函数。当图像采集停止时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When stopping grabbing images, the function will be called. */
        private IKapCallBackProc OnGrabStopProc;
        #endregion

        #region Callback
        /* @brief：本函数被注册为一个回调函数。当图像采集开始时，函数被调用。
         * @param[in] pParam：输入参数。
         *
         * @brief：This function is registered as a callback function. When starting grabbing images, the function will be called.
         * @param[in] pParam：Input parameter. */
        public void OnGrabStartFunc(IntPtr pParam)
        {
            Console.WriteLine("Start grabbing image");
        }

        /* @brief：本函数被注册为一个回调函数。当采集丢帧时，函数被调用。
         * @param[in] pParam：输入参数。
         *
         * @brief：This function is registered as a callback function. When grabbing frame lost, the function will be called.
         * @param[in] pParam：Input parameter. */
        public void OnFrameLostFunc(IntPtr pParam)
        {
            Console.WriteLine("Grab frame lost");
        }

        /* @brief：本函数被注册为一个回调函数。当图像采集超时时，函数被调用。
         * @param[in] pParam：输入参数。
         *
         * @brief：This function is registered as a callback function. When grabbing images time out, the function will be called.
         * @param[in] pParam：Input parameter. */
        public void OnTimeoutFunc(IntPtr pParam)
        {
            Console.WriteLine("Grab image timeout");
        }

        /* @brief：本函数被注册为一个回调函数。当一帧图像采集完成时，函数被调用。
         * @param[in] pParam：输入参数。
         *
         * @brief：This function is registered as a callback function. When a frame of image grabbing ready, the function will be called.
         * @param[in] pParam：Input parameter. */
        public void OnFrameReadyFunc(IntPtr pParam)
        {
            MainForm.ImageWait.WaitOne();
            LogHelper.WriteInfo("Grab frame ready");

            IntPtr hDev = (IntPtr)pParam;
            IntPtr pUserBuffer = IntPtr.Zero;
            int nFrameSize = 0;
            int nFrameCount = 0;
            IKapBoard.IKAPBUFFERSTATUS status = new IKapBoard.IKAPBUFFERSTATUS();

            IKapBoard.IKapGetInfo(hDev, (uint)INFO_ID.IKP_FRAME_COUNT, ref nFrameCount);
            IKapBoard.IKapGetBufferStatus(hDev, m_nCurFrameIndex, ref status);
            

            // 当图像缓冲区满时。
            //
            // When the buffer is full.
            if (status.uFull == 1)
            {
                // 获取一帧图像的大小。
                //
                // Get the size of a frame of image.
                IKapBoard.IKapGetInfo(hDev, (uint)INFO_ID.IKP_FRAME_SIZE, ref nFrameSize);

                // 获取缓冲区地址。
                //
                // Get the buffer address.
                IKapBoard.IKapGetBufferAddress(hDev, m_nCurFrameIndex, ref pUserBuffer);
                //IntPtr pData = new IntPtr();
                //CopyMemory(pData, pUserBuffer, 1024);
                HOperatorSet.GenImage1(out MainForm.hImage[0], "byte", Halcon.hv_Width[0], Halcon.hv_Height[0], pUserBuffer);
                MainForm.ImageEvent.Set();
                // 保存图像。
                //
                // Save image.

                IKapBoard.IKapSaveBuffer(hDev,m_nCurFrameIndex,m_strFileName,(int)ImageCompressionFalg.IKP_DEFAULT_COMPRESSION);

            }
            m_nCurFrameIndex++;
            m_nCurFrameIndex = m_nCurFrameIndex % m_nTotalFrameCount;
        }

        /* @brief：本函数被注册为一个回调函数。当图像采集停止时，函数被调用。
         * @param[in] pParam：输入参数。
         *
         * @brief：This function is registered as a callback function. When stopping grabbing images, the function will be called.
         * @param[in] pParam：Input parameter. */
        public void OnGrabStopFunc(IntPtr pParam)
        {
            LogHelper.WriteInfo("Stop grabbing image");
        }
        #endregion

        #region member function

        /* @brief：初始化IKapC 运行环境。
         *
         * @brief：Initialize IKapC runtime environment. */
        private void InitEnvironment()
        {
            // IKapC 函数返回值。
            //
            // Return value of IKapC functions.
            uint res = (uint)ItkStatusErrorId.ITKSTATUS_OK;

            res = IKapCLib.ItkManInitialize();
            CheckIKapC(res);
        }

        /* @brief：释放 IKapC 运行环境。
         *
         * @brief：Release IKapC runtime environment. */
        private void ReleaseEnvironment()
        {
            IKapCLib.ItkManTerminate();
        }

        /* @brief：配置相机设备。
         *
         * @brief：Configure camera device. */
        private void ConfigureCamera()
        {
            uint res = (uint)ItkStatusErrorId.ITKSTATUS_OK;
            uint numCameras = 0;

            // 枚举可用相机的数量。在打开相机前，必须调用 ItkManGetDeviceCount() 函数。
            //
            // Enumerate the number of available cameras. Before opening the camera, ItkManGetDeviceCount() function must be called.
            res = IKapCLib.ItkManGetDeviceCount(ref numCameras);
            CheckIKapC(res);

            // 当没有连接的相机时。
            //
            // When there is no connected cameras.
            if (numCameras == 0)
            {
                LogHelper.WriteInfo("No camera.\n");
                IKapCLib.ItkManTerminate();
                Console.ReadLine();
                Environment.Exit(1);
            }

            // 打开CameraLink相机。
            //
            // Open CameraLink camera.
            for (uint i = 0; i < numCameras; i++)
            {
                IKapCLib.ITKDEV_INFO di = new IKapCLib.ITKDEV_INFO();

                // 获取相机设备信息。
                //
                // Get camera device information.
                res = IKapCLib.ItkManGetDeviceInfo(i, ref di);
                LogHelper.WriteInfo("Using camera: serial: {0}, name: {1}, interface: {2}.\n" + di.SerialNumber + di.FullName + di.DeviceClass);

                // 当设备为 CameraLink 相机且序列号正确时。
                //
                // When the device is CameraLink camera and the serial number is proper.
                if (di.DeviceClass == "CameraLink" && di.SerialNumber != "")
                {
                    IKapCLib.ITK_CL_DEV_INFO cl_board_info = new IKapCLib.ITK_CL_DEV_INFO();

                    // 打开相机。
                    //
                    // Open camera.
                    res = IKapCLib.ItkDevOpen(i, (int)ItkDeviceAccessMode.ITKDEV_VAL_ACCESS_MODE_EXCLUSIVE, ref m_hCamera);
                    CheckIKapC(res);

                    // 获取 CameraLink 相机设备信息。
                    //
                    // Get CameraLink camera device information.
                    res = IKapCLib.ItkManGetCLDeviceInfo(i, ref cl_board_info);
                    CheckIKapC(res);

                    // 打开采集卡。
                    //
                    // Open frame grabber.
                    m_hBoard = IKapBoard.IKapOpen(cl_board_info.HostInterface, cl_board_info.BoardIndex);
                    if (m_hBoard.Equals(-1))
                        CheckIKapBoard((int)IKapBoardClassLibrary.ErrorCode.IKStatus_OpenBoardFail);

                    break;
                }
            }
        }

        /* @brief：配置采集卡设备。
         *
         * @brief：Configure frame grabber device. */
        private void ConfigureFrameGrabber()
        {
            int ret = (int)IKapBoardClassLibrary.ErrorCode.IK_RTN_OK;
            string configFileName;

            // 导入配置文件。
            //
            // Load configuration file.
            configFileName = m_strCamName;
            if (configFileName == null)
            {
                LogHelper.WriteInfo("Fail to get configuration, using default setting!\n");
            }
            else
            {
                ret = IKapBoard.IKapLoadConfigurationFromFile(m_hBoard, configFileName);
                CheckIKapBoard(ret);
            }

            // 设置图像缓冲区帧数。
            //
            // Set frame count of buffer.
            ret = IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_FRAME_COUNT, m_nTotalFrameCount);
            CheckIKapBoard(ret);

            // 设置超时时间。
            //
            // Set time out time.
            int timeout = -1;
            ret = IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_TIME_OUT, timeout);
            CheckIKapBoard(ret);

            // 设置采集模式。
            //
            // Set grab mode.
            int grab_mode = (int)GrabMode.IKP_GRAB_NON_BLOCK;
            ret = IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_GRAB_MODE, grab_mode);
            CheckIKapBoard(ret);

            // 设置传输模式。
            //
            // Set transfer mode.
            int transfer_mode = (int)FrameTransferMode.IKP_FRAME_TRANSFER_SYNCHRONOUS_NEXT_EMPTY_WITH_PROTECT;
            ret = IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_FRAME_TRANSFER_MODE, transfer_mode);
            CheckIKapBoard(ret);

            // 注册回调函数
            //
            // Register callback functions.
            OnGrabStartProc = new IKapCallBackProc(OnGrabStartFunc);
            ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_GrabStart, Marshal.GetFunctionPointerForDelegate(OnGrabStartProc), m_hBoard);
            CheckIKapBoard(ret);
            OnFrameReadyProc = new IKapCallBackProc(OnFrameReadyFunc);
            ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_FrameReady, Marshal.GetFunctionPointerForDelegate(OnFrameReadyProc), m_hBoard);
            CheckIKapBoard(ret);
            OnFrameLostProc = new IKapCallBackProc(OnFrameLostFunc);
            ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_FrameLost, Marshal.GetFunctionPointerForDelegate(OnFrameLostProc), m_hBoard);
            CheckIKapBoard(ret);
            OnTimeoutProc = new IKapCallBackProc(OnTimeoutFunc);
            ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_TimeOut, Marshal.GetFunctionPointerForDelegate(OnTimeoutProc), m_hBoard);
            CheckIKapBoard(ret);
            OnGrabStopProc = new IKapCallBackProc(OnGrabStopFunc);
            ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_GrabStop, Marshal.GetFunctionPointerForDelegate(OnGrabStopProc), m_hBoard);
            CheckIKapBoard(ret);
        }

        /* @brief：清除回调函数。
         *
         * @brief：Unregister callback functions. */
        private void UnRegisterCallback()
        {
            int ret = (int)IKapBoardClassLibrary.ErrorCode.IK_RTN_OK;
            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_GrabStart);
            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_FrameReady);
            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_FrameLost);
            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_TimeOut);
            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_GrabStop);
        }

        /* @brief：关闭设备。
         *
         * @brief：Close device. */
        public void CloseIKapBoard()
        {
            try
            {
                // 关闭采集卡设备。
                //
                // Close frame grabber device.
                if (!m_hBoard.Equals(-1))
                {
                    IKapBoard.IKapClose(m_hBoard);
                    m_hBoard = (IntPtr)(-1);
                }

                // 关闭相机设备。
                //
                // Close camera device.
                if (!m_hCamera.Equals(-1))
                {
                    IKapCLib.ItkDevClose(m_hCamera);
                    m_hCamera = (IntPtr)(-1);
                }              
            }
            catch (Exception e)
            {
                
            }
            
        }
        #endregion
        public static bool StartGrabImage(IKapLineCam grab)
        {
            try
            {
                int ret = 0;
                // 开始图像采集。
                //
                // Start grabbing images.
                ret = IKapBoard.IKapStartGrab(grab.m_hBoard, 0);
                CheckIKapBoard(ret);
                // 等待图像采集结束。
                //
                // Wait for grabbing images finished.
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            
        }
        public static bool StopGrabImage(IKapLineCam grab)
        {
            try
            {
                // 停止图像采集。
                //
                // Stop grabbing images.
                int ret = 0;
                ret = IKapBoard.IKapStopGrab(grab.m_hBoard);
                CheckIKapBoard(ret);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            
        }
        public static bool ColseDevice(IKapFramCam grab)
        {
            try
            {
                // 清除回调函数。
                //
                // Unregister callback functions.
                grab.UnRegisterCallback();

                // 关闭设备。
                //
                // Close device.
                grab.CloseIKapBoard();

                // 释放 IKapC 运行环境。
                //
                // Release IKapC runtime environment.
                grab.ReleaseEnvironment();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
}
        public static bool OpenDevice(IKapFramCam grab)
        {
            try
            {
                // 初始化 IKapC 运行环境。
                //
                // Initialize IKapC runtime environment.
                grab.InitEnvironment();

                // 配置相机设备。
                //
                // Configure camera device.
                grab.ConfigureCamera();

                // 配置采集卡设备。
                //
                // Configure frame grabber device.
                grab.ConfigureFrameGrabber();
                m_hCameraResult = true;
                return true;
            }
            catch (Exception e) 
            {
                m_hCameraResult=false;
                return false;
            }
        }
    }
}
