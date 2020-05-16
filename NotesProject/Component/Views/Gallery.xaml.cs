using NotesProject.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
    /// <summary>
    /// Interaction logic for Gallery.xaml
    /// </summary>
    public partial class Gallery : UserControl
    {
        public Gallery()
        {
            InitializeComponent();

            this.Drop += Gallery_Drop;

            Populate();
        }

        private void Populate()
        {
            string filepath = Folders.Images;

            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            DirectoryInfo directory = new DirectoryInfo(filepath);
            FileInfo[] files = directory.GetFiles("*.*", SearchOption.AllDirectories)
                .Where(f => f.FullName.EndsWith(".jpg") || f.FullName.EndsWith(".png") || f.FullName.EndsWith(".gif") || f.FullName.EndsWith(".jpeg"))
                .OrderByDescending(f => f.LastWriteTime)
                .ToArray();

            ImagesPanel.Children.Clear();
            foreach (FileInfo file in files)
            {
                ImagesPanel.Children.Add(
                    new Image()
                    {
                        Source = new BitmapImage(new Uri(file.FullName, UriKind.RelativeOrAbsolute))
                    });
            }
        }

        private void Gallery_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                string[] imageFiles = files.Where(f => f.EndsWith(".jpg") || f.EndsWith(".png") || f.EndsWith(".gif") || f.EndsWith(".jpeg")).ToArray();

                foreach (string file in imageFiles)
                {
                    DateTime imageDate = File.GetLastWriteTime(file);
                    string filepath = Folders.Images + imageDate.Year + "/" + imageDate.Month + "/";
                    if (!Directory.Exists(filepath)) Directory.CreateDirectory(filepath);

                    int index = Directory.GetFiles(filepath, "*.*", SearchOption.TopDirectoryOnly)
                        .Count(f => f.EndsWith(".jpg") || f.EndsWith(".png") || f.EndsWith(".gif") || f.EndsWith(".jpeg")) + 1;
                    string filename = index + System.IO.Path.GetExtension(file);

                    File.Move(file, filepath + filename);
                }

                Populate();
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string link = (string)e.Data.GetData(DataFormats.Text);

                WebRequest request = WebRequest.Create(link);
                WebResponse response = request.GetResponse();
                if (response.ContentType.Contains("image/"))
                {
                    WebClient client = new WebClient();

                    string filepath = Folders.Images + DateTime.Now.Year + "/" + DateTime.Now.Month + "/";
                    if (!Directory.Exists(filepath)) Directory.CreateDirectory(filepath);

                    int index = Directory.GetFiles(filepath, "*.*", SearchOption.TopDirectoryOnly)
                        .Count(f => f.EndsWith(".jpg") || f.EndsWith(".png") || f.EndsWith(".gif") || f.EndsWith(".jpeg")) + 1;
                    string filename = index + ".png";

                    client.DownloadFile(link, filepath + filename);

                    Populate();
                }
            }
        }
    }
}
