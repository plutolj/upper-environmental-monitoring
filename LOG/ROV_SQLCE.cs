using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using ROV_SCU_V1._00._4._0;
using WFA_AUTO_oo;

namespace ROV_SQLCE
{
    /// <summary>
    /// ni
    /// </summary>
    public class SQLServerCE
    {
        /// <summary>
        /// name
        /// </summary>
        protected string SqlCeFilename = "";
        /// <summary>
        /// link
        /// </summary>
        protected SqlCeConnection myConn = null;
        /// <summary>
        /// 创建SqlCeBD
        /// </summary>
        /// <param name="filename">*.sdf文件名</param>
        /// <returns>创建成功否</returns>
        protected bool CreatSqlCeFile(string filename)
        {
            if (File.Exists(filename))
            {
                string[] str = filename.Split('.');
                if (str.Length == 2)
                {
                    filename = str[0] + "("
                        + DateTime.Now.Year.ToString("0000")
                        + DateTime.Now.Day.ToString("00")
                        + DateTime.Now.Hour.ToString("00")
                        + DateTime.Now.Minute.ToString("00")
                        + DateTime.Now.Second.ToString("00")
                        + ")" + "." + str[1];
                    MessageBox.Show("文件已经存在！将使用文件名如下：\n"
                                    + filename);

                }
                else
                {
                    MessageBox.Show("文件名字不符合要求！");
                    return false;
                }

                //myConn.Open();


            }
            SqlCeEngine engine = new SqlCeEngine("Data Source = " + @filename);
            engine.CreateDatabase();
            myConn = new SqlCeConnection("Data Source = " + @filename);
            try
            {
                myConn.Open();
            }
            catch (SqlCeException e)
            {
                MessageBox.Show(e.Message);
                myConn = null;
                return false;
            }
            SqlCeFilename = filename;
            return true;
        }

        protected bool OpenSqlCeFile(string filename)
        {
            myConn = new SqlCeConnection("Data Source = " + @filename);
            try
            {
                myConn.Open();
            }
            catch (SqlCeException e)
            {
                MessageBox.Show(e.Message);
                myConn = null;
                return false;
            }
            SqlCeFilename = filename;
            return true;
        }
        /// <summary>
        /// 添加新的表格
        /// </summary>
        /// <param name="CommandText">"CREATE TABLE TestTbl(col1 int PRIMARY KEY, col2 ntext, col3 money)";</param>
        protected void AddNewTable(string CommandText)
        {
            if (myConn == null) return;
            SqlCeCommand cmd = myConn.CreateCommand();
            cmd.CommandText = CommandText;// "CREATE TABLE TestTbl(col1 int PRIMARY KEY, col2 ntext, col3 money)";
            cmd.ExecuteNonQuery();
            //cmd.Parameters

        }
        /// <summary>
        /// 添加新的行
        /// </summary>
        /// <param name="CommandText"> "INSERT INTO TestTbl(col1, col2, col3) VALUES (0, 'abc', 15.66)";</param>
        protected void AddTableRows(string CommandText)
        {
            SqlCeCommand cmd = myConn.CreateCommand();
            cmd.CommandText = CommandText;// "INSERT INTO TestTbl(col1, col2, col3) VALUES (0, 'abc', 15.66)";
            cmd.ExecuteNonQuery();

        }
        /// <summary>
        /// 获取表格
        /// </summary>
        /// <param name="CommandText">"select * from TestTbl ";</param>
        /// <returns></returns>
        protected DataTable GetDatTable(string CommandText, string TableName)
        {
            // SqlCeConnection mySqlConnection = new SqlCeConnection(connectionString);
            //创建Command对象  TableName
            SqlCeCommand mySqlCommand = myConn.CreateCommand();
            string sqlStatement = CommandText;// "select * from TestTbl ";
            //设置CommandText属性  
            mySqlCommand.CommandText = sqlStatement;
            //设置CommandTimeout属性  
            // mySqlCommand.CommandTimeout = 20;  
            //生成SqlDataAdapter对象  
            SqlCeDataAdapter mySqlDataAdapter = new SqlCeDataAdapter();
            //设置SelectCommand属性  
            mySqlDataAdapter.SelectCommand = mySqlCommand;
            //生成DataSet对象，存储Select语句的结果  
            DataSet myDataSet = new DataSet();
            // mySqlDataAdapter.  
            //打开数据库  
            //myConn.Open();
            //用SqlDataAdapter对象的Fill()方法从表中取行  
            //其中stuInfo是新起的表名，以存储结果  
            mySqlDataAdapter.Fill(myDataSet, TableName);
            //关闭数据库  
            //myConn.Close();

            //用tables属性取得指定的表  
            return myDataSet.Tables[TableName];
        }




