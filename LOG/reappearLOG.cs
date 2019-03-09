using System;
using System.Windows.Forms;
using System.Threading;
using ROV_SQLCE;

namespace ROV_SCU_V1._00._4._0
{
    public partial class reappearLOG : Form
    {
        public reappearLOG()
        {
            InitializeComponent();
            timer1.Interval = 5;
            timer1.Start();
            label1.Text = (nplay + 1).ToString() + "/" + Datlen.ToString();
        }
        
        public bool playingM = true;
        public string FileName = "";
        ROV_LOG myRovLog = null;
        ROV_LOG_DATA Rldats = new ROV_LOG_DATA();
        public ROV_LOG_DATAs RLDATp = new ROV_LOG_DATAs();
        public bool DatasIsOK = false;
        public int Datlen = 0;
        bool loadM = false;

        public int nplay = 0;
        private void but_openlog_Click(object sender, EventArgs e)
        {
            OpenFileDialog opfd = new OpenFileDialog();
            opfd.Filter = "LOG文件(*.sdf,*.SDF)|*.sdf;*.SDF";
            if (opfd.ShowDialog() == DialogResult.OK)
            {
                FileName = opfd.FileName;
                myRovLog = new ROV_LOG(FileName,"open");
                loadM = true;
            }
            if (loadM)
            {
                label1.Text = "Loading " + FileName;
                Thread myLoadThread = new Thread(new ThreadStart(LoadDatas));
                DatasIsOK = false;
                myLoadThread.Start();
                label1.Text = (nplay + 1).ToString() + "/" + Datlen.ToString();
                lastTime = DateTime.Now;
            }
        }

        void LoadDatas()
        {
            
            if (myRovLog!=null)
            {
                Rldats = myRovLog.GetAllLogDatas();
                Datlen = myRovLog.nLoadDat;
                Datlen = Rldats.ROV_LOG_DATA_List.Count;
                DatasIsOK = true ;
            }
        }
        public bool refreshMark=false;
        bool playM = false;
        double runtime=0;
        double runtime2 = 0;
        DateTime lastTime = DateTime.Now;
        DateTime NowTime = DateTime.Now;
        TimeSpan Dtsp;
        double Dt = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (DatasIsOK)
            {
                if (loadM)
                {
                     trackBar1.Maximum = Datlen;
                     if (Datlen <= 0) return;
                     trackBar1.Value = 1;
                     nplay = 0;
                     runtime = Rldats.ROV_LOG_DATA_List[nplay].RunTime;
                     this.Text = "重播器：" + FileName + "::" + Rldats.beginTime.ToString();
                     if (Datlen >= 1)
                     {
                         nplay = 0;
                    //    refreshMark = true;
                        //更新第一帧
                        RLDATp = Rldats.ROV_LOG_DATA_List[nplay];
                        refreshMark = true;
                     }
                     if (Datlen > 1)
                     runtime2 = Rldats.ROV_LOG_DATA_List[1].RunTime;
                     
                    loadM = false;
                    
                }
               
                if (playM)
                {
                    if (nplay + 1 < Datlen)
                    {

                        NowTime = DateTime.Now;
                        Dtsp = NowTime - lastTime;
                        Dt = Dtsp.TotalSeconds;
                        if (runtime + Dt >= runtime2)
                        {
                            lastTime += Dtsp;
                            runtime2 = Rldats.ROV_LOG_DATA_List[nplay + 1].RunTime;
                           
                            //更新
                            RLDATp = Rldats.ROV_LOG_DATA_List[nplay];
                            runtime = RLDATp.RunTime;
                            refreshMark = true;
                            nplay++;
                        }
                    }
                    else
                    {
                        //更新
                        RLDATp = Rldats.ROV_LOG_DATA_List[nplay];
                        runtime = RLDATp.RunTime;
                        refreshMark = true;
                        playM = false;
                    }
                    trackBar1.Value = nplay + 1;
                    label1.Text = (nplay + 1).ToString() + "/" + Datlen.ToString();
                }
                //else
                //{
                //    lastTime = DateTime.Now;
                //}
                 
            }
        }

        private void reappearLOG_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("确定要退出吗？", "谨慎退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true; //取消关闭操作   
                return;
            }
            else
            {
                playingM = false;
                Thread.Sleep(100);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            playM = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            playM = false;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            nplay = Math.Min(Rldats.ROV_LOG_DATA_List.Count-1, trackBar1.Value-1);


         //   runtime = Rldats.ROV_LOG_DATA_List[nplay].RunTime;
            if (nplay >=0) 
            RLDATp = Rldats.ROV_LOG_DATA_List[nplay];
            if (nplay < Rldats.ROV_LOG_DATA_List.Count - 1)
            {
                runtime2 = Rldats.ROV_LOG_DATA_List[nplay + 1].RunTime;
            }
            else
            { 
            }
            runtime = RLDATp.RunTime;
            refreshMark = true;
            label1.Text = (nplay + 1).ToString() + "/" + Datlen.ToString();
        }

    }
}
