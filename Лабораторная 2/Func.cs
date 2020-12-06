using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV.OCR; //модуль оптического распознавания символов

namespace Лабораторная_2
{
    class Func
    {

        public Image<Gray, byte> bg = null;
        public Image<Bgr, byte> sourceImage; //глобальная переменная


        BackgroundSubtractorMOG2 subtractor = new BackgroundSubtractorMOG2(1000, 32, true);

        public void Source(string fileName)
        {
            sourceImage = new Image<Bgr, byte>(fileName);

        }

        private Image<Gray, byte> FilterMask(Image<Gray, byte> mask)
        {
            var anchor = new Point(-1, -1);
            var borderValue = new MCvScalar(1);
            // создание структурного элемента заданного размера и формы для морфологических операций
            var kernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(3, 3), anchor);
            // заполнение небольших тёмных областей
            var closing = mask.MorphologyEx(MorphOp.Close, kernel, anchor, 1, BorderType.Default,
            borderValue);
            // удаление шумов
            var opening = closing.MorphologyEx(MorphOp.Open, kernel, anchor, 1, BorderType.Default,
            borderValue);
            // расширение для слияния небольших смежных областей
            var dilation = opening.Dilate(7);
            // пороговое преобразование для удаления теней
            var threshold = dilation.ThresholdBinary(new Gray(240), new Gray(255));
            return threshold;
        }

        public Image<Bgr, byte> obl(Mat frame, int tb1)
        {
            Image<Gray, byte> cur = frame.ToImage<Gray, byte>();

            var foregroundMask = cur.CopyBlank();
            foregroundMask = FilterMask(foregroundMask);

            subtractor.Apply(cur, foregroundMask);

            foregroundMask._ThresholdBinary(new Gray(100), new Gray(255));

            foregroundMask.Erode(3);
            foregroundMask.Dilate(4);

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(foregroundMask, contours, null, RetrType.External, ChainApproxMethod.ChainApproxTc89L1);

            var output = frame.ToImage<Bgr, byte>().Copy();

            for (int i = 0; i < contours.Size; i++)
            {
                if (CvInvoke.ContourArea(contours[i]) > tb1) //игнорирование маленьких контуров
                {
                    Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
                    output.Draw(rect, new Bgr(Color.Blue), 1);
                }
            }
            return output;
        }


        public Image<Bgr, byte> obl2(Mat frame, int tb1)
        {

            Image<Gray, byte> cur = frame.ToImage<Gray, byte>();
            Image<Bgr, byte> curBgr = frame.ToImage<Bgr, byte>();

            if (bg == null) { bg = cur; }

            Image<Gray, byte> diff = bg.AbsDiff(cur);

            diff._ThresholdBinary(new Gray(100), new Gray(255));

            diff.Erode(3);
            diff.Dilate(4);

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();

            CvInvoke.FindContours(diff, contours, null, RetrType.External, ChainApproxMethod.ChainApproxTc89L1);

            var output = curBgr;

            for (int i = 0; i < contours.Size; i++)
            {
                if (CvInvoke.ContourArea(contours[i]) > tb1) //игнорирование маленьких контуров
                {
                    Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
                    output.Draw(rect, new Bgr(Color.Blue), 1);
                }
            }
            return output;
        }
    }
}