        public long GetBdSize()
        {
            string dirPath = this.SqlCeFilename;
            long len = 0; //判断该路径是否存在（是否为文件夹） 
            if (!Directory.Exists(dirPath))
            {
                //定义一个FileInfo对象，是指与filePath所指向的文件相关联，以获取其大小 
                FileInfo fileInfo = new FileInfo(dirPath);
                len = fileInfo.Length;

            }
            else
            {
                ////定义一个DirectoryInfo对象 
                //DirectoryInfo di = new DirectoryInfo(dirPath);   
                ////通过GetFiles方法，获取di目录中的所有文件的大小 
                //foreach (FileInfo fi in di.GetFiles()) 
                //{ 
                //    len += fi.Length; 
                //} 
                ////获取di中所有的文件夹，并存到一个新的对象数组中，以进行递归 
                //DirectoryInfo[] dis = di.GetDirectories(); 
                //if (dis.Length > 0)
                //{ 
                //    for (int i = 0; i < dis.Length; i++)
                //    { 
                //        len += GetDirectoryLength(dis[i].FullName); 
                //    } 
                //} 
                return 0;
            }
            return len;
        }
    }

    /// <summary>
    /// ROV日志数据库
    /// </summary>
    public class SqlCe_ROV : SQLServerCE
    {

