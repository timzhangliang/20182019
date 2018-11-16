using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using Model;
using Newtonsoft.Json;
using SqlSugar;
using Newtonsoft.Json;

namespace DataCollectWinform
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            //for (int i = 0; i < 100000; i++)
            //{
            //    var db = DBHelper.GetInstance();
            //    try
            //    {
            //        var getByWhere = db.Queryable<T_BF_UserInfo>().Where(it => it.F_ID == 1).ToList();
            //        LogHelper.WriteLog("zhengque  " + DateTime.Now.ToString());
            //    }
            //    catch (Exception ex)
            //    {
            //        LogHelper.WriteLog("cuowu", ex);

            //        throw;
            //    }
            //}

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var db = DBHelper.GetInstance();
            T_EquipmentStatus entity = new T_EquipmentStatus();
            entity.F_EquipmentAlarm = 0;
            entity.F_EquipmentID = "0";
            entity.F_EquipmentStatus = 0;
            for (int i = 0; i < 500; i++)
            {
                db.Insertable(entity).ExecuteCommand();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 1; i <= 500; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(alter));
                t.Start(i);
            }

        }

        private void alter(object i)
        {
            try
            {
                var db = DBHelper.GetInstance();
                int j = 0;
                while (true)
                {
                    j++;
                    T_EquipmentStatus entity = new T_EquipmentStatus();
                    entity.F_EquipmentAlarm = 0;
                    entity.F_EquipmentID = "0";
                    entity.F_EquipmentStatus = j % 100;
                    entity.F_ID = Convert.ToInt32(i);
                    db.Updateable(entity).ExecuteCommand();
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {

                LogHelper.WriteLog("cuowu", ex);
                throw;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            int maxCount = 40000;
            List<SqlSugarClient> collection = new List<SqlSugarClient>();
            for (int i = 0; i < maxCount; i++)
            {
                Console.WriteLine(string.Format("成功创建连接对象{0}", i));
                var db = DBHelper.GetInstance();
                db.Open();
                collection.Add(db);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var db = DBHelper.GetInstance();
            List<string> field = db.Ado.SqlQuery<string>("select name from syscolumns Where ID=OBJECT_ID('T_BF_EqmStatusTag')");
            //string field = db.Ado.GetString("select name from syscolumns Where ID=OBJECT_ID('{0}')", "T_BF_EqmCurrentInfo");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString =" Data Source = localhost; Initial Catalog = DB_Main; Integrated Security = False; User ID = sa; Password = 19921023zl; Connect Timeout = 15; Encrypt = False; TrustServerCertificate = True; Pooling = true; Max Pool Size = 40000; Min Pool Size = 0; ", DbType = SqlSugar.DbType.SqlServer, IsAutoCloseConnection = true });
            //for (int i = 0; i < 1000; i++)
            //{
            //    db.Ado.ExecuteCommand("insert into T_BF_UserInfo values('chufa3','chufa','chufa',1,GETDATE(),'chufa','chufa')");
            //}
            //db.Ado.ExecuteCommand("insert into T_BF_UserInfo values('chufa3','chufa','chufa',1,GETDATE(),'chufa','chufa')");
            for (int i = 1; i <= 100; i++)
            {
                Thread t = new Thread(new ThreadStart(insert));
                t.Start();
            }
        }

        public void insert()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = " Data Source = localhost; Initial Catalog = DB_Main; Integrated Security = False; User ID = sa; Password = 19921023zl; Connect Timeout = 15; Encrypt = False; TrustServerCertificate = True; Pooling = true; Max Pool Size = 40000; Min Pool Size = 0; ", DbType = SqlSugar.DbType.SqlServer, IsAutoCloseConnection = true });
            for (int i = 0; i < 100; i++)
            {
                db.Ado.ExecuteCommand("insert into T_BF_UserInfo values('chufa3','chufa','chufa',1,GETDATE(),'chufa','chufa')");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = " Data Source = localhost; Initial Catalog = BMS; Integrated Security = False; User ID = sa; Password = 19921023zl; Connect Timeout = 15; Encrypt = False; TrustServerCertificate = True; Pooling = true; Max Pool Size = 40000; Min Pool Size = 0; ", DbType = SqlSugar.DbType.SqlServer, IsAutoCloseConnection = true });

            var dt = db.Ado.GetDataTable("select F_EqmTag from T_BF_EqmInfo where F_ID=4");
            var str = dt.Rows[0][0].ToString();
            Dictionary<string, string> obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(str);
            var mm = obj["read"];
            for (int i = 0; i < 300; i++)
            {
                // if (obj[i.ToString()] == null)
                {
                    obj[i.ToString()] = i.ToString();
                }
            }
            obj["0"] = "1111";
            string str2 = JsonConvert.SerializeObject(obj);
            db.Updateable<T_BF_EqmInfo>()
                .UpdateColumns(it => it.F_EqmTag == str2)
                .Where(it => it.F_ID == 4)
                .ExecuteCommand();
            //var mm = (Dictionary<string,string>)(obj);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = " Data Source = 172.16.1.251; Initial Catalog = MEIJIE; Integrated Security = False; User ID = sa; Password = QC@2018; Connect Timeout = 15; Encrypt = False; TrustServerCertificate = True; Pooling = true; Max Pool Size = 40000; Min Pool Size = 0; ", DbType = SqlSugar.DbType.SqlServer, IsAutoCloseConnection = true });

            var dt = db.Ado.GetDataTable("select * from T_GasTest ");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            var db = DBHelper.GetInstance();
            var dt = db.Queryable<T_MeterModel>().Where(it => it.F_ID == 1).ToList();
            var mm = dt[0];
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //try
            //{


            //    try
            //    {
            //        throw new Exception();
            //        var m = 2;
            //    }
            //    catch (Exception ex)
            //    {

            //        throw ex;
            //    }
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
            var db = DBHelper.GetInstance();
            var list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == "1111").ToList();
            if (list.Count == 0) db.Insertable<T_BF_ProductInfo>(new { F_QRCode = "1111" }).ExecuteCommand();
            list = db.Queryable<T_BF_ProductInfo>().Where(it => it.F_QRCode == "1111").OrderBy(it => it.F_Time, OrderByType.Desc).ToList();
            string detailstr = list[0].F_ProductDetail;
            Dictionary<string, string> detaildic;
            if (detailstr == null)
                detaildic = new Dictionary<string, string>();
            else
                detaildic = JsonConvert.DeserializeObject<Dictionary<string, string>>(detailstr);
            detaildic["表型序号"] = "3";
            bool flag = detaildic.ContainsKey("123");
            detailstr = JsonConvert.SerializeObject(detaildic);
            db.Updateable<T_BF_ProductInfo>()
               .UpdateColumns(it => it.F_ProductDetail == detailstr)
               .Where(it => it.F_QRCode == "1111")
               .ExecuteCommand();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = " Data Source = localhost; Initial Catalog = DB_Main; Integrated Security = False; User ID = sa; Password = 19921023zl; Connect Timeout = 15; Encrypt = False; TrustServerCertificate = True; Pooling = true; Max Pool Size = 40000; Min Pool Size = 0; ", DbType = SqlSugar.DbType.SqlServer, IsAutoCloseConnection = true });

            while (true)
            {
                var dt = db.Ado.GetDataTable($"insert into T_BF_UserGroupInfo values('1','{DateTime.Now.ToString()}') ");
                Thread.Sleep(3000);
            }
        }
        public void update()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = " Data Source = localhost; Initial Catalog = DBCMPS_QDWB; Integrated Security = False; User ID = sa; Password = 19921023zl; Connect Timeout = 15; Encrypt = False; TrustServerCertificate = True; Pooling = true; Max Pool Size = 40000; Min Pool Size = 0; ", DbType = SqlSugar.DbType.SqlServer, IsAutoCloseConnection = true });
            for (int i = 0; i < 100; i++)
            {
                db.Ado.ExecuteCommand($"update T_BF_EqmCurrentInfo set F005='{i}',F006='{i}',F007='{i}' where F_ID=1");
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            for (int i = 1; i <= 100; i++)
            {
                Thread t = new Thread(new ThreadStart(update));
                t.Start();
            }
        }
    }
}
