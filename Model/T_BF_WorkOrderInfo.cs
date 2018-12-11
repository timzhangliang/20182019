/**
* 命名空间: Model 
* 类 名： T_BF_WorkOrderInfo
* 描 述：
*
* Ver      负责人        变更内容            变更日期
* ──────────────────────────────────────────────────────────────
* V1.0     张亮          初版                2018-11-30 10:19:46 
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
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace Model
{
    public class T_BF_WorkOrderInfo
    {
        [SugarColumn(Length = 40, IsNullable = false)]
        public string ID { get; set; }
        [SugarColumn(IsNullable = false)]
        public int Num { get; set; }
        [SugarColumn(Length = 40, IsNullable = false)]
        public string QRCodePre { get; set; }
        [SugarColumn(Length = 40, IsNullable = true)]
        public string WL { get; set; }
        [SugarColumn(Length = 10, IsNullable = true)]
        public string LineNo { get; set; }
        [SugarColumn(Length = 10, IsNullable = true)]
        public string Section { get; set; }
        [SugarColumn(Length = 40, IsNullable = true)]
        public string Note { get; set; }
        [SugarColumn(Length = Int32.MaxValue, IsNullable = true)]
        public string Detail { get; set; }
        [SugarColumn(IsNullable = false)]
        public DateTime? RecieveTime { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime? StartTime { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime? EndTime { get; set; }
    }
}
