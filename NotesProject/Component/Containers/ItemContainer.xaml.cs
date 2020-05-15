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
    /// <summary>
    /// Interaction logic for ItemContainer.xaml
    /// </summary>
    public partial class ItemContainer : UserControl
    {
        public string Title { get; set; }

        public List<SubItem> SubItems
        {
            set
            {
                StackMessages.Children.Clear();
                foreach (SubItem subItem in value)
                {
                    StackMessages.Children.Add(
                        new MessageComponent()
                        {
                            Message = subItem.Text
                        });
                }
            }
        }

        public ItemContainer()
        {
            InitializeComponent();
        }
    }
}
