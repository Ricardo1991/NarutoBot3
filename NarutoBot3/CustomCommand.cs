﻿using NarutoBot3.Properties;
using System;
using System.Collections.Generic;
using System.IO;

namespace NarutoBot3
{
    public class CustomCommand
    {
        private string name;
        private string format;
        private string author;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string Format
        {
            get
            {
                return format;
            }

            set
            {
                format = value;
            }
        }

        public string Author
        {
            get
            {
                return author;
            }

            set
            {
                author = value;
            }
        }

        public CustomCommand(string author, string name, string format)
        {
            this.Name = name;
            this.Format = format;
            this.Author = author;
        }

        public static List<CustomCommand> LoadCustomCommands()
        {
            List<CustomCommand> command = new List<CustomCommand>();
            string line;

            command.Clear();

            if (File.Exists("TextFiles/customCommands.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/customCommands.txt");
                    while (sr.Peek() >= 0)
                    {
                        line = sr.ReadLine();
                        string[] splitLine = line.Split(new char[] { ' ' }, 3);

                        command.Add(new CustomCommand(splitLine[0], splitLine[1], splitLine[2]));
                    }
                    sr.Close();
                }
                catch
                {
                }
            }
            else
            {
                Settings.Default.help_Enabled = false;
                Settings.Default.Save();
            }

            return command;
        }

        public static void SaveCustomCommands(List<CustomCommand> commands)
        {
            using (StreamWriter newTask = new StreamWriter("TextFiles/customCommands.txt", false))
            {
                foreach (CustomCommand q in commands)
                {
                    newTask.WriteLine(q.Author + " " + q.Name + " " + q.Format);
                }
            }
        }

        public static bool CommandExists(string name, List<CustomCommand> commands)
        {
            foreach (CustomCommand q in commands)
            {
                if (String.Compare(q.Name, name, true) == 0)
                    return true;
            }

            return false;
        }

        public static CustomCommand GetCustomCommandByName(string name, List<CustomCommand> commands)
        {
            foreach (CustomCommand q in commands)
            {
                if (String.Compare(q.Name, name, true) == 0)
                    return q;
            }

            return null;
        }

        public static void RemoveCommandByName(string name, List<CustomCommand> commands)
        {
            foreach (CustomCommand q in commands)
            {
                if (String.Compare(q.Name, name, true) == 0)
                {
                    commands.Remove(GetCustomCommandByName(name, commands));
                    return;
                }
            }
        }
    }
}