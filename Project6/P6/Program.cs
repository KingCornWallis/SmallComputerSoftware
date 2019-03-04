using System;
using System.Collections;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Display;
using System.Drawing;
using TinyCLRApplication1.Properties;
using GHIElectronics.TinyCLR.Devices.Gpio;

namespace TinyCLRApplication1
{
    class Program
    {
        static Graphics screen;
        static GpioPin led = GpioController.GetDefault().OpenPin(
        GHIElectronics.TinyCLR.Pins.G400D.GpioPin.PC18);
        private static string[] tetriminos = { "O", "I", "L", "J", "T", "S", "Z" };
        private static Color[] colors = { Color.Yellow, Color.Teal, Color.White, Color.Blue, Color.Purple, Color.Green, Color.Red };
        //private static Random random = new Random();
        private static int x = 4;
        private static int rotation = 0;

        static void Main()
        {

            var displayController = DisplayController.GetDefault();

            SolidBrush mine = new SolidBrush(Color.Red);
            var font = Resources.GetFont(Resources.FontResources.timesnew);
            led.SetDriveMode(GpioPinDriveMode.Output);



            displayController.ApplySettings(new LcdControllerSettings
            {
                Width = 480,
                Height = 272,
                PixelClockRate = 20000000,
                PixelPolarity = false,
                OutputEnablePolarity = true,
                OutputEnableIsFixed = false,
                HorizontalFrontPorch = 2,
                HorizontalBackPorch = 2,
                HorizontalSyncPulseWidth = 41,
                HorizontalSyncPolarity = false,
                VerticalFrontPorch = 2,
                VerticalBackPorch = 2,
                VerticalSyncPulseWidth = 10,
                VerticalSyncPolarity = false,
            });
            // Some needed objects
            screen = Graphics.FromHdc(displayController.Hdc);
            var GreenPen = new Pen(Color.Green);
            // Start Drawing (to memroy)
            screen.Clear(Color.Black);
            screen.DrawImage(Resources.GetBitmap(Resources.BitmapResources.Hello), (480 / 2) - (272 / 2), 0);
            //screen.DrawEllipse(GreenPen, 10, 10, 20, 10);
            //screen.DrawString("Using TinyCLR 0.6.0",font, mine, 20, 250);
            // Flush the memory to the display. This is a very fast operation.
            screen.Flush();
            GpioPin padUp = GpioController.GetDefault().OpenPin(
            GHIElectronics.TinyCLR.Pins.G400D.GpioPin.PA24);
            GpioPin padDown = GpioController.GetDefault().OpenPin(
            GHIElectronics.TinyCLR.Pins.G400D.GpioPin.PA4);
            GpioPin padRight = GpioController.GetDefault().OpenPin(
            GHIElectronics.TinyCLR.Pins.G400D.GpioPin.PD9);
            GpioPin padLeft = GpioController.GetDefault().OpenPin(
            GHIElectronics.TinyCLR.Pins.G400D.GpioPin.PD7);
            int locationX = 480 / 2;
            Thread.Sleep(1000);

            while (true)
            {

                if (padRight.Read() == 0)
                {
                    if (locationX <= 480 - 50)
                        locationX += 20;
                }
                else if (padLeft.Read() == 0)
                {
                    if (locationX > 0 - 20)
                        locationX -= 20;

                }
                else if (padDown.Read() == 0)
                {
                    if (locationY > 0)
                        locationY += 10;
                }
                else if (padUp.Read() == 0)
                {
                    if (rotation > 270)
                        rotation = 0;
                    else
                        rotation += 90;
                    DetermineRotation(colors, tetriminos, x, locationX, rotation);
                }

                UpdateScreen(locationX, rotation);
                Thread.Sleep(200);
            }
        }
        static bool ledValue = false;
        static int locationY = -20;
        static void UpdateScreen(int locationX, int rotation)
        {

            var BorderPen = new Pen(Color.Gray, 3);
            var GreenFiller = new Pen(Color.Green, 22);
            var RedPen = new Pen(Color.Red);
            screen.Clear(Color.Black);
            if (locationY < 250)
            {

                locationY += 20;
                locationX += 20;
                if (rotation > 0)
                    DetermineRotation(colors, tetriminos, x, locationX, rotation);
                else
                    DetermineShape(colors, tetriminos, x, locationX);

            }

            else
            {
                if (x < 7)
                    x += 1;
                else
                    x = 0;
                Thread.Sleep(50);
                //screen.DrawRectangle(RedPen, locationX, locationY, 20, 20);
                
                locationY = -20;
            }//Rectangle.System.Windows.Shapes


            screen.Flush();

            if (!ledValue)
            {
                led.Write(GpioPinValue.High);
                ledValue = true;
            }
            else
            {
                led.Write(GpioPinValue.Low);
                ledValue = false;
            }

        }

