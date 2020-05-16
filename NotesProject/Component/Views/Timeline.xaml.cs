using Microsoft.Win32;
using NotesProject.Component.Buttons;
using NotesProject.Component.Containers;
using NotesProject.Config;
using NotesProject.Control;
using NotesProject.Control.Database;
using NotesProject.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NotesProject.Component.Views
{
    public partial class Timeline : UserControl
    {
        private MainDB MyDatabase;
        private Interpreter MyInterpreter;
        private Runner MyRunner;

        private QueryType State;
        private bool StateChanged;

        private List<Item> Items;

        private Modes Mode;

        public delegate void ItemClickHandler(object sender, ItemClickEventArgs e);
        public event ItemClickHandler ItemClick;

        public string LastValue
        {
            get
            {
                return (MainDB.GetPrevious().Data as Item).ToString();
            }
        }

        public object DialogResult { get; private set; }

        public Timeline()
        {
            this.MyDatabase = new MainDB();
            this.MyInterpreter = new Interpreter();
            this.MyRunner = new Runner();

            InitializeComponent();

            this.Items = new List<Item>();

            this.ButtonText.Click += ButtonText_Click;
            this.ButtonImage.Click += ButtonImage_Click;

            // Register Events
            //this.KeyUp += MainWindow_KeyUp;
            //TextBoxInput.KeyUp += TextBoxInput_KeyUp;
            //TextBoxInput.PreviewKeyDown += TextBoxInput_PreviewKeyDown;
            //TextBoxInput.TextChanged += TextBoxInput_TextChanged;
            //DataObject.AddPastingHandler(TextBoxInput, OnPaste);

            Populate();

            //TextBoxInput.Focus();
        }

        private void Populate()
        {
            OperationResult listResult = MainDB.ListAll();
            if (listResult.Code > 0)
            {
                List<Item> items = (List<Item>)listResult.Data;
                //DateTime currentDate = items[0].Date;
                //AddDate(currentDate);
                foreach (Item item in items)
                {
                    // Refreshes current date labelling
                    //if (!item.Date.ToShortDateString().Equals(currentDate.ToShortDateString()))
                    //{
                    //    currentDate = item.Date;
                    //    AddDate(currentDate);
                    //}

                    ItemContainer itemContainer = new ItemContainer(item);
                    itemContainer.BodyVisibility = Visibility.Collapsed;
                    itemContainer.ButtonDone.Click += (sender, e) => ItemButtonDone_Click(sender, e, itemContainer);
                    //ImageLabelButton button = new ImageLabelButton()
                    //{
                    //    Icon = GetIcon(item.Type),
                    //    Text = item.Value
                    //};

                    //button.Click += (sender, e) => Item_Click(sender, e, item);

                    //button.Click += (sender, e) => Item_Click(sender, e, item);

                    StackChat.Children.Add(itemContainer);
                }
            }

            Scroller.ScrollToBottom();

            State = QueryType.Default;
        }

        private void ItemButtonDone_Click(object sender, RoutedEventArgs e, ItemContainer itemContainer)
        {
            MainDB.Remove(itemContainer.MyItem);
            StackChat.Children.Remove(itemContainer);
        }

        public void SetState(QueryType state)
        {
            this.State = state;
            //switch (this.State)
            //{
            //    default:
            //        ButtonOptions.Icon = new BitmapImage(new Uri("pack://application:,,,/NotesProject;component/Images/Icons/Symbols/circle.png", UriKind.RelativeOrAbsolute));
            //        break;

            //    case QueryType.WebSearch:
            //        ButtonOptions.Icon = new BitmapImage(new Uri("pack://application:,,,/NotesProject;component/Images/Icons/Things/magnifier.png", UriKind.RelativeOrAbsolute));
            //        break;

            //    case QueryType.Search:
            //        ButtonOptions.Icon = new BitmapImage(new Uri("pack://application:,,,/NotesProject;component/Images/Icons/Things/magnifier.png", UriKind.RelativeOrAbsolute));
            //        break;
            //}
        }

        public void AddMessage(string value)
        {
            // Recognize
            Item newItem = MyInterpreter.Parse(value, State);

            // Save
            MainDB.Add(newItem);

            // Execute
            MyRunner.Do(newItem);

            // Add to UI
            ItemContainer container = new ItemContainer(newItem);

            StackChat.Children.Add(container);

            Scroller.ScrollToBottom();
        }

        public void AddImage(string title, string filepath)
        {
            // Recognize
            Item newItem = new Item()
            {
                Date = DateTime.Now,
                Filename = filepath,
                Type = ItemType.Image,
                Topic = "",
                Value = title
            };

            // Save
            MainDB.Add(newItem);

            // Add to UI
            ItemContainer container = new ItemContainer(newItem);

            StackChat.Children.Add(container);

            Scroller.ScrollToBottom();
        }

        public void TreatImage(BitmapSource bitmap)
        {
            string filepath = Folders.Images + DateTime.Now.Year + "/" + DateTime.Now.Month + "/";

            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            int index = Directory.EnumerateFiles(filepath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(f => f.EndsWith(".jpg") || f.EndsWith(".png") || f.EndsWith(".gif") || f.EndsWith(".jpeg")).Count() + 1;

            string filename = index + ".png";
            string title = DateTime.Now.ToString().Replace('/', '-').Replace(':', '-').Replace(' ', '-');

            using (FileStream fileStream = new FileStream(filepath + filename, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(fileStream);
            }

            AddImage(title, filepath + filename);
        }

        private void ButtonText_Click(object sender, RoutedEventArgs e)
        {
            MessageComponent component = new MessageComponent();
            component.KeyUp += (sender2, e2) => Component_KeyUp(sender2, e2, component);
            component.ButtonCancel.Click += (sender1, e) =>
            {
                StackChat.Children.Remove(component);
            };

            StackChat.Children.Add(component);

        }

        private void ButtonImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open Image";
            dlg.Filter = "Images (*.jpg;*.png;*.gif;*.jpeg)|*.jpg;*.png;*.gif;*.jpeg";

            if (dlg.ShowDialog() == true)
            {
                AddImage(dlg.SafeFileName, dlg.FileName);
            }
        }

        private void Component_KeyUp(object sender, KeyEventArgs e, MessageComponent component)
        {
            switch (e.Key)
            {
                case Key.Return:
                    AddMessage(component.Text);
                    StackChat.Children.Remove(component);
                    break;

                case Key.Escape:
                    StackChat.Children.Remove(component);
                    break;
            }
        }
    }

    public class ItemClickEventArgs : EventArgs
    {
        public Item Item { get; set; }

        public ItemClickEventArgs(Item item)
        {
            this.Item = item;
        }
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
