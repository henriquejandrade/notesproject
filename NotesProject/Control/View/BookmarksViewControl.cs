using NotesProject.Component.Buttons;
using NotesProject.Component.Views;
using NotesProject.Control.Database;
using NotesProject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NotesProject.Control.View
{
    class BookmarksViewControl
    {
        public Bookmarks MyView;

        private BookmarksDB Database;

        private BackgroundWorker WebsiteFetchWorker;

        public BookmarksViewControl()
        {
            this.Database = new BookmarksDB();

            this.Database.CreateDb();

            this.WebsiteFetchWorker = new BackgroundWorker();
            this.WebsiteFetchWorker.DoWork += WebsiteFetchWorker_DoWork;
            this.WebsiteFetchWorker.RunWorkerCompleted += WebsiteFetchWorker_RunWorkerCompleted;

            this.MyView = new Bookmarks();
        }

        public void Populate()
        {
            MyView.StackChat.Children.Clear();

            OperationResult listResult = Database.ListAll();
            if (listResult.Code > 0)
            {
                List<Bookmark> items = (List<Bookmark>)listResult.Data;
                foreach (Bookmark item in items)
                {
                    //ImageSource icon = new BitmapImage(new Uri(item.Favicon, UriKind.RelativeOrAbsolute));

                    ImageLabelButton button = new ImageLabelButton()
                    {
                        Text = item.Title,
                        Icon = new BitmapImage(new Uri(item.Favicon, UriKind.RelativeOrAbsolute))
                    };

                    button.Click += (sender, e) => Bookmark_Click(sender, e, item.Url);

                    MyView.StackChat.Children.Add(button);
                }
            }
        }

        public void Add(string value)
        {
            WebsiteFetchWorker.RunWorkerAsync(new Bookmark(value));
        }

        private void WebsiteFetchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Bookmark bookmark = (Bookmark)e.Argument;

            WebClient x = new WebClient();

            string source = x.DownloadString(new UriBuilder(bookmark.Url).Uri);

            bookmark.Title = Regex.Match(source, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
                            RegexOptions.IgnoreCase).Groups["Title"].Value;

            bookmark.Favicon = "https://www.google.com/s2/favicons?domain=" + bookmark.Url.Replace("https://", "").Replace("http://", ""); //bookmark.Title.Replace(" ", "") + ".png";

            //x.DownloadFile("https://www.google.com/s2/favicons?domain=" + bookmark.Url.Replace("https://", "").Replace("http://", ""), bookmark.Favicon);

            e.Result = bookmark;
        }

        private void WebsiteFetchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MyView.Dispatcher.Invoke(() =>
                {
                    MyView.StackChat.Children.Add(new ImageLabelButton() { Text = "Error!" });
                });
            }
            else
            {
                Bookmark bookmark = e.Result as Bookmark;
                Database.Add(bookmark);

                MyView.Dispatcher.Invoke(() =>
                {
                    ImageLabelButton button = new ImageLabelButton()
                    {
                        Text = bookmark.Title,
                        Icon = new BitmapImage(new Uri(bookmark.Favicon, UriKind.RelativeOrAbsolute))
                        //IconExtensions.ToImageSource(Icon.ExtractAssociatedIcon(bookmark.Favicon))
                    };

                    button.Click += (sender, e) => Bookmark_Click(sender, e, bookmark.Url);

                    MyView.StackChat.Children.Add(button);
                });
            }
        }

        private void Bookmark_Click(object sender, System.Windows.RoutedEventArgs e, string url)
        {
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                    Arguments = url
                }
            };

            process.Start();
        }
    }

    public static class IconExtensions
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);


        public static ImageSource ToImageSource(this Icon icon)
        {
            Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                throw new Win32Exception();
            }

            return wpfBitmap;
        }
    }
}
