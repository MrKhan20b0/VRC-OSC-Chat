using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
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

namespace VRCChatBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {



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


        } // end textChangedEventHandler




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
