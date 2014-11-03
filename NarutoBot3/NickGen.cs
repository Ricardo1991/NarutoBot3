using System;
using System.Collections.Generic;
using System.Linq;

namespace NarutoBot3
{
    static class NickGen
    {
        static int lineNumber;

        public static int LineNumber
        {
            get { return lineNumber; }
            set { lineNumber = value; }
        }

        static List<string> nickStrings;

        static public string GenerateNick(List<string> _nickStrings, int lineNumber)
        {
            nickStrings = new List<string>();
            LineNumber = lineNumber;

            foreach (string s in _nickStrings)
                nickStrings.Add(s);

            return Creator(false,false,false,false);
        }

        static public string GenerateNick(List<string> _nickStrings, int lineNumber, bool rd_numb, bool rd_uppr, bool rd_switch, bool rd_ique)
        {
            nickStrings = new List<string>();
            LineNumber = lineNumber;
            foreach (string s in _nickStrings)
                nickStrings.Add(s);

            return Creator(rd_numb, rd_uppr, rd_switch, rd_ique);
        }

        static string letterSwitch(string nick_gen)//Used in the nick gen
        {
            Random rnd = new Random();
            int changed = 0;
            int i = 0;
            int letras = 0;

            while (changed == 0 || letras == 0)
            {
                i = 0;
                while (i < nick_gen.Length)
                {
                    letras = 1;
                    if (nick_gen[i] == 'e' || nick_gen[i] == 'E')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + '3' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    else if (nick_gen[i] == 'a' || nick_gen[i] == 'A')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + '4' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    else if (nick_gen[i] == 't' || nick_gen[i] == 'T')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + '7' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    else if (nick_gen[i] == 'o' || nick_gen[i] == 'O')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + '0' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    else if (nick_gen[i] == 'i' || nick_gen[i] == 'I')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + '1' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    else if (nick_gen[i] == 's' || nick_gen[i] == 'S')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + '5' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    else if (nick_gen[i] == 'z' || nick_gen[i] == 'Z')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + '2' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }

                    i++;
                }
            }
            return nick_gen;
        }
        static string randomNumber(string nick_gen, int size = 2)//Used in the nick gen
        {
            Random rnd = new Random();
            switch (size)
            {
                case 1: nick_gen = nick_gen + rnd.Next(0, 9); break;
                case 2: nick_gen = nick_gen + rnd.Next(0, 99); ; break;
                case 3: nick_gen = nick_gen + rnd.Next(0, 999); ; break;
                case 4: nick_gen = nick_gen + rnd.Next(0, 9999); ; break;

                default: nick_gen = nick_gen + rnd.Next(0, 99); ; break;

            }

            return nick_gen;
        }
        static string randomUpper(string nick_gen)//Used in the nick gen
        {
            Random rnd = new Random();
            int changed = 0;
            int i = 0;
            int letras = 0;
            char[] alphabet = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            while (changed == 0 || letras == 0)
            {
                i = 0;
                while (i < nick_gen.Length)
                {
                    letras = 1;
                    if (alphabet.Any((s) => nick_gen[i].Equals(s)))
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + nick_gen[i].ToString().ToUpper() + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                            
                        }
                    }
                    i++;
                }
            }

            return nick_gen;
        }
        static string addSuffix(string nick_gen, string suffix)//Used in the nick gen
        {
            string last = nick_gen[nick_gen.Length - 1].ToString();
            if (last == "a".ToString() || last == "e".ToString() || last == "i".ToString() || last == "o".ToString() || last == "u".ToString())
            {
                nick_gen = nick_gen.Substring(0, nick_gen.Length - 1);
            }
            nick_gen = nick_gen + suffix;

            return nick_gen;

        }

        static string Creator(bool rd_numb, bool rd_uppr, bool rd_switch, bool rd_ique) //Used to create nicks
        {
            Random rnd = new Random();
            int rd = rnd.Next(lineNumber);
            int rdd;
            do
            {
                rdd = rnd.Next(lineNumber);
            } while (rd == rdd);

            string nick_gen;
            string part1 = nickStrings[rd];
            string part2 = nickStrings[rdd];

            nick_gen = part1 + part2;

            if (rd_ique)
                nick_gen = addSuffix(nick_gen, "ique");
            if (rd_numb)
                nick_gen = randomNumber(nick_gen);

            if (rd_switch)
                nick_gen = letterSwitch(nick_gen);

            if (rd_uppr)
                nick_gen = randomUpper(nick_gen);

            return nick_gen;
        }  

    }

}