using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;//引用文件读入读出程序集

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string[][] array;//定义一个二维数组存放导入的txt文件数据

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fils = new OpenFileDialog();//实例化
            fils.Filter = "文本文件|*.txt|所有文件|*.*";
            if (fils.ShowDialog() == DialogResult.OK)
            {
                string sfil = fils.FileName;//定义存储文件名的字符串
                try
                {
                    FileStream file = new FileStream(sfil, FileMode.Open);
                    StreamReader sr = new StreamReader(file, Encoding.GetEncoding("gb2312"));
                    string s = sr.ReadLine();//将sr中的读取到s中，读取一个文本行
                    textBox1.Clear();//清空文本框值
                    while (s != null)//直到文本行为空时结束
                    {
                        textBox1.Text += s + "\r\n";//显示，遇到\r\n（回车换行符），该行结束并返回
                        s = sr.ReadLine();//继续读取一个文本行
                    }
                    sr.Close();//关闭文件流
                }
                catch { }
            }
            try
            {
                string sm = textBox1.Text;//定义textbox中的文件
                string[] t1 = sm.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                int b = t1.GetLength(0);//获取行数
                List<string[]> t2 = new List<string[]>();
                for (int i = 0; i < b; i++)//按行读取
                {
                    t2.Add(t1[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));//按空格分隔读取数据
                }
                array = t2.ToArray();//将t2转换为数组
            }
            catch { MessageBox.Show("文件内容有误！"); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double f = double.Parse(array[8][0]) * 0.001;//摄影机主距

            JZ xyf1 = new JZ(3, 6);
            JZ xyf2 = new JZ(3, 6);
            for (int i = 0; i <6; i++)
            {
                xyf1[0, i] = double.Parse(array[i + 1][1]) / 1000;
                xyf1[1, i] = double.Parse(array[i + 1][2]) / 1000;
                xyf1[2, i] = -f;
                xyf2[0, i] = double.Parse(array[i + 1][3]) / 1000;
                xyf2[1, i] = double.Parse(array[i + 1][4]) / 1000;
                xyf2[2, i] = -f;
            }//导入像平面坐标
           
            double t0 = 0, m0 = 0, v0 = 0, f0 = 0, w0 = 0, k0 = 0, by = 0, bz = 0;//定义最初值

            JZ R1 = new JZ(3, 3);
            JZ R2 = new JZ(3, 3);
            //R1
            R1[0, 0] = Math.Cos(t0) * Math.Cos(v0) - Math.Sin(t0) * Math.Sin(m0) * Math.Sin(v0);
            R1[0, 1] = -Math.Cos(t0) * Math.Sin(v0) - Math.Sin(t0) * Math.Sin(m0) * Math.Cos(v0);
            R1[0, 2] = -Math.Sin(t0) * Math.Cos(m0);
            R1[1, 0] = Math.Cos(m0) * Math.Sin(v0);
            R1[1, 1] = Math.Cos(m0) * Math.Cos(v0);
            R1[1, 2] = -Math.Sin(m0);
            R1[2, 0] = Math.Sin(t0) * Math.Cos(v0) + Math.Cos(t0) * Math.Sin(m0) * Math.Sin(v0);
            R1[2, 1] = -Math.Sin(t0) * Math.Sin(v0) + Math.Cos(t0) * Math.Sin(m0) * Math.Cos(v0);
            R1[2, 2] = Math.Cos(t0) * Math.Cos(m0);//计算旋转矩阵各元素值

            //R2
            R2[0, 0] = Math.Cos(f0) * Math.Cos(k0) - Math.Sin(f0) * Math.Sin(w0) * Math.Sin(k0);
            R2[0, 1] = -Math.Cos(f0) * Math.Sin(k0) - Math.Sin(f0) * Math.Sin(w0) * Math.Cos(k0);
            R2[0, 2] = -Math.Sin(f0) * Math.Cos(w0);
            R2[1, 0] = Math.Cos(w0) * Math.Sin(k0);
            R2[1, 1] = Math.Cos(w0) * Math.Cos(k0);
            R2[1, 2] = -Math.Sin(w0);
            R2[2, 0] = Math.Sin(f0) * Math.Cos(k0) + Math.Cos(f0) * Math.Sin(w0) * Math.Sin(k0);
            R2[2, 1] = -Math.Sin(f0) * Math.Sin(k0) + Math.Cos(f0) * Math.Sin(w0) * Math.Cos(k0);
            R2[2, 2] = Math.Cos(f0) * Math.Cos(w0);//计算旋转矩阵各元素值 

            JZ XYZ1 = new JZ(3, 6);
            JZ XYZ2 = new JZ(3, 6);
            XYZ1 = R1.Multiply(xyf1);
            XYZ2 = R2.Multiply(xyf2);
            double bx = xyf1[0, 0] - xyf2[0, 0];

            JZ N = new JZ(2, 6);
            JZ X = new JZ(5, 1);
            JZ Q = new JZ(6, 1);
            JZ A = new JZ(6, 5);
            do
            {
                R2[0, 0] = Math.Cos(f0) * Math.Cos(k0) - Math.Sin(f0) * Math.Sin(w0) * Math.Sin(k0);
                R2[0, 1] = -Math.Cos(f0) * Math.Sin(k0) - Math.Sin(f0) * Math.Sin(w0) * Math.Cos(k0);
                R2[0, 2] = -Math.Sin(f0) * Math.Cos(w0);
                R2[1, 0] = Math.Cos(w0) * Math.Sin(k0);
                R2[1, 1] = Math.Cos(w0) * Math.Cos(k0);
                R2[1, 2] = -Math.Sin(w0);
                R2[2, 0] = Math.Sin(f0) * Math.Cos(k0) + Math.Cos(f0) * Math.Sin(w0) * Math.Sin(k0);
                R2[2, 1] = -Math.Sin(f0) * Math.Sin(k0) + Math.Cos(f0) * Math.Sin(w0) * Math.Cos(k0);
                R2[2, 2] = Math.Cos(f0) * Math.Cos(w0);//计算旋转矩阵各元素值
                XYZ2 = R2.Multiply(xyf2);

                
                by = bx * m0;
                bz = bx * v0;
                for (int i = 0; i < 6; i++)
                {
                   
                    //计算点投影系数
                    N[0, i] = (bx * XYZ2[2, i] - bz * XYZ2[0, i]) / (XYZ1[0, i] * XYZ2[2, i] - XYZ2[0, i] * XYZ1[2, i]);
                    N[1, i] = (bx * XYZ1[2, i] - bz * XYZ1[0, i]) / (XYZ1[0, i] * XYZ2[2, i] - XYZ2[0, i] * XYZ1[2, i]);

                    Q[i, 0] = N[0, i] * XYZ1[1, i] - N[1, i] * XYZ2[1, i] - by;

                    A[i, 0] = bx;
                    A[i, 1] = -bx * XYZ2[1, i] / XYZ2[2, i];
                    A[i, 2] = -N[1, i] * XYZ2[0, i] * XYZ2[1, i] / XYZ2[2, i];
                    A[i, 3] = -N[1, i] * (XYZ2[2, i] + XYZ2[1, i] * XYZ2[1, i] / XYZ2[2, i]);
                    A[i, 4] = XYZ2[0, i] * N[1, i];
                }

                JZ AT = new JZ();
                JZ ATL = new JZ();
                JZ ATA = new JZ();
                JZ ATAn = new JZ();
                JZ ATAnAT = new JZ();
                AT = A.Transpose();
                ATA = AT.Multiply(A);
                ATAn = ATA.InvertGaussJordan();
                ATAnAT = ATAn.Multiply(AT);
                X = ATAnAT.Multiply(Q);

                m0 = m0 + X[0, 0];
                v0 = v0 + X[1, 0];
                f0 = f0 + X[2, 0];
                w0 = w0 + X[3, 0];
                k0 = k0 + X[4, 0];

            } while (Math.Abs(X[0, 0]) >= 0.00003 || Math.Abs(X[1, 0]) >= 0.00003 || Math.Abs(X[2, 0]) >= 0.00003 || Math.Abs(X[3, 0]) >= 0.00003 || Math.Abs(X[4, 0]) >= 0.00003);

            double m;//求中误差
            double[] mk = new double[5]; double l;
            JZ T = new JZ(); T = (A.Multiply(X) - Q).Transpose().Multiply(A.Multiply(X) - Q);
            l = T[0, 0];
            m = Math.Sqrt(l / (6 - 5));
            JZ P = new JZ(6, 6);
            P = A.Transpose().Multiply(A).InvertGaussJordan();
            for (int t = 0; t < 5; t++)
            {
                mk[t] = (Math.Sqrt(P[t, t])) * m;
            }



            //textBox2.Text += "m: " + m0.ToString() + "\r\n";
            //textBox2.Text += "v: " + v0.ToString() + "\r\n";
            //textBox2.Text += "f: " + f0.ToString() + "\r\n";
            //textBox2.Text += "w: " + w0.ToString() + "\r\n";
            //textBox2.Text += "k: " + k0.ToString() + "\r\n";

            //textBox2.Text += "m的精度: " + mk[0].ToString() + "\r\n";
            //textBox2.Text += "v的精度: " + mk[1].ToString() + "\r\n";
            //textBox2.Text += "f的精度: " + mk[2].ToString() + "\r\n";
            //textBox2.Text += "w的精度: " + mk[3].ToString() + "\r\n";
            //textBox2.Text += "k的精度: " + mk[4].ToString() + "\r\n";

            JZ XYZm = new JZ(3, 6);//计算模型点坐标
            for (int i=0;i<6;i++)
            {
                XYZm[0, i] = N[0, i] * XYZ1[0, i];
                XYZm[1, i] = 0.5 * (N[0, i] * XYZ1[1, i] + N[1, i] * XYZ2[1, i] + bx * m0);
                XYZm[2, i] = N[0, i] * XYZ1[2, i];
            }

            int mm = 37000;//像片比例尺分母
            JZ XYZp = new JZ(3, 6);//计算模型点的摄影测量坐标
            for (int i = 0; i < 6; i++)
            {
                XYZp[0, i] = mm * N[0, i] * XYZ1[0, i];
                XYZp[1, i] = 0.5 * (N[0, i] * XYZ1[1, i] + N[1, i] * XYZ2[1, i] + bx * m0) * mm;
                XYZp[2, i] = mm * f + mm * N[0, i] * XYZ1[2, i];
            }

            JZ XYZtp = new JZ(3, 6);//计算各点的地面摄影测量坐标
            JZ R = new JZ(3, 3);
            JZ dXYZ = new JZ(3, 6);
            double f00 = 0.0527; double w00 = 0.1426; double k00 = 0.2478;

            double numda = 1.00156;
           
            R[0, 0] = Math.Cos(f00) * Math.Cos(k00) - Math.Sin(f00) * Math.Sin(w00) * Math.Sin(k00);
            R[0, 1] = -Math.Cos(f00) * Math.Sin(k00) - Math.Sin(f00) * Math.Sin(w00) * Math.Cos(k00);
            R[0, 2] = -Math.Sin(f00) * Math.Cos(w00);
            R[1, 0] = Math.Cos(w00) * Math.Sin(k00);
            R[1, 1] = Math.Cos(w00) * Math.Cos(k00);
            R[1, 2] = -Math.Sin(w00);
            R[2, 0] = Math.Sin(f00) * Math.Cos(k00) + Math.Cos(f00) * Math.Sin(w00) * Math.Sin(k00);
            R[2, 1] = -Math.Sin(f00) * Math.Sin(k00) + Math.Cos(f00) * Math.Sin(w00) * Math.Cos(k00);
            R[2, 2] = Math.Cos(f00) * Math.Cos(w00);

            for (int i=0;i<6;i++)
            {
                dXYZ[0, i] = 6385.067;
                dXYZ[1, i] = 1954.325;
                dXYZ[2, i] = 724.215;
            }
            XYZtp = R.Multiply(numda).Multiply(XYZp).Add(dXYZ);

            textBox2.Text += "模型点坐标: " + "\r\n";
            for (int i = 0; i < 6; i++)
            {
                textBox2.Text += "(" + XYZm[0, i].ToString() + "," + XYZm[1, i].ToString() + "," + XYZm[2, i].ToString() + ")" + "\r\n";
            }
            textBox2.Text += "模型点的摄影测量坐标: " + "\r\n";
            for (int i = 0; i < 6; i++)
            {
                textBox2.Text += "(" + XYZp[0, i].ToString() + "," + XYZp[1, i].ToString() + "," + XYZp[2, i].ToString() + ")" + "\r\n";
            }
            textBox2.Text += "各点的地面摄影测量坐标: " + "\r\n";
            for (int i = 0; i < 6; i++)
            {
                textBox2.Text += "(" + XYZtp[0, i].ToString() + "," + XYZtp[1, i].ToString() + "," + XYZtp[2, i].ToString() + ")" + "\r\n";
            }


        }
    }
}
