/**
* 命名空间: Common 
* 类 名： ThreadCollect
* 描 述：
*
* Ver      负责人        变更内容            变更日期
* ──────────────────────────────────────────────────────────────
* V1.0     张亮          初版                2018-09-04 16:30:50 
*
* Copyright (c) 2018 724pride Corporation. All rights reserved. 
*┌──────────────────────────────────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．       │
*│　版权所有：中船重工鹏力（南京）智能装备系统有限公司　　　　　　　　　　　　　　              │
*└──────────────────────────────────────────────────────────────┘
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Model;
using Newtonsoft.Json;
using S7Func;
using SqlSugar;


namespace Common
{
    #region 连接ping通类
    public class PingComputer
    {
        private Thread f_PingThread;  //ping通线程
        public string f_pingip;
        public static event ViewEventHandler OnPingnotConnected;
        public PingComputer(string ip)
        {
            this.f_pingip = ip;
            f_PingThread = new Thread(PingFun);
            f_PingThread.Start();     //开启ping通线程
        }

        public void PingFun()
        {
            for (int i = 0; i < 4; i++)   //ping4次 连续ping不通就不通了
            {
                if (PingConnect(f_pingip))
                    return;
            }
            LogRecord logRcd = new LogRecord();
            logRcd.ip = f_pingip;
            logRcd.log = "IP地址不通";
            logRcd.time = DateTime.Now.ToString("MM-dd HH:mm:ss");
            //OnPingnotConnected(this, new ViewLogEventArgs(logRcd));  //ping失败
        }

        public bool PingConnect(string ip)
        {
            try
            {
                if (ip == "") return false;
                if (ip == "127.0.0.1") return true;
                Ping pingSender = new Ping();
                PingOptions options = new PingOptions();
                options.DontFragment = true;
                string data = "pingip";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 1000;
                //如果网络连接成功，PING就应该有返回；否则，网络连接有问题
                PingReply reply = pingSender.Send(ip, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else return false;
            }
            catch
            {
                return false;
            }
        }
    }

    #endregion
    #region 前期准备并且调用线程类
    public class PrePareForCollect
    {
        public List<CollectData> f_ThreadList = new List<CollectData>();

        #region 创建所有ping线程
        public void PingThreadCreate()
        {
            var db = DBHelper.GetInstance();
            try
            {
                var eqmipList = db.Queryable<T_BF_EqmInfo>().ToList();
                if (eqmipList == null) return;
                if (eqmipList.Count <= 0) return;
                //foreach (string ip in eqmipList)
                //{
                //    PingComputer data = new PingComputer(ip);
                //}
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("创建所有ping线程失败", ex);
            }
        }

        #endregion
        #region 关闭所有线程
        public void CloseAllThread()
        {
            if (f_ThreadList.Count > 0)
            {
                foreach (CollectData thread in f_ThreadList)
                {
                    thread.CloseThread();
                }
                f_ThreadList.Clear();
                LogHelper.WriteLog("关闭所有采集线程!");
            }
        }
        #endregion
        #region  创建所有采集线程  不同ip端口创建一个线程

        public string CreateAllThreadToCollect()
        {
            try
            {
                var db = DBHelper.GetInstance();
                var eqmipList = db.Queryable<T_BF_EqmInfo>().ToList();
                if (eqmipList == null) return "PLC信息配置错误";
                if (eqmipList.Count <= 0) return "PLC信息配置错误";
                foreach (T_BF_EqmInfo eqm in eqmipList)
                {
                    CollectData data = new CollectData();
                    data.f_PlcSet = eqm;
                    data.firstCollect = true;

                    data.InitCollect();
                    f_ThreadList.Add(data);
                }

                return "";
            }
            catch (Exception ex)
            {

                return ex.ToString();
            }
           
        }

        #endregion
    }


    #endregion
    #region 线程类
    public class CollectData
    {
        #region 变量声明
        public Thread f_collectThread;  //采集线程
        private S7Client f_eipclient;  //连接
        public T_BF_EqmInfo f_PlcSet;//设备信息
        public bool firstCollect = true; //第一次取得数据
        public int f_waittime = 2000;//读取数据间隔时间
        private PingComputer f_pingconnect;
        public string readTag;
        public string writeTag;
        public string QRCode;
        //采集设备信息成功事件
        public static event ViewEventHandler OnConnectStatusInfo;
        public LogRecord f_logrcd = new LogRecord();  //日志记录
        #endregion
        public void CloseThread()  //关闭线程
        {
            try
            {

                f_eipclient.Dispose();
                f_collectThread.Abort();
            }
            catch
            { }
        }
        #region 多线程采集数据
        public void InitCollect()
        {
            f_eipclient = new S7Client(f_PlcSet.F_EqmIP, f_PlcSet.F_EqmPort);
            f_pingconnect = new PingComputer(f_PlcSet.F_EqmIP);
            Dictionary<string, string> obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(f_PlcSet.F_EqmTag);
            readTag = obj["read"];
            writeTag = obj["write"];
            f_collectThread = new Thread(CollectFun);
            f_collectThread.IsBackground = true;
            f_collectThread.Start();     //开启线程 采集
        }

        public void CollectFun()
        {
            OnConnectStatusInfo(this, new ViewLogEventArgs(GetLogText("启动线程")));
            TcpCollect:
            try
            {
                f_eipclient.Dispose();
                Thread.Sleep(4000);
                if (!f_pingconnect.PingConnect(f_pingconnect.f_pingip))
                {
                    OnConnectStatusInfo(this, new ViewLogEventArgs(GetLogText("IP地址不通")));
                    goto TcpCollect;
                }
                if (!f_eipclient.StartClient())
                {
                    OnConnectStatusInfo(this, new ViewLogEventArgs(GetLogText("socket连接失败")));
                    goto TcpCollect;
                }
                if (!f_eipclient.SendSession())
                {
                    OnConnectStatusInfo(this, new ViewLogEventArgs(GetLogText("EIP连接失败")));
                    goto TcpCollect;
                }
                if (!f_eipclient.SendComOpen())
                {
                    OnConnectStatusInfo(this, new ViewLogEventArgs(GetLogText("Fwd打开失败")));
                    goto TcpCollect;
                }
                OnConnectStatusInfo(this, new ViewLogEventArgs(GetLogText("正常")));
                while (true)
                {
                    //OnConnectStatusInfo(this, new ViewLogEventArgs(GetLogText("正常")));
                    //if (readTag == "DB322")
                    {

                        if (!f_eipclient.SendTagName(readTag, 140, 0))
                        {
                            OnConnectStatusInfo(this, new ViewLogEventArgs(GetLogText("读取PLC数据失败")));
                            goto TcpCollect;
                        }
                        else
                        {
                            OnConnectStatusInfo(this, new ViewLogEventArgs(GetLogText("正常")));
                            byte[] readresult = f_eipclient.galRecvBytesResult;
                            if (readresult[99] == 1) //扫到码
                            {
                                //下发参数
                                QRCode = ChangeToInfo(f_eipclient.galRecvBytesResult, 108, 32, 6, 0);
                                int flag = SendParam(f_PlcSet.F_EqmCode, f_PlcSet.F_EqmIP, f_PlcSet.F_EqmPort, writeTag);
                                if (flag == 1)
                                    SendParam(f_PlcSet.F_EqmCode, f_PlcSet.F_EqmIP, f_PlcSet.F_EqmPort, readTag, 102, 2, 1);
                                else
                                {
                                    SendParam(f_PlcSet.F_EqmCode, f_PlcSet.F_EqmIP, f_PlcSet.F_EqmPort, readTag, 102, 2, 2);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                OnConnectStatusInfo(this, new ViewLogEventArgs(GetLogText("采集数据异常或已关闭")));
                goto TcpCollect;
            }
        }

        #endregion

        #region//下发标志位

        public void SendParam(string eqm, string ip, int port, string writetag, int start, int len, int value)
        {
            List<int> flag = new List<int>();
            flag.Add(value);
            f_eipclient.WriteTagData(writetag, start, len, flag);
        }

        #endregion
        #region //下发扫码参数

        public int SendParam(string eqm, string ip, int port, string writetag)
        {
            try
            {
                string altertable = null;//删除更新结果表
                List<int> writevalue = new List<int>();
                //var db = DBHelper.GetInstance();
                switch (eqm)
                {
                    #region 初校
                    case "chujiao":
                        try
                        {
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 124));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 126));
                            for (int i = 0; i < 5; i++) writevalue.Add(0);
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 108));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 110));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 112));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 114));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 116));
                            writevalue.Add(0);
                            writevalue.Add(0);
                            writevalue.Add(1);
                            writevalue.Add(1);
                            using (var db = DBHelper.GetInstance())//产品表 更新或插入测试方案序号
                            {
                                var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).ToList();
                                if (list.Count == 0) db.Insertable<T_BF_ProductInfo>(new { F_QRCode = QRCode }).ExecuteCommand();
                                list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                string detailstr = list[0].F_ProductDetail;
                                Dictionary<string, string> detaildic;
                                if (detailstr == null)
                                    detaildic = new Dictionary<string, string>();
                                else
                                    detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                detaildic["表型序号"] = "1";
                                detailstr = JsonConvert.SerializeObject(detaildic);
                                db.Updateable<T_BF_ProductInfo>()
                                   .UpdateColumns(it => it.F_ProductDetail == detailstr)
                                   .Where(it => it.F_QRCode == QRCode)
                                   .ExecuteCommand();
                            }
                            using (var db = DBHelper.GetInstance())//方案相关信息
                            {
                                var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                string detailstr = list[0].F_ProductDetail;
                                Dictionary<string, string> detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                var resultlist = db.Queryable<T_MeterModel>().Where(it => it.F_ID == Convert.ToInt32(detaildic["表型序号"])).ToList();
                                writevalue.Add(resultlist[0].F_ID);
                                writevalue.Add(1);
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_Model));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_GasTestID));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_GearTestID));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_TransCoffe));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_StandardID));
                            }


                            f_eipclient.WriteTagData(writetag, 0, 46, writevalue);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLog("初校下发错误", ex);
                            throw ex;
                        }
                        break;
                    #endregion
                    #region 复校1 
                    case "fujiao1":
                        try
                        {
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 124));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 126));
                            for (int i = 0; i < 5; i++) writevalue.Add(0);
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 108));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 110));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 112));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 114));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 116));
                            writevalue.Add(0);
                            writevalue.Add(0);
                            writevalue.Add(1);
                            writevalue.Add(1);
                            using (var db = DBHelper.GetInstance())//产品表 更新或插入测试方案序号
                            {
                                var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).ToList();
                                if (list.Count == 0)
                                {
                                    db.Insertable<T_BF_ProductInfo>(new { F_QRCode = QRCode }).ExecuteCommand();
                                    list =
                                        db.Queryable<T_BF_ProductInfo>()
                                            .Where(it => it.F_QRCode == QRCode)
                                            .OrderBy(it => it.F_Time, OrderByType.Desc)
                                            .ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    Dictionary<string, string> detaildic;
                                    if (detailstr == null)
                                        detaildic = new Dictionary<string, string>();
                                    else
                                        detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    detaildic["表型序号"] = "1";
                                    detailstr = JsonConvert.SerializeObject(detaildic);
                                    db.Updateable<T_BF_ProductInfo>()
                                        .UpdateColumns(it => it.F_ProductDetail == detailstr)
                                        .Where(it => it.F_QRCode == QRCode)
                                        .ExecuteCommand();
                                }
                            }
                            using (var db = DBHelper.GetInstance())//方案相关信息
                            {
                                var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                string detailstr = list[0].F_ProductDetail;
                                Dictionary<string, string> detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                var resultlist = db.Queryable<T_MeterModel>().Where(it => it.F_ID == Convert.ToInt32(detaildic["表型序号"])).ToList();
                                writevalue.Add(resultlist[0].F_ID);
                                writevalue.Add(1);
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_Model));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_GasTestID));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_GearTestID));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_TransCoffe));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_StandardID));
                            }
                            using (var db = DBHelper.GetInstance()) //初教结果
                            {
                                string chujiaoresult = "-1";
                                var resultlist =
                                    db.Queryable<T_TestResult>()
                                        .Where(it => it.F_FirstorSecond == 1 && it.F_SerialNum == QRCode)
                                        .OrderBy(it => it.F_TransTime, OrderByType.Desc)
                                        .ToList();
                                if (resultlist.Count > 0)//初校结果最新数据
                                {
                                    if (resultlist[0].F_FinalResult == "1")
                                        chujiaoresult = resultlist[0].F_FinalData;
                                    else chujiaoresult = "1000";
                                    //更新初校结果
                                    var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    Dictionary<string, string> detaildic;
                                    if (detailstr == null)
                                        detaildic = new Dictionary<string, string>();
                                    else
                                        detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    detaildic["二期初校结果"] = chujiaoresult;
                                    detailstr = JsonConvert.SerializeObject(detaildic);
                                    db.Updateable<T_BF_ProductInfo>()
                                       .UpdateColumns(it => it.F_ProductDetail == detailstr)
                                       .Where(it => it.F_QRCode == QRCode)
                                       .ExecuteCommand();
                                    //删除临时表数据
                                    db.Deleteable<T_TestResult>()
                                        .Where(it => it.F_FirstorSecond == 1 && it.F_SerialNum == QRCode)
                                        .ExecuteCommand();
                                }
                                else if (resultlist.Count <= 0)//产品信息表查找初校结果
                                {
                                    var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    if (detailstr == null)
                                        throw new Exception();
                                    Dictionary<string, string> detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    if (!detaildic.ContainsKey("二期初校结果"))
                                        throw new Exception();
                                    chujiaoresult = detaildic["二期初校结果"];
                                }
                                if (chujiaoresult != "-1")
                                    writevalue.Add(Convert.ToInt32(chujiaoresult));
                            }
                            f_eipclient.WriteTagData(writetag, 0, 48, writevalue);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLog("复校1下发错误", ex);
                            throw ex;
                        }
                        break;
                    #endregion
                    #region 复校2
                    case "fujiao2":
                        try
                        {
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 124));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 126));
                            for (int i = 0; i < 5; i++) writevalue.Add(0);
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 108));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 110));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 112));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 114));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 116));
                            writevalue.Add(0);
                            writevalue.Add(0);
                            writevalue.Add(1);
                            writevalue.Add(1);
                            using (var db = DBHelper.GetInstance())//产品表 更新或插入测试方案序号
                            {
                                var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).ToList();
                                if (list.Count == 0)
                                {
                                    db.Insertable<T_BF_ProductInfo>(new { F_QRCode = QRCode }).ExecuteCommand();
                                    list =
                                        db.Queryable<T_BF_ProductInfo>()
                                            .Where(it => it.F_QRCode == QRCode)
                                            .OrderBy(it => it.F_Time, OrderByType.Desc)
                                            .ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    Dictionary<string, string> detaildic;
                                    if (detailstr == null)
                                        detaildic = new Dictionary<string, string>();
                                    else
                                        detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    detaildic["表型序号"] = "1";
                                    detailstr = JsonConvert.SerializeObject(detaildic);
                                    db.Updateable<T_BF_ProductInfo>()
                                        .UpdateColumns(it => it.F_ProductDetail == detailstr)
                                        .Where(it => it.F_QRCode == QRCode)
                                        .ExecuteCommand();
                                }
                            }
                            using (var db = DBHelper.GetInstance())//方案相关信息
                            {
                                var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                string detailstr = list[0].F_ProductDetail;
                                Dictionary<string, string> detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                var resultlist = db.Queryable<T_MeterModel>().Where(it => it.F_ID == Convert.ToInt32(detaildic["表型序号"])).ToList();
                                writevalue.Add(resultlist[0].F_ID);
                                writevalue.Add(1);
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_Model));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_GasTestID));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_GearTestID));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_TransCoffe));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_StandardID));
                            }
                            using (var db = DBHelper.GetInstance()) //初教结果
                            {
                                string chujiaoresult = "-1";
                                var resultlist =
                                    db.Queryable<T_TestResult>()
                                        .Where(it => it.F_FirstorSecond == 1 && it.F_SerialNum == QRCode)
                                        .OrderBy(it => it.F_TransTime, OrderByType.Desc)
                                        .ToList();
                                if (resultlist.Count > 0)//初校结果最新数据
                                {
                                    if (resultlist[0].F_FinalResult == "1")
                                        chujiaoresult = resultlist[0].F_FinalData;
                                    else chujiaoresult = "1000";
                                    //更新初校结果
                                    var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    Dictionary<string, string> detaildic;
                                    if (detailstr == null)
                                        detaildic = new Dictionary<string, string>();
                                    else
                                        detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    detaildic["二期初校结果"] = chujiaoresult;
                                    detailstr = JsonConvert.SerializeObject(detaildic);
                                    db.Updateable<T_BF_ProductInfo>()
                                       .UpdateColumns(it => it.F_ProductDetail == detailstr)
                                       .Where(it => it.F_QRCode == QRCode)
                                       .ExecuteCommand();
                                    //删除临时表数据
                                    db.Deleteable<T_TestResult>()
                                        .Where(it => it.F_FirstorSecond == 1 && it.F_SerialNum == QRCode)
                                        .ExecuteCommand();
                                }
                                else if (resultlist.Count <= 0)//产品信息表查找初校结果
                                {
                                    var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    if (detailstr == null)
                                        throw new Exception();
                                    Dictionary<string, string> detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    if (!detaildic.ContainsKey("二期初校结果"))
                                        throw new Exception();
                                    chujiaoresult = detaildic["二期初校结果"];
                                }
                                if (chujiaoresult != "-1")
                                    writevalue.Add(Convert.ToInt32(chujiaoresult));
                            }
                            f_eipclient.WriteTagData(writetag, 0, 48, writevalue);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLog("复校2下发错误", ex);
                            throw ex;
                        }
                        break;
                    #endregion
                    #region 返修下线
                    case "fanxiuxiaxian":
                        try
                        {
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 124));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 126));
                            for (int i = 0; i < 5; i++) writevalue.Add(0);
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 108));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 110));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 112));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 114));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 116));
                            writevalue.Add(0);
                            writevalue.Add(0);
                            writevalue.Add(1);
                            writevalue.Add(1);
                            using (var db = DBHelper.GetInstance())//产品表 更新或插入测试方案序号
                            {
                                var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).ToList();
                                if (list.Count == 0)
                                {
                                    db.Insertable<T_BF_ProductInfo>(new { F_QRCode = QRCode }).ExecuteCommand();
                                    list =
                                        db.Queryable<T_BF_ProductInfo>()
                                            .Where(it => it.F_QRCode == QRCode)
                                            .OrderBy(it => it.F_Time, OrderByType.Desc)
                                            .ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    Dictionary<string, string> detaildic;
                                    if (detailstr == null)
                                        detaildic = new Dictionary<string, string>();
                                    else
                                        detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    detaildic["表型序号"] = "1";
                                    detailstr = JsonConvert.SerializeObject(detaildic);
                                    db.Updateable<T_BF_ProductInfo>()
                                        .UpdateColumns(it => it.F_ProductDetail == detailstr)
                                        .Where(it => it.F_QRCode == QRCode)
                                        .ExecuteCommand();
                                }
                            }
                            using (var db = DBHelper.GetInstance())//方案相关信息
                            {
                                var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                string detailstr = list[0].F_ProductDetail;
                                Dictionary<string, string> detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                var resultlist = db.Queryable<T_MeterModel>().Where(it => it.F_ID == Convert.ToInt32(detaildic["表型序号"])).ToList();
                                writevalue.Add(resultlist[0].F_ID);
                                writevalue.Add(1);
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_Model));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_GasTestID));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_GearTestID));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_TransCoffe));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_StandardID));
                            }
                            using (var db = DBHelper.GetInstance()) //初教结果
                            {
                                string chujiaoresult = "-1";
                                var resultlist =
                                    db.Queryable<T_TestResult>()
                                        .Where(it => it.F_FirstorSecond == 1 && it.F_SerialNum == QRCode)
                                        .OrderBy(it => it.F_TransTime, OrderByType.Desc)
                                        .ToList();
                                if (resultlist.Count > 0)//初校结果最新数据
                                {
                                    if (resultlist[0].F_FinalResult == "1")
                                        chujiaoresult = resultlist[0].F_FinalData;
                                    else chujiaoresult = "1000";
                                    //更新初校结果
                                    var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    Dictionary<string, string> detaildic;
                                    if (detailstr == null)
                                        detaildic = new Dictionary<string, string>();
                                    else
                                        detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    detaildic["二期初校结果"] = chujiaoresult;
                                    detailstr = JsonConvert.SerializeObject(detaildic);
                                    db.Updateable<T_BF_ProductInfo>()
                                       .UpdateColumns(it => it.F_ProductDetail == detailstr)
                                       .Where(it => it.F_QRCode == QRCode)
                                       .ExecuteCommand();
                                    //删除临时表数据
                                    db.Deleteable<T_TestResult>()
                                        .Where(it => it.F_FirstorSecond == 1 && it.F_SerialNum == QRCode)
                                        .ExecuteCommand();
                                }
                                else if (resultlist.Count <= 0)//产品信息表查找初校结果
                                {
                                    var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    if (detailstr == null)
                                        throw new Exception();
                                    Dictionary<string, string> detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    if (!detaildic.ContainsKey("二期初校结果"))
                                        throw new Exception();
                                    chujiaoresult = detaildic["二期初校结果"];
                                }
                                if (chujiaoresult != "-1")
                                    writevalue.Add(Convert.ToInt32(chujiaoresult));
                            }
                            using (var db = DBHelper.GetInstance())//复校结果
                            {
                                var resultlist =
                                   db.Queryable<T_TestResult>()
                                       .Where(it => it.F_FirstorSecond == 2 && it.F_SerialNum == QRCode)
                                       .OrderBy(it => it.F_TransTime, OrderByType.Desc)
                                       .ToList();
                                var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                string detailstr = list[0].F_ProductDetail;
                                //没有复校数据抛出错误，不下发参数
                                if (resultlist.Count <= 0 && detailstr == null) throw new Exception();
                                Dictionary<string, string> detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                if (!detaildic.ContainsKey("二期复校结果1") && resultlist.Count <= 0)
                                    throw new Exception();
                                if (detaildic.ContainsKey("二期复校结果1") && detaildic["二期复校结果1"] == "0" && resultlist.Count <= 0)
                                    throw new Exception();
                                if (resultlist.Count > 0)//有最新复查数据
                                {
                                    //第一次复查
                                    if (!detaildic.ContainsKey("二期复校结果1"))
                                    {
                                        if (resultlist[0].F_FinalResult == "1")
                                        {
                                            detaildic["二期复校结果1"] = "1";
                                            detaildic["二期复校结果2"] = "0";
                                        }
                                        else
                                        {
                                            detaildic["二期复校结果1"] = "1000";
                                            detaildic["二期复校结果2"] = "0";
                                        }
                                    }
                                    else if (detaildic["二期复校结果1"] == "0")
                                    {
                                        if (resultlist[0].F_FinalResult == "1")
                                        {
                                            detaildic["二期复校结果1"] = "1";
                                            detaildic["二期复校结果2"] = "0";
                                        }
                                        else
                                        {
                                            detaildic["二期复校结果1"] = "1000";
                                            detaildic["二期复校结果2"] = "0";
                                        }
                                    }
                                    else //更新第二次复查数据
                                    {
                                        if (resultlist[0].F_FinalResult == "1")
                                        {
                                            detaildic["二期复校结果2"] = "1";
                                        }
                                        else
                                        {
                                            detaildic["二期复校结果2"] = "1000";
                                        }
                                    }
                                    //更新产品信息表
                                    detailstr = JsonConvert.SerializeObject(detaildic);
                                    db.Updateable<T_BF_ProductInfo>()
                                       .UpdateColumns(it => it.F_ProductDetail == detailstr)
                                       .Where(it => it.F_QRCode == QRCode)
                                       .ExecuteCommand();
                                    //删除中间结果表信息
                                    db.Deleteable<T_TestResult>()
                                      .Where(it => it.F_FirstorSecond == 2 && it.F_SerialNum == QRCode)
                                      .ExecuteCommand();
                                    //添加下发结果
                                    writevalue.Add(Convert.ToInt16(detaildic["二期复校结果1"]));
                                    writevalue.Add(Convert.ToInt16(detaildic["二期复校结果2"]));
                                }
                                else //直接从产品信息表下发数据
                                {
                                    writevalue.Add(Convert.ToInt16(detaildic["二期复校结果1"]));
                                    writevalue.Add(Convert.ToInt16(detaildic["二期复校结果2"]));
                                }
                            }
                            f_eipclient.WriteTagData(writetag, 0, 52, writevalue);

                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLog("返修下线下发错误", ex);
                            throw ex;
                        }
                        break;
                    #endregion
                    #region 装齿移栽
                    case "zhuangchiyizai":
                        try
                        {
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 124));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 126));
                            for (int i = 0; i < 5; i++) writevalue.Add(0);
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 108));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 110));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 112));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 114));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 116));
                            writevalue.Add(0);
                            writevalue.Add(0);
                            writevalue.Add(1);
                            writevalue.Add(1);
                            using (var db = DBHelper.GetInstance())//产品表 更新或插入测试方案序号
                            {
                                var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).ToList();
                                if (list.Count == 0)
                                {
                                    db.Insertable<T_BF_ProductInfo>(new { F_QRCode = QRCode }).ExecuteCommand();
                                    list =
                                        db.Queryable<T_BF_ProductInfo>()
                                            .Where(it => it.F_QRCode == QRCode)
                                            .OrderBy(it => it.F_Time, OrderByType.Desc)
                                            .ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    Dictionary<string, string> detaildic;
                                    if (detailstr == null)
                                        detaildic = new Dictionary<string, string>();
                                    else
                                        detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    detaildic["表型序号"] = "1";
                                    detailstr = JsonConvert.SerializeObject(detaildic);
                                    db.Updateable<T_BF_ProductInfo>()
                                        .UpdateColumns(it => it.F_ProductDetail == detailstr)
                                        .Where(it => it.F_QRCode == QRCode)
                                        .ExecuteCommand();
                                }
                            }
                            using (var db = DBHelper.GetInstance())//方案相关信息
                            {
                                var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                string detailstr = list[0].F_ProductDetail;
                                Dictionary<string, string> detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                var resultlist = db.Queryable<T_MeterModel>().Where(it => it.F_ID == Convert.ToInt32(detaildic["表型序号"])).ToList();
                                writevalue.Add(resultlist[0].F_ID);
                                writevalue.Add(1);
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_Model));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_GasTestID));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_GearTestID));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_TransCoffe));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_StandardID));
                            }
                            using (var db = DBHelper.GetInstance()) //初教结果
                            {
                                string chujiaoresult = "-1";
                                var resultlist =
                                    db.Queryable<T_TestResult>()
                                        .Where(it => it.F_FirstorSecond == 1 && it.F_SerialNum == QRCode)
                                        .OrderBy(it => it.F_TransTime, OrderByType.Desc)
                                        .ToList();
                                if (resultlist.Count > 0)//初校结果最新数据
                                {
                                    if (resultlist[0].F_FinalResult == "1")
                                        chujiaoresult = resultlist[0].F_FinalData;
                                    else chujiaoresult = "1000";
                                    //更新初校结果
                                    var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    Dictionary<string, string> detaildic;
                                    if (detailstr == null)
                                        detaildic = new Dictionary<string, string>();
                                    else
                                        detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    detaildic["二期初校结果"] = chujiaoresult;
                                    detailstr = JsonConvert.SerializeObject(detaildic);
                                    db.Updateable<T_BF_ProductInfo>()
                                       .UpdateColumns(it => it.F_ProductDetail == detailstr)
                                       .Where(it => it.F_QRCode == QRCode)
                                       .ExecuteCommand();
                                    //删除临时表数据
                                    db.Deleteable<T_TestResult>()
                                        .Where(it => it.F_FirstorSecond == 1 && it.F_SerialNum == QRCode)
                                        .ExecuteCommand();
                                }
                                else if (resultlist.Count <= 0)//产品信息表查找初校结果
                                {
                                    var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    if (detailstr == null)
                                        throw new Exception();
                                    Dictionary<string, string> detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    if (!detaildic.ContainsKey("二期初校结果"))
                                        throw new Exception();
                                    chujiaoresult = detaildic["二期初校结果"];
                                }
                                if (chujiaoresult != "-1")
                                    writevalue.Add(Convert.ToInt32(chujiaoresult));
                            }
                            f_eipclient.WriteTagData(writetag, 0, 48, writevalue);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLog("装齿移栽下发错误", ex);
                            throw ex;
                        }
                        break;
                    #endregion
                    #region 预读1
                    case "yudu1":
                        try
                        {
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 124));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 126));
                            for (int i = 0; i < 5; i++) writevalue.Add(0);
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 108));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 110));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 112));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 114));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 116));
                            writevalue.Add(0);
                            writevalue.Add(0);
                            writevalue.Add(1);
                            writevalue.Add(1);
                            using (var db = DBHelper.GetInstance())//产品表 更新或插入测试方案序号
                            {
                                var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).ToList();
                                if (list.Count == 0)
                                {
                                    db.Insertable<T_BF_ProductInfo>(new { F_QRCode = QRCode }).ExecuteCommand();
                                    list =
                                        db.Queryable<T_BF_ProductInfo>()
                                            .Where(it => it.F_QRCode == QRCode)
                                            .OrderBy(it => it.F_Time, OrderByType.Desc)
                                            .ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    Dictionary<string, string> detaildic;
                                    if (detailstr == null)
                                        detaildic = new Dictionary<string, string>();
                                    else
                                        detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    detaildic["表型序号"] = "1";
                                    detailstr = JsonConvert.SerializeObject(detaildic);
                                    db.Updateable<T_BF_ProductInfo>()
                                        .UpdateColumns(it => it.F_ProductDetail == detailstr)
                                        .Where(it => it.F_QRCode == QRCode)
                                        .ExecuteCommand();
                                }
                            }
                            using (var db = DBHelper.GetInstance())//方案相关信息
                            {
                                var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                string detailstr = list[0].F_ProductDetail;
                                Dictionary<string, string> detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                var resultlist = db.Queryable<T_MeterModel>().Where(it => it.F_ID == Convert.ToInt32(detaildic["表型序号"])).ToList();
                                writevalue.Add(resultlist[0].F_ID);
                                writevalue.Add(1);
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_Model));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_GasTestID));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_GearTestID));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_TransCoffe));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_StandardID));
                            }
                            using (var db = DBHelper.GetInstance()) //初教结果
                            {
                                string chujiaoresult = "-1";
                                var resultlist =
                                    db.Queryable<T_TestResult>()
                                        .Where(it => it.F_FirstorSecond == 1 && it.F_SerialNum == QRCode)
                                        .OrderBy(it => it.F_TransTime, OrderByType.Desc)
                                        .ToList();
                                if (resultlist.Count > 0)//初校结果最新数据
                                {
                                    if (resultlist[0].F_FinalResult == "1")
                                        chujiaoresult = resultlist[0].F_FinalData;
                                    else chujiaoresult = "1000";
                                    //更新初校结果
                                    var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    Dictionary<string, string> detaildic;
                                    if (detailstr == null)
                                        detaildic = new Dictionary<string, string>();
                                    else
                                        detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    detaildic["二期初校结果"] = chujiaoresult;
                                    detailstr = JsonConvert.SerializeObject(detaildic);
                                    db.Updateable<T_BF_ProductInfo>()
                                       .UpdateColumns(it => it.F_ProductDetail == detailstr)
                                       .Where(it => it.F_QRCode == QRCode)
                                       .ExecuteCommand();
                                    //删除临时表数据
                                    db.Deleteable<T_TestResult>()
                                        .Where(it => it.F_FirstorSecond == 1 && it.F_SerialNum == QRCode)
                                        .ExecuteCommand();
                                }
                                else if (resultlist.Count <= 0)//产品信息表查找初校结果
                                {
                                    var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    if (detailstr == null)
                                        throw new Exception();
                                    Dictionary<string, string> detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    if (!detaildic.ContainsKey("二期初校结果"))
                                        throw new Exception();
                                    chujiaoresult = detaildic["二期初校结果"];
                                }
                                if (chujiaoresult != "-1")
                                    writevalue.Add(Convert.ToInt32(chujiaoresult));
                            }
                            f_eipclient.WriteTagData(writetag, 0, 48, writevalue);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLog("预读1下发错误", ex);
                            throw ex;
                        }
                        break;
                    #endregion
                    #region 预读2
                    case "yudu2":
                        try
                        {
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 124));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 126));
                            for (int i = 0; i < 5; i++) writevalue.Add(0);
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 108));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 110));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 112));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 114));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 116));
                            writevalue.Add(0);
                            writevalue.Add(0);
                            writevalue.Add(1);
                            writevalue.Add(1);
                            using (var db = DBHelper.GetInstance())//产品表 更新或插入测试方案序号
                            {
                                var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).ToList();
                                if (list.Count == 0)
                                {
                                    db.Insertable<T_BF_ProductInfo>(new { F_QRCode = QRCode }).ExecuteCommand();
                                    list =
                                        db.Queryable<T_BF_ProductInfo>()
                                            .Where(it => it.F_QRCode == QRCode)
                                            .OrderBy(it => it.F_Time, OrderByType.Desc)
                                            .ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    Dictionary<string, string> detaildic;
                                    if (detailstr == null)
                                        detaildic = new Dictionary<string, string>();
                                    else
                                        detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    detaildic["表型序号"] = "1";
                                    detailstr = JsonConvert.SerializeObject(detaildic);
                                    db.Updateable<T_BF_ProductInfo>()
                                        .UpdateColumns(it => it.F_ProductDetail == detailstr)
                                        .Where(it => it.F_QRCode == QRCode)
                                        .ExecuteCommand();
                                }
                            }
                            using (var db = DBHelper.GetInstance())//方案相关信息
                            {
                                var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                string detailstr = list[0].F_ProductDetail;
                                Dictionary<string, string> detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                var resultlist = db.Queryable<T_MeterModel>().Where(it => it.F_ID == Convert.ToInt32(detaildic["表型序号"])).ToList();
                                writevalue.Add(resultlist[0].F_ID);
                                writevalue.Add(1);
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_Model));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_GasTestID));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_GearTestID));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_TransCoffe));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_StandardID));
                            }
                            using (var db = DBHelper.GetInstance()) //初教结果
                            {
                                string chujiaoresult = "-1";
                                var resultlist =
                                    db.Queryable<T_TestResult>()
                                        .Where(it => it.F_FirstorSecond == 1 && it.F_SerialNum == QRCode)
                                        .OrderBy(it => it.F_TransTime, OrderByType.Desc)
                                        .ToList();
                                if (resultlist.Count > 0)//初校结果最新数据
                                {
                                    if (resultlist[0].F_FinalResult == "1")
                                        chujiaoresult = resultlist[0].F_FinalData;
                                    else chujiaoresult = "1000";
                                    //更新初校结果
                                    var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    Dictionary<string, string> detaildic;
                                    if (detailstr == null)
                                        detaildic = new Dictionary<string, string>();
                                    else
                                        detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    detaildic["二期初校结果"] = chujiaoresult;
                                    detailstr = JsonConvert.SerializeObject(detaildic);
                                    db.Updateable<T_BF_ProductInfo>()
                                       .UpdateColumns(it => it.F_ProductDetail == detailstr)
                                       .Where(it => it.F_QRCode == QRCode)
                                       .ExecuteCommand();
                                    //删除临时表数据
                                    db.Deleteable<T_TestResult>()
                                        .Where(it => it.F_FirstorSecond == 1 && it.F_SerialNum == QRCode)
                                        .ExecuteCommand();
                                }
                                else if (resultlist.Count <= 0)//产品信息表查找初校结果
                                {
                                    var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    if (detailstr == null)
                                        throw new Exception();
                                    Dictionary<string, string> detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    if (!detaildic.ContainsKey("二期初校结果"))
                                        throw new Exception();
                                    chujiaoresult = detaildic["二期初校结果"];
                                }
                                if (chujiaoresult != "-1")
                                    writevalue.Add(Convert.ToInt32(chujiaoresult));
                            }
                            f_eipclient.WriteTagData(writetag, 0, 48, writevalue);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLog("预读2下发错误", ex);
                            throw ex;
                        }
                        break;
                    #endregion
                    #region 装计数器1
                    case "zhuangjishuqi1":
                        try
                        {
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 124));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 126));
                            for (int i = 0; i < 5; i++) writevalue.Add(0);
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 108));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 110));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 112));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 114));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 116));
                            writevalue.Add(0);
                            writevalue.Add(0);
                            writevalue.Add(1);
                            writevalue.Add(1);
                            using (var db = DBHelper.GetInstance())//产品表 更新或插入测试方案序号
                            {
                                var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).ToList();
                                if (list.Count == 0)
                                {
                                    db.Insertable<T_BF_ProductInfo>(new { F_QRCode = QRCode }).ExecuteCommand();
                                    list =
                                        db.Queryable<T_BF_ProductInfo>()
                                            .Where(it => it.F_QRCode == QRCode)
                                            .OrderBy(it => it.F_Time, OrderByType.Desc)
                                            .ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    Dictionary<string, string> detaildic;
                                    if (detailstr == null)
                                        detaildic = new Dictionary<string, string>();
                                    else
                                        detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    detaildic["表型序号"] = "1";
                                    detailstr = JsonConvert.SerializeObject(detaildic);
                                    db.Updateable<T_BF_ProductInfo>()
                                        .UpdateColumns(it => it.F_ProductDetail == detailstr)
                                        .Where(it => it.F_QRCode == QRCode)
                                        .ExecuteCommand();
                                }
                            }
                            using (var db = DBHelper.GetInstance())//方案相关信息
                            {
                                var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                string detailstr = list[0].F_ProductDetail;
                                Dictionary<string, string> detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                var resultlist = db.Queryable<T_MeterModel>().Where(it => it.F_ID == Convert.ToInt32(detaildic["表型序号"])).ToList();
                                writevalue.Add(resultlist[0].F_ID);
                                writevalue.Add(1);
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_Model));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_GasTestID));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_GearTestID));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_TransCoffe));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_StandardID));
                            }
                            using (var db = DBHelper.GetInstance()) //初教结果
                            {
                                string chujiaoresult = "-1";
                                var resultlist =
                                    db.Queryable<T_TestResult>()
                                        .Where(it => it.F_FirstorSecond == 1 && it.F_SerialNum == QRCode)
                                        .OrderBy(it => it.F_TransTime, OrderByType.Desc)
                                        .ToList();
                                if (resultlist.Count > 0)//初校结果最新数据
                                {
                                    if (resultlist[0].F_FinalResult == "1")
                                        chujiaoresult = resultlist[0].F_FinalData;
                                    else chujiaoresult = "1000";
                                    //更新初校结果
                                    var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    Dictionary<string, string> detaildic;
                                    if (detailstr == null)
                                        detaildic = new Dictionary<string, string>();
                                    else
                                        detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    detaildic["二期初校结果"] = chujiaoresult;
                                    detailstr = JsonConvert.SerializeObject(detaildic);
                                    db.Updateable<T_BF_ProductInfo>()
                                       .UpdateColumns(it => it.F_ProductDetail == detailstr)
                                       .Where(it => it.F_QRCode == QRCode)
                                       .ExecuteCommand();
                                    //删除临时表数据
                                    db.Deleteable<T_TestResult>()
                                        .Where(it => it.F_FirstorSecond == 1 && it.F_SerialNum == QRCode)
                                        .ExecuteCommand();
                                }
                                else if (resultlist.Count <= 0)//产品信息表查找初校结果
                                {
                                    var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    if (detailstr == null)
                                        throw new Exception();
                                    Dictionary<string, string> detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    if (!detaildic.ContainsKey("二期初校结果"))
                                        throw new Exception();
                                    chujiaoresult = detaildic["二期初校结果"];
                                }
                                if (chujiaoresult != "-1")
                                    writevalue.Add(Convert.ToInt32(chujiaoresult));
                            }
                            f_eipclient.WriteTagData(writetag, 0, 48, writevalue);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLog("装计数器1下发错误", ex);
                            throw ex;
                        }
                        break;
                    #endregion
                    #region 装计数器2
                    case "zhuangjishuqi2":
                        try
                        {
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 124));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 126));
                            for (int i = 0; i < 5; i++) writevalue.Add(0);
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 108));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 110));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 112));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 114));
                            writevalue.Add(BitConverter.ToInt16(f_eipclient.galRecvBytesResult, 116));
                            writevalue.Add(0);
                            writevalue.Add(0);
                            writevalue.Add(1);
                            writevalue.Add(1);
                            using (var db = DBHelper.GetInstance())//产品表 更新或插入测试方案序号
                            {
                                var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).ToList();
                                if (list.Count == 0)
                                {
                                    db.Insertable<T_BF_ProductInfo>(new { F_QRCode = QRCode }).ExecuteCommand();
                                    list =
                                        db.Queryable<T_BF_ProductInfo>()
                                            .Where(it => it.F_QRCode == QRCode)
                                            .OrderBy(it => it.F_Time, OrderByType.Desc)
                                            .ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    Dictionary<string, string> detaildic;
                                    if (detailstr == null)
                                        detaildic = new Dictionary<string, string>();
                                    else
                                        detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    detaildic["表型序号"] = "1";
                                    detailstr = JsonConvert.SerializeObject(detaildic);
                                    db.Updateable<T_BF_ProductInfo>()
                                        .UpdateColumns(it => it.F_ProductDetail == detailstr)
                                        .Where(it => it.F_QRCode == QRCode)
                                        .ExecuteCommand();
                                }
                            }
                            using (var db = DBHelper.GetInstance())//方案相关信息
                            {
                                var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                string detailstr = list[0].F_ProductDetail;
                                Dictionary<string, string> detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                var resultlist = db.Queryable<T_MeterModel>().Where(it => it.F_ID == Convert.ToInt32(detaildic["表型序号"])).ToList();
                                writevalue.Add(resultlist[0].F_ID);
                                writevalue.Add(1);
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_Model));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_GasTestID));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_GearTestID));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_TransCoffe));
                                writevalue.Add(Convert.ToInt16(resultlist[0].F_StandardID));
                            }
                            using (var db = DBHelper.GetInstance()) //初教结果
                            {
                                string chujiaoresult = "-1";
                                var resultlist =
                                    db.Queryable<T_TestResult>()
                                        .Where(it => it.F_FirstorSecond == 1 && it.F_SerialNum == QRCode)
                                        .OrderBy(it => it.F_TransTime, OrderByType.Desc)
                                        .ToList();
                                if (resultlist.Count > 0)//初校结果最新数据
                                {
                                    if (resultlist[0].F_FinalResult == "1")
                                        chujiaoresult = resultlist[0].F_FinalData;
                                    else chujiaoresult = "1000";
                                    //更新初校结果
                                    var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    Dictionary<string, string> detaildic;
                                    if (detailstr == null)
                                        detaildic = new Dictionary<string, string>();
                                    else
                                        detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    detaildic["二期初校结果"] = chujiaoresult;
                                    detailstr = JsonConvert.SerializeObject(detaildic);
                                    db.Updateable<T_BF_ProductInfo>()
                                       .UpdateColumns(it => it.F_ProductDetail == detailstr)
                                       .Where(it => it.F_QRCode == QRCode)
                                       .ExecuteCommand();
                                    //删除临时表数据
                                    db.Deleteable<T_TestResult>()
                                        .Where(it => it.F_FirstorSecond == 1 && it.F_SerialNum == QRCode)
                                        .ExecuteCommand();
                                }
                                else if (resultlist.Count <= 0)//产品信息表查找初校结果
                                {
                                    var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == QRCode).OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
                                    string detailstr = list[0].F_ProductDetail;
                                    if (detailstr == null)
                                        throw new Exception();
                                    Dictionary<string, string> detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
                                    if (!detaildic.ContainsKey("二期初校结果"))
                                        throw new Exception();
                                    chujiaoresult = detaildic["二期初校结果"];
                                }
                                if (chujiaoresult != "-1")
                                    writevalue.Add(Convert.ToInt32(chujiaoresult));
                            }
                            f_eipclient.WriteTagData(writetag, 0, 48, writevalue);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLog("装计数器2下发错误", ex);
                            throw ex;
                        }
                        break;
                        #endregion
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 2;
                OnConnectStatusInfo(this, new ViewLogEventArgs(GetLogText("下发参数失败")));
            }
        }

        #endregion

        #region 回调记录日志
        private LogRecord GetLogText(string loginfo, bool okstatus = false)
        {
            f_logrcd.name = f_PlcSet.F_EqmDesc;
            f_logrcd.ip = f_PlcSet.F_EqmIP;
            f_logrcd.log = loginfo;
            f_logrcd.okstatus = okstatus;
            f_logrcd.time = DateTime.Now.ToString("F");
            return f_logrcd;
        }
        #endregion
        #region 解析  type 0.string 1.dint 2.int 3.real 4.int[] 5.intToreal 6.intTostring 7.intTorealEx
        private string ChangeToInfo(byte[] src, int lenB, int lenE, int type, int pointNum)  //type 0.string 1.dint 2.int 3.real 5.intToreal 6.intTostring
        {
            try
            {
                int typeas = type;
                if (f_PlcSet.F_PlcType == 1)  //slc500  int 数组解析
                {
                    if (typeas == 0) typeas = 6;
                    else if (typeas == 1) typeas = 2;  //只用前一int 后一int不用
                    else if (typeas == 3) typeas = 5;
                }
                string dst = "";
                switch (typeas)
                {
                    case 0:  //string
                        {
                            int len = BitConverter.ToInt32(src, lenB);
                            dst = Encoding.UTF8.GetString(src, lenB + 4, len); break;   //string  前4字节为string使用长度
                        }
                    case 1: dst = BitConverter.ToInt32(src, lenB).ToString(); break;    //dint
                    case 2: dst = BitConverter.ToInt16(src, lenB).ToString(); break;  //int
                    case 3: dst = BitConverter.ToSingle(src, lenB).ToString(); break;    //real
                    case 5:    //intToreal     前两位为整数  后两位为小数（除以10000） 
                        {
                            float dstf = 0f;
                            dstf = (float)BitConverter.ToInt16(src, lenB) + ((float)BitConverter.ToInt16(src, lenB + 2) / 10000);
                            string fpoint = "F" + pointNum.ToString();
                            dst = dstf.ToString(fpoint);
                        }
                        break;
                    case 6: dst = Encoding.Default.GetString(src, lenB, lenE); break;   //intTostring
                    case 7:  //inttorealEx  前两位为整数 后两位为除以的数
                        {
                            float dstf = 0f;
                            dstf = (float)BitConverter.ToInt16(src, lenB) / (float)BitConverter.ToInt16(src, lenB + lenE - 2);
                            dst = dstf.ToString();
                        }
                        break;

                }
                dst = dst.Replace('\0'.ToString(), string.Empty);
                dst = dst.Replace('\''.ToString(), string.Empty);
                return dst.Trim();
            }
            catch
            {
                return "";
            }
        }
        #endregion
    }
    #endregion

}
