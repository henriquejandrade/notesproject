using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

namespace NotesProject.Model
{
    public class Item : INotifyPropertyChanged
    {
        public DateTime Date { get; set; }
        public string Filename { get; set; }
        public ItemType Type { get; set; }
        public string Topic { get; set; }
        private string _Value;
        public string Value
        {
            get { return _Value; }
            set
            {
                if (value != _Value)
                {
                    _Value = value;
                    Notify("Value");
                }
            }
        }
        public List<SubItem> SubItems { get; set; }

        public Item() { }

        public Item(XElement item)
        {
            Date = DateTime.Parse(item.Element("date").Value);
            Filename = item.Element("file").Value;
            Type = (ItemType)Enum.Parse(typeof(ItemType), item.Element("type").Value);
            Topic = item.Element("topic").Value;
            Value = item.Element("value").Value;

            SubItems = new List<SubItem>();
            foreach (XElement element in item.Elements("subitems"))
            {
                SubItems.Add(new SubItem(element));
            }
        }

        public XElement ToXElement()
        {
            XElement item = new XElement("item");
            item.Add(new XElement("date", Date.ToString()));
            item.Add(new XElement("file", Filename));
            item.Add(new XElement("type", Type.ToString()));
            item.Add(new XElement("topic", Topic));
            item.Add(new XElement("value", Value));

            XElement subitems = new XElement("subitems");
            foreach (SubItem subItem in SubItems)
            {
                subitems.Add(subItem.ToXElement());
            }
            item.Add(subitems);

            return item;
        }

        override
        public string ToString()
        {
            switch (Type)
            {
                case ItemType.Search:
                    return "websearch " + Value;
                default:
                    return Value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void Notify(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public enum ItemType
    {
        Text,
        Search,
        Image,
        Video,
        Url,
        File
    }
}
