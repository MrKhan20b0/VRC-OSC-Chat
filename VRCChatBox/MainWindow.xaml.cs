using CoreOSC;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Path = System.IO.Path;

namespace VRCChatBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string VERSION = "0.02";

        private bool updateAvailable = false;
        private Process updaterProcess;

        private string _charactersRemainingString;
        public string CharactersRemainingString
        {
            get
            {
                return _charactersRemainingString;
            }
            set
            {
                _charactersRemainingString = value;
                OnPropertyChanged(nameof(_charactersRemainingString));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();

            CheckForUpdate();


            // Link pathways from MainWindow.xaml to this object
            //DataContext= this;

            ((INotifyCollectionChanged)OldMessagesListView.Items).CollectionChanged += ListView_CollectionChanged;

            // TODO: make the splash label show a random Arabic letter upon each startup


            char[] ArabicLetters = {
                '\u0627', //Alef
                '\u0628', //Ba
                '\u062A', //Ta
                '\u062B', //Tha
                '\u062C', //Jeem
                '\u062D', //Haa
                '\u062E', //Kha

                '\u062F', //Dal
                '\u0630', //Dhal
                '\u0631', //ra
                '\u0632', //Zayn

                '\u0633', //seen
                '\u0634', //sheen
                '\u0635', //Saud
                '\u0636', //Daud

                '\u0637', //T~a
                '\u0638', //Z~a
                '\u0639', //Ein
                '\u063A', //Ghein

                '\u0641', //Fa
                '\u0642', //Qaf
                '\u0643', //Kaf
                '\u0644', //Lam

                '\u0645', //Meem
                '\u0646', //noon
                '\u0647', //ha
                '\u0648', //wow

                '\u064A', //Ya
                '\u0622', //Alif Maddah
                '\u0629', //Ta marbuta
                '\u0649', //Alif maqsurah
            };

            string[] args = Environment.GetCommandLineArgs();
            bool finishedUpdate = false;

            Random random = new Random();
            int randomNumber = random.Next(0, ArabicLetters.Length);
            SplashLabel.Content = ArabicLetters[randomNumber];

            var sb = new Storyboard();
            var da = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
            da.BeginTime = TimeSpan.FromSeconds(1);
            Storyboard.SetTargetProperty(da, new PropertyPath("Opacity"));
            Storyboard.SetTarget(da, Splash);
            sb.Children.Add(da);
            sb.Completed += (s1, e1) =>
            {
                Panel.SetZIndex(Splash, -1);

                if (finishedUpdate)
                {
                    SplashLabel.Content = ArabicLetters[randomNumber];
                    SplashUpdateCompleteMessage.Visibility = Visibility.Collapsed;
                }

                sb = new Storyboard();
                da = new DoubleAnimation(0, 0.05, new Duration(TimeSpan.FromSeconds(1.5)));
                //da.BeginTime = TimeSpan.FromSeconds(3);
                Storyboard.SetTargetProperty(da, new PropertyPath("Opacity"));
                Storyboard.SetTarget(da, Splash);
                sb.Children.Add(da);
                sb.Begin();
            };
            
            
            sb.Begin();

            System.Diagnostics.Debug.WriteLine("1");

            

            

            if (args != null && args.Length == 2)
            {
                if (args[1] == "updated")
                {
                    finishedUpdate = true;
                    SplashLabel.Content = "◯";
                    SplashUpdateCompleteMessage.Visibility= Visibility.Visible;
                }
            }

            

        }
        


        private void ListView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // scroll the new item into view   
                OldMessagesListView.ScrollIntoView(e.NewItems[0]);
            }
        }

        

        private void TextChangedhandler(object sender, TextChangedEventArgs args)
        {
            // Omitted Code: Insert code that does something whenever
            // the text changes...
            System.Diagnostics.Debug.WriteLine("Len: " + TextBox.Text.Length);

            //CharactersRemainingString = $"{144 - TextBox.Text.Length}/144";

            //TODO: Send OSC Typing command


            

        }

        private void CheckForUpdate()
        {
            if (File.Exists("./auto-updater/auto-updater.exe"))
            {
                updaterProcess = new Process();
                var startInfo = new ProcessStartInfo("./auto-updater/auto-updater.exe");
                startInfo.UseShellExecute = false;
                string[] args = {
                VERSION,
                Process.GetCurrentProcess().Id.ToString(),
                "VRCOSCCHAT.zip",
                "VRCChatBox.exe",
                "updated",
                "needUpdate",
                "https://github.com/MrKhan20b0/VRC-OSC-Chat/releases/latest/download"
                };

                startInfo.Arguments = string.Join(' ', args);
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardInput= true;
                startInfo.CreateNoWindow = true;

                updaterProcess.StartInfo = startInfo;
                updaterProcess.OutputDataReceived += ShowUpdatePrompt;
                updaterProcess.Start();
                updaterProcess.BeginOutputReadLine();
            }
           
        }


        public void ShowUpdatePrompt(object sender, DataReceivedEventArgs args)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (args.Data == "needUpdate")
                    {
                        Panel.SetZIndex(TopBar, 3);
                        updateAvailable = true;
                        UpdatePrompt.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        CancelUpdate();
                    }
                });
            } 
            catch (TaskCanceledException e)
            {
                // Current known reason for this: User shut down program while updating
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
            
           
        }

        private void DoUpdate()
        {
            // Updater process is waiting for parent process to die to do update
            if (updateAvailable)
            {
                updaterProcess.StandardInput.WriteLine("update");
                Environment.Exit(0);
            }
        }

        private void CancelUpdate()
        {
            // Updater process is waiting for parent process to die to do update
            if (updaterProcess != null)
            {
                updaterProcess.StandardInput.WriteLine("cancel");
                updaterProcess.Kill();
            }
            Panel.SetZIndex(TopBar, 0);
            UpdatePrompt.Visibility = Visibility.Collapsed;
        }




        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }



            // This will automatically scroll to bottom when clicing the border
            /*MessageList.SelectedIndex = MessageList.Items.Count - 1;
            MessageList.ScrollIntoView(MessageList.SelectedItem);*/
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void ButtonMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            CancelUpdate();
            Application.Current.Shutdown();
        }

        private void PromptButtonAccept_Click(object sender, RoutedEventArgs e)
        {
            DoUpdate();
        }

        private void PromptButtonDecline_Click(object sender, RoutedEventArgs e)
        {
            CancelUpdate();
        }
    }
}