        public const int nCKT_SWITCHS = 50;
        public const int nCKT_ANALOG = 100;
        public const int nSENSERS = 100;
        public const int nAlARM = 50;
        public int nloadingData = 0;
        /// <summary>
        /// 创建一个定义好的数据库及表格
        /// </summary>
        /// <param name="_filename">*.sdf文件名(含路径)</param>
        /// <param name="Control">open or creat</param>
        public SqlCe_ROV(string _filename, string Control)
        {
            this.myConn = null;
            if (Control == "open")
            {
                this.OpenSqlCeFile(_filename);
            }
            else if (Control == "creat")
            {
                this.CreatSqlCeFile(_filename);//UNIQE
                //this.AddNewTable(
                //               "CREATE TABLE ROV_ALLDAT(Time datetime PRIMARY KEY ,RunTime numeric(8,2) Unique NOT NULL,CKT_SWITCHS binary(" + nCKT_SWITCHS.ToString() + ")"
                //               + ",CKT_ANALOG binary(" + nCKT_ANALOG.ToString() + "),SENSERS binary(" + nSENSERS.ToString() + ")"
                //               + ")"
                //               );
                this.AddNewTable(
                             "CREATE TABLE ROV_ALLDAT(NUM int PRIMARY KEY,Time datetime  Unique NOT NULL ,RunTime numeric(12,6) Unique NOT NULL,"
                             + "JYW_ID int,JD_ID int,Temp int,"
                             + "Humidity int ,PM int "//Light1 int,
                             //+ "Acc_x numeric(8,5),Acc_y numeric(8,5),Acc_z numeric(8,5),"
                             //+ "Gry_x numeric(9,6),Gry_y numeric(9,6),Gry_z numeric(9,6),"
                             //+ "Vx numeric(7,3),Vy numeric(7,3),Vz numeric(7,3),"
                             //+ "Rev1 int,Rev2 int,Rev3 int,Rev4 int"
                             + ")"
                             );
                //this.AddNewTable(
                //                "CREATE TABLE ROV_MarkT(Time datetime PRIMARY KEY ,RunTime numeric(8,2) Unique NOT NULL,AlARM binary(" + nAlARM.ToString() + ")"
                //                + ")"
                //                );
                //this.AddNewTable(
                //                "CREATE TABLE ROV_ALARM(Time datetime PRIMARY KEY ,RunTime numeric(8,2) Unique NOT NULL,AlARM binary(" + nAlARM.ToString() + ")"
                //                + ")"
                //                );
                //this.AddNewTable(
                //                "CREATE TABLE CF_YP_Information(CF_Number nvarchar(100) PRIMARY KEY  ,YP_AID nvarchar(100) Unique NOT NULL,"
                //                + "YP_name nvarchar(50) NOT NULL,YP_danweijiliang numeric(7,2) NOT NULL,YP_shuliang numeric(8,2) NOT NULL,YP_danjia money,"
                //                + "YP_Pihao nvarchar(100),YP_Changjia nvarchar(100),YP_dateTime datetime,"
                //                + "foreign key(CF_Number) references CF_Information(CF_ID)"
                //    // + "PRIMARY KEY(CF_Number2),"
                //    // + "FOREIGN KEY(CF_Number2) REFERENCES CF_Information(CF_Number),"
                //                + ")"
                //                );
                // PRIMARY KEY( EmployeeID ),
                // FOREIGN KEY (SkillID) REFERENCES Skills ( Id )

                //SqlCeCommand cmd = myConn.CreateCommand();
                //cmd.CommandText = "ALTER TABLE CF_YP_Information ADD FOREIGN KEY CF_YP_Information ( CF_Number ) REFERENCES CF_Information ( CF_Number )";// "INSERT INTO TestTbl(col1, col2, col3) VALUES (0, 'abc', 15.66)";
                //cmd.ExecuteNonQuery();
                //             ALTER TABLE EmployeeSkills2
                //ADD FOREIGN KEY SkillFK ( SkillID )
                //REFERENCES Skills2 ( ID );
            }
            else
            {
                return;
            }
            SqlCeFilename = _filename;

        }

        /// <summary>
        /// 关闭数据库
        /// </summary>
        public void CloseSqlCe()
        {
            if (this.myConn == null)
            {
                this.myConn.Close();
            }
        }

