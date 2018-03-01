using System;
using System.Collections.Generic;
using System.Linq;

namespace NarutoBot3
{
    internal static class NickGenerator
    {
        private static int lineNumber;
        private static List<string> nickStrings;

        public static int LineNumber
        {
            set { lineNumber = value; }
        }

        static public string GenerateNick(List<string> _nickStrings, int lineNumber, bool rd_numb, bool rd_uppr, bool rd_switch, bool rd_ique)
        {
            nickStrings = new List<string>();
            LineNumber = lineNumber;
            foreach (string s in _nickStrings)
                nickStrings.Add(s);

            return GenerateNick(rd_numb, rd_uppr, rd_switch, rd_ique);
        }

        static public string ReplaceCharacter(int position, string word, char newChar)
        {
            return word.Substring(0, position) + newChar + word.Substring(position + 1);
        }

        private static string LetterToNumber(string nick_gen)
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
                            nick_gen = ReplaceCharacter(i, nick_gen, '3');
                            changed = 1;
                            letras = 0;
                        }
                    }
                    else if (nick_gen[i] == 'a' || nick_gen[i] == 'A')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = ReplaceCharacter(i, nick_gen, '4');
                            changed = 1;
                            letras = 0;
                        }
                    }
                    else if (nick_gen[i] == 't' || nick_gen[i] == 'T')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = ReplaceCharacter(i, nick_gen, '7');
                            changed = 1;
                            letras = 0;
                        }
                    }
                    else if (nick_gen[i] == 'o' || nick_gen[i] == 'O')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = ReplaceCharacter(i, nick_gen, '0');
                            changed = 1;
                            letras = 0;
                        }
                    }
                    else if (nick_gen[i] == 'i' || nick_gen[i] == 'I')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = ReplaceCharacter(i, nick_gen, '1');
                            changed = 1;
                            letras = 0;
                        }
                    }
                    else if (nick_gen[i] == 's' || nick_gen[i] == 'S')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = ReplaceCharacter(i, nick_gen, '5');
                            changed = 1;
                            letras = 0;
                        }
                    }
                    else if (nick_gen[i] == 'z' || nick_gen[i] == 'Z')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = ReplaceCharacter(i, nick_gen, '2');
                            changed = 1;
                            letras = 0;
                        }
                    }

                    i++;
                }
            }
            return nick_gen;
        }

        private static string AppendNumber(string nick_gen, int size = 2)
        {
            Random rnd = new Random();

            nick_gen = nick_gen + rnd.Next(0, ((int)Math.Pow(10, size) - 1));

            return nick_gen;
        }

        private static string AddUpperCase(string nick_gen)
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
                            nick_gen = ReplaceCharacter(i, nick_gen, Char.ToUpper(nick_gen[i]));
                            changed = 1;
                            letras = 0;
                        }
                    }
                    i++;
                }
            }

            return nick_gen;
        }

        private static string AddSuffix(string nick_gen, string suffix)
        {
            string last = nick_gen[nick_gen.Length - 1].ToString();
            if (last == "a".ToString() || last == "e".ToString() || last == "i".ToString() || last == "o".ToString() || last == "u".ToString())
            {
                nick_gen = nick_gen.Substring(0, nick_gen.Length - 1);
            }
            nick_gen = nick_gen + suffix;

            return nick_gen;
        }

        private static string GenerateNick(bool rd_numb, bool rd_uppr, bool rd_switch, bool sufix)
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

            if (sufix)
                nick_gen = AddSuffix(nick_gen, "ique");
            if (rd_numb)
                nick_gen = AppendNumber(nick_gen);

            if (rd_switch)
                nick_gen = LetterToNumber(nick_gen);

            if (rd_uppr)
                nick_gen = AddUpperCase(nick_gen);

            return nick_gen;
        }
    }
}