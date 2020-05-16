using NotesProject.Component;
using NotesProject.Component.Buttons;
using NotesProject.Component.Containers;
using NotesProject.Component.Views;
using NotesProject.Control;
using NotesProject.Control.Database;
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
    public partial class MainWindow : Window
    {
        private Timeline MyTimeline;

        public MainWindow()
        {
            InitializeComponent();

            this.MyTimeline = new Timeline();
            //this.MyTimeline.ItemClick += MyTimeline_ItemClick;


            MainGrid.Children.Add(MyTimeline);
        }

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                MyTimeline.TreatImage(Clipboard.GetImage());
            }
        }
    }
}
