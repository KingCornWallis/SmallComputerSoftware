using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading;

namespace EE356P3JMD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int N;
        private int prog1, prog2, prog3, prog4;
        private double[] randomA, randomB, result1, result2, result3, result4;
        private Random ru = new Random();
        static BackgroundWorker bkw1 = new BackgroundWorker();
        static BackgroundWorker bkw2 = new BackgroundWorker();
        static BackgroundWorker bkw3 = new BackgroundWorker();

        static BackgroundWorker bkw4 = new BackgroundWorker();
     
        private AutoResetEvent stopProgress1 = new AutoResetEvent(false);
        private AutoResetEvent stopProgress2 = new AutoResetEvent(false);
        private AutoResetEvent stopProgress3 = new AutoResetEvent(false);
        private AutoResetEvent stopProgress4 = new AutoResetEvent(false);
        bool validInput = false;

        private double sum1, sum2, sum3, sum4, mainSum;

        public MainWindow()
        {
            InitializeComponent();

            stopProgress1.Reset();
            
            bkw1.DoWork += new DoWorkEventHandler(multiplyMethodNu);
            bkw1.ProgressChanged += new ProgressChangedEventHandler(bkw1_ProgressChanged);
            bkw1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bkw1_RunWorkerCompleted);

            bkw1.WorkerReportsProgress = true;
            bkw1.WorkerSupportsCancellation = true;

            bkw2.WorkerReportsProgress = true;
            bkw2.WorkerSupportsCancellation = true;

            
            bkw3.WorkerReportsProgress = true;
            bkw3.WorkerSupportsCancellation = true;


            bkw4.WorkerReportsProgress = true;
            bkw4.WorkerSupportsCancellation = true;
        }

        private void runTest1_Click(object sender, RoutedEventArgs e)
        {
            initializeValues();
            if (validInput)
            {
                
                multiplyMethod();
                pythonMethod();
                trigMethod();
                exponentMethod();
                runAndAddResults();
                resultBox.Text = mainSum.ToString();
            }
            else
                return;
        }

        private void runTest2_Click(object sender, RoutedEventArgs e)
        {

            initializeValues();
            if (validInput)
            {
                
                Thread t1 = new Thread(multiplyMethod);
                t1.Start();
                Thread t2 = new Thread(pythonMethod);
                t2.Start();
                Thread t3 = new Thread(trigMethod);
                t3.Start();
                Thread t4 = new Thread(exponentMethod);
                t4.Start();
                runAndAddResults();
                resultBox.Text = mainSum.ToString();
                
            }
            else
                return;
        }

        private void runTest3_Click(object sender, RoutedEventArgs e)
        {

            initializeValues();
            if (validInput)
            {
            //MessageBox.Show(randomA[0].ToString());

                bkw1.DoWork += new DoWorkEventHandler(workerOne);
                bkw1.RunWorkerAsync("Message to Worker");
                stopProgress1.WaitOne();
                bkw2.DoWork += new DoWorkEventHandler(workerTwo);
                bkw2.RunWorkerAsync("Message to Worker");
                stopProgress2.WaitOne();
                bkw3.DoWork += new DoWorkEventHandler(workerThree);
                bkw3.RunWorkerAsync("Message to Worker");
                stopProgress3.WaitOne();
                bkw4.DoWork += new DoWorkEventHandler(workerFour);
                bkw4.RunWorkerAsync("Message to Worker");
                
                
                
                
                stopProgress4.WaitOne();


                runAndAddResults();
                    resultBox.Text = mainSum.ToString();
                
            }
            else
                return;
        }

        private void runTest4_Click(object sender, RoutedEventArgs e)
        {
            initializeValues();
            if (validInput)
            {
                pgb1.Value = 0;
                runTest4.IsEnabled = false;
                cancelMethod1.IsEnabled = true;
                cancelMethod2.IsEnabled = true;
                cancelMethod3.IsEnabled = true;
                cancelMethod4.IsEnabled = true;

                bkw1.RunWorkerAsync("Message to Worker");

                stopProgress1.WaitOne();

                bkw2.DoWork += new DoWorkEventHandler(pythonMethodNu);
                bkw2.ProgressChanged += new ProgressChangedEventHandler(bkw2_ProgressChanged);
                bkw2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bkw2_RunWorkerCompleted);
                bkw2.RunWorkerAsync("Message to Worker");
                stopProgress2.WaitOne();


                bkw3.DoWork += new DoWorkEventHandler(trigMethodNu);
                bkw3.ProgressChanged += new ProgressChangedEventHandler(bkw3_ProgressChanged);
                bkw3.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bkw3_RunWorkerCompleted);
                bkw3.RunWorkerAsync("Message to Worker");
                stopProgress3.WaitOne();



                bkw4.DoWork += new DoWorkEventHandler(exponentMethodNu);
                bkw4.ProgressChanged += new ProgressChangedEventHandler(bkw4_ProgressChanged);
                bkw4.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bkw4_RunWorkerCompleted);
                bkw4.RunWorkerAsync("Message to Worker");
                
                stopProgress1.WaitOne();
                stopProgress2.WaitOne();
                stopProgress3.WaitOne();
                stopProgress4.WaitOne();


                runAndAddResults();
                resultBox.Text = mainSum.ToString();
                runTest4.IsEnabled = true;
                stopProgress1.Reset();
            }
            else
                return;
        }
        //These Methods are used by Tests 1-3 vvv
        private void multiplyMethod() 
        {
            sum1 = 0;
            for (int i = 0; i < N; i++)
            {

                result1[i] = randomA[i] * randomB[i];
                sum1 += result1[i];
            }
            stopProgress1.Set(); //Signals Method is finished
        }
        private void pythonMethod()
        {
            sum2 = 0;
            for (int i = 0; i < N; i++)
            {
                result2[i] = Math.Sqrt(randomA[i] + randomB[i]);
                sum2 += result2[i];
            }
            stopProgress2.Set(); //Signals Method is finished
        }
        private void trigMethod()
        {
            sum3 = 0;
            for (int i = 0; i < N; i++)
            {
                result3[i] = Math.Sin(randomA[i]) + Math.Cos(randomB[i]);
                sum3 += result3[i]; 
            }
            stopProgress3.Set(); //Signals Method is finished
        }
        private void exponentMethod()
        {
            sum4 = 0;
            for (int i = 0; i < N; i++)
            {
                result4[i] = Math.Pow(randomA[i], 0.8) + Math.Pow(randomB[i], 0.25);
                sum4 += result4[i];
            }
            stopProgress4.Set(); //Signals Method is finished
        }
        //These Methods are used by Tests 1-3 ^^^

        private double runAndAddResults() //Universal Function that completes a Test
        { 
            mainSum = sum1 + sum2 + sum3 + sum4;
            return mainSum;
        }

        private void initializeValues() //Universal Function that begins each Test
        {
            try
            {
                N = Convert.ToInt32(nValue.Text);
                if (N <= 50000000)
                {
                    validInput = true;
                    randomA = new double[N];
                    randomB = new double[N];
                    result1 = new double[N];
                    result2 = new double[N];
                    result3 = new double[N];
                    result4 = new double[N];
                    for (int i = 0; i < N; i++)
                    {
                        randomA[i] = ru.NextDouble();
                        randomB[i] = ru.NextDouble();
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a smaller number");
                    return;
                }
            }
            catch  (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // vvv These 'Workers' serve as a bridge between the BackgroundWorker and Methods in Test 3 vvv //
        private void workerOne(object sender, DoWorkEventArgs e)
        {multiplyMethod();}
        private void workerTwo(object sender, DoWorkEventArgs e)
        {pythonMethod();}
        private void workerThree(object sender, DoWorkEventArgs e)
        {trigMethod();}
        private void workerFour(object sender, DoWorkEventArgs e)
        {exponentMethod();}
        // ^^^ These 'Workers' serve as a bridge between the BackgroundWorker and Methods in Test 3 ^^^ //

        // vvv These are the 4 original methods modified to work with BackgroundWorkers in Test 4 vvv //
        private void multiplyMethodNu(object sender, DoWorkEventArgs e)
        {
            //MessageBox.Show("haha");
            sum1 = 0;
            for (int i = 0; i < N; i++)
            {
                //if(bkw1.CancellationPending)
                //{

                //}
                result1[i] = randomA[i] * randomB[i];
                sum1 += result1[i];
                if (i % N/100 == 0 )
                {
                    Thread.Sleep(1);
                    int w = i * 100 / N;
                   
                    bkw1.ReportProgress(w);
                   
                }
            }

            e.Result = "Done";
            
            stopProgress1.Set();
        }
        private void pythonMethodNu(object sender, DoWorkEventArgs e)
        {

            sum2 = 0;
            for (int i = 0; i < N; i++)
            {
                result2[i] = Math.Sqrt(randomA[i] + randomB[i]);
                sum2 += result2[i];
                //bkw2.ReportProgress((i / N) * 100);
            }
            e.Result = "Done";
            stopProgress2.Set();
        }
        private void trigMethodNu(object sender, DoWorkEventArgs e)
        {

            sum3 = 0;
            for (int i = 0; i < N; i++)
            {
                result3[i] = Math.Sin(randomA[i]) + Math.Cos(randomB[i]);
                sum3 += result3[i];
                //bkw3.ReportProgress((i / N) * 100);

            }
            e.Result = "Done";
            stopProgress3.Set();
        }
        private void exponentMethodNu(object sender, DoWorkEventArgs e)
        {

            sum4 = 0;
            for (int i = 0; i < N; i++)
            {
                result4[i] = Math.Pow(randomA[i], 0.8) + Math.Pow(randomB[i], 0.25);
                sum4 += result4[i];
                //bkw4.ReportProgress((i / N) * 100);
            }
            e.Result = "Done";
            stopProgress4.Set();
        }
        // ^^^ These are the 4 original methods modified to work with BackgroundWorkers in Test 4 ^^^ //


        //These are event handlers for the 4 BackgroundWorkers used in Test 4

        //BackgroundWorker 1
        private void bkw1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
           pgb1.Value = e.ProgressPercentage;    //Write to progress bar
           txtPercent1.Text = (e.ProgressPercentage).ToString();  //and text box
        }
        private void bkw1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            cancelMethod1.IsEnabled = false;
            pgb1.Value = 100;
            txtPercent1.Text = (e.Result).ToString();
        }
        private void cancelMethod1_Click(object sender, RoutedEventArgs e)
        {
            bkw1.CancelAsync();
            cancelMethod1.IsEnabled = false;
        }

   
        void bkw2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
           pgb2.Value = e.ProgressPercentage;    //Write to progress bar
           txtPercent2.Text = (e.ProgressPercentage).ToString();  //and text box
        }
        void bkw2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            cancelMethod2.IsEnabled = false;
            pgb2.Value = 0;
           txtPercent2.Text = (e.Result).ToString();
        }
        private void cancelMethod2_Click(object sender, RoutedEventArgs e)
        {
            bkw2.CancelAsync();
            cancelMethod2.IsEnabled = false;
        }

        //BackgroundWorker 3
        void bkw3_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgb3.Value = e.ProgressPercentage;    //Write to progress bar
            txtPercent3.Text = (e.ProgressPercentage).ToString();  //and text box
        }
        void bkw3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            cancelMethod3.IsEnabled = false;
            pgb3.Value = 0;
            txtPercent3.Text = (e.Result).ToString();
        }
        private void cancelMethod3_Click(object sender, RoutedEventArgs e)
        {
            bkw1.CancelAsync();
            cancelMethod3.IsEnabled = false;
        }

        //BackgroundWorker 4
        void bkw4_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
           pgb4.Value = e.ProgressPercentage;    //Write to progress bar
           txtPercent4.Text = (e.ProgressPercentage).ToString();  //and text box
        }
        void bkw4_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            cancelMethod4.IsEnabled = false;
            pgb4.Value = 0;
            txtPercent4.Text = (e.Result).ToString();
        }
        private void cancelMethod4_Click(object sender, RoutedEventArgs e)
        {
            bkw1.CancelAsync();
            cancelMethod4.IsEnabled = false;
        }

        //This section includes textbox modifiers

        private void setMax_btn_Click(object sender, RoutedEventArgs e)
        {
            nValue.Text = "50000000";
        }

        private void nValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textbox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
            nValue.MaxLength = 8;
        }

        //This section includes optional function-less buttons
        private void helpBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This program is a computer benchmarking program that tests threading by doing extensive mathematical calculations." +
      "The first test will not use threading; it is used as a sort of baseline. " +
      "The second test will use simple threads for each calculation. " +
      "The third test will use the BackgroundWorker class to perform each calculation. " +
      "The fourth test will use the BackgroundWorker class with cancelling and progress bars. " + Environment.NewLine +
        "To use, simply enter a value or press Set Max and select a test" +
            Environment.NewLine + Environment.NewLine + "Contact Support: jd262@evansville.edu");
        }

        private void aboutBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("           Basic Computer Bechmark" + Environment.NewLine +
      "              Jacob Darabaris - 2017" + Environment.NewLine +
      "                 All rights reserved" + Environment.NewLine);
        }
        
    }
}
