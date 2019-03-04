using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.ComponentModel;

namespace EE356P5JMD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] Players = { "", "", "", "" };
        private bool[] playerIndex = new bool[4];
        private int[] beginningRolls = new int[4];
        private bool[] greenHomeArray = new bool[4]; //start positions
        private bool[] greenBoardArray = new bool[28];
        private bool[] greenFinishArray = new bool[4];
        private bool[] redHomeArray = new bool[4]; //start positions
        private bool[] redBoardArray = new bool[28];
        private bool[] redFinishArray = new bool[4];
        private bool[] blueHomeArray = new bool[4]; //start positions
        private bool[] blueBoardArray = new bool[28];
        private bool[] blueFinishArray = new bool[4];
        private bool[] yellowHomeArray = new bool[4]; //start positions
        private bool[] yellowBoardArray = new bool[28];
        private bool[] yellowFinishArray = new bool[4];
        private bool[] masterBoardArray = new bool[28];
        private int green = 0; //used throughout to reference players
        private int red = 1;
        private int blue = 2;
        private int yellow = 3;
        private int numPlayers = 0; //a player is added when they set their name
        private int highRollsCount = 0;
        private int[] highRolls = new int[4];
        //private int[] currentPlayer = ;
        private int highestRoll = 0;
        private int turnCounter = 1;
        private int firstPlayer = 0;
        private int x = 0; //cycles through players; resets after 3 (arrays start at 0!)
        
        private bool P1Set, P2Set, P3Set, P4Set;
        private bool orderDetermined = false;
        private Random ru = new Random();

        delegate void SetTextCallback(String text);
        delegate void SetIntCallbCk(int theadnum);
        BackgroundWorker backgroundWorker1 = new BackgroundWorker();
        

        public MainWindow()
        {
            InitializeComponent();
            //Roll_Dice_Button.Visibility = Visibility.Hidden;
            

        }

        BackgroundWorker[] bkw1 = new BackgroundWorker[100];
        Socket client;
        NetworkStream[] ns = new NetworkStream[100];
        StreamReader[] sr = new StreamReader[100];
        StreamWriter[] sw = new StreamWriter[100];
        List<int> AvailableClientNumbers = new List<int>(100);
        List<int> UsedClientNumbers = new List<int>(100);


        int clientcount = 0;

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            String printtext;
            TcpListener newsocket = new TcpListener(IPAddress.Any, 9090);  //Create TCP Listener on server
            newsocket.Start();
            for (int i = 0; i < 7; i++)
            {
                AvailableClientNumbers.Add(i);
            }
            while (AvailableClientNumbers.Count > 0)
            {
                InsertText("waiting for client");                   //wait for connection
                printtext = "Available Clients = " + AvailableClientNumbers.Count;
                InsertText(printtext);                   //wait for connection
                client = newsocket.AcceptSocket();     //Accept Connection
                clientcount = AvailableClientNumbers.First();
                AvailableClientNumbers.Remove(clientcount);
                ns[clientcount] = new NetworkStream(client);                            //Create Network stream
                sr[clientcount] = new StreamReader(ns[clientcount]);
                sw[clientcount] = new StreamWriter(ns[clientcount]);
                string welcome = "welcome";
                InsertText("client connected");
                sw[clientcount].WriteLine(/*"server: " +*/ welcome);     //Stream Reader and Writer take away some of the overhead of keeping track of Message size.  By Default WriteLine and ReadLine use Line Feed to delimit the messages
                sw[clientcount].Flush();
                bkw1[clientcount] = new BackgroundWorker();
                bkw1[clientcount].DoWork += new DoWorkEventHandler(client_DoWork);
                bkw1[clientcount].RunWorkerAsync(clientcount);
                UsedClientNumbers.Add(clientcount);
            }

        }

        private void client_DoWork(object sender, DoWorkEventArgs e)
        {
            int clientnum = (int)e.Argument;
            bkw1[clientnum].WorkerSupportsCancellation = true; //;

            while (true)
            {
                string inputStream;
                try
                {
                    inputStream = sr[clientnum].ReadLine();
                    InsertText(inputStream);

                    

                    foreach (int t in UsedClientNumbers)
                    {
                        sw[t].WriteLine(inputStream);
                        sw[t].Flush();
                    }
                    if (inputStream == "disconnect" || inputStream == "exit")
                    {
                        sr[clientnum].Close();
                        sw[clientnum].Close();
                        ns[clientnum].Close();
                        InsertText("Client " + clientnum + " has disconnected");
                        KillMe(clientnum);
                        break;
                    }
                    try
                    {
                        string[] parseStream = inputStream.Split(' ');
                        string clientName = parseStream[0];
                        string command = parseStream[1];
                        string key = parseStream[2];

                        if (command == "join")
                        {
                            if (key == "green")
                            {
                                Thread setGreen = new Thread(() => Green_Player_Set(clientName));
                                this.Dispatcher.Invoke(() =>
                                {
                                    setGreen.Start();
                                });
                            }
                            if (key == "red")
                            {
                                Thread setRed = new Thread(() => Red_Player_Set(clientName));
                                this.Dispatcher.Invoke(() =>
                                {
                                    setRed.Start();
                                });
                            }
                            if (key == "blue")
                            {
                                Thread setBlue = new Thread(() => Blue_Player_Set(clientName));
                                this.Dispatcher.Invoke(() =>
                                {
                                    setBlue.Start();
                                });
                            }

                            if (key == "yellow")
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    Thread setYellow = new Thread(() => Yellow_Player_Set(clientName));
                                    setYellow.Start();
                                });
                            }
                        }
                        if (command == "start" && key == "game")
                        {
                            Thread startThread = new Thread(Start_Button);
                            this.Dispatcher.Invoke(() =>
                            {
                                startThread.Start();
                            });
                        }
                        if (command == "roll" && key == "dice")
                        {
                            Thread rollThread = new Thread(new ThreadStart(Roll_Dice_Button));
                            this.Dispatcher.Invoke(() =>
                            {
                                rollThread.Start();
                            });
                        }
                        if (command == "cini")
                            MessageBox.Show(clientcount.ToString());
                    }
                    catch
                    { }
                }

                catch
                {
                    sr[clientnum].Close();
                    sw[clientnum].Close();
                    ns[clientnum].Close();
                    InsertText("Client " + clientnum + " has disconnected");
                    KillMe(clientnum);
                    break;
                }
            }
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {

            foreach (int t in UsedClientNumbers)
            {
                sw[t].WriteLine("server: " + textBox1.Text);
                sw[t].Flush();
            }
            InsertText("You: " + textBox1.Text);
            textBox1.Text = "";

        }

        private void InsertText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.listBox1.Dispatcher.CheckAccess())
            {
                this.listBox1.Items.Insert(0, (text));

            }
            else
            {
                listBox1.Dispatcher.BeginInvoke(new SetTextCallback(InsertText), text);
            }
        }

        private void KillMe(int threadnum)
        {
            if (this.listBox1.Dispatcher.CheckAccess())
            {
                UsedClientNumbers.Remove(threadnum);
                AvailableClientNumbers.Add(threadnum);
                bkw1[threadnum].CancelAsync();
                bkw1[threadnum].Dispose();
                bkw1[threadnum] = null;
                GC.Collect();

            }
            else
            {
                listBox1.Dispatcher.BeginInvoke(new SetIntCallbCk(KillMe), threadnum);
            }

        }

        private void serverStart_Click(object sender, RoutedEventArgs e)
        {
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);

            backgroundWorker1.RunWorkerAsync("Message to Worker");
        }

        private void Case_Handler()
        {
            //if a piece makes it around the board
            
            //if two pieces occupy the same spot
        }
        private void Start_Button()
        {
            if (numPlayers > 1)
            {

                //Start_Button.Visibility = Visibility.Hidden;
                //Green_Player_Set.Visibility = Visibility.Hidden;
                //Red_Player_Set.Visibility = Visibility.Hidden;
                //Blue_Player_Set.Visibility = Visibility.Hidden;
                //Yellow_Player_Set.Visibility = Visibility.Hidden;
                //Roll_Dice_Button.Visibility = Visibility.Visible;
                //comparator = new int[numPlayers];
                loadPlayerIndex();
                //buttonActivate = true;
                Player_Check();

                Turn_Text.Text = Players[x];
                Turn_Count_Text.Text = turnCounter.ToString();

            }
            else
                MessageBox.Show("You need at least 2 players to play!");
        }
        private void Roll_Dice_Button()
        {
            //Roll_Dice_Button.Visibility = Visibility.Hidden;
            Player_Check();
            int roll = ru.Next(1, 7);

            if (orderDetermined)
            {
                Dice_Text.Text = Players[x] + " rolled a " + roll.ToString() + "!" + Environment.NewLine;
                InsertText(Players[x] + " rolled a " + roll.ToString() + "!");
                //while()
                Turn_Handler(roll);
            }
            else
            {
                Dice_Text.Text += Players[x] + " rolled a " + roll.ToString() + "!" + Environment.NewLine;
                Determine_First_Player(roll);
            }

        }
        private void Turn_Handler(int roll)
        {


            Cycle_Players();
            Player_Check();
            turnCounter += 1;
            Turn_Count_Text.Text = (turnCounter).ToString();
            //MessageBox.Show(x.ToString());
            Turn_Text.Text = Players[x];
            //Roll_Dice_Button.Visibility = Visibility.Visible;

        }
        private void Determine_First_Player(int roll)
        {
            //Player_Check();
            if (turnCounter <= numPlayers) //for the beginning of the game; decides who goes first
            {
                //Turn_Text.Text = Players[x]; //displays the current players turn
                //comparator[i] = roll;
                beginningRolls[x] = roll;
                if (roll > highestRoll)
                {
                    highestRoll = roll;
                    Messages.Text = "Highest Roll is " + roll.ToString() + " by " + Players[x] + System.Environment.NewLine;

                }
                else if (roll == highestRoll)
                {
                    Messages.Text += " and " + Players[x];
                }
            }
            if (turnCounter == numPlayers)
            {
                for (int i = 0; i < x; i++)
                {
                    if (highestRoll == beginningRolls[i])
                    {
                        highRolls[i] = beginningRolls[i];
                        highRollsCount += 1;
                    }
                }
                if (highRollsCount > 1)
                    x = ru.Next(0, highRollsCount);
                while (highRolls[x] == 0)
                    Cycle_Players();
                //Messages.Text = Players[x] + " goes first by coin toss!";     
                //else
                firstPlayer = x;

                Messages.Text = Players[x] + " goes first!";
                if (highRollsCount > 1)
                    Messages.Text += " (By coin toss)";
                x = firstPlayer - 1; //we subtract 1 because x will be incremented at the end of this function
            }

            if (turnCounter > numPlayers) //for the rest of the game          
            {
                orderDetermined = true;
                turnCounter = 0;
            }
            //Cycle through players
            Cycle_Players();
            Player_Check();
            turnCounter += 1;
            Turn_Count_Text.Text = (turnCounter).ToString();
            //MessageBox.Show(x.ToString());

            Turn_Text.Text = Players[x];
            //Roll_Dice_Button.Visibility = Visibility.Visible;
        }
        private void Cycle_Players()
        {
            if (x >= 3)
                x = 0;
            else
                x += 1;
        }
        private void Player_Check() //The player array may not be full (less than 4 players)
        {
            while (!playerIndex[x]) //while the player index is empty
            {
                Cycle_Players();
            }
        }
        private void loadPlayerIndex()
        {
            if (Players[green] != "")
                playerIndex[green] = true; //0
            if (Players[red] != "")
                playerIndex[red] = true; //1
            if (Players[blue] != "")
                playerIndex[blue] = true; //2
            if (Players[yellow] != "")
                playerIndex[yellow] = true; //3
        }
        private void Green_Player_Set(string name)
        {
            if (!P1Set)
            {
                P1Set = true;
                //Green_Player_Set.Visibility = Visibility.Hidden;
                this.Dispatcher.Invoke(() =>
                {
                    Green_Base_Title.Text = name;
                    Players[green] = name;
                    numPlayers += 1;
                    Green_Home_H.Fill = new SolidColorBrush(System.Windows.Media.Colors.Green);
                    greenHomeArray[0] = true;
                    Green_Home_O.Fill = new SolidColorBrush(System.Windows.Media.Colors.Green);
                    greenHomeArray[1] = true;
                    Green_Home_M.Fill = new SolidColorBrush(System.Windows.Media.Colors.Green);
                    greenHomeArray[2] = true;
                    Green_Home_E.Fill = new SolidColorBrush(System.Windows.Media.Colors.Green);
                    greenHomeArray[3] = true;
                });
            }

        }
        private void Red_Player_Set(string name)
        {
            if (!P2Set)
            {
                P2Set = true;
                this.Dispatcher.Invoke(() =>
                {
                    //Red_Player_Set.Visibility = Visibility.Hidden;
                    Red_Base_Title.Text = name;
                    Players[red] = name;
                    numPlayers += 1;
                    Red_Home_H.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    redHomeArray[0] = true;
                    Red_Home_O.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    redHomeArray[1] = true;
                    Red_Home_M.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    redHomeArray[2] = true;
                    Red_Home_E.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    redHomeArray[3] = true;
                });
            }
        }
        private void Blue_Player_Set(string name)
        {
            if (!P3Set)
            {
                P3Set = true;
                this.Dispatcher.Invoke(() =>
                {
                    //Blue_Player_Set.Visibility = Visibility.Hidden;
                    Blue_Base_Title.Text = name;
                    Players[blue] = name;
                    numPlayers += 1;
                    Blue_Home_H.Fill = new SolidColorBrush(System.Windows.Media.Colors.Blue);
                    blueHomeArray[0] = true;
                    Blue_Home_O.Fill = new SolidColorBrush(System.Windows.Media.Colors.Blue);
                    blueHomeArray[1] = true;
                    Blue_Home_M.Fill = new SolidColorBrush(System.Windows.Media.Colors.Blue);
                    blueHomeArray[2] = true;
                    Blue_Home_E.Fill = new SolidColorBrush(System.Windows.Media.Colors.Blue);
                    blueHomeArray[3] = true;
                });
            }
        }
        private void Yellow_Player_Set(string name)
        {
            if (!P4Set)
            {
                P4Set = true;
                this.Dispatcher.Invoke(() =>
                {
                    //Yellow_Player_Set.Visibility = Visibility.Hidden;
                    Yellow_Base_Title.Text = name;
                    Players[yellow] = name;
                    numPlayers += 1;
                    Yellow_Home_H.Fill = new SolidColorBrush(System.Windows.Media.Colors.Yellow);
                    yellowHomeArray[0] = true;
                    Yellow_Home_O.Fill = new SolidColorBrush(System.Windows.Media.Colors.Yellow);
                    yellowHomeArray[1] = true;
                    Yellow_Home_M.Fill = new SolidColorBrush(System.Windows.Media.Colors.Yellow);
                    yellowHomeArray[2] = true;
                    Yellow_Home_E.Fill = new SolidColorBrush(System.Windows.Media.Colors.Yellow);
                    yellowHomeArray[3] = true;
                });
            }
        }



    }
}
