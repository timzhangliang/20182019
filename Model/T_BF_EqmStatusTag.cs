/**
* 命名空间: Model 
* 类 名： T_BF_EqmStatusTag
* 描 述：
*
* Ver      负责人        变更内容            变更日期
* ──────────────────────────────────────────────────────────────
* V1.0     张亮          初版                2018-09-27 10:57:01 
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
    public class T_BF_EqmStatusTag
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int F_ID { get; set; }
        [SugarColumn(Length = 30, IsNullable = false)]
        public string F_TagName { get; set; }
        [SugarColumn(Length = 60, IsNullable = true)]
        public string F_TagDesc { get; set; }
        [SugarColumn(IsNullable = true)]
        public short F_TagKeyType { get; set; }
        [SugarColumn(IsNullable = true)]
        public int F_TagStoreAddr { get; set; }
        [SugarColumn(IsNullable = true)]
        public int F_TagAdrrLen { get; set; }
        [SugarColumn(IsNullable = true)]
        public short F_TagType { get; set; }
        [SugarColumn(Length = 20,IsNullable = false)]
        public string F_DataField { get; set; }
    }
}
