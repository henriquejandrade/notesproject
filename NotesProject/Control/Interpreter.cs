using NotesProject.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace NotesProject.Control
{
    class Interpreter
    {
        public Item Parse(string value, QueryType state)
        {
            Item newItem = new Item()
            {
                Date = DateTime.Now,
                Filename = "",
                Type = ItemType.Text,
                Topic = "",
                Value = value.Trim()
            };

            switch (state)
            {
                case QueryType.Default:
                    if (value.ToLower().IndexOf("http") == 0)
                    {
                        newItem.Type = ItemType.Url;
                        newItem.Value = value;
                    }
                    else
                    {
                        newItem.Value = value;
                    }
                    break;

                case QueryType.WebSearch:
                    newItem.Type = ItemType.Search;
                    newItem.Value = value;
                    break;

                case QueryType.Search:
                    // Performs local search
                    break;
            }

            return newItem;
        }
    }
}
