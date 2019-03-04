using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Text.RegularExpressions;
// compile with: /unsafe
namespace EE356P2JMD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Threading.DispatcherTimer T;

        public MainWindow()
        {
            InitializeComponent();
            T = new System.Windows.Threading.DispatcherTimer();
            T.Tick += new EventHandler(linkEvent);
        }
        
        private string rsource, lsource;
        private int stepCount;
        private int rowCounter;
        //BitmapImage image;

        public BitmapImage ImageSource { get; private set; }

        private void fileOpenLeftImageMenu_Click(object sender, RoutedEventArgs e)
        {
            //Window win = new Window();
            try
            {
                OpenFileDialog ofd = new OpenFileDialog()
                {
                    Filter = "Image Files (*.png)|*.png|All (*.*)|*.*"
                };
                ofd.RestoreDirectory = true;
                ofd.ShowDialog();

                ImageSource a = new BitmapImage(new Uri(ofd.FileName));
                leftImage.Source = a;
                lsource = a.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fileOpenRightImageMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog()
                {
                    Filter = "Image Files (*.png)|*.png|All (*.*)|*.*"
                };
                ofd.RestoreDirectory = true;
                ofd.ShowDialog();

                //rightImage.Source = new BitmapImage(new Uri(ofd.FileName));

                ImageSource imgSource = new BitmapImage(new Uri(ofd.FileName));
                rightImage.Source = imgSource;
                rsource = imgSource.ToString();
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void mergeButton_Click(object sender, RoutedEventArgs e)
        {

            if ((!string.IsNullOrWhiteSpace(stepBox.Text)) && (!string.IsNullOrWhiteSpace(incBox.Text)))
            {
                stepCount = Convert.ToInt32(stepBox.Text);
                rowCounter = Convert.ToInt32(incBox.Text);
            }
            else
            {
                rowCounter = 1;
                stepCount = 1;
            }
            try
            {
                if ((((BitmapImage)leftImage.Source).UriSource != null) && (((BitmapImage)rightImage.Source).UriSource != null))
                {
                   // MessageBox.Show("MergeStart");
                    T.Interval = new System.TimeSpan(0, 0, 0, 1, 0); //1 sec
                    T.Start();  
                }
            }
            catch
            {
                MessageBox.Show("Please load 2 pictures.");
            }
        }
        private void ProcessBitmap(string lsource, string rsource, int stepCount, int rowCounter)//, BitmapImage image)
        {
            //Assumes pictures are the same size and found in the c: directory
            //first pictures data is used interchangeably with the merged imaged
            //string url_1 = lsource;
            //string url_2 = rsource;
            string lString = "";
            string rString = "";
            //MessageBox.Show(lString);
            for (int i = 0; i < lsource.Length; i++)
            {
                if (lsource[i].Equals('C'))
                {
                    while (i < lsource.Length)
                    {
                        lString += lsource[i];
                        i++;
                    }

                }
            }
            for (int j = 0; j < rsource.Length; j++)
            {
                if (rsource[j].Equals('C'))
                {
                    while (j < rsource.Length)
                    {
                        rString += rsource[j];
                        j++;
                    }
                }
            }
            

            Bitmap bmp1 = new Bitmap(lString);
            Bitmap bmp2 = new Bitmap(rString);

            unsafe
            {
                BitmapData bitmapData1 = bmp1.LockBits(new System.Drawing.Rectangle(0, 0, bmp1.Width, bmp1.Height),
                                                            ImageLockMode.ReadWrite, bmp1.PixelFormat);
                BitmapData bitmapData2 = bmp2.LockBits(new System.Drawing.Rectangle(0, 0, bmp2.Width, bmp2.Height),
                                                            ImageLockMode.ReadWrite, bmp2.PixelFormat);
                int bytesPerPixel1 = System.Drawing.Bitmap.GetPixelFormatSize(bmp1.PixelFormat) / 8;
                int bytesPerPixel2 = System.Drawing.Bitmap.GetPixelFormatSize(bmp2.PixelFormat) / 8;
                int heightInPixels1 = bitmapData1.Height;
                int heightInPixels2 = bitmapData2.Height;
                int widthInBytes1 = bitmapData1.Width * bytesPerPixel1;
                int widthInBytes2 = bitmapData2.Width * bytesPerPixel2;
                byte* ptrFirstPixel1 = (byte*)bitmapData1.Scan0;
                byte* ptrFirstPixel2 = (byte*)bitmapData2.Scan0;

                for (int y = 0; y < heightInPixels1; y++)
                {
                    byte* currentLine1 = ptrFirstPixel1 + (y * bitmapData1.Stride);
                    byte* currentLine2 = ptrFirstPixel2 + (y * bitmapData2.Stride);
                    for (int x = 0; x < widthInBytes1; x = x + bytesPerPixel1)
                    {
                        int oldBlue1 = currentLine1[x];
                        int oldBlue2 = currentLine2[x];
                        int oldGreen1 = currentLine1[x + 1];
                        int oldGreen2 = currentLine2[x + 1];
                        int oldRed1 = currentLine1[x + 2];
                        int oldRed2 = currentLine2[x + 2];

                        currentLine2[x] = (byte)oldBlue2;
                        currentLine2[x + 1] = (byte)oldGreen2;
                        currentLine2[x + 2] = (byte)oldRed2;

                        //Merge Equation; 2nd picture merges into first
                        //calculate new pixel value
                        //currentLine1[x] = (byte)((oldBlue1 - OldBlue2)/stepCount);
                        currentLine1[x] = (byte)(oldBlue1 - rowCounter * (oldBlue1 - oldBlue2) / stepCount);
                        currentLine1[x + 1] = (byte)(oldGreen1 - rowCounter * (oldGreen1 - oldGreen2) / stepCount);
                        currentLine1[x + 2] = (byte)(oldRed1 - rowCounter * (oldRed1 - oldRed2) / stepCount);
                        //rowCounter++;



                    }
                }
                //txtbox.Text = rowCounter.ToString();
                bmp1.UnlockBits(bitmapData1);
                bmp2.UnlockBits(bitmapData2);
            }
            MemoryStream ms = new MemoryStream();
            bmp1.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            //return image;
            //Converter(bmp1);
            mergedImage.Source = image;

        }

        private void stepBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textbox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void aboutMenu_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("           Simple Image Merger" + Environment.NewLine +
              "           Jacob Darabaris - 2017" + Environment.NewLine +
              "              All rights reserved");
        }

        private void helpMenu_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("First load in two picture of the same dimensions using the File menu. " +
                "Then you can enter the number of steps the merger will use or leave blank. " +
                "Lastly click the 'Merge' button and watch! " +
                "You can also pause the process at any time and save the morphed image using the file menu." +
                Environment.NewLine + Environment.NewLine + "Contact Support: jd262@evansville.edu");
        }

        /*public BitmapImage Converter(Bitmap src)
        {
            
        }*/
        

        private void linkEvent(object sender, EventArgs e)
        {
            ProcessBitmap(lsource, rsource, stepCount++, rowCounter);//, image);
            //MessageBox.Show("MergeStart");
            if (stepCount == rowCounter + 1)
            {
                MessageBox.Show("MergeEnd");
                T.Stop();
            }
        }

        private void fileSaveMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                SaveFileDialog sfd = new SaveFileDialog()
                {
                    Filter = "Text Files(*.txt)|*.txt|All(*.*)|*"
                };

                if (sfd.ShowDialog() == true)
                {
                    var image = Clipboard.GetImage();
                    using (var fileStream = new FileStream("C:\\", FileMode.Create))
                    {
                        BitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(image));
                        encoder.Save(fileStream);
                    }

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            T.Stop();
        }

        private void fileExitMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
