using NarutoBot3.Properties;
using System;
using System.Collections.Generic;
using System.IO;

namespace NarutoBot3
{
    public class CustomCommand
    {
        public string name;
        public string format;
        public string author;

        public CustomCommand(string author, string name, string format)
        {
            this.name = name;
            this.format = format;
            this.author = author;
        }

        public static List<CustomCommand> loadCustomCommands()
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

        public static void saveCustomCommands(List<CustomCommand> commands)
        {
            using (StreamWriter newTask = new StreamWriter("TextFiles/customCommands.txt", false))
            {
                foreach (CustomCommand q in commands)
                {
                    newTask.WriteLine(q.author + " " + q.name + " " + q.format);
                }
            }
        }

        public static bool commandExists(string name, List<CustomCommand> commands)
        {
            foreach (CustomCommand q in commands)
            {
                if (String.Compare(q.name, name, true) == 0)
                    return true;
            }

            return false;
        }

        public static CustomCommand getCustomCommandByName(string name, List<CustomCommand> commands)
        {
            foreach (CustomCommand q in commands)
            {
                if (String.Compare(q.name, name, true) == 0)
                    return q;
            }

            return null;
        }

        public static void RemoveCommandByName(string name, List<CustomCommand> commands)
        {
            foreach (CustomCommand q in commands)
            {
                if (String.Compare(q.name, name, true) == 0)
                {
                    commands.Remove(getCustomCommandByName(name, commands));
                    return;
                }
            }
        }
    }
}