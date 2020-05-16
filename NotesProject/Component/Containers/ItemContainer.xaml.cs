using NotesProject.Component.Buttons;
using NotesProject.Control.Database;
using NotesProject.Model;
using System;
using System.Collections.Generic;
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

namespace NotesProject.Component.Containers
{
    public partial class ItemContainer : UserControl
    {
        public Item MyItem { get; set; }

        public Visibility BodyVisibility
        {
            get { return GridBody.Visibility; }
            set { GridBody.Visibility = value; }
        }

        public ItemContainer() { }

        public ItemContainer(Item item)
        {
            this.MyItem = item;

            InitializeComponent();
            this.ButtonTitle.Icon = LoadIcon();
            this.ButtonTitle.Text = MyItem.Value;

            this.GridControls.Visibility = Visibility.Collapsed;

            ButtonTitle.Click += ButtonTitle_Click;
            ButtonNewText.Click += ButtonNewText_Click;

            // Draws image if available
            ImageFrame.Visibility = MyItem.Type == ItemType.Image ? Visibility.Visible : Visibility.Collapsed;
            if (MyItem.Type == ItemType.Image)
            {
                ImageFrame.Source = new BitmapImage(new Uri(MyItem.Filename, UriKind.RelativeOrAbsolute));
            }

            foreach (SubItem subItem in item.SubItems)
            {
                this.StackSubItems.Children.Add(subItemButtonFactory(subItem));
            }
        }

        private ImageSource LoadIcon()
        {
            switch (MyItem.Type)
            {
                case ItemType.Text:
                    return new BitmapImage(new Uri("pack://application:,,,/NotesProject;component/Images/Icons/Things/paper.png", UriKind.RelativeOrAbsolute));
                
                case ItemType.Image:
                    return new BitmapImage(new Uri("pack://application:,,,/NotesProject;component/Images/Icons/Things/picture.png", UriKind.RelativeOrAbsolute));

                case ItemType.Url:
                    return new BitmapImage(new Uri("pack://application:,,,/NotesProject;component/Images/Icons/Nature/globe.png", UriKind.RelativeOrAbsolute));

                case ItemType.Search:
                    return new BitmapImage(new Uri("pack://application:,,,/NotesProject;component/Images/Icons/Things/magnifier.png", UriKind.RelativeOrAbsolute));

                default: return null;
            }
        }

        private ImageLabelButton subItemButtonFactory(SubItem subItem)
        {
            ImageLabelButton subItemButton = new ImageLabelButton() { Text = subItem.Text };

            subItemButton.Click += (sender, e) =>
            {
                StackSubItems.Children.Remove(subItemButton);
                MyItem.SubItems.Remove(subItem);
                MainDB.Update(MyItem);
            };

            return subItemButton;
        }

        private void AddSubItem(string text)
        {
            SubItem subItem = new SubItem() { Text = text };

            this.StackSubItems.Children.Add(subItemButtonFactory(subItem));
            MyItem.SubItems.Add(subItem);
            MainDB.Update(MyItem);
        }

        private void ButtonTitle_Click(object sender, RoutedEventArgs e)
        {
            BodyVisibility = BodyVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            GridControls.Visibility = BodyVisibility;
        }

        private void ButtonNewText_Click(object sender, RoutedEventArgs e)
        {
            MessageComponent message = new MessageComponent();

            message.ButtonCancel.Click += (sender1, e1) =>
            {
                StackSubItems.Children.Remove(message);
            };

            message.KeyUp += (sender2, e2) =>
            {
                switch (e2.Key)
                {
                    case Key.Return:
                        AddSubItem(message.Text);
                        StackSubItems.Children.Remove(message);
                        break;

                    case Key.Escape:
                        StackSubItems.Children.Remove(message);
                        break;
                }
            };

            StackSubItems.Children.Add(message);
        }
    }
}
