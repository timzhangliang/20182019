/**
* 命名空间: Model 
* 类 名： T_BF_ProductInfo
* 描 述：
*
* Ver      负责人        变更内容            变更日期
* ──────────────────────────────────────────────────────────────
* V1.0     张亮          初版                2018-11-10 10:35:30 
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
    public class T_BF_ProductInfo
    {
        [SugarColumn(IsNullable = true)]
        public DateTime? F_Time { get; set; }
        [SugarColumn(Length = 40, IsNullable = true)]
        public string F_QRCode { get; set; }
        [SugarColumn(Length = 30, IsNullable = true)]
        public string F_ModelName { get; set; }
        [SugarColumn(Length = 30, IsNullable = true)]
        public string F_RegularCode { get; set; }
        [SugarColumn(Length = 40, IsNullable = true)]
        public string F_CoreCode { get; set; }
        [SugarColumn(IsNullable = true)]
        public int F_ProductOK { get; set; }
        [SugarColumn(Length = int.MaxValue, IsNullable = true)]
        public string F_ProductDetail { get; set; }

    }
}
