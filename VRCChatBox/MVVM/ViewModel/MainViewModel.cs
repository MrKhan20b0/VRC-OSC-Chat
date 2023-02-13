using BunnyChat.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using VRCChatBox.MVVM.Model;

namespace VRCChatBox.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public ObservableCollection<ChatItem> Messages { get; set; }

        /* Commands */
        public RelayCommand SendCommand { get; set; }

        public RelayCommand CopyOldMessage { get; set; }

        private string _message = string.Empty;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel() 
        {
            Messages = new ObservableCollection<ChatItem>();

            CopyOldMessage = new RelayCommand(o =>
            {
                System.Diagnostics.Debug.WriteLine("OldMessage" + o);
                Message = (string) o;
            });

            SendCommand = new RelayCommand(o =>
            {
                Messages.Add(new ChatItem
                {
                    Message = Message,
                    Time = DateTime.Now.ToString("hh:mm tt")
                });

                System.Diagnostics.Debug.WriteLine("hello world" + Message);


                // Reset message property
                Message = "";
            });

            
        }
    }
}
