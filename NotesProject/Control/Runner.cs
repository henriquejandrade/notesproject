using NotesProject.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NotesProject.Control
{
    class Runner
    {
        public void Do(Model.Item item)
        {
            switch (item.Type)
            {
                case ItemType.Search:
                    Process process = new Process()
                    {
                        StartInfo = new ProcessStartInfo()
                        {
                            FileName = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                            Arguments = "\"https://www.google.com/search?q=" + Uri.EscapeDataString(item.Value) + "\""
                        }
                    };

                    process.Start();
                    break;
            }
        }
    }
}
