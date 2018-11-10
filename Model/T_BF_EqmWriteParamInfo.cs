/**
* 命名空间: Model 
* 类 名： T_BF_EqmWriteParamInfo
* 描 述：
*
* Ver      负责人        变更内容            变更日期
* ──────────────────────────────────────────────────────────────
* V1.0     张亮          初版                2018-09-27 11:38:55 
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
    public class T_BF_EqmWriteParamInfo
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int F_ID { get; set; }
        [SugarColumn(Length = 30, IsNullable = true)]
        public string F_ServiceName { get; set; }
        [SugarColumn(Length = 60, IsNullable = true)]
        public string F_ParamGroup { get; set; }
        [SugarColumn(Length = 60, IsNullable = true)]
        public string F_ParamName { get; set; }
        [SugarColumn(Length = 30, IsNullable = true)]
        public string F_ParamValue { get; set; }
        [SugarColumn(Length = 200, IsNullable = true)]
        public string F_ParamDesc { get; set; }
        [SugarColumn( IsNullable = true)]
        public int F_TagStoreAddr { get; set; }
        [SugarColumn(IsNullable = true)]
        public int F_TagStoreEddr { get; set; }
        [SugarColumn(IsNullable = true)]
        public int F_TagAddrLen { get; set; }
        [SugarColumn(IsNullable = true)]
        public short F_TagType { get; set; }
        [SugarColumn(IsNullable = true)]
        public short F_ParamIndex { get; set; }
        [SugarColumn(IsNullable = true)]
        public short F_ParamType { get; set; }
        [SugarColumn(IsNullable = true)]
        public short F_PlcType { get; set; }
    }
}