        string bytes2HexSrt(byte[] bytes)
        {
            string hexstr = "";
            foreach (byte InByte in bytes)
            {
                hexstr += String.Format("{0:X2} ", InByte);
            }  
             return hexstr;
        }
     
        
        /// <summary>
        /// 添加所有数据多行
        /// </summary>
        /// <param name="RLDs">所添加数据</param>
        /// <returns></returns>
        public int AddAllDatas(MyLOG_DATAs[] RLDsLines)
        {
            int reanswer = 0;
            DataTable dts = new DataTable();
            dts.Columns.AddRange(new DataColumn[] { 
                new DataColumn("NUM",typeof(int)),
                new DataColumn("Time",typeof(DateTime)),
                new DataColumn("RunTime",typeof(double)),

                new DataColumn("JYW_ID",typeof(int)),
                new DataColumn("JD_ID",typeof(int)),
                new DataColumn("Temp",typeof(int)),

                new DataColumn("Humidity",typeof(int )),
                new DataColumn("Light0",typeof(int )),
            //    new DataColumn("Light1",typeof(int)),

                //new DataColumn("Acc_x",typeof(double)),
                //new DataColumn("Acc_y",typeof(double)),
                //new DataColumn("Acc_z",typeof(double)),

                //new DataColumn("Gry_x",typeof(double)),
                //new DataColumn("Gry_y",typeof(double)),
                //new DataColumn("Gry_z",typeof(double)),

                //new DataColumn("Vx",typeof(float)),
                //new DataColumn("Vy",typeof(float)),
                //new DataColumn("Vz",typeof(float)),

                //new DataColumn("Rev1",typeof(int)),
                //new DataColumn("Rev2",typeof(int)),
                //new DataColumn("Rev3",typeof(int)),
                //new DataColumn("Rev4",typeof(int))
            });
            foreach (MyLOG_DATAs RLDs in RLDsLines)
            {
                DataRow r = dts.NewRow();
                r[0] = RLDs.NUM;
                r[1] = RLDs.Time;
                r[2] = RLDs.RunTime;

                r[3] = RLDs.logData.JYW_ID;
                r[4] = RLDs.logData.JD_ID;
                r[5] = RLDs.logData.Temperture;

                r[6] = RLDs.logData.Humidity;
                r[7] = RLDs.logData.PM;
                //r[8] = RLDs.Altitude;

                //r[9] = RLDs.Ax;
                //r[10] = RLDs.Ay;
                //r[11] = RLDs.Az;

                //r[12] = RLDs.Gx;
                //r[13] = RLDs.Gy;
                //r[14] = RLDs.Gz;

                //r[15] = RLDs.Vx;
                //r[16] = RLDs.Vy;
                //r[17] = RLDs.Vz;
                
                dts.Rows.Add(r); 
            }    
            SqlCeCommand cmd = myConn.CreateCommand();
            SqlCeDataAdapter myAdapter = new SqlCeDataAdapter(cmd);
            SqlCeCommandBuilder myCommandBuilder = new SqlCeCommandBuilder(myAdapter);
            myAdapter.InsertCommand = myCommandBuilder.GetInsertCommand();
          //  cmd.CommandText = CommandText;// "INSERT INTO TestTbl(col1, col2, col3) VALUES (0, 'abc', 15.66)";
           // cmd.ExecuteNonQuery();
           // cmd.Parameters.AddRange(paras);
            try
            {
            //    this.AddTableRows(CommandText);
            //    reanswer=cmd.ExecuteNonQuery();
                foreach(DataRow dr in dts.Rows)
                {
                    dr.SetAdded();
                }
                myAdapter.Update(dts);//, "ROV_ALLDAT"
                dts.AcceptChanges(); 
                reanswer = 1;
            }
            catch (SqlCeException cee)
            {
                MessageBox.Show(cee.Message);
                reanswer = -1;
            }
            #region
            //CommandText = "INSERT INTO CF_YP_Information (CF_Number,YP_AID,YP_name,YP_danweijiliang,YP_shuliang,YP_danjia,YP_Pihao,YP_Changjia,YP_dateTime) ";
            //if (CF_I.yaopins != null)
            //{
            //    for (int i = 0; i < CF_I.yaopins.Length; i++)
            //    {
            //        if (i == CF_I.yaopins.Length - 1)
            //        {
            //            CommandText += "select '" + CF_I.CF_ID.ToString() + "','"
            //                                      + CF_I.yaopins[i].YP_ID.ToString() + "','"
            //                                      + CF_I.yaopins[i].YP_Name.ToString() + "','"
            //                                      + CF_I.yaopins[i].YP_DWJL.ToString() + "','"
            //                                      + CF_I.yaopins[i].YP_SL.ToString() + "','"
            //                                      + CF_I.yaopins[i].YP_DJ.ToString() + "','"
            //                                      + CF_I.yaopins[i].YP_PH.ToString() + "','"
            //                                      + CF_I.yaopins[i].YP_CJ.ToString() + "','"
            //                                      + CF_I.yaopins[i].YP_datetime.ToString()
            //                                      + "'"; //'1003','周宏伟','男','12' 
            //        }
            //        else
            //        {
            //            CommandText += "select '" + CF_I.CF_ID.ToString() + "','"
            //                + CF_I.yaopins[i].YP_ID.ToString() + "','"
            //                + CF_I.yaopins[i].YP_Name.ToString() + "','"
            //                + CF_I.yaopins[i].YP_DWJL.ToString() + "','"
            //                + CF_I.yaopins[i].YP_SL.ToString() + "','"
            //                + CF_I.yaopins[i].YP_DJ.ToString() + "','"
            //                + CF_I.yaopins[i].YP_PH.ToString() + "','"
            //                + CF_I.yaopins[i].YP_CJ.ToString() + "','"
            //                + CF_I.yaopins[i].YP_datetime.ToString()
            //                + "' union all"; //'1003','周宏伟','男','12' 
            //        }
            //    }
            //}
            //try
            //{
            //    this.AddTableRows(CommandText);
            //}
            //catch (SqlCeException cee)
            //{
            //    MessageBox.Show(cee.Message);
            //}
            #endregion
            return reanswer;
        }
        /// <summary>
        /// 添加所有数据一行
        /// </summary>
        /// <param name="RLDs">所添加数据</param>
        /// <returns></returns>
        public int AddAllDatasOnce(MyLOG_DATAs RLDs)
        {
           int reanswer = 0;
            SqlCeParameter[] paras = new SqlCeParameter[]
             { 
                 new SqlCeParameter("@NUM",RLDs.NUM),new SqlCeParameter("@Time",RLDs.Time),new SqlCeParameter("@RunTime",RLDs.RunTime)
                 ,new SqlCeParameter("@JYW_ID",RLDs.logData.JYW_ID),new SqlCeParameter("@JD_ID",RLDs.logData.JD_ID),new SqlCeParameter("@Temp",RLDs.logData.Temperture.MyValue)
                 ,new SqlCeParameter("@Humidity",RLDs.logData.Humidity.MyValue),new SqlCeParameter("@PM0",RLDs.logData.PM.MyValue)
             };
            SqlCeCommand cmd = myConn.CreateCommand();
            cmd.CommandText = "INSERT INTO ROV_ALLDAT VALUES(@NUM,@Time,@RunTime,@JYW_ID,@JD_ID,@Temp,@Humidity,@PM0) ";
            cmd.Parameters.AddRange(paras);
            try
            {
                reanswer = cmd.ExecuteNonQuery();
            }
            catch (SqlCeException cee)
            {
                MessageBox.Show(cee.Message);
                reanswer = -1;
            }
   
            return reanswer;
        }
       