        static void DetermineShape(Color[] colors, string[] tetriminos, int random, int locationX)
        {
            var BorderPen = new Pen(Color.Gray, 5);
            var FillerPen = new Pen(colors[random], 20);

            if (tetriminos[random] == "O")
            {
                screen.DrawLine(FillerPen, locationX + 10, locationY + 10, locationX + 10, locationY + 10);
                screen.DrawLine(FillerPen, locationX + 30, locationY + 10, locationX + 30, locationY + 10);
                screen.DrawLine(FillerPen, locationX + 10, locationY + 30, locationX + 10, locationY + 30);
                screen.DrawLine(FillerPen, locationX + 30, locationY + 30, locationX + 30, locationY + 30);
                screen.DrawRectangle(BorderPen, locationX, locationY, 20, 20);
                screen.DrawRectangle(BorderPen, locationX + 20, locationY, 20, 20);
                screen.DrawRectangle(BorderPen, locationX, locationY + 20, 20, 20);
                screen.DrawRectangle(BorderPen, locationX + 20, locationY + 20, 20, 20);
            }
            if (tetriminos[random] == "I")
            {
                screen.DrawLine(FillerPen, locationX + 10, locationY + 10, locationX + 10, locationY + 10);
                screen.DrawLine(FillerPen, locationX + 10, locationY + 30, locationX + 10, locationY + 30);
                screen.DrawLine(FillerPen, locationX + 10, locationY + 50, locationX + 10, locationY + 50);
                screen.DrawLine(FillerPen, locationX + 10, locationY + 70, locationX + 10, locationY + 70);
                screen.DrawRectangle(BorderPen, locationX, locationY, 20, 20);
                screen.DrawRectangle(BorderPen, locationX, locationY + 20, 20, 20);
                screen.DrawRectangle(BorderPen, locationX, locationY + 40, 20, 20);
                screen.DrawRectangle(BorderPen, locationX, locationY + 60, 20, 20);
            }
            if (tetriminos[random] == "L")
            {
                screen.DrawLine(FillerPen, locationX + 10, locationY + 10, locationX + 10, locationY + 10);
                screen.DrawLine(FillerPen, locationX + 10, locationY + 30, locationX + 10, locationY + 30);
                screen.DrawLine(FillerPen, locationX + 10, locationY + 50, locationX + 10, locationY + 50);
                screen.DrawLine(FillerPen, locationX + 30, locationY + 50, locationX + 30, locationY + 50);
                screen.DrawRectangle(BorderPen, locationX, locationY, 20, 20);
                screen.DrawRectangle(BorderPen, locationX, locationY + 20, 20, 20);
                screen.DrawRectangle(BorderPen, locationX, locationY + 40, 20, 20);
                screen.DrawRectangle(BorderPen, locationX + 20, locationY + 40, 20, 20);
            }
            if (tetriminos[random] == "J")
            {
                screen.DrawLine(FillerPen, locationX + 10, locationY + 10, locationX + 10, locationY + 10);
                screen.DrawLine(FillerPen, locationX + 10, locationY + 30, locationX + 10, locationY + 30);
                screen.DrawLine(FillerPen, locationX + 10, locationY + 50, locationX + 10, locationY + 50);
                screen.DrawLine(FillerPen, locationX - 10, locationY + 50, locationX - 10, locationY + 50);
                screen.DrawRectangle(BorderPen, locationX, locationY, 20, 20);
                screen.DrawRectangle(BorderPen, locationX, locationY + 20, 20, 20);
                screen.DrawRectangle(BorderPen, locationX, locationY + 40, 20, 20);
                screen.DrawRectangle(BorderPen, locationX - 20, locationY + 40, 20, 20);
            }
            if (tetriminos[random] == "T")
            {
                screen.DrawLine(FillerPen, locationX + 30, locationY + 10, locationX + 30, locationY + 10);
                screen.DrawLine(FillerPen, locationX + 30, locationY + 30, locationX + 30, locationY + 30);
                screen.DrawLine(FillerPen, locationX + 10, locationY + 30, locationX + 10, locationY + 30);
                screen.DrawLine(FillerPen, locationX + 50, locationY + 30, locationX + 50, locationY + 30);
                screen.DrawRectangle(BorderPen, locationX + 20, locationY, 20, 20);
                screen.DrawRectangle(BorderPen, locationX + 20, locationY + 20, 20, 20);
                screen.DrawRectangle(BorderPen, locationX, locationY + 20, 20, 20);
                screen.DrawRectangle(BorderPen, locationX + 40, locationY + 20, 20, 20);
            }
            if (tetriminos[random] == "S")
            {
                screen.DrawLine(FillerPen, locationX + 10, locationY + 10, locationX + 10, locationY + 10);
                screen.DrawLine(FillerPen, locationX + 10, locationY + 30, locationX + 10, locationY + 30);
                screen.DrawLine(FillerPen, locationX - 10, locationY + 30, locationX - 10, locationY + 30);
                screen.DrawLine(FillerPen, locationX + 30, locationY + 10, locationX + 30, locationY + 10);
                screen.DrawRectangle(BorderPen, locationX, locationY, 20, 20);
                screen.DrawRectangle(BorderPen, locationX, locationY + 20, 20, 20);
                screen.DrawRectangle(BorderPen, locationX - 20, locationY + 20, 20, 20);
                screen.DrawRectangle(BorderPen, locationX + 20, locationY, 20, 20);
            }
            if (tetriminos[random] == "Z")
            {
                screen.DrawLine(FillerPen, locationX + 10, locationY + 10, locationX + 10, locationY + 10);
                screen.DrawLine(FillerPen, locationX + 10, locationY + 30, locationX + 10, locationY + 30);
                screen.DrawLine(FillerPen, locationX - 10, locationY + 10, locationX - 10, locationY + 10);
                screen.DrawLine(FillerPen, locationX + 30, locationY + 30, locationX + 30, locationY + 30);
                screen.DrawRectangle(BorderPen, locationX, locationY, 20, 20);
                screen.DrawRectangle(BorderPen, locationX, locationY + 20, 20, 20);
                screen.DrawRectangle(BorderPen, locationX - 20, locationY, 20, 20);
                screen.DrawRectangle(BorderPen, locationX + 20, locationY + 20, 20, 20);
            }
        }
        static void DetermineRotation(Color[] colors, string[] tetriminos, int random, int locationX, int rotation)
        {
            var BorderPen = new Pen(Color.Gray, 5);
            var FillerPen = new Pen(colors[random], 20);

            if (tetriminos[random] == "O")
            {
                DetermineShape(colors, tetriminos, x, locationX);
            }
            if (tetriminos[random] == "I")
            {
                if (rotation == 90 || rotation == 270)
                {
                    screen.DrawLine(FillerPen, locationX + 10, locationY + 10, locationX + 10, locationY + 10);
                    screen.DrawLine(FillerPen, locationX + 30, locationY + 10, locationX + 30, locationY + 10);
                    screen.DrawLine(FillerPen, locationX + 50, locationY + 10, locationX + 50, locationY + 10);
                    screen.DrawLine(FillerPen, locationX + 70, locationY + 10, locationX + 70, locationY + 10);
                    screen.DrawRectangle(BorderPen, locationX, locationY, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 20, locationY, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 40, locationY, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 60, locationY, 20, 20);
                }
                else
                    DetermineShape(colors, tetriminos, x, locationX);
            }
            if (tetriminos[random] == "L")
            {
                if (rotation == 90)
                {
                    screen.DrawLine(FillerPen, locationX + 10, locationY + 10, locationX + 10, locationY + 10);
                    screen.DrawLine(FillerPen, locationX + 30, locationY + 10, locationX + 30, locationY + 10);
                    screen.DrawLine(FillerPen, locationX + 50, locationY + 10, locationX + 50, locationY + 10);
                    screen.DrawLine(FillerPen, locationX + 10, locationY + 30, locationX + 10, locationY + 30);
                    screen.DrawRectangle(BorderPen, locationX, locationY, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 20, locationY, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 40, locationY, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX, locationY + 20, 20, 20);
                }
                else if (rotation == 180)
                {
                    screen.DrawLine(FillerPen, locationX + 10, locationY + 10, locationX + 10, locationY + 10);
                    screen.DrawLine(FillerPen, locationX + 10, locationY + 30, locationX + 10, locationY + 30);
                    screen.DrawLine(FillerPen, locationX + 10, locationY + 50, locationX + 10, locationY + 50);
                    screen.DrawLine(FillerPen, locationX - 10, locationY + 10, locationX - 10, locationY + 10);
                    screen.DrawRectangle(BorderPen, locationX, locationY, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX, locationY + 20, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX, locationY + 40, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX - 20, locationY, 20, 20);
                }
                else if (rotation == 270)
                {
                    screen.DrawLine(FillerPen, locationX + 10, locationY + 10, locationX + 10, locationY + 10);
                    screen.DrawLine(FillerPen, locationX + 30, locationY + 10, locationX + 30, locationY + 10);
                    screen.DrawLine(FillerPen, locationX + 50, locationY + 10, locationX + 50, locationY + 10);
                    screen.DrawLine(FillerPen, locationX + 50, locationY - 10, locationX + 50, locationY - 10);
                    screen.DrawRectangle(BorderPen, locationX, locationY, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 20, locationY, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 40, locationY, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 40, locationY - 20, 20, 20);
                }
                else
                    DetermineShape(colors, tetriminos, x, locationX);
            }
            if (tetriminos[random] == "J")
            {
                if (rotation == 90)
                {
                    screen.DrawLine(FillerPen, locationX + 10, locationY + 10, locationX + 10, locationY + 10);
                    screen.DrawLine(FillerPen, locationX + 30, locationY + 10, locationX + 30, locationY + 10);
                    screen.DrawLine(FillerPen, locationX + 50, locationY + 10, locationX + 50, locationY + 10);
                    screen.DrawLine(FillerPen, locationX + 10, locationY - 10, locationX + 10, locationY - 10);
                    screen.DrawRectangle(BorderPen, locationX, locationY, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 20, locationY, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 40, locationY, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX, locationY - 20, 20, 20);
                }
                else if (rotation == 180)
                {
                    screen.DrawLine(FillerPen, locationX + 10, locationY + 10, locationX + 10, locationY + 10);
                    screen.DrawLine(FillerPen, locationX + 10, locationY + 30, locationX + 10, locationY + 30);
                    screen.DrawLine(FillerPen, locationX + 10, locationY + 50, locationX + 10, locationY + 50);
                    screen.DrawLine(FillerPen, locationX + 30, locationY + 10, locationX + 30, locationY + 10);
                    screen.DrawRectangle(BorderPen, locationX, locationY, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX, locationY + 20, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX, locationY + 40, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 20, locationY, 20, 20);
                }
                else if (rotation == 270)
                {
                    screen.DrawLine(FillerPen, locationX + 10, locationY + 10, locationX + 10, locationY + 10);
                    screen.DrawLine(FillerPen, locationX + 30, locationY + 10, locationX + 30, locationY + 10);
                    screen.DrawLine(FillerPen, locationX + 50, locationY + 10, locationX + 50, locationY + 10);
                    screen.DrawLine(FillerPen, locationX + 50, locationY + 30, locationX + 50, locationY + 30);
                    screen.DrawRectangle(BorderPen, locationX, locationY, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 20, locationY, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 40, locationY, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 40, locationY + 20, 20, 20);
                }
                else
                    DetermineShape(colors, tetriminos, x, locationX);
            }
            if (tetriminos[random] == "T")
            {
                
                if (rotation == 90)
                {
                    screen.DrawLine(FillerPen, locationX + 30, locationY + 10, locationX + 30, locationY + 10);
                    screen.DrawLine(FillerPen, locationX + 30, locationY + 30, locationX + 30, locationY + 30);
                    screen.DrawLine(FillerPen, locationX + 30, locationY + 50, locationX + 30, locationY + 50);
                    screen.DrawLine(FillerPen, locationX + 50, locationY + 30, locationX + 50, locationY + 30);
                    screen.DrawRectangle(BorderPen, locationX + 20, locationY, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 20, locationY + 20, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 20, locationY + 40, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 40, locationY + 20, 20, 20);
                }
                if (rotation == 180)
                {
                    screen.DrawLine(FillerPen, locationX + 10, locationY + 30, locationX + 10, locationY + 30);
                    screen.DrawLine(FillerPen, locationX + 30, locationY + 30, locationX + 30, locationY + 30);
                    screen.DrawLine(FillerPen, locationX + 30, locationY + 50, locationX + 30, locationY + 50);
                    screen.DrawLine(FillerPen, locationX + 50, locationY + 30, locationX + 50, locationY + 30);
                    screen.DrawRectangle(BorderPen, locationX, locationY + 20, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 20, locationY + 20, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 20, locationY + 40, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 40, locationY + 20, 20, 20);
                }
                if (rotation == 270)
                {
                    screen.DrawLine(FillerPen, locationX + 30, locationY + 10, locationX + 30, locationY + 10);
                    screen.DrawLine(FillerPen, locationX + 30, locationY + 30, locationX + 30, locationY + 30);
                    screen.DrawLine(FillerPen, locationX + 30, locationY + 50, locationX + 30, locationY + 50);
                    screen.DrawLine(FillerPen, locationX + 10, locationY + 30, locationX + 10, locationY + 30);
                    screen.DrawRectangle(BorderPen, locationX + 20, locationY, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 20, locationY + 20, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX + 20, locationY + 40, 20, 20);
                    screen.DrawRectangle(BorderPen, locationX, locationY + 20, 20, 20);
                }
                else
                    DetermineShape(colors, tetriminos, x, locationX);
            }
            if (tetriminos[random] == "S")
            {
                screen.DrawLine(FillerPen, locationX + 10, locationY + 10, locationX + 10, locationY + 10);
                screen.DrawLine(FillerPen, locationX + 10, locationY + 30, locationX + 10, locationY + 30);
                screen.DrawLine(FillerPen, locationX - 10, locationY + 30, locationX - 10, locationY + 30);
                screen.DrawLine(FillerPen, locationX + 30, locationY + 10, locationX + 30, locationY + 10);
                screen.DrawRectangle(BorderPen, locationX, locationY, 20, 20);
                screen.DrawRectangle(BorderPen, locationX, locationY + 20, 20, 20);
                screen.DrawRectangle(BorderPen, locationX - 20, locationY + 20, 20, 20);
                screen.DrawRectangle(BorderPen, locationX + 20, locationY, 20, 20);
            }
            if (tetriminos[random] == "Z")
            {
                screen.DrawLine(FillerPen, locationX + 10, locationY + 10, locationX + 10, locationY + 10);
                screen.DrawLine(FillerPen, locationX + 10, locationY + 30, locationX + 10, locationY + 30);
                screen.DrawLine(FillerPen, locationX - 10, locationY + 10, locationX - 10, locationY + 10);
                screen.DrawLine(FillerPen, locationX + 30, locationY + 30, locationX + 30, locationY + 30);
                screen.DrawRectangle(BorderPen, locationX, locationY, 20, 20);
                screen.DrawRectangle(BorderPen, locationX, locationY + 20, 20, 20);
                screen.DrawRectangle(BorderPen, locationX - 20, locationY, 20, 20);
                screen.DrawRectangle(BorderPen, locationX + 20, locationY + 20, 20, 20);
            }
        }
    }
}