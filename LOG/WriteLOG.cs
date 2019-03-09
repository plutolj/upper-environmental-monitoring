using System;
using System.Windows.Forms;
using System.Threading;
using ROV_SQLCE;
using ROV_SCU_V1._00._4._0;
//using WFA_AUTO_oo;

namespace WFA_AUTO_oo
{
    public partial class Form1 : Form
    {
        float LOGts = 0.0995F;
        MyLOG_DATAs Rldats = new MyLOG_DATAs();
        Thread WriteLogThread = null;
        string LOGPath = "D://MyLOG";
        bool WriteLogIni()
        {
          //  WriteLogThread = new Thread(new ThreadStart(WritingLog));
          
            myRovLog = new ROV_LOG(LOGPath);
            if (myRovLog.MyLogPath == null)
                return false;
            LogWritingM = true;
             NUM=0;
            //WriteLogThread.Start();
            return true;
        }
        void StopWriteLog()
        {
            LogWritingM = false;
            Thread.Sleep(10);
            myRovLog.CloseROV_LOG();
            myRovLog = null;
        }
        ROV_LOG myRovLog = null;
        bool LogWritingM = false;

        int NUM=0;
        void WriteLog(Data_JD MyDJD)
        {
            if (LogWritingM)
            {


                // lastTime += Dtsp;
                Rldats.NUM = NUM;
                NUM++;
                Rldats.Time = DateTime.Now;
                Rldats.RunTime = MyDJD.RunTime;
                Rldats.logData.JYW_ID = MyDJD.JYW_ID;
                Rldats.logData.JD_ID = MyDJD.JD_ID;
                Rldats.logData.Temperture.MyValue = MyDJD.Temperture.MyValue;
                Rldats.logData.Humidity.MyValue = MyDJD.Humidity.MyValue;
                Rldats.logData.PM.MyValue = MyDJD.PM.MyValue;

                //Rldats.Heading = UAVStatus.planeheading;
                //Rldats.Pitch = UAVStatus.pitch;
                //Rldats.Roll = UAVStatus.roll;
                //Rldats.Lat =  double.Parse(UAVStatus.planesPOS.Y.ToString("F6"));
                //Rldats.Lon = double.Parse(UAVStatus.planesPOS.X.ToString("F6"));
                //Rldats.Altitude = UAVStatus.gaodu;
                //Rldats.Ax = UAVStatus.Ax;
                //Rldats.Ay = UAVStatus.Ay;
                //Rldats.Az = UAVStatus.Az;
                //Rldats.Gx = UAVStatus.Wx;
                //Rldats.Gy = UAVStatus.Wy;
                //Rldats.Gz = UAVStatus.Wz;
                //Rldats.Vx = UAVStatus.Vx;
                //Rldats.Vy = UAVStatus.Vy;
                //Rldats.Vz = UAVStatus.Vz;
                //Rldats.Rev1 = (int)(UAVStatus.ZL_YM * 1000);
                //Rldats.Rev2 = (int)(UAVStatus.ZL_FY * 1000);
                //Rldats.Rev3 = (int)(UAVStatus.ZL_PH * 1000);
                //Rldats.Rev4 = (int)(UAVStatus.ZL_GZ * 1000);
                // Rldats.SENSERsBytes = RovStatus.getBytesLog();
                // Rldats.RControlanBytes = RovControler.ControlerRecord();
                myRovLog.Adddat2LogBD(Rldats);
            }

               
        }
        /// <summary>
        /// 日志记录线程
        /// </summary>
        void WritingLog()
        {
            
            DateTime lastTime = DateTime.Now;
            DateTime NowTime = DateTime.Now;
            TimeSpan Dtsp;
            double  Dt = 0;
             NUM = 0;
            while (LogWritingM)
            {
                NowTime = DateTime.Now;
                Dtsp=NowTime - lastTime;
                Dt = Dtsp.TotalSeconds;
                if (Dt >= LOGts)
                {
                    
                    lastTime += Dtsp;
                    Rldats.NUM = NUM;
                    NUM++;
                    Rldats.Time = NowTime;
                    Rldats.RunTime = Rldats.logData.RunTime;
             //       Rldats.logData.JYW_ID=
                    //Rldats.Heading = UAVStatus.planeheading;
                    //Rldats.Pitch = UAVStatus.pitch;
                    //Rldats.Roll = UAVStatus.roll;
                    //Rldats.Lat =  double.Parse(UAVStatus.planesPOS.Y.ToString("F6"));
                    //Rldats.Lon = double.Parse(UAVStatus.planesPOS.X.ToString("F6"));
                    //Rldats.Altitude = UAVStatus.gaodu;
                    //Rldats.Ax = UAVStatus.Ax;
                    //Rldats.Ay = UAVStatus.Ay;
                    //Rldats.Az = UAVStatus.Az;
                    //Rldats.Gx = UAVStatus.Wx;
                    //Rldats.Gy = UAVStatus.Wy;
                    //Rldats.Gz = UAVStatus.Wz;
                    //Rldats.Vx = UAVStatus.Vx;
                    //Rldats.Vy = UAVStatus.Vy;
                    //Rldats.Vz = UAVStatus.Vz;
                    //Rldats.Rev1 = (int)(UAVStatus.ZL_YM * 1000);
                    //Rldats.Rev2 = (int)(UAVStatus.ZL_FY * 1000);
                    //Rldats.Rev3 = (int)(UAVStatus.ZL_PH * 1000);
                    //Rldats.Rev4 = (int)(UAVStatus.ZL_GZ * 1000);
                   // Rldats.SENSERsBytes = RovStatus.getBytesLog();
                   // Rldats.RControlanBytes = RovControler.ControlerRecord();
                    myRovLog.Adddat2LogBD(Rldats);

                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

      

        private void Form1_Load(object sender, EventArgs e)
        {
        
        }

        #region reappearLOG
        //void ReappearLogINI()
        //{
        //    Thread ReapearTh = new Thread(new ThreadStart(Reappearing));
        //    myReappear = new reappearLOG();
        //    myReappear.Show();
        //    ReapearTh.Start();
        //}
        //reappearLOG myReappear = null;
        //bool ReappearingM = false;
        ///// <summary>
        ///// 日志回放线程
        ///// </summary>
        //void Reappearing()
        //{
        //   // if (myReappear == null) return;
        //    ReappearingM = true;
        //    while (ReappearingM)
        //    {
        //        if (myReappear.refreshMark)
        //        {
        //            UAVStatus.planeheading = myReappear.RLDATp.Heading;
        //            UAVStatus.pitch = myReappear.RLDATp.Pitch;
        //            UAVStatus.roll = myReappear.RLDATp.Roll;
        //            UAVStatus.planesPOS.X = (float)myReappear.RLDATp.Lon;
        //            UAVStatus.planesPOS.Y = (float)myReappear.RLDATp.Lat;
        //            UAVStatus.gaodu = (float)myReappear.RLDATp.Altitude;

        //            UAVStatus.Vx = myReappear.RLDATp.Vx;
        //            UAVStatus.Vy = myReappear.RLDATp.Vy;
        //            UAVStatus.Vz = myReappear.RLDATp.Vz;

        //            UAVStatus.Ax = (float)myReappear.RLDATp.Ax;
        //            UAVStatus.Ay = (float)myReappear.RLDATp.Ay;
        //            UAVStatus.Az = (float)myReappear.RLDATp.Az;

        //            UAVStatus.Wx = (float)myReappear.RLDATp.Gx;
        //            UAVStatus.Wy = (float)myReappear.RLDATp.Gy;
        //            UAVStatus.Wz = (float)myReappear.RLDATp.Gz;

        //            UAVStatus.ZL_YM = (float)(myReappear.RLDATp.Rev1*0.001F);
        //            UAVStatus.ZL_FY = (float)(myReappear.RLDATp.Rev2 * 0.001F);
        //            UAVStatus.ZL_PH = (float)(myReappear.RLDATp.Rev3 * 0.001F);
        //            UAVStatus.ZL_GZ = (float)(myReappear.RLDATp.Rev4 * 0.001F);
        //            UAVStatus.planelines.Add(new PointD(UAVStatus.planesPOS.X, UAVStatus.planesPOS.Y));
        //            //RovStatus.readFrombytes(myReappear.RLDATp.SENSERsBytes);
        //            //RovStatus.ROVStatusReadedMark = true;
        //            //if (receiveEventformrov != null)
        //            //{ receiveEventformrov(this, ControledStyle.ROV_Motors); } 
        //            myReappear.refreshMark = false;
        //            gMapRefreshMark = true;
        //        }
        //        else 
        //        {
        //            Thread.Sleep(2);
        //        }
        //        ReappearingM = myReappear.playingM;
        //    }
        //    myReappear = null;
        //}
        #endregion
    }
}