        ///// <summary>
        ///// 添加报警数据
        ///// </summary>
        ///// <param name="alarm">报警类</param>
        //public void AddALARMDatas(ROVALARM alarm)
        //{

        //    byte[] ALARM = alarm.ALARM;

        //    string CommandText = "";
        //    CommandText = "INSERT INTO ROV_ALARM"
        //                            + " VALUES ('" + DateTime.Now.ToString() + "','" + alarm.RunTime.ToString()// + "','" + CF_I.CF_Fushu.ToString() + "','" + CF_I.CF_Time.ToString()// 2009-02-23 12:46:46.450
        //                                            + "','" + "0x" + bytes2HexSrt(ALARM) 
        //                               + "')";        
        //    try
        //    {
        //        this.AddTableRows(CommandText);
        //    }
        //    catch (SqlCeException cee)
        //    {
        //        MessageBox.Show(cee.Message);
        //    }
          

        //}
        /// <summary>
        /// 获取ROV_ALLDAT中某时间段数据
        /// </summary>
        /// <param name="Start">起始时间</param>
        /// <param name="Over">结束时间</param>
        /// <returns>数据表</returns>
        public DataTable GetAllDatasWithDate(DateTime Start, DateTime Over)
        {
            string CommandText = "select * from ROV_ALLDAT where Time  >='" + Start.Date.ToShortDateString() + "' and  CF_DateTime  <='" + (Over + TimeSpan.FromDays(1)).Date.ToShortDateString() + "'";


            return this.GetDatTable(CommandText, "ROV_ALLDAT");
        }
        /// <summary>
        /// 获取ROV_ALLDAT中所有段数据
        /// </summary>
        /// <returns>数据表</returns>
        public DataTable GetAllDatas()
        {
            string CommandText = "select * from ROV_ALLDAT ";


            return this.GetDatTable(CommandText, "ROV_ALLDAT");
        }
        //ROV_ALARM
        /// <summary>
        /// 获取ROV_ALARM中某时间段数据
        /// </summary>
        /// <param name="Start">起始时间</param>
        /// <param name="Over">结束时间</param>
        /// <returns>数据表</returns>
        public DataTable GetROV_ALARMWithDate(DateTime Start, DateTime Over)
        {
            string CommandText = "select * from ROV_ALARM where Time  >='" + Start.Date.ToShortDateString() + "' and  Time  <='" + (Over + TimeSpan.FromDays(1)).Date.ToShortDateString() + "'";


            return this.GetDatTable(CommandText, "ROV_ALARM");
        }
        /// <summary>
        /// 获取ROV_ALARM中所有段数据
        /// </summary>
        /// <returns>数据表</returns>
        public DataTable GetALARMDatasforTable()
        {
            string CommandText = "select * from ROV_ALARM ";


            return this.GetDatTable(CommandText, "ROV_ALARM");
        }

