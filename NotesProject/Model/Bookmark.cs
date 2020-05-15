using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace NotesProject.Model
{
    class Bookmark
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Favicon { get; set; }
        public long Views { get; set; }

        public Bookmark() { }

        public Bookmark(string url)
        {
            this.Url = url;
        }

        public Bookmark(XElement item)
        {
            Title = item.Element("title").Value;
            Url = item.Element("url").Value;
            Favicon = item.Element("favicon").Value;
            Views = long.Parse(item.Element("views").Value);
        }

        public XElement ToXElement()
        {
            XElement item = new XElement("bookmark");
            item.Add(new XElement("title", Title));
            item.Add(new XElement("url", Url));
            item.Add(new XElement("favicon", Favicon));
            item.Add(new XElement("views", Views));

            return item;
        }
    }
}
