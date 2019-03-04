using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace EE356P1jd262
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private Random ru = new Random();

        private int winWidth, winHeight;
        private int penColor = 0;
        private const int NUMPOINTS = 400;
   
        private void btnDraw_Click(object sender, RoutedEventArgs e)
        {
            double[,] data = new double[NUMPOINTS, 2]; //Data to be plotted.
            double[,] ellipseData = new double[NUMPOINTS, 2];
            int i;
            double x, y, x1, y1;
            int nuX, nuY;
            double thickness;
            winWidth = (int)imgPlot.Width;
            winHeight = (int)imgPlot.Height;
            bool colorError = false;
            if (!string.IsNullOrWhiteSpace(seedBox.Text))
            {
                errorBlock.Text = "";
                /*if (string.IsNullOrWhiteSpace(seedBox.Text))
                {
                    ru = new Random();
                }*/
                ru = new Random(Convert.ToInt32(seedBox.Text));

                Pen[] penArray = new Pen[9];
                Brush[] brushArray = new Brush[9];

                DrawingVisual vis = new DrawingVisual();
                DrawingContext dc = vis.RenderOpen();
                GetRandom(data);
                //offsetx = winWidth/2;offsety = winHeight/2;

                colorError = true;
                if (Black.IsChecked == true)
                {
                    penArray[0] = new Pen(Brushes.Black, 1);
                    brushArray[0] = Brushes.Black;
                    colorError = false;
                }
                if (Blue.IsChecked == true)
                {
                    penArray[1] = new Pen(Brushes.Blue, 1);
                    brushArray[1] = Brushes.Blue;
                    colorError = false;
                }
                if (Red.IsChecked == true)
                {
                    penArray[2] = new Pen(Brushes.Red, 1);
                    brushArray[2] = Brushes.Red;
                    colorError = false;
                }
                if (Green.IsChecked == true)
                {
                    penArray[3] = new Pen(Brushes.Green, 1);
                    brushArray[3] = Brushes.Green;
                    colorError = false;
                }
                if (Yellow.IsChecked == true)
                {
                    penArray[4] = new Pen(Brushes.Yellow, 1);
                    brushArray[4] = Brushes.Yellow;
                    colorError = false;
                }
                if (Orange.IsChecked == true)
                {
                    penArray[5] = new Pen(Brushes.Orange, 1);
                    brushArray[5] = Brushes.Orange;
                    colorError = false;
                }
                if (Pink.IsChecked == true)
                {
                    penArray[6] = new Pen(Brushes.Pink, 1);
                    brushArray[6] = Brushes.Pink;
                    colorError = false;
                }
                if (Purple.IsChecked == true)
                {
                    penArray[7] = new Pen(Brushes.Purple, 1);
                    brushArray[7] = Brushes.Purple;
                    colorError = false;
                }
                if (Brown.IsChecked == true)
                {
                    penArray[8] = new Pen(Brushes.Brown, 1);
                    brushArray[8] = Brushes.Brown;
                    colorError = false;
                }

                if (colorError == true)
                    { errorBlock.Text = "Please select at least one color"; }
                for (int k = 0; k < 20; k++)
                {
                    int bound = ru.Next(2, 12);
                    thickness = ru.Next(1, 11);
                    GetPointOnEllipse(ellipseData);
                    //int index = ru.Next(0, penArray.GetLength(0));

                    for (i = 1; i < data.GetLength(0) - 1; i += 2) //begins lines
                    {

                        x = (data[i, 0]) * winWidth;
                        y = (data[i, 1]) * winHeight;
                        x1 = (data[i + 1, 0]) * winWidth;
                        y1 = (data[i + 1, 1]) * winHeight;
                        int index = ru.Next(0, penArray.Length);
                        if (colorError == false)
                        {
                            dc.DrawLine(new Pen(brushArray[index], thickness), new Point(x, y), new Point(x1, y1));
                            if (thickness <= bound)
                            { thickness += 1; }
                        }
                    }
                }
                for (int k = 0; k < 100; k++) //begins ellipses
                {
                    int bound = ru.Next(2, 12);
                    thickness = ru.Next(1, 11);
                    GetPointOnEllipse(ellipseData);
                    int index = ru.Next(0, brushArray.GetLength(0));

                    for (i = 1; i < data.GetLength(0) - 1; i++)
                    {
                        x = (ellipseData[i, 0]);
                        y = (ellipseData[i, 1]);
                        x1 = (ellipseData[i + 1, 0]);
                        y1 = (ellipseData[i + 1, 1]);
                        if (colorError == false)
                        {
                            dc.DrawLine(new Pen(brushArray[index], thickness), new Point(x, y), new Point(x1, y1));
                            if (thickness <= bound)
                            { thickness += 1; }
                        }

                    }

                }
                for (i = 1; i < data.GetLength(0) - 1; i += 2) //begins  blotches
                {
                    x = (data[i, 0]) * winWidth;
                    y = (data[i, 1]) * winHeight;
                    for (int j = 6; j < 30; j += 6)
                    {
                        nuX = ru.Next(0, j);
                        nuY = ru.Next(0, j);

                        //nuX = ru.NextDouble();
                        //nuY = ru.NextDouble();
                        int index = ru.Next(0, brushArray.Length);
                        if (colorError == false)
                        {
                            dc.DrawEllipse(brushArray[index], null, new Point(x, y), nuX, nuY);
                        }
                    }
                }


                dc.Close();
                RenderTargetBitmap bmp = new RenderTargetBitmap(984, 541, 96, 96, PixelFormats.Pbgra32);
                bmp.Render(vis);
                imgPlot.Source = bmp;
            }
            else
            {
                errorBlock.Text = "ERROR: Please enter a positive integer";
            }
        }

       

        private void mnuColor_Click(object sender, RoutedEventArgs e)
        { }
        private void Black_Checked(object sender, RoutedEventArgs e)
        {
            penColor = 0;
        }
        private void Blue_Checked(object sender, RoutedEventArgs e)
        {
            penColor = 1;
        }
        private void Red_Checked(object sender, RoutedEventArgs e)
        {
            penColor = 2;
        }
        private void Green_Checked(object sender, RoutedEventArgs e)
        {
            penColor = 3;
        }
        private void Yellow_Checked(object sender, RoutedEventArgs e)
        {
            penColor = 4;
        }
        private void Orange_Checked(object sender, RoutedEventArgs e)
        {
            penColor = 5;
        }
        private void Pink_Checked(object sender, RoutedEventArgs e)
        {
            penColor = 6;
        }
     
        private void Purple_Checked(object sender, RoutedEventArgs e)
        {
            penColor = 7;         
        }
        private void Brown_Checked(object sender, RoutedEventArgs e)
        {
            penColor = 8;
        }
        private void seedGenerator_Click(object sender, RoutedEventArgs e)
        {
            ru = new Random();
            seedBox.Text = ru.Next().ToString();
        }

        private void GetRandom(double[,] data)
        {
            int i;
            for (i = 0; i < data.GetLength(0); i++)
            {
                data[i, 0] = ru.NextDouble();
                data[i, 1] = ru.NextDouble();
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            
            Close();
        }
        
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string black = "";
            string blue = "";
            string red = "";
            string green = "";
            string yellow = "";
            string orange = "";
            string pink = "";
            string purple = "";
            string brown = "";

            if (Black.IsChecked == true)
                black = "black";
            if (Blue.IsChecked == true)
                blue = "blue";
            if (Red.IsChecked == true)
                red = "red";
            if (Green.IsChecked == true)
                green = "green";
            if (Yellow.IsChecked == true)
                yellow = "yellow";
            if (Orange.IsChecked == true)
                orange = "orange";
            if (Pink.IsChecked == true)
                pink = "pink";
            if (Purple.IsChecked == true)
                purple = "purple";
            if (Brown.IsChecked == true)
                brown = "brown";



            string fileText = seedBox.Text + Environment.NewLine + Environment.NewLine +
                black + Environment.NewLine + blue + Environment.NewLine + red + Environment.NewLine +
                green + Environment.NewLine + yellow + Environment.NewLine + orange + Environment.NewLine +
                pink + Environment.NewLine + purple + Environment.NewLine + brown;

            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "Text Files(*.txt)|*.txt|All(*.*)|*"
            };

            if (sfd.ShowDialog()==true)
            {
                File.WriteAllText(sfd.FileName, fileText);

            }
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            string black = "";
            string blue = "";
            string red = "";
            string green = "";
            string yellow = "";
            string orange = "";
            string pink = "";
            string purple = "";
            string brown = "";
            Window win = new Window();
            try
            {   
                OpenFileDialog ofd = new OpenFileDialog()
                {
                    Filter = "Text Files(*.txt)|*.txt|All(*.*)|*"
                };
                ofd.ShowDialog();
                StreamReader reader = new StreamReader(ofd.FileName);
                

                string currentLine= reader.ReadLine();
                int seedFromFile;
                int.TryParse(currentLine, out seedFromFile);
                seedBox.Text = currentLine;
                while ((currentLine = reader.ReadLine()) != null)
                {
             

                    if (currentLine == "black")
                        Black.IsChecked = true;
                    if (currentLine == "blue")
                        Blue.IsChecked = true;
                    if (currentLine == "red")
                        Red.IsChecked = true;
                    if (currentLine == "green")
                        Green.IsChecked = true;
                    if (currentLine == "yellow")
                        Yellow.IsChecked = true;
                    if (currentLine == "orange")
                        Orange.IsChecked = true;
                    if (currentLine == "pink")
                        Pink.IsChecked = true;
                    if (currentLine == "purple")
                        Purple.IsChecked = true;
                    if (currentLine == "brown")
                        Brown.IsChecked = true;
                        
                }

            }
            catch (Exception ex)
            {
                //Console.WriteLine("The file could not be read:");
                errorBlock.Text=ex.Message;
            }
            
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("First select the colors you want in your painting. " +
                "Then press the 'Generate Seed' button or enter your own value. " +
                "Lastly click the 'Draw' button and behold! " +
                "You can then save your drawing and exit the program using the File menu." +
                Environment.NewLine + Environment.NewLine + "Contact Support: jd262@evansville.edu");
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Jackson Pollock Painting Generator" + Environment.NewLine +
                "           Jacob Darabaris - 2017" + Environment.NewLine +
                "              All rights reserved");
        }

    

        private void seedBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void GetPointOnEllipse(double[,] ellipseData)
        {
            double r1, r2, theta = (ru.NextDouble() * (2 * Math.PI)), t;
            double h, k;
            int i = 0;
            r1 = imgPlot.Width; r2 = imgPlot.Height;
            h = imgPlot.Width / (2 * (ru.NextDouble())); k = imgPlot.Height / 2 * (ru.NextDouble());
            t = ru.NextDouble() * (2 * Math.PI - (-2) * Math.PI) + (-2) * Math.PI;
            for (i = 0; i < ellipseData.GetLength(0); i++)
            {
                ellipseData[i, 0] = h + r1 * Math.Cos(t - theta);
                ellipseData[i, 1] = k + r2 * Math.Sin(t);
                t += Math.PI / 40;
            }
        }
    }
}

