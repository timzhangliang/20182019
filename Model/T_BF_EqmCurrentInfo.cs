/**
* 命名空间: Model 
* 类 名： T_BF_EqmCurrentInfo
* 描 述：
*
* Ver      负责人        变更内容            变更日期
* ──────────────────────────────────────────────────────────────
* V1.0     张亮          初版                2018-12-03 13:37:52 
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
    public class T_BF_EqmCurrentInfo
    {
        [SugarColumn(IsNullable = true)]
        public int F_ID { get; set; }
        [SugarColumn(IsNullable = true)]
        public short F_ProductLine { get; set; }
        [SugarColumn(IsNullable = true)]
        public short F_EqmLine { get; set; }
        [SugarColumn(Length = 20, IsNullable = false)]
        public string F_EqmIP { get; set; }
        [SugarColumn(Length = 20, IsNullable = false)]
        public string F_EqmCode { get; set; }
        [SugarColumn(Length = 60, IsNullable = true)]
        public string F_EqmDesc { get; set; }
        [SugarColumn(Length = 30, IsNullable = true)]
        public string ModelName { get; set; }
        [SugarColumn(Length = 15, IsNullable = true)]
        public string F_EqmNum { get; set; }
        [SugarColumn(IsNullable = true)]
        public int Online { get; set; }
        [SugarColumn(IsNullable = true)]
        public int Status { get; set; }
        [SugarColumn(IsNullable = true)]
        public int Running { get; set; }
        [SugarColumn(IsNullable = true)]
        public int Alarm { get; set; }
        [SugarColumn(IsNullable = true)]
        public int Enabled { get; set; }
        [SugarColumn(IsNullable = true)]
        public int CountOK { get; set; }
        [SugarColumn(IsNullable = true)]
        public int CountTotal { get; set; }
        [SugarColumn(IsNullable = true)]
        public int CycleTime { get; set; }
        [SugarColumn(IsNullable = true)]
        public int PlanCount { get; set; }
        [SugarColumn(IsNullable = true)]
        public int ModelID { get; set; }[SugarColumn(Length = 30, IsNullable = true)]
        public string WorkOrderID { get; set; }
        [SugarColumn(Length = 40, IsNullable = true)]
        public string ProductID { get; set; }
        [SugarColumn(IsIgnore= true)]
        public int Updated { get; set; }
        }
}
