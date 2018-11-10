﻿/**
* 命名空间: Model 
* 类 名： T_BF_Classes
* 描 述：
*
* Ver      负责人        变更内容            变更日期
* ──────────────────────────────────────────────────────────────
* V1.0     张亮          初版                2018-09-27 11:14:27 
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
    public class T_BF_Classes
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int F_ID { get; set; }
        [SugarColumn(Length = 50, IsNullable = false)]
        public string F_ClassName { get; set; }
        [SugarColumn(Length = 10, IsNullable = false)]
        public string F_BeginTime { get; set; }
        [SugarColumn(Length = 10, IsNullable = false)]
        public string F_FinishTime { get; set; }
        [SugarColumn(Length = 50, IsNullable = true)]
        public string F_LastEditor { get; set; }
        [SugarColumn(Length = 30, IsNullable = true)]
        public string F_LastEditTime { get; set; }
        [SugarColumn(Length = 200, IsNullable = true)]
        public string F_Comment { get; set; }
    }
}
