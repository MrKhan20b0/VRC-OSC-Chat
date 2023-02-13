using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace VRCChatBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string message = string.Empty;

        public string CurrentMessage
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
                OnPropertyChanged(nameof(CurrentMessage));
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
        }


        /*private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessageHandler(null, null);
            }
        }

        private void TextChangedhandler(object sender, TextChangedEventArgs args)
        {
            // Omitted Code: Insert code that does something whenever
            // the text changes...
            System.Diagnostics.Debug.WriteLine("Len: "+message.Length);

            if (message.Length >= 144)
            {
                System.Diagnostics.Debug.WriteLine("Setting to RED");
                TextBox.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF2463AE");
            }
            else
            {
                TextBox.Foreground = Brushes.AntiqueWhite;
            }
        } // end textChangedEventHandler
    

        private void SendMessageHandler(object? sender, RoutedEventArgs? e)
        {
            System.Diagnostics.Debug.WriteLine("Current Text:" + message);
            TextBox.Text= string.Empty;
        }*/


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
