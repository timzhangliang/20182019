/**
* 命名空间: $rootnamespace$ 
* 类 名： Class1
* 描 述：
*
* Ver      负责人        变更内容            变更日期
* ──────────────────────────────────────────────────────────────
* V1.0     张亮          初版                2018-09-04 15:18:02 
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
    public class T_BF_UserInfo
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int F_ID { get; set; }
        [SugarColumn(Length = 50)]
        public string F_UserName { get; set; }
        [SugarColumn(Length = 50)]
        public string F_Password { get; set; }
        [SugarColumn(IsNullable = true)]
        public int F_GroupID { get; set; }
        [SugarColumn(Length = 30)]
        public string F_ActionTime { get; set; }
        [SugarColumn(Length = 200, IsNullable = true)]
        public string F_Comment { get; set; }
        [SugarColumn(IsNullable = false)]
        public bool F_IsDeleted { get; set; }
    }
}
