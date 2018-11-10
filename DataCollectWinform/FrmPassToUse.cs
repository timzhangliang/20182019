/**
* 命名空间: DataCollectWinform 
* 类 名： FrmPassToUse
* 描 述：
*
* Ver      负责人        变更内容            变更日期
* ──────────────────────────────────────────────────────────────
* V1.0     张亮          初版                2018-10-17 9:36:47 
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
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;

namespace DataCollectWinform
{
    public partial class FrmPassToUse : DevExpress.XtraEditors.XtraForm
    {
        public FrmPassToUse()
        {
            InitializeComponent();
        }
        private bool CheckInput()   // 验证输入密码信息
        {

            if (txtPassNew.Text.Trim().Equals(""))
            {
                dxErrorProvider1.SetError(txtPassNew, "请输入确认密码");
                txtPassNew.Focus();
                return false;
            }
            if (txtPassNew.Text.Trim() !=ConfigHelper.GetAppConfig("password"))
            {
                MessageBox.Show("密码不正确，无法操作！");
                return false;
            }
            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)  // 密码
        {
            if (!CheckInput()) return;
            this.DialogResult = DialogResult.OK;
        }
    }
}
