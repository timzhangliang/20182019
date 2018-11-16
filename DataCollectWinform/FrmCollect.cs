/**
* 命名空间: DataCollectWinform 
* 类 名： FrmCollect
* 描 述：
*
* Ver      负责人        变更内容            变更日期
* ──────────────────────────────────────────────────────────────
* V1.0     张亮          初版                2018-10-16 17:54:20 
*
* Copyright (c) 2018 724pride Corporation. All rights reserved. 
*┌──────────────────────────────────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．       │
*│　版权所有：中船重工鹏力（南京）智能装备系统有限公司　　　　　　　　　　　　　　              │
*└──────────────────────────────────────────────────────────────┘
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using S7Func;
namespace DataCollectWinform
{
    public partial class FrmCollect : Form
    {
        private delegate void SetListBoxCallBack(LogRecord str);
        private SetListBoxCallBack MsgPingCallBack;   //服务端或客户端连接状态信息委托
        private SetListBoxCallBack MsgEqmInfoCallBack;
        public PrePareForCollect f_collect = new PrePareForCollect();

        private DateTime start;
        public FrmCollect()
        {
            InitializeComponent();
            MsgPingCallBack = new SetListBoxCallBack(PingLogMsg);
            PingComputer.OnPingnotConnected += new ViewEventHandler(Ping_OnConnectLogEvent);

            MsgEqmInfoCallBack = new SetListBoxCallBack(EqmInfoLogMsg);
            CollectData.OnConnectStatusInfo += new ViewEventHandler(EqmInfo_OnConnectLogEvent);

            listEqmInfo.View = View.Details;//设置视图
            listEqmInfo.Columns.Add("设备编号", 140, HorizontalAlignment.Left);
            listEqmInfo.Columns.Add("IP地址", 140, HorizontalAlignment.Left);
            listEqmInfo.Columns.Add("状态", 140, HorizontalAlignment.Left);
            listEqmInfo.Columns.Add("更新时间", 200, HorizontalAlignment.Left);
        }

        private void FrmCollect_FormClosing(object sender, FormClosingEventArgs e)
        {
            FrmPassToUse frm = new FrmPassToUse();
            if (frm.ShowDialog() != DialogResult.OK)
            {
                e.Cancel = true;
            }
        }

        private void TsBTnStart_Click(object sender, EventArgs e)
        {
            string msg = "";
            msg = f_collect.CreateAllThreadToCollect();
            if (msg.Equals(""))
            {
                TsBTnStart.Visible = false;
                TsBtnClose.Visible = true;
                start = DateTime.Now;
                timer1.Enabled = true;
            }
            else
            {
                MessageBox.Show(msg);
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string time = Convert.ToString(DateTime.Now - start);
            int flag = time.LastIndexOf(':');
            TbTime.Text = time.Substring(0, flag+3);
        }

        private void TsBtnClose_Click(object sender, EventArgs e)
        {
            FrmPassToUse frm = new FrmPassToUse();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                if (f_collect != null)
                {
                    f_collect.CloseAllThread();
                    TsBTnStart.Visible = true;
                    TsBtnClose.Visible = false;
                    timer1.Enabled = false;
                    TbTime.Text = null;
                    listEqmInfo.Items.Clear();
                }
            }

        }

        private void Ping_OnConnectLogEvent(object sender, ViewLogEventArgs e)  //ping通测试回调委托事件
        {
            try
            {
                listEqmInfo.Invoke(MsgPingCallBack, e._str);
            }
            catch
            {

            }
        }
        private void PingLogMsg(LogRecord message)
        {

            //int flag = 0;
            //for (int i = 0; i < listEqmInfo.Items.Count; i++)
            //{
            //    if (listEqmInfo.Items[i].SubItems[0].Text == message.name)
            //    {
            //        flag++;
            //        listEqmInfo.Items[i].SubItems[2].Text = message.log;
            //        listEqmInfo.Items[i].SubItems[3].Text = message.time;
            //        if (message.log != "正常")
            //        {
            //            listEqmInfo.Items[i].BackColor = Color.FromArgb(255, 200, 200);
            //        }
            //    }
            //}
            //if (flag == 0)
            //{
            //    ListViewItem item = new ListViewItem(message.name);
            //    item.SubItems.Add(message.ip);
            //    item.SubItems.Add(message.log);
            //    item.SubItems.Add(message.time);
            //    item.EnsureVisible();
            //    listEqmInfo.Items.Insert(0, item);
            //    if (message.log != "正常")
            //    {
            //        listEqmInfo.Items[0].BackColor = Color.FromArgb(255, 200, 200);
            //    }
            //}

        }

        private void EqmInfo_OnConnectLogEvent(object sender, ViewLogEventArgs e)//设备数据接收成功回调委托事件
        {
            try
            {
                listEqmInfo.Invoke(MsgEqmInfoCallBack, e._str);
            }
            catch
            {

            }
        }

        private void EqmInfoLogMsg(LogRecord message)
        {
            RefreshList(listEqmInfo, message);
        }
        private void RefreshList(ListView list, LogRecord message)
        {
            int flag = 0;
            for (int i = 0; i < listEqmInfo.Items.Count; i++)
            {
                if (list.Items[i].SubItems[0].Text == message.name)
                {
                    flag++;
                    list.Items[i].SubItems[2].Text = message.log;
                    list.Items[i].SubItems[3].Text = message.time;
                    if (message.log != "正常")
                    {
                        listEqmInfo.Items[i].BackColor = Color.FromArgb(255, 200, 200);
                    }
                    else
                    {
                        listEqmInfo.Items[i].BackColor = Color.FromArgb(255, 255, 255);
                    }
                }
            }
            if (flag == 0)
            {
                ListViewItem item = new ListViewItem(message.name);
                item.SubItems.Add(message.ip);
                item.SubItems.Add(message.log);
                item.SubItems.Add(message.time);
                item.EnsureVisible();
                listEqmInfo.Items.Insert(0, item);
                if (message.log != "正常")
                {
                    listEqmInfo.Items[0].BackColor = Color.FromArgb(255, 200, 200);
                }
                else
                {
                    listEqmInfo.Items[0].BackColor = Color.FromArgb(255, 255, 255);
                }
            }
        }

        private void FrmCollect_Load(object sender, EventArgs e)
        {
            string msg = "";
            msg = f_collect.CreateAllThreadToCollect();
            if (msg.Equals(""))
            {
                TsBTnStart.Visible = false;
                TsBtnClose.Visible = true;
                start = DateTime.Now;
                timer1.Enabled = true;
            }
            else
            {
                MessageBox.Show(msg);
            }
        }
    }
}
