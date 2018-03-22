using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace TopServer2k18
{
    class ImageProcessing
    {
        private static readonly CascadeClassifier classifier = new CascadeClassifier(@"classifier\haarcascade_frontalface_alt2.xml");
        private static readonly CascadeClassifier classifierRusPlateNumber = new CascadeClassifier(@"classifier\haarcascade_russian_plate_number.xml");
        private ConnectDataBase connectDataBase;

        public ImageProcessing()
        {
            connectDataBase = new ConnectDataBase();
        }

        public Image ByteArrayToImage(byte[] buffer, int count)
        {
            
            int width = BitConverter.ToInt32(buffer, 0);
            int height = BitConverter.ToInt32(buffer, 4);

            int it = 8;
            if (width == 320 && height == 240)
            {
                Bitmap bimg = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        bimg.SetPixel(j, i, Color.FromArgb(0, buffer[it + 2], buffer[it + 1], buffer[it + 0]));
                        it += 3;
                    }
                }
                Image<Bgr, byte> nextFrame = new Image<Bgr, byte>(bimg);
                Image<Gray, byte> grayImage = nextFrame.Convert<Gray, byte>();

                {


                    Rectangle[] rectangles = classifier.DetectMultiScale(grayImage, 1.1, 3, new Size(100, 100), new Size(width, height));
                    foreach (var rect in rectangles)
                    {
                        nextFrame.Draw(rect, new Bgr(Color.Blue), 2);

                        // В каждом квадрате ищем Олега
                        // ...

                        connectDataBase.Insert(false);
                    }
                }

                // 21.03.18 сверх секретное донесение от гестапо-олегдиев 
                // "ну для такого случая скручу номера"
                // "кек сук теперь таки реал номер принесу"
                {
                    Rectangle[] rectangles = classifierRusPlateNumber.DetectMultiScale(grayImage, 1.1, 3, new Size(100, 20), new Size(width, height));
                    foreach (var rect in rectangles)
                    {
                        nextFrame.Draw(rect, new Bgr(Color.Green), 2);

                        // В каждом квадрате ищем Олега ... номер от машины, да да для того что бы олег попал в дом, ему нужно скрутить свой номер с машины и показать на камеру...
                        // Peresvet228  - будующее в каждый дом!
                        connectDataBase.Insert(true, "Oleg Diev");
                    }
                }

                return nextFrame.ToBitmap();
            }

            return null;

        }
    }
}