        public MyLOG_DATA GetALLDatas()
        {
            MyLOG_DATA ALL_LOG = new MyLOG_DATA();
            SqlCeCommand mySqlCommand = myConn.CreateCommand();
            string sqlStatement = "select * from ROV_ALLDAT ";// "select * from TestTbl ";
            //设置CommandText属性  
            mySqlCommand.CommandText = sqlStatement;
          
            bool frist = true;
            SqlCeDataReader sqlreader = mySqlCommand.ExecuteReader();
            nloadingData = 0;
            //                                 0      1          2             3        4
            // "INSERT INTO ROV_ALLDAT VALUES(@Time,@RunTime,@CKT_SWITCHS,@CKT_ANALOG,@SENSERS) ";
            //"INSERT INTO ROV_ALLDAT VALUES(@NUM,@Time,@RunTime,@Heading,@Pitch,@Roll,@Lon,@Lat,@Altitude,@Acc_x,@Acc_y,@Acc_z,@Gry_x,@Gry_y,@Gry_z"
            //                  + ",@Vx,@Vy,@Vz,@Rev1,@Rev2,@Rev3,@Rev4) ";
            while (sqlreader.Read())
            {
                MyLOG_DATAs alldat = new MyLOG_DATAs();

                alldat.NUM = (int)sqlreader[0];
                alldat.Time = (DateTime)sqlreader[1];
                alldat.RunTime = double.Parse( sqlreader[2].ToString());//
                alldat.logData.JYW_ID = int.Parse(sqlreader[3].ToString());//
                alldat.logData.JD_ID = int.Parse(sqlreader[4].ToString());//(float)sqlreader[4];
                alldat.logData.Temperture.MyValue = float.Parse(sqlreader[5].ToString());//(float)sqlreader[5];
                alldat.logData.Humidity.MyValue = float.Parse(sqlreader[6].ToString());//(double)sqlreader[6];
                alldat.logData.PM.MyValue = float.Parse(sqlreader[7].ToString());//(double)sqlreader[7];
                //alldat.Altitude = float.Parse(sqlreader[8].ToString());// (double)sqlreader[8];
                //alldat.Ax = double.Parse(sqlreader[9].ToString());//(double)sqlreader[9];
                //alldat.Ay = double.Parse(sqlreader[10].ToString());// (double)sqlreader[10];
                //alldat.Az = double.Parse(sqlreader[11].ToString());//(double)sqlreader[11];
                //alldat.Gx = double.Parse(sqlreader[12].ToString());//(double)sqlreader[12];
                //alldat.Gy = double.Parse(sqlreader[13].ToString());//(double)sqlreader[13];
                //alldat.Gz = double.Parse(sqlreader[14].ToString());//(double)sqlreader[14];
                //alldat.Vx = float.Parse(sqlreader[15].ToString());// (float)sqlreader[12];
                //alldat.Vy = float.Parse(sqlreader[16].ToString());// (float)sqlreader[13];
                //alldat.Vz = float.Parse(sqlreader[17].ToString());// (float)sqlreader[14];
                //alldat.Rev1 = int.Parse(sqlreader[18].ToString());
                //alldat.Rev2 = int.Parse(sqlreader[19].ToString());
                //alldat.Rev3 = int.Parse(sqlreader[20].ToString());
                //alldat.Rev4 = int.Parse(sqlreader[21].ToString());
                if (frist)
                {
                    frist = false;
                    ALL_LOG.beginTime = (DateTime)sqlreader["Time"];
                }
                ALL_LOG.ROV_LOG_DATA_List.Add(alldat);
                nloadingData++;
            }
            return ALL_LOG;
        }
       

    }
    /// <summary>
    /// ROV_LOG数据类
    /// </summary>
    /// 
    public class ROV_ALARM_LOG_DATA
    {
        /// <summary>
        /// 启动软件时间
        /// </summary>
        public DateTime beginTime = DateTime.UtcNow;
        /// <summary>
        /// 保存时间间隔，单位s
        /// </summary>
        public float Dt = 0.1F;
      
