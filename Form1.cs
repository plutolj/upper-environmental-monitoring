using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using System.IO.Ports;

namespace WFA_AUTO_oo
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            zedGraphControl4.Hide();
            panel2.Width = 1080;
            panel2.Height = (N_JD / 5 + 1) * 110 + 10;
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    myD_JD[i * 5 + j] = new Data_JD(i * 5 + j + 1, 1);
                    myShowJD[i * 5 + j] = new showJDn(5+j*205, 5 + i * 110, 200, 105);
                    myShushidu[i * 5 + j] = new Shushidu();
                    if (i * 5 + j < N_JD)
                    {
                        comboBox1.Items.Add((i * 5 + j + 1).ToString() + "#");
                    }
                }
            }
            myD_JD[0].JDIP = 0x1265;
            myD_JD[1].JDIP = 0x2fdd;
            showLinesINI();
            appStartT = DateTime.Now;
            _list_T = myD_JD[0].list_T;
            _list_H = myD_JD[0].list_H;
            _list_P= myD_JD[0].list_P;
            myPane_T.CurveList.Clear();
            myCurve_T = myPane_T.AddCurve("温度曲线", _list_T, System.Drawing.Color.Red, SymbolType.None);
            myPane_H.CurveList.Clear();
            myCurve_H = myPane_H.AddCurve("湿度曲线", _list_H, System.Drawing.Color.Blue, SymbolType.None);
            myPane_P.CurveList.Clear();
            myCurve_P = myPane_P.AddCurve("PM2.5曲线", _list_P, System.Drawing.Color.GreenYellow, SymbolType.None);
            string [] ports=SerialPort.GetPortNames();
            if (ports.Length > 0)
            {
                for (int i = 0; i < ports.Length; i++)
                {
                    toolStripComboBox1.Items.Add(ports[i]);
                }
                toolStripComboBox1.Text = ports[0];
            }
            timer1.Interval = 500;
            timer1.Start();
            tabControl1.TabPages.Clear();
            tabControl1.TabPages.Add(tabPage3);
            this.MainMenuStrip.Hide();// = null;
            
           
              //  splitContainer1.Panel1Collapsed = true;
        }

        private void 曲线图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (曲线图ToolStripMenuItem.Checked == false)
            //{
            //    曲线图ToolStripMenuItem.Checked = true;
            //  //  splitContainer1.Panel1Collapsed = false;

            //}
            //else
            //{
            //    曲线图ToolStripMenuItem.Checked = false;
            //  //  splitContainer1.Panel1Collapsed = true; ;
            //}
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        int N_JD = 2;
        int Selec_JD = 0;
        /// <summary>
        /// 全局
        /// </summary>
        
        double  RunTime = 0;//单位：小时
        const int Nmax = 100;
        DateTime appStartT = DateTime.Now;
        Data_JD[] myD_JD = new Data_JD[Nmax];
        float[] reJD_LastT = new float[Nmax];
        showJDn[] myShowJD = new showJDn[Nmax];
        Shushidu[] myShushidu = new Shushidu[Nmax];
        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < N_JD; i++)
            {
                myShowJD[i].Refresh(myD_JD[i], e.Graphics);
            }
        }

        byte[] buffer = new byte[13];
        Data_JD DJDbufer = new Data_JD();
      //  Data_JD[] DJDQJ = new Data_JD[100];
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if (serialPort1.BytesToRead > 12)
            {
                if (serialPort1.ReadByte() == 10)
                {
                    serialPort1.Read(buffer, 2, 11);
                    if (buffer[12] == 0xFF)
                    {
                        DJDbufer.dateTime = DateTime.Now;
                        DJDbufer.JDIP = buffer[4] + buffer[5] * 256;
                        DJDbufer.Temperture.MyValue = buffer[6] - 50;
                        DJDbufer.Humidity.MyValue = buffer[7];
                        DJDbufer.PM.MyValue = buffer[8] * 250 + buffer[9];
                        writeJDList(DJDbufer);
                    }
                }
            }
        }
        void writeJDList(Data_JD MyDJD)
        {
            RunTime = (MyDJD.dateTime - appStartT).TotalHours;
            for(int i=0;i<N_JD;i++)
            {
                if (MyDJD.JDIP == myD_JD[i].JDIP)
                {
                    myD_JD[i].dateTime = MyDJD.dateTime;
                    myD_JD[i].RunTime = (float)RunTime;
                    myD_JD[i].nGot = 60;
                    
                    myD_JD[i].PM.MyValue = MyDJD.PM.MyValue;
                    myD_JD[i].Temperture.MyValue = MyDJD.Temperture.MyValue;
                    myD_JD[i].Humidity.MyValue = MyDJD.Humidity.MyValue;
                    myD_JD[i].list_T.Add(RunTime,MyDJD.Temperture.MyValue );
                    myD_JD[i].list_H.Add(RunTime,MyDJD.Humidity.MyValue);
                    myD_JD[i].list_P.Add(RunTime,MyDJD.PM.MyValue );
                    reJD_LastT[i] = (float)RunTime;
                    myD_JD[i].countEve();
                    if (myD_JD[i].Temperture.MyValue > myD_JD[i].Temperture.AlarmMin && myD_JD[i].Temperture.MyValue < myD_JD[i].Temperture.AlarmMax
                        && myD_JD[i].Humidity.MyValue > myD_JD[i].Humidity.AlarmMin && myD_JD[i].Humidity.MyValue < myD_JD[i].Humidity.AlarmMax
                        && myD_JD[i].PM.MyValue > myD_JD[i].PM.AlarmMin && myD_JD[i].PM.MyValue < myD_JD[i].PM.AlarmMax
                        )
                    {
                        myShushidu[i].Shushi++;
                    }
                    else if (myD_JD[i].Temperture.MyValue > myD_JD[i].Temperture.AlarmMin && myD_JD[i].Temperture.MyValue < myD_JD[i].Temperture.AlarmMax
                       || myD_JD[i].Humidity.MyValue > myD_JD[i].Humidity.AlarmMin && myD_JD[i].Humidity.MyValue < myD_JD[i].Humidity.AlarmMax
                       || myD_JD[i].PM.MyValue > myD_JD[i].PM.AlarmMin && myD_JD[i].PM.MyValue < myD_JD[i].PM.AlarmMax
                       )
                    {
                        myShushidu[i].YiBan++;
                    }
                    else
                    //if (myD_JD[i].Temperture.MyValue > myD_JD[i].Temperture.AlarmMin && myD_JD[i].Temperture.MyValue < myD_JD[i].Temperture.AlarmMax
                    //   && myD_JD[i].Humidity.MyValue > myD_JD[i].Humidity.AlarmMin && myD_JD[i].Humidity.MyValue < myD_JD[i].Humidity.AlarmMax
                    //   && myD_JD[i].Light.MyValue > myD_JD[i].Light.AlarmMin && myD_JD[i].Light.MyValue < myD_JD[i].Light.AlarmMax
                    //   )
                    {
                        myShushidu[i].BuShushi++;
                    }
                    WriteLog(myD_JD[i]);
                    break;
                }
            }
           

        }

        public GraphPane myPane_H;
        public GraphPane myPane_T;
        public GraphPane myPane_P;
        private LineItem myCurve_H;
        private LineItem myCurve_T;
        private LineItem myCurve_P;
        /// <summary>
        /// 温度曲线
        /// </summary>
        public PointPairList _list_T = new PointPairList();
        /// <summary>
        /// 湿度曲线
        /// </summary>
        public PointPairList _list_H = new PointPairList();
        /// <summary>
        /// PM2.5曲线
        /// </summary>
        public PointPairList _list_P = new PointPairList();
        void showLinesINI()
        {
            myPane_T = zedGraphControl1.GraphPane;
            myPane_T.Title.Text = "温度";
            myPane_T.XAxis.Title.Text = "t（h）";
            myPane_T.YAxis.Title.Text = "(℃)";
            myCurve_T = myPane_T.AddCurve("温度曲线", _list_T, System.Drawing.Color.Red, SymbolType.None);
           // myCurve_Tset = myPane_T.AddCurve("设定温度", list_Tset, System.Drawing.Color.Red, SymbolType.None);
            myPane_H = zedGraphControl2.GraphPane;
            myPane_H.Title.Text = "湿度";
            myPane_H.XAxis.Title.Text = "t（h）";
            myPane_H.YAxis.Title.Text = "(HR)";
            myCurve_H = myPane_H.AddCurve("湿度曲线", _list_H, System.Drawing.Color.Blue, SymbolType.None);

            myPane_P = zedGraphControl3.GraphPane;
            myPane_P.Title.Text = "PM2.5";
            myPane_P.XAxis.Title.Text = "t（μg/m3）";
            myPane_P.YAxis.Title.Text = "(μg/m3)";
            myCurve_P = myPane_P.AddCurve("PM2.5曲线", _list_P, System.Drawing.Color.GreenYellow, SymbolType.None);

            zedGraphControl1.AxisChange();
            zedGraphControl1.Refresh();
            zedGraphControl2.AxisChange();
            zedGraphControl2.Refresh();
            zedGraphControl3.AxisChange();
            zedGraphControl3.Refresh();

           // CreateChart(zedGraphControl4);
          
        }
        PieItem segment1;// = myPane.AddPieSlice(200, Color.Green, Color.White, 45f, 0, "舒适");
        PieItem segment3;// = myPane.AddPieSlice(30, Color.Blue, Color.White, 45f, 0, "不舒适");
        // PieItem segment4 = myPane.AddPieSlice(10.21, Color.LimeGreen, Color.White, 45f, 0, "West");
        PieItem segment2;//= myPane.AddPieSlice(140, Color.Gold, Color.White, 45f, 0, "一般");
        
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int njd=comboBox1.SelectedIndex;
            if (njd < N_JD)
            {
                Selec_JD = njd;
                
                _list_T = myD_JD[njd].list_T;
                _list_H = myD_JD[njd].list_H;
                _list_P = myD_JD[njd].list_P;
                myPane_T.CurveList.Clear();
                myCurve_T = myPane_T.AddCurve("温度曲线", _list_T, System.Drawing.Color.Red, SymbolType.None);
                myPane_H.CurveList.Clear();
                myCurve_H = myPane_H.AddCurve("湿度曲线", _list_H, System.Drawing.Color.Blue, SymbolType.None);
                //myCurve_L = myPane_L.AddCurve("湿度曲线", _list_L, System.Drawing.Color.GreenYellow, SymbolType.None);
                myPane_P.CurveList.Clear();
                myCurve_P = myPane_P.AddCurve("PM2.5曲线", _list_P, System.Drawing.Color.GreenYellow, SymbolType.None);
            }
        }

        private void 刷新端口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripComboBox1.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length > 0)
            {
                for (int i = 0; i < ports.Length; i++)
                {
                    toolStripComboBox1.Items.Add(ports[i]);
                }
                toolStripComboBox1.Text = ports[0];
            }
        }

        private void 开启串口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (开启串口ToolStripMenuItem.Checked)
            {
                serialPort1.Close();
                开启串口ToolStripMenuItem.Checked = false;
            }
            else
            {
                try
                {
                    serialPort1.PortName = toolStripComboBox1.Text;
                    serialPort1.BaudRate = 115200;

                    serialPort1.Open();
                    开启串口ToolStripMenuItem.Checked = true;
                }
                catch(Exception exx)
                {
                    MessageBox.Show(exx.Message);
 
                }
            }
        }

        Shushidu shushidu = new Shushidu();
        private void timer1_Tick(object sender, EventArgs e)
        {
            label3.Text = myD_JD[Selec_JD].JDaddress;
            if (myD_JD[Selec_JD].Temperture.MyValue >= myD_JD[Selec_JD].Temperture.AlarmMin && myD_JD[Selec_JD].Temperture.MyValue <= myD_JD[Selec_JD].Temperture.AlarmMax)
            { label2.ForeColor = Color.Green; }
            else
            { label2.ForeColor = Color.Red; }
            if (myD_JD[Selec_JD].Humidity.MyValue >= myD_JD[Selec_JD].Humidity.AlarmMin && myD_JD[Selec_JD].Humidity.MyValue <= myD_JD[Selec_JD].Humidity.AlarmMax)
            { label7.ForeColor = Color.Green; }
            else
            { label7.ForeColor = Color.Red; }
            if (myD_JD[Selec_JD].PM.MyValue >= myD_JD[Selec_JD].PM.AlarmMin && myD_JD[Selec_JD].PM.MyValue <= myD_JD[Selec_JD].PM.AlarmMax)
            { label8.ForeColor = Color.Green; }
            else
            { label8.ForeColor = Color.Red; }
           
            label2.Text = myD_JD[Selec_JD].Temperture.MyValue.ToString() + "℃";
            label7.Text = myD_JD[Selec_JD].Humidity.MyValue.ToString() + "%RH";
            label13.Text = myD_JD[Selec_JD].ETemp.ToString("F2")+ "℃";
            label14.Text = myD_JD[Selec_JD].EHumidity.ToString("F2") + "%RH";
       
            
            RunTime = (DateTime.Now - appStartT).TotalHours;
      
            zedGraphControl1.AxisChange();
            zedGraphControl1.Refresh();
            zedGraphControl2.AxisChange();
            zedGraphControl2.Refresh();
            zedGraphControl3.AxisChange();
            zedGraphControl3.Refresh();
            panel2.Refresh();
            for (int i = 0; i < N_JD; i++)
            {
                if (myD_JD[i].nGot>0) myD_JD[i].nGot--;

            }
            if (segment1 != null)
            {

                if (myShushidu[Selec_JD].Shushi != 0 || myShushidu[Selec_JD].YiBan != 0 || myShushidu[Selec_JD].BuShushi != 0)
                {
                    segment1.Value = myShushidu[Selec_JD].Shushi;
                    segment2.Value = myShushidu[Selec_JD].YiBan;
                    segment3.Value = myShushidu[Selec_JD].BuShushi;
                    segment1.Label.Text = "舒适：" + (100 * segment1.Value / (segment1.Value + segment2.Value + segment3.Value)).ToString("F1") + "%";
                    segment2.Label.Text = "一般：" + (100 * segment2.Value / (segment1.Value + segment2.Value + segment3.Value)).ToString("F1") + "%";
                    segment3.Label.Text = "不舒适：" + (100 * segment3.Value / (segment1.Value + segment2.Value + segment3.Value)).ToString("F1") + "%";
                }
            }
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "Administrator")
            {
                if (textBox2.Text == "1234")
                {
                    tabControl1.TabPages.Clear();
                    tabControl1.TabPages.Add(tabPage1);
                    tabControl1.TabPages.Add(tabPage2);
                    this.MainMenuStrip.Show();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 开始记录数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (开始记录数据ToolStripMenuItem.Checked)
            {
                StopWriteLog();
                开始记录数据ToolStripMenuItem.Checked = false;
            }
            else
            {
                WriteLogIni();
                开始记录数据ToolStripMenuItem.Checked = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                myD_JD[Selec_JD].zeroEve();
            }
        }

       

    
        

        
        

       
    }
}
