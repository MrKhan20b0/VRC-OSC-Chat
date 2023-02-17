using BunnyChat.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using VRCChatBox.MVVM.Model;
using VRCOSC;

namespace VRCChatBox.MVVM.ViewModel
{
    enum CharType
    {
        WhiteSpace,
        Latin,
        Arabic,
        ArabicNumeral,
        Other,
        Unkown
    }

    class MainViewModel : ObservableObject
    {
        private string _vrcAddress = "127.0.0.1";
        private int _vrcPort = 9000;
        public ObservableCollection<ChatItem> Messages { get; set; }

        /* Commands */
        public RelayCommand SendCommand { get; set; }

        public RelayCommand CopyOldMessage { get; set; }
        public RelayCommand AppendOldMessage { get; set; }
        public RelayCommand MessageChanged { get; set; }


        private string _charactersRemainingString;
        public string CharactersRemainingString
        {
            get { return _charactersRemainingString; }
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

                SendTyping(Message.Length > 0);
            }
        }

        private bool lastTypingBoolSent = true;

        private TextBlock _dummyTextBlock = new TextBlock();

        private const int VRCHAT_MAX_TEXT_LENGTH = 144;

        public MainViewModel()
        {
            _dummyTextBlock.FontFamily = new FontFamily("Arial");
            Messages = new ObservableCollection<ChatItem>();


            MessageChanged = new RelayCommand(o => { });

            CopyOldMessage = new RelayCommand(o =>
            {
                System.Diagnostics.Debug.WriteLine("OldMessage" + o);
                Message = (string)o;
            });

            AppendOldMessage = new RelayCommand(o =>
            {
                System.Diagnostics.Debug.WriteLine("OldMessage" + o);
                Message += (string)o;

                if (Message.Length >= VRCHAT_MAX_TEXT_LENGTH)
                {
                    Message = Message.Substring(0, VRCHAT_MAX_TEXT_LENGTH);
                }
            });

            SendCommand = new RelayCommand(o =>
            {

                if (string.IsNullOrEmpty(Message))
                {
                    return;
                }

                SendMessage();


                Messages.Add(new ChatItem
                {
                    Message = Message,
                    Time = DateTime.Now.ToString("hh:mm tt")
                });

                


                // Reset message property
                Message = "";
            });


            Message = string.Empty;


        }

 /*       private int MessageActualLength()
        {
            // For every space, we add the "Arabic Character" (U + 061c) Character.
            // This must be done, or else text that shhould be on a new line, wont be in VRChat.
            // IDK why, but adding u061c fixes it.
            // Therefore, every space takes up 2 spaces instead of 1
            return Message.Length + Message.Count(o => 0 == ' ');
        }*/

        public void SendTyping(bool IsTyping)
        {
            // Send typing command
            //if (lastTypingBoolSent != IsTyping)
            {
                var message = new CoreOSC.OscMessage("/chatbox/typing", IsTyping);
                var sender = new CoreOSC.UDPSender(_vrcAddress, _vrcPort);
                sender.Send(message);

                
            }

            lastTypingBoolSent = IsTyping;
        }

        public static IEnumerable<Tuple<string, CharType>> SplitOnTransition(string str)
        {
            StringBuilder builder = new StringBuilder();
            CharType previousType = CharType.Unkown;
            foreach (char c in str)
            {
                CharType type;
                if (char.IsWhiteSpace(c))
                    type = previousType;
                //           0                Z
                else if (c >= '\u0030' && c <= '\u007A') // Latin Chars
                {
                    type = CharType.Latin;
                }
                else if ((c >= '\u0660' && c <= '\u0669') || (c >= '\u06F0' && c <= '\u06F9')) // Arabic Numerals (Numerals do not need to be flipped or shaped)
                {
                    type = CharType.ArabicNumeral;
                }
                else if (c >= '\u0600' && c <= '\u06FF') // Arabic Chars
                {
                    type = CharType.Arabic;
                }
                else
                    type = CharType.Other;               // Untested type

                if (previousType != CharType.Unkown && type != previousType)
                {
                    yield return Tuple.Create(builder.ToString(), previousType);
                    builder.Clear();
                }

                builder.Append(c);
                previousType = type;
            }

            if (builder.Length > 0)
                yield return Tuple.Create(builder.ToString(), previousType);
        }

        public void SendMessage()
        {
            if (Message == string.Empty) { return; }

            // /chatbox/input s b
            foreach (char c in Message)
            {
                System.Diagnostics.Debug.WriteLine(c == '\u0645');
                System.Diagnostics.Debug.WriteLine(c);
                System.Diagnostics.Debug.WriteLine(c.GetType());

            }

            StringBuilder processedText = new StringBuilder();


            CharType prevType = CharType.Unkown;
            foreach (Tuple<string, CharType> st in SplitOnTransition(Message))
            {
                System.Diagnostics.Debug.WriteLine($"Split: {st.Item1} ({st.Item2})");
                
                CharType type = st.Item2;
                string msg = st.Item1;


                if (type == CharType.Arabic)
                {
                    msg = ArabicReshaper.Reshape(msg);
                    processedText.Append(msg);
                }
                else
                {
                    processedText.Append(msg);
                }

                prevType = type;
            }

            // TODO: We need to reverse the order of Arabic Holy shit this is a lot of work.
            //       Making this compatible with arabic sucks

            // TODO: Not a fix, but we could spererate the sentence by terminal punctuation.
            //       We then reverse sentence ordering to fix bottom-to-top issue.
           
            //currentText = ArabicReshaper.Reshape(currentText);
            var message = new CoreOSC.OscMessage("/chatbox/input", processedText.ToString().Replace(" ", " \u061c"), true);
            

            // This uses the invisible times unicode char. It makes the message length accurate, but the spaces are large in VRChat
            //var message = new CoreOSC.OscMessage("/chatbox/input", processedText.ToString().Replace(" ", "\u2062"), true);


            System.Diagnostics.Debug.WriteLine("Processed Text Length: " + processedText.Length + "   Size: " + MeasureString(processedText.ToString()));
            
            var sender = new CoreOSC.UDPSender(_vrcAddress, _vrcPort);
            sender.Send(message);
        }


        
        private Size MeasureString(string candidate)
        {
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(this._dummyTextBlock.FontFamily, this._dummyTextBlock.FontStyle, this._dummyTextBlock.FontWeight, this._dummyTextBlock.FontStretch),
                this._dummyTextBlock.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                VisualTreeHelper.GetDpi(this._dummyTextBlock).PixelsPerDip);

            return new Size(formattedText.Width, formattedText.Height);
        }

    }
}
