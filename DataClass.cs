using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZedGraph;

namespace WFA_AUTO_oo
{
    /// <summary>
    /// 传感器
    /// </summary>
    public  class SenserValue
    {
        /// <summary>
        /// 传感器数值
        /// </summary>
        public float MyValue=0;
        /// <summary>
        /// 上限报警值
        /// </summary>
        public float AlarmMax=45;
        /// <summary>
        /// 下限报警值
        /// </summary>
        public float AlarmMin=10;
        public SenserValue(float alarmMin,float alarmMax)
        {
            AlarmMax=alarmMax;
            AlarmMin=alarmMin;
        }
    }
    public class Shushidu
    {
        public int Shushi = 0;
        public int YiBan = 0;
        public int BuShushi = 0;
        public void clear()
        {
            Shushi = 0;
            YiBan = 0;
            BuShushi = 0;
        }
    }

    public  class Data_JD
    {
        /// <summary>
        /// 局域网ID
        /// </summary>
        public int JYW_ID = -1;
        /// <summary>
        /// 节点ID
        /// </summary>
        public int JD_ID = -1;

        public int nGot = 0;
        /// <summary>
        /// 温度
        /// </summary>
        public SenserValue Temperture =new SenserValue(20,32);
        public double ETemp = 0;
        public double SdtTemp = 0;
        /// <summary>
        /// 湿度
        /// </summary>
        public SenserValue Humidity = new SenserValue(20, 50);
        public double EHumidity = 0;
        public double SdtHumidity = 0;
        /// <summary>
        /// PM
        /// </summary>
        public SenserValue PM = new SenserValue(100, 4000);
        public double EPM = 0;
        public double SdtPM = 0;
        /// <summary>
        /// 节点IP（节点网络地址）
        /// </summary>
        public int JDIP = -1;
        public string JDaddress = "未知";

        /// <summary>
        /// 运行时间 h
        /// </summary>
        public float RunTime = 0;
        public DateTime dateTime = DateTime.Now;
        /// <summary>
        /// 温度曲线
        /// </summary>
        public PointPairList list_T = new PointPairList();
        /// <summary>
        /// 湿度曲线
        /// </summary>
        public PointPairList list_H = new PointPairList();
        /// <summary>
        /// PM2.5曲线
        /// </summary>
        public PointPairList list_P= new PointPairList();

        public Data_JD()
        { }
        public Data_JD(int _JDID,int _JYWID)
        {
            JYW_ID = _JYWID;
            JD_ID = _JDID;
            JDaddress =_JYWID.ToString()+"#网络_"+ JD_ID.ToString() + "#节点";
        }
        public int len=0;
        public float EveT=0;
        public float EveH = 0;
        public float EveP = 0;
        public void countEve()
        {
            len++;
            EveT = EveT * (len - 1) / len + Temperture.MyValue / len;
            EveH = EveH * (len - 1) / len + Humidity.MyValue / len;
           // EveP = EveP * (len - 1) / len PM.MyValue / len;
            EHumidity = EveH;
            ETemp = EveT;
            EPM = EveP;

        }
        public void zeroEve()
        {
            len = 0;
            EveT=0;
            EveH = 0;
            EveP = 0;
            EHumidity = EveH;
            ETemp = EveT;
            EPM = EveP;
        }


    }
}
