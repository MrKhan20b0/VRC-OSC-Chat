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
        private const string VERSION = "0.0";


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



            
        }

/*
        private void Update()
        {
            

            // TODO: This has to be tested once we make it public

            *//*Task.Run(() =>
            {
                SaveFile("https://github.com/reactiveui/ReactiveUI/releases/latest/download", "C:\\Users\\Adam\\Documents\\PJR9K\\VRC-OSC-Chat\\VRCChatBox\\bin\\Debug\\v0.1.zip");
            }).Wait();*//*


            // TODO: remove this
            var currentPath = System.IO.Directory.GetCurrentDirectory();
            System.Diagnostics.Debug.WriteLine(currentPath);
            Directory.CreateDirectory(currentPath);
            ZipFile.ExtractToDirectory("../v0.1.zip", currentPath + SALT_DIRECTORY_EXT, true);
            RestartApplicationNewVersion();

        }

        private async Task SaveFile(string fileUrl, string pathToSave)
        {
            // See https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient
            // for why, in the real world, you want to use a shared instance of HttpClient
            // rather than creating a new one for each request


            var httpClient = new HttpClient();

            var httpResult = await httpClient.GetAsync(fileUrl);

            System.Diagnostics.Debug.WriteLine(httpResult.RequestMessage.RequestUri);
            string uriString = httpResult.RequestMessage.RequestUri.ToString();

            float newVersion = float.Parse(uriString.Substring(uriString.Length - 7, 3), CultureInfo.InvariantCulture.NumberFormat);
            System.Diagnostics.Debug.WriteLine(newVersion);





            httpClient.Dispose();

            System.Diagnostics.Debug.WriteLine("1");


            byte[] buffer = await DownloadFile(fileUrl);

            File.WriteAllBytes(pathToSave, buffer);


            *//*var currentPath = System.IO.Directory.GetCurrentDirectory();
            System.Diagnostics.Debug.WriteLine(currentPath);
            Directory.CreateDirectory(currentPath);
            ZipFile.ExtractToDirectory("../v0.1.zip", currentPath + SALT_DIRECTORY_EXT, true);
            RestartApplicationNewVersion();*//*

        }

        public static async Task<byte[]> DownloadFile(string url)
        {
            using (var client = new HttpClient())
            {

                using (var result = await client.GetAsync(url))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        return await result.Content.ReadAsByteArrayAsync();
                    }

                }
            }
            return null;
        }

        private static void RestartApplicationNewVersion()
        {
            string? currentExecutablePath = Process.GetCurrentProcess()?.MainModule?.FileName;

            System.Diagnostics.Debug.WriteLine(currentExecutablePath);

            if (string.IsNullOrEmpty(currentExecutablePath))
            {
                System.Diagnostics.Debug.WriteLine("Failed To Restart: Could Not Find Main Module's File Name");
                return;
            }

            var splitPath = currentExecutablePath.Split("\\");

            splitPath[splitPath.Length - 2] += SALT_DIRECTORY_EXT;

            string newExcutablePath = string.Join("\\", splitPath);

            System.Diagnostics.Debug.WriteLine(newExcutablePath);



            if (!File.Exists(newExcutablePath))
            {
                System.Diagnostics.Debug.WriteLine("Failed To Restart: Could Not Find Main Module's File Name");
            }
            else
            {
                Process.Start(newExcutablePath);
                Application.Current.Shutdown();
            }

        }*/


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


            CreateChildProcess();

        }

        private void CreateChildProcess()
        {
            Process p = new Process();
            var startInfo = new ProcessStartInfo("./auto-updater/auto-updater.exe");
            startInfo.UseShellExecute = false;
            string[] args = { VERSION, Process.GetCurrentProcess().Id.ToString(), "v0.1.zip", "vrc_osc_chat.exe"};
            startInfo.Arguments = string.Join(' ', args);
            startInfo.RedirectStandardOutput= true;
            
            p.StartInfo = startInfo;
            p.OutputDataReceived += (sender, args) => System.Diagnostics.Debug.WriteLine("received output: {0}", args.Data);
            p.Start();
            p.BeginOutputReadLine();


            Thread.Sleep(1000);
            p.Kill();
            //Environment.Exit(0) ;
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
            Application.Current.Shutdown();
        }
    }
}
