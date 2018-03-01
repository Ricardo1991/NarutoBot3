using NarutoBot3.Properties;
using System.Collections.Generic;
using System.IO;
using TextMarkovChains;

namespace NarutoBot3
{
    class StringLibrary
    {
        private List<string> rls = new List<string>();
        private List<string> hlp = new List<string>();
        private List<string> tri = new List<string>();
        private List<string> kill = new List<string>();
        private List<string> facts = new List<string>();
        private List<string> quotes = new List<string>();
        private List<string> funk = new List<string>();
        private List<string> nickGenStrings;
        private List<CustomCommand> customCommands = new List<CustomCommand>();

        private List<int> killsUsed = new List<int>();
        private List<int> factsUsed = new List<int>();

        private TextMarkovChain killgen = new TextMarkovChain();

        public List<string> Rules { get => rls; set => rls = value; }
        public List<string> Help { get => hlp; set => hlp = value; }
        public List<string> Trivia { get => tri; set => tri = value; }
        public List<string> Kill { get => kill; set => kill = value; }
        public List<string> Facts { get => facts; set => facts = value; }
        public List<string> Quotes { get => quotes; set => quotes = value; }
        public List<string> Funk { get => funk; set => funk = value; }
        public List<string> NickGenStrings { get => nickGenStrings; set => nickGenStrings = value; }
        public List<CustomCommand> CustomCommands { get => customCommands; set => customCommands = value; }
        public List<int> KillsUsed { get => killsUsed; set => killsUsed = value; }
        public List<int> FactsUsed { get => factsUsed; set => factsUsed = value; }
        public TextMarkovChain Killgen { get => killgen; set => killgen = value; }

        public StringLibrary()
        {
            ReloadLibrary();
        }

        public bool ReloadLibrary()
        {
            ReadHelp();                 //Help text
            ReadTrivia();               //Trivia strings
            ReadKills();                //Read the killstrings
            ReadFacts();                //Read the factStrings
            ReadNickGen();              //For the Nick generator
            ReadQuotes();
            ReadFunk();
            ReadRules();
            CustomCommands = CustomCommand.loadCustomCommands();

            return true;
        }

        public bool ReloadLibrary(string name)
        {
            switch (name.ToLower())
            {
                case "all":
                    ReloadLibrary();
                    break;
                case "rules":
                case "rule":
                    ReadRules();
                    break;

                case "help":
                    ReadHelp();
                    break;

                case "nick":
                case "nicks":
                    ReadNickGen();
                    break;

                case "trivia":
                case "trivias":
                    ReadTrivia();
                    break;

                case "kills":
                case "kill":
                    ReadKills();
                    break;

                case "facts":
                case "fact":
                    ReadFacts();
                    break;

                case "quotes":
                case "quote":
                    ReadQuotes();
                    break;

                case "funk":
                    ReadFunk();
                    break;

                default:
                    return false;
            }
            return true;
        }

        public bool SaveLibrary()
        {
            saveFunk();
            saveQuotes();


            return true;
        }

        public bool SaveLibrary(string name)
        {
            switch (name.ToLower())
            {
                case "all":
                    SaveLibrary();
                    break;
                case "rules":
                case "rule":
                    
                    break;

                case "help":
                   
                    break;

                case "nick":
                case "nicks":
                    
                    break;

                case "trivia":
                case "trivias":
                    
                    break;

                case "kills":
                case "kill":
                    
                    break;

                case "facts":
                case "fact":
                    
                    break;

                case "quotes":
                case "quote":
                    saveQuotes();
                    break;

                case "funk":
                    saveFunk();
                    break;

                default:
                    return false;
            }
            return true;
        }

        private void ReadKills()
        {
            Kill.Clear();
            KillsUsed.Clear();
            Killgen = new TextMarkovChain();

            if (File.Exists("TextFiles/kills.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/kills.txt");
                    while (sr.Peek() >= 0)
                    {
                        string killS = sr.ReadLine();

                        if (killS.Length > 1 && !(killS[0] == '/' && killS[1] == '/'))
                        {
                            Kill.Add(killS);
                            Killgen.feed(killS);
                        }
                    }

                    sr.Close();
                }
                catch
                {
                }
            }
            else
            {
                Settings.Default.killEnabled = false;
                Settings.Default.Save();
            }
        }

        private void ReadFacts()
        {
            Facts.Clear();
            FactsUsed.Clear();

            if (File.Exists("TextFiles/facts.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/facts.txt");
                    while (sr.Peek() >= 0)
                    {
                        string factS = sr.ReadLine();

                        if (factS.Length > 1 && !(factS[0] == '/' && factS[1] == '/'))
                        {
                            Facts.Add(factS);
                        }
                    }

                    sr.Close();
                }
                catch
                {
                }
            }
            else
            {
                Settings.Default.factsEnabled = false;
                Settings.Default.Save();
            }
        }

        private void ReadRules()
        {
            Rules.Clear();
            if (File.Exists("TextFiles/rules.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/rules.txt");
                    while (sr.Peek() >= 0)
                    {
                        Rules.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                catch
                {
                }
            }
            else
            {
                Settings.Default.rules_Enabled = false;
                Settings.Default.Save();
            }
        }

        private void ReadHelp()
        {
            Help.Clear();
            if (File.Exists("TextFiles/help.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/help.txt");
                    while (sr.Peek() >= 0)
                    {
                        Help.Add(sr.ReadLine());
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
        }

        private void ReadTrivia() //Reads the Trivia stuff
        {
            Trivia.Clear();

            if (File.Exists("TextFiles/trivia.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/trivia.txt");
                    while (sr.Peek() >= 0)
                    {
                        Trivia.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                catch
                {
                }
            }
            else
            {
                Settings.Default.triviaEnabled = false;
                Settings.Default.Save();
            }
        }
        private void ReadNickGen()//These are for the Nick gen
        {
            NickGenStrings = new List<string>();
            NickGenStrings.Clear();
            if (File.Exists("TextFiles/nickGen.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/nickGen.txt");
                    while (sr.Peek() >= 0)
                    {
                        NickGenStrings.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                catch
                {
                }
            }
            else
            {
                Settings.Default.nickEnabled = false;
                Settings.Default.Save();
            }
        }

        private void ReadQuotes()
        {
            Quotes = new List<string>();
            Quotes.Clear();

            if (File.Exists("TextFiles/quotes.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/quotes.txt");
                    while (sr.Peek() >= 0)
                    {
                        Quotes.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                catch
                {
                }
            }
        }

        private void saveQuotes()
        {
            using (StreamWriter newTask = new StreamWriter("TextFiles/quotes.txt", false))
            {
                foreach (string q in Quotes)
                {
                    newTask.WriteLine(q);
                }
            }
        }

        private void ReadFunk()
        {
            Funk = new List<string>();
            Funk.Clear();

            if (File.Exists("TextFiles/funk.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/funk.txt");
                    while (sr.Peek() >= 0)
                    {
                        Funk.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                catch
                {
                }
            }
        }

        private void saveFunk()
        {
            using (StreamWriter newTask = new StreamWriter("TextFiles/Funk.txt", false))
            {
                foreach (string q in Funk)
                {
                    newTask.WriteLine(q);
                }
            }
        }
    }
}
