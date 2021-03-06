﻿/**
* 命名空间: Model 
* 类 名： T_MeterModel
* 描 述：
*
* Ver      负责人        变更内容            变更日期
* ──────────────────────────────────────────────────────────────
* V1.0     张亮          初版                2018-10-25 10:53:11 
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
    public class T_MeterModel
    {
        [SugarColumn(IsNullable = false)]
        public int F_ID { get; set; }
        [SugarColumn(Length = 1, IsNullable = false)]
        public string F_Direction { get; set; }
        [SugarColumn(IsNullable = false)]
        public int F_GasTestID { get; set; }
        [SugarColumn(IsNullable = false)]
        public int F_GearTestID { get; set; }
        [SugarColumn(Length = 10, IsNullable = false)]
        public string F_TransCoffe { get; set; }
        [SugarColumn(IsNullable = false)]
        public int F_StandardID { get; set; }
        [SugarColumn(Length = 40, IsNullable = false)]
        public string F_Model { get; set; }

    }
}
