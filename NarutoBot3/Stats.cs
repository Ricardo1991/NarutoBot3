using Newtonsoft.Json;
using System.IO;

namespace NarutoBot3
{
    internal class StatsManager
    {
        private Stats session = new Stats();
        private Stats lifetime = new Stats();

        public StatsManager()
        {
            LoadData();
        }

        public void SaveData()
        {
            TextWriter WriteFileStream = new StreamWriter("stats.json", false);

            WriteFileStream.Write(JsonConvert.SerializeObject(lifetime, Formatting.Indented));

            WriteFileStream.Close();
        }

        public void LoadData()
        {
            try
            {
                TextReader stream = new StreamReader("stats.json");
                string json = stream.ReadToEnd();
                JsonConvert.PopulateObject(json, lifetime);
                stream.Close();
            }
            catch
            {
                lifetime = new Stats();
            }
        }

        public void Kill()
        {
            session.IncrementKill();
            lifetime.IncrementKill();
            SaveData();
        }

        public void Fact()
        {
            session.IncrementFact();
            lifetime.IncrementFact();
            SaveData();
        }

        public void Youtube()
        {
            session.IncrementYoutube();
            lifetime.IncrementYoutube();
            SaveData();
        }

        public void Nick()
        {
            session.IncrementNick();
            lifetime.IncrementNick();
            SaveData();
        }

        public void Funk()
        {
            session.IncrementFunk();
            lifetime.IncrementFunk();
            SaveData();
        }

        public void Shuffle()
        {
            session.IncrementShuffle();
            lifetime.IncrementShuffle();
            SaveData();
        }

        public void Choose()
        {
            session.IncrementChoose();
            lifetime.IncrementChoose();
            SaveData();
        }

        public void Quote()
        {
            session.IncrementQuote();
            lifetime.IncrementQuote();
            SaveData();
        }

        public void Trivia()
        {
            session.IncrementTrivia();
            lifetime.IncrementTrivia();
            SaveData();
        }

        public void Poke()
        {
            session.IncrementPoke();
            lifetime.IncrementPoke();
            SaveData();
        }

        public void Wiki()
        {
            session.IncrementWiki();
            lifetime.IncrementWiki();
            SaveData();
        }

        public void Temperature()
        {
            session.IncrementTemperature();
            lifetime.IncrementTemperature();
            SaveData();
        }

        public void Time()
        {
            session.IncrementTime();
            lifetime.IncrementTime();
            SaveData();
        }

        public void Greet()
        {
            session.IncrementGreet();
            lifetime.IncrementGreet();
            SaveData();
        }

        public void Rules()
        {
            session.IncrementRules();
            lifetime.IncrementRules();
            SaveData();
        }

        public void Help()
        {
            session.IncrementHelp();
            lifetime.IncrementHelp();
            SaveData();
        }

        public void Anime()
        {
            session.IncrementAnime();
            lifetime.IncrementAnime();
            SaveData();
        }

        public void Roll()
        {
            session.IncrementRoll();
            lifetime.IncrementRoll();
            SaveData();
        }

        public void Question()
        {
            session.IncrementQuestion();
            lifetime.IncrementQuestion();
            SaveData();
        }

        public void Tell()
        {
            session.IncrementTell();
            lifetime.IncrementTell();
            SaveData();
        }

        public int[] GetKill()
        {
            return new int[2] { session.Kill, lifetime.Kill };
        }

        public int[] GetYoutube()
        {
            return new int[2] { session.Youtube, lifetime.Youtube };
        }

        public int[] GetQuestion()
        {
            return new int[2] { session.Question, lifetime.Question };
        }

        public int[] GetNick()
        {
            return new int[2] { session.Nick, lifetime.Nick };
        }

        public int[] GetFunk()
        {
            return new int[2] { session.Funk, lifetime.Funk };
        }

        public int[] GetShuffle()
        {
            return new int[2] { session.Shuffle, lifetime.Shuffle };
        }

        public int[] GetChoose()
        {
            return new int[2] { session.Choose, lifetime.Choose };
        }

        public int[] GetQuote()
        {
            return new int[2] { session.Quote, lifetime.Quote };
        }

        public int[] GetTrivia()
        {
            return new int[2] { session.Trivia, lifetime.Trivia };
        }

        public int[] GetPoke()
        {
            return new int[2] { session.Poke, lifetime.Poke };
        }

        public int[] GetWiki()
        {
            return new int[2] { session.Wiki, lifetime.Wiki };
        }

        public int[] GetTemperature()
        {
            return new int[2] { session.Temperature, lifetime.Temperature };
        }

