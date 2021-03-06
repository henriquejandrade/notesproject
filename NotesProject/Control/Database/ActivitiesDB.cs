﻿using NotesProject.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace NotesProject.Control.Database
{
    class MainDB
    {
        public static OperationResult CreateDb()
        {
            OperationResult result = new OperationResult(-1, "error");

            if (!File.Exists("db.xml"))
            {
                XDocument dbxml = new XDocument();
                XElement rootNode = new XElement("items");
                dbxml.Add(rootNode);
                dbxml.Save("db.xml");

                result = new OperationResult(1, "success");
            }

            return result;
        }

        public static OperationResult Add(Item item)
        {
            OperationResult result = new OperationResult(0, "couldn't add");

            try
            {
                XDocument db = XDocument.Load("db.xml");
                XElement items = db.Element("items");
                items.Add(item.ToXElement());
                db.Save("db.xml");

                result = new OperationResult(1, "add successful");
            }
            catch
            {
                result = new OperationResult(-1, "couldn't add");
            }

            return result;
        }

        public static OperationResult ListAll()
        {
            OperationResult result = new OperationResult(0, "couldn't list");

            try
            {
                XDocument db = XDocument.Load("db.xml");
                XElement root = db.Element("items");
                List<XElement> nodes = root.Elements("item").ToList();
                List<Item> items = new List<Item>();
                foreach (XElement node in nodes)
                {
                    items.Add(new Item(node));
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

        public static OperationResult GetPrevious()
        {
            OperationResult result = new OperationResult(0, "couldn't find");

            XDocument db = XDocument.Load("db.xml");
            XElement root = db.Element("items");
            List<XElement> nodes = root.Elements("item").ToList();

            if (nodes.Count > 0)
            {
                Item previous = new Item(nodes.Last());

                result.Code = 1;
                result.Message = "success";
                result.Data = previous;
            }
            else
            {
                result.Code = -1;
                result.Message = "fail";
            }

            return result;
        }

        public static OperationResult Update(Item item)
        {
            OperationResult result = new OperationResult(0, "couldn't update");

            try
            {
                XDocument db = XDocument.Load("db.xml");
                XElement items = db.Element("items");

                XElement xItem = items.Elements("item").FirstOrDefault(x => x.Element("date").Value == item.Date.ToString());
                if (xItem != null)
                {
                    xItem.Remove();
                    items.Add(item.ToXElement());
                    db.Save("db.xml");

                    result = new OperationResult(1, "update successful");
                }
                else
                {
                    result.Code = -1;
                    result.Message = "didn't find item";
                }
            }
            catch
            {
                result = new OperationResult(-2, "couldn't update");
            }

            return result;
        }

        public static OperationResult Remove(Item item)
        {
            OperationResult result = new OperationResult(0, "couldn't remove");

            try
            {
                XDocument db = XDocument.Load("db.xml");
                XElement items = db.Element("items");

                XElement xItem = items.Elements("item").FirstOrDefault(x => x.Element("date").Value == item.Date.ToString());
                if (xItem != null)
                {
                    xItem.Remove();
                    db.Save("db.xml");

                    result = new OperationResult(1, "remove successful");
                }
                else
                {
                    result.Code = -1;
                    result.Message = "didn't find item";
                }
            }
            catch
            {
                result = new OperationResult(-2, "couldn't remove");
            }

            return result;
        }
    }
}
