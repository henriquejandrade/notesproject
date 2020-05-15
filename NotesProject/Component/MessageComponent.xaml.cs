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

namespace NotesProject.Component
{
    /// <summary>
    /// Interaction logic for Message.xaml
    /// </summary>
    public partial class MessageComponent : UserControl
    {
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(MessageComponent));

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(MessageComponent));

        public string Message { get; set; }

        public MessageComponent()
        {
            InitializeComponent();
        }

        public MessageComponent(string value, ItemType itemType)
        {
            InitializeComponent();
            DataContext = this;

            this.Message = value;
        }
    }
}
