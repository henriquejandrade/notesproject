using NotesProject.Component;
using NotesProject.Control;
using NotesProject.Control.View;
using NotesProject.Model;
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

namespace NotesProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TimelineViewControl MyTimeline;

        private Modes Mode;

        public MainWindow()
        {
            InitializeComponent();

            this.MyTimeline = new TimelineViewControl();
            this.MyTimeline.MyView = Timeline;

            // Register Events
            this.KeyUp += MainWindow_KeyUp;
            TextBoxInput.KeyUp += TextBoxInput_KeyUp;
            TextBoxInput.PreviewKeyDown += TextBoxInput_PreviewKeyDown;
            TextBoxInput.TextChanged += TextBoxInput_TextChanged;
            DataObject.AddPastingHandler(TextBoxInput, OnPaste);

            Populate();

            TextBoxInput.Focus();
        }

        private void Populate()
        {
            MyTimeline.Populate();
        }

        private void ClearState()
        {

        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            //switch (e.Key)
            //{
            //    case Key.Tab:
            //        if (StateChanged)
            //        {
            //            StateChanged = false;
            //            TextBoxInput.Focus();
            //        }
            //        break;
            //}
        }

        private void TextBoxInput_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Return:
                    MyTimeline.AddMessage((sender as TextBox).Text);

                    (sender as TextBox).Clear();
                    break;

                case Key.Up:
                    (sender as TextBox).Text = MyTimeline.LastValue;

                    TextBoxInput.SelectionStart = TextBoxInput.Text.Length;
                    TextBoxInput.SelectionLength = 0;
                    break;

                case Key.Escape:
                    ClearState();
                    break;
            }
        }

        private void TextBoxInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                    string test = (sender as TextBox).Text.Trim().ToLower();

                    switch (Mode)
                    {
                        case Modes.Timeline:
                            if (ReservedWords.Any(s => s.ToLower().Equals(test)))
                            {
                                string testFormat = ReservedWords.SingleOrDefault(s => s.ToLower().Equals(test));
                                QueryType newState = (QueryType)Enum.Parse(typeof(QueryType), testFormat);
                                MyTimeline.SetState(newState);

                                TextBoxInput.Clear();
                                //StateChanged = true;
                            }
                            break;
                    }
                    break;

                default: break;
            }
        }

        private void TextBoxInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = TextBoxInput.Text.ToLower();
            if (!string.IsNullOrEmpty(input.Trim()))
            {
                if (ReservedWords.Any(s => s.ToLower().StartsWith(input) && !s.ToLower().Equals(input)))
                {
                    string reserved = ReservedWords.FirstOrDefault(s => s.ToLower().StartsWith(input));
                    int index = input.Length;
                    TextBoxInput.Text += reserved.Substring(input.Length);
                    TextBoxInput.Select(index, TextBoxInput.Text.Length);
                }
            }
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            bool isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
            if (!isText) return;

            if (!TextBoxInput.IsFocused)
            {
                TextBoxInput.Text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
                TextBoxInput.Focus();
            }
        }

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                TextBoxInput.Focus();
                TextBoxInput.Text = Clipboard.GetText();

                TextBoxInput.SelectionStart = TextBoxInput.Text.Length;
                TextBoxInput.SelectionLength = 0;
            }
        }

        private List<string> ReservedWords = new List<string>()
        {
            "WebSearch",
            "Search"
        };
    }

    public enum Modes
    {
        Timeline,
        Bookmarks
    }

    public enum QueryType
    {
        WebSearch,
        Search,
        Default
    }
}
