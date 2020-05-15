using NotesProject.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NotesProject.Control.Database
{
    class BookmarksDB
    {
        private static string dbpath = "bookmarks.xml";

        public OperationResult CreateDb()
        {
            OperationResult result = new OperationResult(-1, "error");

            if (!File.Exists(dbpath))
            {
                XDocument dbxml = new XDocument();
                XElement rootNode = new XElement("bookmarks");
                dbxml.Add(rootNode);
                dbxml.Save(dbpath);

                result = new OperationResult(1, "success");
            }

            return result;
        }

        public OperationResult Add(Bookmark item)
        {
            OperationResult result = new OperationResult(0, "couldn't add");

            try
            {
                XDocument db = XDocument.Load(dbpath);
                XElement items = db.Element("bookmarks");
                items.Add(item.ToXElement());
                db.Save(dbpath);

                result = new OperationResult(1, "add successful");
            }
            catch
            {
                result = new OperationResult(-1, "couldn't add");
            }

            return result;
        }

        public OperationResult ListAll()
        {
            OperationResult result = new OperationResult(0, "couldn't list");

            try
            {
                XDocument db = XDocument.Load(dbpath);
                XElement root = db.Element("bookmarks");
                List<XElement> nodes = root.Elements("bookmark").ToList();
                List<Bookmark> items = new List<Bookmark>();
                foreach (XElement node in nodes)
                {
                    items.Add(new Bookmark(node));
                }

                result.Code = 1;
                result.Message = "list success";
                result.Data = items;
            }
            catch
            {
                result.Code = -1;
                result.Message = "list failed";
            }

            return result;
        }
    }
}
