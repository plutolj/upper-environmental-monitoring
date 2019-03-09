using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ROV_SQLCE;
using System.Windows.Forms;

namespace ROV_SCU_V1._00._4._0
{
    public  class ROV_LOG
    {
        public bool IsOpen = false;
        public string MyLogPath=null;
        protected DateTime LogBDstartTime = DateTime.UtcNow;
        protected SqlCe_ROV mylogBD = null;
        public int nLoadDat = 0;
        public ROV_LOG(string LogPath,string Creat="creat")
        {
            try
            {
                DirectoryInfo dfo = new DirectoryInfo(LogPath);
            }
            catch (Exception exx)
            {
                MessageBox.Show(exx.Message);
                MyLogPath = null;
                return;
            }

            if (Creat == "creat")
            {
                if (Directory.Exists(LogPath) == false)
                {
                    Directory.CreateDirectory(LogPath);
                }
            }
            MyLogPath = LogPath;
            CreatorOpenLogDB(Creat);
        }
        public void CloseROV_LOG()
        {
            if (mylogBD != null)
            {
                if (this.IsOpen)
                {
                    mylogBD.CloseSqlCe();
                    this.IsOpen = false;
                }
            }
        }
      
        
        public void Adddat2LogBD(MyLOG_DATAs[] Rldat)
        {
            mylogBD.AddAllDatas(Rldat);
        }
        public void Adddat2LogBD(MyLOG_DATAs Rldat)
        {
            mylogBD.AddAllDatasOnce(Rldat);
        }
        //public void AddALARM2LogBD(ROVALARM alarm)
        //{
        //    mylogBD.AddALARMDatas(alarm);
        //}
        public MyLOG_DATA GetAllLogDatas()
        {
           return mylogBD.GetALLDatas();
        }
        protected bool CreatorOpenLogDB(string Creat="creat")
        {
            if(MyLogPath!=null)
            {
                string flilename = "";
                if (Creat == "creat")
                {
                    LogBDstartTime = DateTime.UtcNow;
                    MyLogPath += "//" + string.Format("{0:0000}", LogBDstartTime.Year) + string.Format("{0:00}", LogBDstartTime.Month)
                        + "//";
                    if (Directory.Exists(MyLogPath) == false)
                    {
                        Directory.CreateDirectory(MyLogPath);
                    }
                    flilename = MyLogPath + string.Format("{0:00}", LogBDstartTime.Year) + string.Format("{0:00}", LogBDstartTime.Month) + string.Format("{0:00}", LogBDstartTime.Day)
                        + string.Format("{0:00}", LogBDstartTime.Hour) + string.Format("{0:00}", LogBDstartTime.Minute) + ".sdf";
                }
                else
                {
                    flilename = MyLogPath;
                }

                if (File.Exists(flilename))
                {
                    mylogBD = new SqlCe_ROV(flilename, "open");
                }
                else
                {
                    mylogBD = new SqlCe_ROV(flilename, "creat");
                }
                //mylogBD=new SqlCe_ROV() 
                IsOpen = true;
                return IsOpen;
            }
            else
            {
                return IsOpen;
            }
        }
    }
}
