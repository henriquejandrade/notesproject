using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace NotesProject.Model
{
    public class SubItem
    {
        public string Text { get; set; }
        public ItemType Type { get; set; }

        public SubItem() { }

        public SubItem(XElement subitem)
        {
            Text = subitem.Element("text").Value;
            Type = (ItemType)Enum.Parse(typeof(ItemType), subitem.Element("type").Value);
        }

        public XElement ToXElement()
        {
            XElement subitem = new XElement("subitem");
            subitem.Add(new XElement("text", Text));
            subitem.Add(new XElement("type", Type));

            return subitem;
        }
    }
}
