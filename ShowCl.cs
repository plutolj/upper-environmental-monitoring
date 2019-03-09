using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace WFA_AUTO_oo
{
    public partial class Form1 : Form
    {
        class MyPanel : Panel
        {
            public MyPanel()
            {
                //开启双缓冲
                //SetStyle(ControlStyles.UserPaint, true);
                //SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            }
        }
        class showJDn
        {

            public showJDn()
            {
            }
            public showJDn(int sX,int sY,int _width,int _heih)
            {
                width = _width;
                heigh = _heih;
                loaction.X = sX;
                loaction.Y = sY;
            }
            public Point loaction = new Point(0, 0);
            public int width = 100;
            public int heigh = 80;
            SolidBrush sbrush_black = new SolidBrush(Color.Black);
            SolidBrush sbrush_red = new SolidBrush(Color.Red);
            SolidBrush sbrush_Green = new SolidBrush(Color.Green);
            public void Refresh(Data_JD myJD,Graphics g)
            {
                Font font1=new Font("Arial", 10, FontStyle.Italic);
                g.DrawRectangle(new Pen(Color.Black,2), new Rectangle(loaction, new Size(width, heigh)));
                g.DrawString(myJD.JDaddress, font1, new SolidBrush(Color.Black), loaction.X + 5, loaction.Y + 5);
                if (myJD.Temperture.MyValue >= myJD.Temperture.AlarmMin && myJD.Temperture.MyValue <= myJD.Temperture.AlarmMax)
                {
                    g.DrawString("温度：" + myJD.Temperture.MyValue.ToString() + "℃", font1, sbrush_Green, loaction.X + 5, loaction.Y + 25);
                }
                else
                {
                    g.DrawString("温度：" + myJD.Temperture.MyValue.ToString() + "℃", font1, sbrush_red, loaction.X + 5, loaction.Y + 25);
                }
                if (myJD.Humidity.MyValue >= myJD.Humidity.AlarmMin && myJD.Humidity.MyValue <= myJD.Humidity.AlarmMax)
                {
                    g.DrawString("湿度：" + myJD.Humidity.MyValue.ToString() + "%RH", font1, sbrush_Green, loaction.X + 5, loaction.Y + 45);
                }
                else
                {
                    g.DrawString("湿度：" + myJD.Humidity.MyValue.ToString() + "%RH", font1, sbrush_red, loaction.X + 5, loaction.Y + 45);
                }
                if (myJD.PM.MyValue >= myJD.PM.AlarmMin && myJD.PM.MyValue <= myJD.PM.AlarmMax)
                {
                    g.DrawString("PM2.5：" + myJD.PM.MyValue.ToString() + "μg/m3",font1, sbrush_red, loaction.X + 5, loaction.Y + 65);
                }
                else
                {
                    g.DrawString("PM2.5：" + myJD.PM.MyValue.ToString() + "μg/m3", font1, sbrush_red, loaction.X + 5, loaction.Y + 65);
                }
                
               
            }
        }
    }
}