        public List<ROV_ALARM_LOG> ROV_LOG_DATA_List = new List<ROV_ALARM_LOG>();
    }
    public class ROV_ALARM_LOG
    {
        /// <summary>
        /// 报警数据
        /// </summary>
        public byte[] ALARMBytes = new byte[SqlCe_ROV.nSENSERS];
        /// <summary>
        /// 运行时间，单位s
        /// </summary>
        public double RunTime = 0;
    }
    /// <summary>
    /// LOG数据类
    /// </summary>
    public class MyLOG_DATA
    {
        /// <summary>
        /// 启动软件时间
        /// </summary>
        public DateTime beginTime = DateTime.Now;
        /// <summary>
        /// 保存时间间隔，单位s
        /// </summary>
        public float Dt = 0.1F;
       
        public List<MyLOG_DATAs> ROV_LOG_DATA_List = new List<MyLOG_DATAs>();
    }
    public class MyLOG_DATAs
    {
        public Data_JD logData = new Data_JD();
        public int NUM = 0;

      //  public float Heading = 0;
      //  public float Pitch = 0;//1
      //  public float Roll = 0;//2

      //  public double Gx = 0;
      //  public double Gy = 0;
      //  public double Gz = 0;

      //  public double Ax = 0;
      //  public double Ay = 0;
      //  public double Az = 0;

      //  public float Vy = 0;//3
      //  public float Vx = 0;//4
      //  public float Vz = 0;//5

      //  public double Altitude = 0;//
      //  public double Lat = 0;//
      //  public double Lon = 0;//

      //  public int Rev1 = 0;
      //  public int Rev2 = 0;
      //  public int Rev3 = 0;
      //  public int Rev4 = 0;
      //  /// <summary>
      //  /// 操控台模拟量
      //  /// </summary>
      //  public byte[] RControlanBytes = new byte[SqlCe_ROV.nCKT_ANALOG];
      //  /// <summary>
      //  /// 操控台开关量
      //  /// </summary>
      //  public byte[] RControlSWBytes = new byte[SqlCe_ROV.nCKT_SWITCHS];
      //  /// <summary>
      //  /// 传感器数据
      //  /// </summary>
      //  public byte[] SENSERsBytes = new byte[SqlCe_ROV.nSENSERS];
      //  /// <summary>
      //  /// 报警数据
      //  /// </summary>
      ////  public byte[] ALARMBytes = new byte[SqlCe_ROV.nSENSERS];
        /// <summary>
        /// 运行时间，单位s
        /// </summary>
        public double RunTime = 0;
        /// <summary>
        /// 当前记录时间
        /// </summary>
        public DateTime Time = DateTime.Now;
    }
}
