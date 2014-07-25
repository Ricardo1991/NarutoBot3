using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarutoBot3
{
    class NickGen
    {
        static int lineNumber;

        public static int LineNumber
        {
            get { return lineNumber; }
            set { lineNumber = value; }
        }

        static List<string> nomes;

        static public string NickG(List<string> nome,int lineNumber, bool rd_numb, bool rd_uppr, bool rd_switch, bool rd_ique)
        {
            nomes = new List<string>();
            LineNumber = lineNumber;
            foreach (string s in nome)
                nomes.Add(s);

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
                    if (nick_gen[i] == 'a' || nick_gen[i] == 'A')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + '4' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 't' || nick_gen[i] == 'T')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + '7' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'o' || nick_gen[i] == 'O')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + '0' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'i' || nick_gen[i] == 'I')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + '1' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 's' || nick_gen[i] == 'S')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + '5' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'z' || nick_gen[i] == 'Z')
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

            while (changed == 0 || letras == 0)
            {
                i = 0;
                while (i < nick_gen.Length)
                {
                    letras = 1;
                    if (nick_gen[i] == 'a')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'A' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'b')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'B' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'c')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'C' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'd')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'D' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'e')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'E' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'f')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'F' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'g')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'G' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    } if (nick_gen[i] == 'h')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'H' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'i')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'I' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'j')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'J' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'k')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'K' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'l')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'L' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    } if (nick_gen[i] == 'm')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'M' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'n')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'N' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'o')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'O' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'p')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'P' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'q')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'Q' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'r')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'R' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 's')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'S' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 't')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'T' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'u')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'U' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'v')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'V' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    } if (nick_gen[i] == 'w')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'W' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }

                    if (nick_gen[i] == 'x')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'X' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'y')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'Y' + nick_gen.Substring(i + 1);
                            changed = 1;
                            letras = 0;
                        }
                    }
                    if (nick_gen[i] == 'z')
                    {
                        if (rnd.Next(0, 10) <= 2)
                        {
                            nick_gen = nick_gen.Substring(0, i) + 'Z' + nick_gen.Substring(i + 1);
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
            string part1 = nomes[rd];
            string part2 = nomes[rdd];

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