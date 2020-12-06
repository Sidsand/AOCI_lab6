using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace Лабораторная_2
{
    public partial class Form1 : Form
    {
        public VideoCapture capture;
        
        private Func func = new Func();

        int frameCounter = 0;
        
        public Form1()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog(); // открытие диалога выбора файла
            if (result == DialogResult.OK) // открытие выбранного файла
            {
                string fileName = openFileDialog.FileName;

                capture = new VideoCapture(fileName);
                timer1.Enabled = true;
            }
        }
        
        private void button4_Click(object sender, EventArgs e)
        {
            capture = new VideoCapture();
            capture.ImageGrabbed += ProcessFrame;
            capture.Start();
        }
        
        private void ProcessFrame(object sender, EventArgs e)
        {
            int tb1 = trackBar1.Value;
            var frame = new Mat();
            capture.Retrieve(frame);
            if (func.bg != null)
            {
                imageBox1.Image = func.obl(frame, tb1);
                imageBox2.Image = func.obl2(frame, tb1);
            }
            else
            {
                imageBox1.Image = frame;
                imageBox2.Image = frame;
            }
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            var frame = new Mat();
            capture.Retrieve(frame);
            
            func.bg = frame.ToImage<Gray, byte>();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int tb1 = trackBar1.Value;
            var frame = capture.QueryFrame();
            imageBox1.Image = func.obl(frame, tb1);
            imageBox2.Image = func.obl2(frame, tb1);

            frameCounter++;
            if (frameCounter >= capture.GetCaptureProperty(CapProp.FrameCount))
            {
                frameCounter = 0;
                timer1.Enabled = false;
            }
        }
    }
}
