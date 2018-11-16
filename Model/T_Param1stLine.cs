/**
* 命名空间: Model 
* 类 名： T_Param1stLine
* 描 述：
*
* Ver      负责人        变更内容            变更日期
* ──────────────────────────────────────────────────────────────
* V1.0     张亮          初版                2018-11-16 13:22:19 
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
    public class T_Param1stLine
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int F_ID { get; set; }
        [SugarColumn(Length = 20, IsNullable = false)]
        public string F_EquipType { get; set; }
        [SugarColumn(Length = 10, IsNullable = true)]
        public string Param1 { get; set; }
        [SugarColumn(Length = 10, IsNullable = true)]
        public string Param2 { get; set; }
        [SugarColumn(Length = 10, IsNullable = true)]
        public string Param3 { get; set; }
        [SugarColumn(Length = 10, IsNullable = true)]
        public string Param4 { get; set; }
        [SugarColumn(Length = 10, IsNullable = true)]
        public string Param5 { get; set; }
        [SugarColumn(Length = 10, IsNullable = true)]
        public string Param6 { get; set; }
        [SugarColumn(Length = 10, IsNullable = true)]
        public string Param7 { get; set; }
        [SugarColumn(Length = 10, IsNullable = true)]
        public string Param8 { get; set; }
        [SugarColumn(Length = 10, IsNullable = true)]
        public string Param9 { get; set; }
        [SugarColumn(Length = 10, IsNullable = true)]
        public string Param10 { get; set; }
    }
}
