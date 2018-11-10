/**
* 命名空间: Model 
* 类 名： T_TestData
* 描 述：
*
* Ver      负责人        变更内容            变更日期
* ──────────────────────────────────────────────────────────────
* V1.0     张亮          初版                2018-10-25 13:44:48 
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
    public class T_TestData
    {
        [SugarColumn(IsNullable = false)]
        public int F_FirstorSecond { get; set; }
        [SugarColumn(IsNullable = false)]
        public int F_EquipNum { get; set; }
        [SugarColumn(IsNullable = false)]
        public int F_StationNum { get; set; }
        [SugarColumn(IsNullable = false)]
        public DateTime? F_TransTime { get; set; }
        [SugarColumn(Length = 40, IsNullable = false)]
        public string F_SerialNum { get; set; }
        [SugarColumn(Length = 10, IsNullable = false)]
        public string F_FlowPoint { get; set; }
        [SugarColumn(IsNullable = false)]
        public int F_Times { get; set; }
        [SugarColumn(Length = 10, IsNullable = false)]
        public string F_FlowVolumn { get; set; }
        [SugarColumn(Length = 10, IsNullable = false)]
        public string F_Error { get; set; }
        [SugarColumn(Length = 10, IsNullable = false)]
        public string F_PressureLossValue { get; set; }
        [SugarColumn(Length = 10, IsNullable = false)]
        public string F_AtmoPressure { get; set; }
        [SugarColumn(Length = 10, IsNullable = false)]
        public string F_EnvirTemp { get; set; }
        [SugarColumn(Length = 10, IsNullable = false)]
        public string F_RelatHumidity { get; set; }
        [SugarColumn(Length = 10, IsNullable = false)]
        public string F_DiffValue { get; set; }
    }
}
