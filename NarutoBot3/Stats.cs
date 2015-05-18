using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace NarutoBot3
{
    class StatsManager
    {
        private Stats session = new Stats();
        private Stats lifetime = new Stats();

        public StatsManager() { 
            loadData();
        }

        public void saveData()
        {
            TextWriter WriteFileStream = new StreamWriter("stats.json", false);

            WriteFileStream.Write(JsonConvert.SerializeObject(lifetime));

            WriteFileStream.Close();
        }
        public void loadData()
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

        public void kill()
        {
            session.incrementKill();
            lifetime.incrementKill();
            saveData();
        }
        public void youtube()
        {
            session.incrementYoutube();
            lifetime.incrementYoutube();
            saveData();
        }
        public void nick()
        {
            session.incrementNick();
            lifetime.incrementNick();
            saveData();
        }
        public void giphy()
        {
            session.incrementGiphy();
            lifetime.incrementGiphy();
            saveData();
        }
        public void funk()
        {
            session.incrementFunk();
            lifetime.incrementFunk();
            saveData();
        }
        public void shuffle()
        {
            session.incrementShuffle();
            lifetime.incrementShuffle();
            saveData();
        }
        public void choose()
        {
            session.incrementChoose();
            lifetime.incrementChoose();
            saveData();
        }
        public void quote()
        {
            session.incrementQuote();
            lifetime.incrementQuote();
            saveData();
        }
        public void trivia()
        {
            session.incrementTrivia();
            lifetime.incrementTrivia();
            saveData();
        }
        public void poke()
        {
            session.incrementPoke();
            lifetime.incrementPoke();
            saveData();
        }
        public void wiki()
        {
            session.incrementWiki();
            lifetime.incrementWiki();
            saveData();
        }
        public void temperature()
        {
            session.incrementTemperature();
            lifetime.incrementTemperature();
            saveData();
        }
        public void time()
        {
            session.incrementTime();
            lifetime.incrementTime();
            saveData();
        }
        public void greet()
        {
            session.incrementGreet();
            lifetime.incrementGreet();
            saveData();
        }
        public void rules()
        {
            session.incrementRules();
            lifetime.incrementRules();
            saveData();
        }
        public void help()
        {
            session.incrementHelp();
            lifetime.incrementHelp();
            saveData();
        }
        public void anime()
        {
            session.incrementAnime();
            lifetime.incrementAnime();
            saveData();
        }
        public void roll()
        {
            session.incrementRoll();
            lifetime.incrementRoll();
            saveData();
        }
        public void question()
        {
            session.incrementQuestion();
            lifetime.incrementQuestion();
            saveData();
        }


        public int[] getKill()
        {
            return new int[2] {session.Kill, lifetime.Kill};
        }
        public int[] getYoutube()
        {
            return new int[2] { session.Youtube, lifetime.Youtube };
        }
        public int[] getQuestion()
        {
            return new int[2] { session.Question, lifetime.Question };
        }
        public int[] getNick()
        {
            return new int[2] { session.Nick, lifetime.Nick };
        }
        public int[] getGiphy()
        {
            return new int[2] { session.Giphy, lifetime.Giphy };
        }
        public int[] getFunk()
        {
            return new int[2] { session.Funk, lifetime.Funk };
        }
        public int[] getShuffle()
        {
            return new int[2] { session.Shuffle, lifetime.Shuffle };
        }
        public int[] getChoose()
        {
            return new int[2] { session.Choose, lifetime.Choose };
        }
        public int[] getQuote()
        {
            return new int[2] { session.Quote, lifetime.Quote };
        }
        public int[] getTrivia()
        {
            return new int[2] { session.Trivia, lifetime.Trivia };
        }
        public int[] getPoke()
        {
            return new int[2] { session.Poke, lifetime.Poke };
        }
        public int[] getWiki()
        {
            return new int[2] { session.Wiki, lifetime.Wiki };
        }
        public int[] getTemperature()
        {
            return new int[2] { session.Temperature, lifetime.Temperature };
        }
        public int[] getTime()
        {
            return new int[2] { session.Time, lifetime.Time };
        }
        public int[] getGreet()
        {
            return new int[2] { session.Greet, lifetime.Greet };
        }
        public int[] getRules()
        {
            return new int[2] { session.Rules, lifetime.Rules };
        }
        public int[] getHelp()
        {
            return new int[2] { session.Help, lifetime.Help };
        }
        public int[] getAnime()
        {
            return new int[2] { session.Anime, lifetime.Anime };
        }
        public int[] getRoll()
        {
            return new int[2] { session.Roll, lifetime.Roll };
        }
    }

    class Stats
    {
        int kill;

        public int Kill
        {
            get { return kill; }
            set { kill = value; }
        }
        int youtube;

        public int Youtube
        {
            get { return youtube; }
            set { youtube = value; }
        }
        int question;

        public int Question
        {
            get { return question; }
            set { question = value; }
        }
        int nick;

        public int Nick
        {
            get { return nick; }
            set { nick = value; }
        }
        int giphy;

        public int Giphy
        {
            get { return giphy; }
            set { giphy = value; }
        }
        int funk;

        public int Funk
        {
            get { return funk; }
            set { funk = value; }
        }
        int shuffle;

        public int Shuffle
        {
            get { return shuffle; }
            set { shuffle = value; }
        }
        int choose;

        public int Choose
        {
            get { return choose; }
            set { choose = value; }
        }
        int quote;

        public int Quote
        {
            get { return quote; }
            set { quote = value; }
        }
        int trivia;

        public int Trivia
        {
            get { return trivia; }
            set { trivia = value; }
        }
        int poke;

        public int Poke
        {
            get { return poke; }
            set { poke = value; }
        }
        int wiki;

        public int Wiki
        {
            get { return wiki; }
            set { wiki = value; }
        }
        int temperature;

        public int Temperature
        {
            get { return temperature; }
            set { temperature = value; }
        }
        int time;

        public int Time
        {
            get { return time; }
            set { time = value; }
        }
        int greet;

        public int Greet
        {
            get { return greet; }
            set { greet = value; }
        }
        int rules;

        public int Rules
        {
            get { return rules; }
            set { rules = value; }
        }
        int help;

        public int Help
        {
            get { return help; }
            set { help = value; }
        }
        int anime;

        public int Anime
        {
            get { return anime; }
            set { anime = value; }
        }
        int roll;

        public int Roll
        {
            get { return roll; }
            set { roll = value; }
        }


        public Stats()
        {
            kill = 0;
            youtube = 0;
            question = 0;
            nick = 0;
            giphy = 0;
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
        }




        public void incrementKill() {Kill++;}
        public void incrementYoutube() { Youtube++; }
        public void incrementQuestion() { Question++; }
        public void incrementNick() { Nick++; }
        public void incrementGiphy() { Giphy++; }
        public void incrementFunk() { Funk++; }
        public void incrementShuffle() { Shuffle++; }
        public void incrementChoose() { Choose++; }
        public void incrementQuote() { Quote++; }
        public void incrementTrivia() { Trivia++; }
        public void incrementPoke() { Poke++; }
        public void incrementWiki() { Wiki++; }
        public void incrementTemperature() { Temperature++; }
        public void incrementTime() { Time++; }
        public void incrementGreet() { Greet++; }
        public void incrementRules() { Rules++; }
        public void incrementHelp() { Help++; }
        public void incrementAnime() { Anime++; }
        public void incrementRoll() { Roll++; }

    }
}
