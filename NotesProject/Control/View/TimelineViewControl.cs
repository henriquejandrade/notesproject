using NotesProject.Component;
using NotesProject.Component.Buttons;
using NotesProject.Component.Containers;
using NotesProject.Component.Views;
using NotesProject.Control.Database;
using NotesProject.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NotesProject.Control.View
{
    class TimelineViewControl
    {
        public Timeline MyView { get; set; }

        private MainDB MyDatabase;
        private Interpreter MyInterpreter;
        private Runner MyRunner;

        private QueryType State;
        private bool StateChanged;

        public TimelineViewControl()
        {
            this.MyDatabase = new MainDB();
            this.MyInterpreter = new Interpreter();
            this.MyRunner = new Runner();

            this.MyDatabase.CreateDb();
        }

        public void Populate()
        {
            OperationResult listResult = MyDatabase.ListAll();
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

                    ImageLabelButton button = new ImageLabelButton()
                    {
                        Icon = GetIcon(item.Type),
                        Text = item.Value
                    };
                    button.Click += (sender, e) => Item_Click(sender, e, item);

                    MyView.StackChat.Children.Add(button);
                }
            }

            MyView.Scroller.ScrollToBottom();

            State = QueryType.Default;
        }

        private void Item_Click(object sender, System.Windows.RoutedEventArgs e, Item item)
        {
            MyView.Scroller.Visibility = System.Windows.Visibility.Collapsed;
            MyView.GridContainer.Children.Add(CreateContainer(item));
        }

        private ImageSource GetIcon(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Text:
                    return new BitmapImage(new Uri("pack://application:,,,/NotesProject;component/Images/Icons/Things/paper.png", UriKind.RelativeOrAbsolute));

                case ItemType.Url:
                    return new BitmapImage(new Uri("pack://application:,,,/NotesProject;component/Images/Icons/Things/globe.png", UriKind.RelativeOrAbsolute));

                case ItemType.Search:
                    return new BitmapImage(new Uri("pack://application:,,,/NotesProject;component/Images/Icons/Things/magnifier.png", UriKind.RelativeOrAbsolute));

                default: return null;
            }
        }

        public ItemContainer CreateContainer(Item item)
        {
            return new ItemContainer()
            {
                Title = item.Value,
                SubItems = item.SubItems
            };
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
            MyDatabase.Add(newItem);

            // Execute
            MyRunner.Do(newItem);

            // Add to UI
            // TODO: Add new date label if from day to other
            ImageLabelButton button = new ImageLabelButton()
            {
                Icon = GetIcon(newItem.Type),
                Text = newItem.Value
            };
            button.Click += (sender, e) => Item_Click(sender, e, newItem);

            MyView.StackChat.Children.Add(button);

            MyView.Scroller.ScrollToBottom();
        }

        private void AddDate(DateTime date)
        {
            MyView.StackChat.Children.Add(new TextBlock() { Text = date.ToShortDateString() });
        }

        public string LastValue
        {
            get
            {
                return (MyDatabase.GetPrevious().Data as Item).ToString();
            }
        }
    }
}