        public int[] GetTime()
        {
            return new int[2] { session.Time, lifetime.Time };
        }

        public int[] GetGreet()
        {
            return new int[2] { session.Greet, lifetime.Greet };
        }

        public int[] GetRules()
        {
            return new int[2] { session.Rules, lifetime.Rules };
        }

        public int[] GetHelp()
        {
            return new int[2] { session.Help, lifetime.Help };
        }

        public int[] GetAnime()
        {
            return new int[2] { session.Anime, lifetime.Anime };
        }

        public int[] GetRoll()
        {
            return new int[2] { session.Roll, lifetime.Roll };
        }

        public int[] GetTell()
        {
            return new int[2] { session.Tell, lifetime.Tell };
        }

        public int[] GetFact()
        {
            return new int[2] { session.Fact, lifetime.Fact };
        }
    }

    internal class Stats
    {
        private int kill;

        public int Kill
        {
            get { return kill; }
            set { kill = value; }
        }

        private int fact;

        public int Fact
        {
            get { return fact; }
            set { fact = value; }
        }

        private int youtube;

        public int Youtube
        {
            get { return youtube; }
            set { youtube = value; }
        }

        private int question;

        public int Question
        {
            get { return question; }
            set { question = value; }
        }

        private int nick;

        public int Nick
        {
            get { return nick; }
            set { nick = value; }
        }

        private int funk;

        public int Funk
        {
            get { return funk; }
            set { funk = value; }
        }

        private int shuffle;

        public int Shuffle
        {
            get { return shuffle; }
            set { shuffle = value; }
        }

        private int choose;

        public int Choose
        {
            get { return choose; }
            set { choose = value; }
        }

        private int quote;

        public int Quote
        {
            get { return quote; }
            set { quote = value; }
        }

        private int trivia;

        public int Trivia
        {
            get { return trivia; }
            set { trivia = value; }
        }

        private int poke;

        public int Poke
        {
            get { return poke; }
            set { poke = value; }
        }

        private int wiki;

        public int Wiki
        {
            get { return wiki; }
            set { wiki = value; }
        }

        private int temperature;

        public int Temperature
        {
            get { return temperature; }
            set { temperature = value; }
        }

        private int time;

        public int Time
        {
            get { return time; }
            set { time = value; }
        }

        private int greet;

        public int Greet
        {
            get { return greet; }
            set { greet = value; }
        }

        private int rules;

        public int Rules
        {
            get { return rules; }
            set { rules = value; }
        }

        private int help;

        public int Help
        {
            get { return help; }
            set { help = value; }
        }

        private int anime;

        public int Anime
        {
            get { return anime; }
            set { anime = value; }
        }

        private int roll;

        public int Roll
        {
            get { return roll; }
            set { roll = value; }
        }

        private int tell;

        public int Tell
        {
            get { return tell; }
            set { tell = value; }
        }

        public Stats()
        {
            kill = 0;
            fact = 0;
            youtube = 0;
            question = 0;
            nick = 0;
            funk = 0;
            shuffle = 0;
            choose = 0;
            quote = 0;
            trivia = 0;
            poke = 0;
            wiki = 0;
            temperature = 0;
            time = 0;
            greet = 0;
            rules = 0;
            help = 0;
            anime = 0;
            roll = 0;
            tell = 0;
        }

        public void IncrementKill()
        {
            Kill++;
        }

        public void IncrementFact()
        {
            Fact++;
        }

        public void IncrementYoutube()
        {
            Youtube++;
        }

        public void IncrementQuestion()
        {
            Question++;
        }

        public void IncrementNick()
        {
            Nick++;
        }

        public void IncrementFunk()
        {
            Funk++;
        }

        public void IncrementShuffle()
        {
            Shuffle++;
        }

        public void IncrementChoose()
        {
            Choose++;
        }

        public void IncrementQuote()
        {
            Quote++;
        }

        public void IncrementTrivia()
        {
            Trivia++;
        }

        public void IncrementPoke()
        {
            Poke++;
        }

        public void IncrementWiki()
        {
            Wiki++;
        }

        public void IncrementTemperature()
        {
            Temperature++;
        }

        public void IncrementTime()
        {
            Time++;
        }

        public void IncrementGreet()
        {
            Greet++;
        }

        public void IncrementRules()
        {
            Rules++;
        }

        public void IncrementHelp()
        {
            Help++;
        }

        public void IncrementAnime()
        {
            Anime++;
        }

        public void IncrementRoll()
        {
            Roll++;
        }

        public void IncrementTell()
        {
            Tell++;
        }
    }
}