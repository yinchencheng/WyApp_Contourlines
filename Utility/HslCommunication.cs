﻿using System;
using System.Threading;
using HslCommunication;
using HslCommunication.Core.Net;
using HslCommunication.Profinet.Melsec;
using HslCommunication.Profinet.Omron;
using HslCommunication.Profinet.Siemens;
using HslCommunication.Profinet.Inovance;
using WY_App.Utility;
using HslCommunication.Core;
using System.IO;

using HslCommunication.ModBus;
//power pmac配置
using ODT.PowerPmacComLib;
using ODT.Common.Services;
using ODT.Common.Core;
using System.Windows.Forms;
using System.Collections.Generic;
using static WY_App.Utility.Parameters;
using static OpenCvSharp.FileStorage;

namespace WY_App
{
    internal class HslCommunication
    {
        public static OperateResult _connected ;
        public static NetworkDeviceBase _NetworkTcpDevice;
        static ISyncGpasciiCommunicationInterface communication = null;
        string commands = String.Empty;
        public static string response = String.Empty;

        public static bool plc_connect_result = false;


        public HslCommunication()
        {
            try
            {
                Parameters.plcParams = XMLHelper.BackSerialize<Parameters.PLCParams>("Parameter/PLCParams.xml");
            }
            catch
            {
                Parameters.plcParams = new Parameters.PLCParams();
                XMLHelper.serialize<Parameters.PLCParams>(Parameters.plcParams, "Parameter/PLCParams.xml");
            }
           
            Thread th = new Thread(ini_PLC_Connect);
            th.IsBackground = true;
            th.Start();

            Thread PLC_Read = new Thread(ini_PLC_Read);
            PLC_Read.IsBackground = true;
            PLC_Read.Start();
        }
        public void ini_PLC_Connect()
        {
            if (!Authorization.SetAuthorizationCode("f562cc4c-4772-4b32-bdcd-f3e122c534e3"))
            {
                LogHelper.Log.WriteError(System.DateTime.Now.ToString() + "HslCommunication 组件认证失败，组件只能使用8小时!");
                MainForm.AlarmList.Add(System.DateTime.Now.ToString()+ "HslCommunication 组件认证失败，组件只能使用8小时!");
            }           
            while (!plc_connect_result)
            {
                Thread.Sleep(3000);
                try
                {
                 
                    if ("Omron.OmronFinsNet".Equals(Parameters.commministion.PlcType))
                    {
                        OmronFinsNet Client = new OmronFinsNet();
                        Client.IpAddress = Parameters.commministion.PlcIpAddress;
                        Client.Port = Parameters.commministion.PlcIpPort;
                        Client.SA1 = Convert.ToByte(Parameters.commministion.PlcDevice);
                        _NetworkTcpDevice = Client;
                        _connected = _NetworkTcpDevice.ConnectServer();
                        plc_connect_result = _connected.IsSuccess;
                    }
                    //三菱PLC Melsec.MelsecMcNet通讯
                    else if ("Melsec.MelsecMcNet".Equals(Parameters.commministion.PlcType))
                    {
                        MelsecMcNet Client = new MelsecMcNet();
                        Client.IpAddress = Parameters.commministion.PlcIpAddress;
                        Client.Port = Parameters.commministion.PlcIpPort;
                        _NetworkTcpDevice = Client;
                        _connected = _NetworkTcpDevice.ConnectServer();
                        plc_connect_result = _connected.IsSuccess;
                    }
                  
                    //西门子PLC Siemens.SiemensS7Net通讯
                    else if ("Siemens.SiemensS7Net".Equals(Parameters.commministion.PlcType))
                    {
                        SiemensS7Net Client = new SiemensS7Net((SiemensPLCS)Convert.ToInt16(Parameters.commministion.PlcDevice), Parameters.commministion.PlcIpAddress);
                        Client.IpAddress = Parameters.commministion.PlcIpAddress;
                        Client.Port = Parameters.commministion.PlcIpPort;
                        _NetworkTcpDevice = Client;
                        _connected = _NetworkTcpDevice.ConnectServer();
                        plc_connect_result = _connected.IsSuccess;
                    }
                    //汇川PLC InovanceSerialOverTcp通讯
                    else if ("InovanceSerialOverTcp".Equals(Parameters.commministion.PlcType))
                    {
                        InovanceSerialOverTcp Client = new InovanceSerialOverTcp();
                        Client.IpAddress = Parameters.commministion.PlcIpAddress;
                        Client.Port = Parameters.commministion.PlcIpPort;
                        Client.DataFormat = DataFormat.ABCD;
                        Client.Station = 0x01;
                        Client.Series = InovanceSeries.H5U;
                        Client.AddressStartWithZero = true;
                        _NetworkTcpDevice = Client;
                        _connected = _NetworkTcpDevice.ConnectServer();
                        plc_connect_result = _connected.IsSuccess;
                    }
                    //汇川PLC InovanceTcpNet通讯
                    else if ("InovanceTcpNet".Equals(Parameters.commministion.PlcType))
                    {
                        InovanceTcpNet Client = new InovanceTcpNet();
                        Client.IpAddress = Parameters.commministion.PlcIpAddress;
                        Client.Port = Parameters.commministion.PlcIpPort;
                        Client.DataFormat = DataFormat.CDAB;
                        Client.Station = 0x01;
                        Client.Series = InovanceSeries.H5U;
                        Client.AddressStartWithZero = true;
                        _NetworkTcpDevice = Client;
                        _connected = _NetworkTcpDevice.ConnectServer();
                        plc_connect_result = _connected.IsSuccess;
                    }
                    //ModbusTcpNet通讯
                    else if ("ModbusTcpNet".Equals(Parameters.commministion.PlcType))
                    {
                        ModbusTcpNet Client = new ModbusTcpNet();
                        Client.IpAddress = Parameters.commministion.PlcIpAddress;
                        Client.Port = Parameters.commministion.PlcIpPort;
                        Client.DataFormat = DataFormat.CDAB;
                        Client.Station = 0x01;
                        Client.AddressStartWithZero = true;
                        _NetworkTcpDevice = Client;
                        _connected = _NetworkTcpDevice.ConnectServer();
                        plc_connect_result = _connected.IsSuccess;
                    }
                    //新增通讯添加else if判断创建连接

                    //Parameter.PlcType字符错误或未定义
                    else
                    {
                        LogHelper.Log.WriteError(System.DateTime.Now.ToString() + Parameters.commministion.PlcType + "类型未定义!!!");
                        MainForm.AlarmList.Add(System.DateTime.Now.ToString() + Parameters.commministion.PlcType + "类型未定义!!!");
                        plc_connect_result = false;
                    }
                   
                    if (plc_connect_result)
                    {
                        LogHelper.Log.WriteInfo(System.DateTime.Now.ToString() + Parameters.commministion.PlcType + "连接成功:IP" + Parameters.commministion.PlcIpAddress + "  Port:" + Parameters.commministion.PlcIpPort);
                        MainForm.AlarmList.Add(System.DateTime.Now.ToString() + Parameters.commministion.PlcType + "连接成功:IP" + Parameters.commministion.PlcIpAddress + "  Port:" + Parameters.commministion.PlcIpPort);
                        plc_connect_result = true;
                    }
                    else
                    {
                        LogHelper.Log.WriteError(System.DateTime.Now.ToString() + Parameters.commministion.PlcType + "连接失败:IP" + Parameters.commministion.PlcIpAddress + "  Port:" + Parameters.commministion.PlcIpPort);
                        MainForm.AlarmList.Add(System.DateTime.Now.ToString() + Parameters.commministion.PlcType + "连接失败:IP" + Parameters.commministion.PlcIpAddress + "  Port:" + Parameters.commministion.PlcIpPort);
                        plc_connect_result = false;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Log.WriteError(System.DateTime.Now.ToString() + Parameters.commministion.PlcType + "初始化失败:", ex.Message);
                    MainForm.AlarmList.Add(System.DateTime.Now.ToString() + Parameters.commministion.PlcType + "初始化失败:"+ ex.Message);
                    plc_connect_result = false;
                }
            }
            while (plc_connect_result)
            {
                ////心跳读写，判断PLC是否掉线，不建议线程对plc链接释放重连
                if ("Omron.PMAC.CK3M".Equals(Parameters.commministion.PlcType))
                {
                    plc_connect_result = ReadWritePmacVariables(Parameters.plcParams.HeartBeatAdd);
                    Thread.Sleep(3000);
                }
                else
                {
                    try
                    {
                        if(Parameters.plcParams.HeartBeatAdd!=null|| Parameters.plcParams.HeartBeatAdd!="")
                        {
                            _connected = _NetworkTcpDevice.Write(Parameters.plcParams.HeartBeatAdd, 1);
                            Thread.Sleep(1000);
                            _connected = _NetworkTcpDevice.Write(Parameters.plcParams.HeartBeatAdd, 0);
                            Thread.Sleep(1000);
                            plc_connect_result = true;
                        }
                        else
                        {
                            Thread.Sleep(100000);
                        }
                        
                    }
                    catch
                    {
                        _NetworkTcpDevice.Dispose();
                        _connected.IsSuccess = false;
                        plc_connect_result = false;                        
                    }
                }
            }             
        }
        public void ini_PLC_Read()
        { 
            while(true)
            {
                if(plc_connect_result)
                {
                    if ("Omron.PMAC.CK3M".Equals(Parameters.commministion.PlcType))
                    {

                    }
                    else
                    {
                        
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                  
                }                
            }
        }


        //对终端操作的通用方法/
        public static bool ReadWritePmacVariables(string command)
        {
            var commads = new List<string>();
            List<string> responses;
            commads.Add(command.ToString());
            var communicationStatus = communication.GetResponse(commads, out responses, 3);
            if (communicationStatus == Status.Ok)
            {
                response = string.Join("", responses.ToArray());
                command = null;
                return  true;
            }
            else
            {
                return  false;
            }
        }


        public static  double plc_Readdouble(string ReadAddress)
        {
            return -1;
        }
        public static void plc_WriteDouble()
        {

        }

        public static void plc_WriteBool()
        {
           
        }
    }
}
