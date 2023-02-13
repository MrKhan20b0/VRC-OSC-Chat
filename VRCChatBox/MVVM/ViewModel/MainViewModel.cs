using BunnyChat.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VRCChatBox.MVVM.Model;

namespace VRCChatBox.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public ObservableCollection<ChatItem> Messages { get; set; }

        /* Commands */
        public RelayCommand SendCommand { get; set; }

        public RelayCommand CopyOldMessage { get; set; }


        private string _charactersRemainingString;
        public string CharactersRemainingString
        {
            get { return _charactersRemainingString;  }
            set
            {
                _charactersRemainingString = value;
                OnPropertyChanged();
            }
        }

        private string _message = string.Empty;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged();
                CharactersRemainingString = $"{_message.Length}/144";
                
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

                if (string.IsNullOrEmpty(Message))
                {
                    return;
                }


                Messages.Add(new ChatItem
                {
                    Message = Message,
                    Time = DateTime.Now.ToString("hh:mm tt")
                });

                System.Diagnostics.Debug.WriteLine("hello world" + Message);


                // Reset message property
                Message = "";
            });


            Message= string.Empty;

            
        }
    }
}
