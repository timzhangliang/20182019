/**
* 命名空间: Model 
* 类 名： EqmAlarmInfo
* 描 述：
*
* Ver      负责人        变更内容            变更日期
* ──────────────────────────────────────────────────────────────
* V1.0     张亮          初版                2018-12-03 16:12:17 
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
    public class EqmAlarmInfo
    {
        [SugarColumn(IsNullable = false)]
        public DateTime? Time { get; set; }
        [SugarColumn(IsNullable = true)]
        public int Line { get; set; }
        [SugarColumn(Length = 20, IsNullable = false)]
        public string IP { get; set; }
        [SugarColumn(Length = 20, IsNullable = false)]
        public string EqmCode { get; set; }
        [SugarColumn(Length = 100, IsNullable = true)]
        public string AlarmName { get; set; }
        [SugarColumn(IsNullable = false)]
        public int AlarmType { get; set; }
    }
}
