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
        public string Text { get { return TextBoxInput.Text; } }

        public event KeyEventHandler KeyUp
        {
            add { TextBoxInput.KeyUp += value; }
            remove { TextBoxInput.KeyUp -= value; }
        }

        public MessageComponent()
        {
            InitializeComponent();

            this.Loaded += MessageComponent_Loaded;
        }

        private void MessageComponent_Loaded(object sender, RoutedEventArgs e)
        {
            TextBoxInput.Focus();
        }
    }
}
